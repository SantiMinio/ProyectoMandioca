﻿using UnityEngine;
using System;
using System.Linq;

namespace Tools.StateMachine
{
    public class JabaliPushAttack : JabaliStates
    {
        float pushSpeed;
        Action DealDamage;
        Action PlayCombat;
        float maxSpeed;
        GameObject feedbackCharge;
        SoundPool pool;
        AudioSource source;
        Func<Transform> GetTarget;


        float timer;
        float timePush = 10;

        public JabaliPushAttack(EState<JabaliEnemy.JabaliInputs> myState, EventStateMachine<JabaliEnemy.JabaliInputs> _sm, float _speed,
                                Action _DealDamage, GameObject _feedbackCharge, Action _PlayCombat, string _wooshSound) : base(myState, _sm)
        {
            maxSpeed = _speed;
            pushSpeed = maxSpeed / 2;
            DealDamage = _DealDamage;
            feedbackCharge = _feedbackCharge;
            PlayCombat = _PlayCombat;
            pool = AudioManager.instance.GetSoundPool(_wooshSound);
        }

        protected override void Enter(EState<JabaliEnemy.JabaliInputs> input)
        {
            base.Enter(input);
            feedbackCharge.SetActive(true);
            feedbackCharge.GetComponentsInChildren<ParticleSystem>()
                .ToList()
                .ForEach(x => x.Play());
            PlayCombat();
            anim.SetTrigger("ChargeOk");
            source = pool.Get();
            source.transform.position = root.transform.position;
            source.Play();
        }

        protected override void Update()
        {
            if (pushSpeed < maxSpeed)
            {
                pushSpeed += Time.deltaTime;

                if (pushSpeed > maxSpeed)
                    pushSpeed = maxSpeed;
            }

            rb.velocity = new Vector3(root.forward.x * pushSpeed, rb.velocity.y, root.forward.z * pushSpeed);
            DealDamage();

            timer += Time.deltaTime;

            if (timer >= timePush)
                sm.SendInput(JabaliEnemy.JabaliInputs.IDLE);

            base.Update();
        }

        protected override void Exit(JabaliEnemy.JabaliInputs input)
        {
            feedbackCharge.SetActive(false);
            feedbackCharge.GetComponentsInChildren<ParticleSystem>()
                .ToList()
                .ForEach(x => x.Stop());

            base.Exit(input);
            rb.velocity = Vector3.zero;
            combatDirector.AttackRelease(enemy, enemy.CurrentTarget());
            source.Stop();
            pool.ReturnToPool(source);

            timer = 0;
        }
    }
}

