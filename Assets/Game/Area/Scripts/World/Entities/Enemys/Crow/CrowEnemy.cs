﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Tools.StateMachine;
using UnityEngine.Serialization;

public class CrowEnemy : EnemyBase
{
    public AnimationCurve animEmisive;

    [Header("Combat Options")]
    [SerializeField] int damage = 2;
    [SerializeField] float rotationSpeed = 8;
    [SerializeField] float knockback = 20;
    [SerializeField] float attackRecall = 2;
    public DummySpecialAttack dummySpecialAttack;

    private CombatDirector director;

    [Header("Life Options")]
    [SerializeField] float recallTime = 1;

    private bool cooldown = false;
    private float timercooldown = 0;

    [Header("Feedback")]
    [SerializeField] AnimEvent anim = null;
    [SerializeField] Animator animator = null;
    private Material[] myMat;
    [SerializeField] Color onHitColor = Color.white;
    [SerializeField] float onHitFlashTime = 0.1f;
    [SerializeField] RagdollComponent ragdoll = null;

    [SerializeField] EffectBase petrifyEffect = null;
    EventStateMachine<CrowInputs> sm;

    public DataBaseCrowParticles particles;
    public DataBaseCrowSounds sounds;

    [System.Serializable]
    public class DataBaseCrowParticles
    {
        public ParticleSystem castParticles = null;
        public ParticleSystem takeDmg = null;
    }

    [System.Serializable]
    public class DataBaseCrowSounds
    {
        public AudioClip takeDmgSound;
        public AudioClip attackSound;
    }

    protected override void OnInitialize()
    {
        base.OnInitialize();
        Main.instance.eventManager.TriggerEvent(GameEvents.ENEMY_SPAWN, new object[] { this });
        ParticlesManager.Instance.GetParticlePool(particles.castParticles.name, particles.castParticles, 3);
        ParticlesManager.Instance.GetParticlePool(particles.takeDmg.name, particles.takeDmg, 8);

        var smr = GetComponentInChildren<SkinnedMeshRenderer>();
        if (smr != null)
            myMat = smr.materials;

        AudioManager.instance.GetSoundPool(sounds.takeDmgSound.name, AudioGroups.GAME_FX, sounds.takeDmgSound);
        AudioManager.instance.GetSoundPool(sounds.attackSound.name, AudioGroups.GAME_FX, sounds.attackSound, true);

        rb = GetComponent<Rigidbody>();
        anim.Add_Callback("DealDamage", DealDamage);
        Main.instance.AddEntity(this);

        IAInitialize(Main.instance.GetCombatDirector());

        petrifyEffect?.AddStartCallback(() => sm.SendInput(CrowInputs.PETRIFIED));
        petrifyEffect?.AddEndCallback(() => sm.SendInput(CrowInputs.IDLE));
    }
    protected override void OnReset()
    {
        ragdoll.Ragdoll(false, Vector3.zero);
    }
    public override void Zone_OnPlayerExitInThisRoom()
    {
        //Debug.Log("Player enter the room");
        IAInitialize(Main.instance.GetCombatDirector());
    }
    public override void Zone_OnPlayerEnterInThisRoom(Transform who)
    {
        sm.SendInput(CrowInputs.DISABLE);
    }
    public override void IAInitialize(CombatDirector _director)
    {
        director = _director;
        if (sm == null)
            SetStates();
        else
        {
            sm.SendInput(CrowInputs.IDLE);
        }

        director.AddNewTarget(this);

        canupdate = true;
    }
    protected override void OnUpdateEntity()
    {
        if (canupdate)
        {
            if (!death)
            {
                if (combat)
                {
                    if (Vector3.Distance(Main.instance.GetChar().transform.position, transform.position) > combatDistance + 2)
                    {
                        director.DeadEntity(this, entityTarget);
                        entityTarget = null;
                        combat = false;
                    }
                }

                if (!combat && entityTarget == null)
                {
                    if (Vector3.Distance(Main.instance.GetChar().transform.position, transform.position) <= combatDistance)
                    {
                        director.AddToList(this, Main.instance.GetChar());
                        SetTarget(Main.instance.GetChar());
                        combat = true;
                    }
                }
            }

            if (sm != null)
            {
                sm.Update();
            }

            if (cooldown)
            {
                if (timercooldown < recallTime) timercooldown = timercooldown + 1 * Time.deltaTime;
                else { cooldown = false; timercooldown = 0; }
            }
        }
    }
    protected override void OnPause() { }
    protected override void OnResume() { }

    #region Attack
    public void DealDamage() { /* te re tiro la piedra wacho */ }

    //public void AttackEntity(DamageReceiver e)
    //{
    //    dmgData.SetDamage(damage).SetDamageTick(false).SetDamageType(Damagetype.Normal).SetKnockback(knockback)
    //.SetPositionAndDirection(transform.position);
    //    Attack_Result takeDmg = e.TakeDamage(dmgData);

    //    if (takeDmg == Attack_Result.parried)
    //    {
    //        combatComponent.Stop();
    //        sm.SendInput(CrowInputs.PARRIED);
    //    }
    //}

    public override void ToAttack() => attacking = true;
    #endregion

    #region Life Things
    public GenericLifeSystem Life() => lifesystem;

    protected override void TakeDamageFeedback(DamageData data)
    {
        if (sm.Current.Name == "Idle")
        {
            attacking = false;
            director.ChangeTarget(this, data.owner, entityTarget);
        }

        AudioManager.instance.PlaySound(sounds.takeDmgSound.name);

        sm.SendInput(CrowInputs.TAKE_DAMAGE);

        ParticlesManager.Instance.PlayParticle(particles.takeDmg.name, transform.position + Vector3.up);
        cooldown = true;

        StartCoroutine(OnHitted(myMat, onHitFlashTime, onHitColor));
    }

    protected override void Die(Vector3 dir)
    {
        sm.SendInput(CrowInputs.DIE);
        if (dir == Vector3.zero)
            ragdoll.Ragdoll(true, -rootTransform.forward);
        else
            ragdoll.Ragdoll(true, dir);
        death = true;
        director.RemoveTarget(this);
        Main.instance.RemoveEntity(this);
    }

    protected override bool IsDamage()
    {
        if (cooldown || Invinsible || sm.Current.Name == "Die") return true;
        else return false;
    }
    #endregion

    protected override void OnFixedUpdate() { }
    protected override void OnTurnOff()
    {
        if (sm.Current.Name == "Die") gameObject.SetActive(false);

        sm.SendInput(CrowInputs.DISABLE);
        if (combat)
        {
            director.DeadEntity(this, entityTarget);
            entityTarget = null;
            combat = false;
        }
    }
    protected override void OnTurnOn()
    {
        sm.SendInput(CrowInputs.IDLE);
    }

    #region STATE MACHINE THINGS
    public enum CrowInputs { IDLE, BEGIN_ATTACK, DIE, DISABLE, TAKE_DAMAGE, PETRIFIED, CHASING };
    void SetStates()
    {
        var idle = new EState<CrowInputs>("Idle");
        var chasing = new EState<CrowInputs>("Chasing");
        var attack = new EState<CrowInputs>("Begin_Attack");
        var takeDamage = new EState<CrowInputs>("Take_Damage");
        var die = new EState<CrowInputs>("Die");
        var disable = new EState<CrowInputs>("Disable");
        var petrified = new EState<CrowInputs>("Petrified");

        ConfigureState.Create(idle)
            .SetTransition(CrowInputs.TAKE_DAMAGE, takeDamage)
            .SetTransition(CrowInputs.DIE, die)
            .SetTransition(CrowInputs.PETRIFIED, petrified)
            .SetTransition(CrowInputs.DISABLE, disable)
            .SetTransition(CrowInputs.CHASING, chasing)
            .Done();

        ConfigureState.Create(chasing)
            .SetTransition(CrowInputs.IDLE, idle)
            .SetTransition(CrowInputs.BEGIN_ATTACK, attack)
            .SetTransition(CrowInputs.TAKE_DAMAGE, takeDamage)
            .SetTransition(CrowInputs.DIE, die)
            .SetTransition(CrowInputs.PETRIFIED, petrified)
            .SetTransition(CrowInputs.DISABLE, disable)
            .Done();

        ConfigureState.Create(attack)
            .SetTransition(CrowInputs.IDLE, idle)
            .SetTransition(CrowInputs.DIE, die)
            .SetTransition(CrowInputs.PETRIFIED, petrified)
            .SetTransition(CrowInputs.DISABLE, disable)
            .Done();

        ConfigureState.Create(petrified)
            .SetTransition(CrowInputs.IDLE, idle)
            .SetTransition(CrowInputs.BEGIN_ATTACK, attack)
            .SetTransition(CrowInputs.DIE, die)
            .SetTransition(CrowInputs.DISABLE, disable)
            .Done();

        ConfigureState.Create(takeDamage)
            .SetTransition(CrowInputs.IDLE, idle)
            .SetTransition(CrowInputs.DISABLE, disable)
            .SetTransition(CrowInputs.PETRIFIED, petrified)
            .SetTransition(CrowInputs.DIE, die)
            .Done();

        ConfigureState.Create(die)
            .Done();

        ConfigureState.Create(disable)
            .SetTransition(CrowInputs.IDLE, idle)
            .Done();

        sm = new EventStateMachine<CrowInputs>(idle, null);

        var head = Main.instance.GetChar();

        new CrowIdle(idle, sm, distancePos, rotationSpeed, this).SetAnimator(animator).SetRoot(rootTransform).SetDirector(director);

        new CrowChasing(chasing, sm, IsAttack, distancePos, rotationSpeed, this).SetDirector(director).SetRoot(rootTransform);

        new CrowAttack(attack, sm, attackRecall, this).SetAnimator(animator).SetDirector(director);

        new CrowTakeDmg(takeDamage, sm, recallTime).SetAnimator(animator);

        new DummyStunState<CrowInputs>(petrified, sm);

        new CrowDead(die, sm, ragdoll).SetAnimator(animator).SetDirector(director).SetRigidbody(rb);

        new DummyDisableState<CrowInputs>(disable, sm, EnableObject, DisableObject);
    }

    void DisableObject()
    {
        canupdate = false;
        combat = false;
    }

    void EnableObject() => Initialize();

    #endregion
}