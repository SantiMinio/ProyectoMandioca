﻿using System.Collections;
using UnityEngine;

public class TotemRoot : Totem
{
    [SerializeField] private float range = 20;

    [SerializeField] private float effectDuration = 10;

    [SerializeField] ParticleSystem onRootParticles = null;
    [SerializeField] CombatArea spawneablePosition = null;
    [SerializeField] EffectName effectOwner = EffectName.OnRoot;

    [SerializeField] Animator anim = null;
    [SerializeField] AnimEvent animEvent = null;
    [SerializeField] Collider col = null;

    CharacterHead myChar;

    protected override void OnInitialize()
    {
        base.OnInitialize();

        myChar = Main.instance.GetChar();

        StartCoroutine(CheckDistance());

        ParticlesManager.Instance.GetParticlePool(onRootParticles.name, onRootParticles);

        animEvent.Add_Callback("TeleportAnim", TeleportForAnim);

        damageReceiver.AddInvulnerability(Damagetype.All);
    }

    IEnumerator CheckDistance()
    {
        while (true)
        {
            if (!onUpdate && Vector3.Distance(myChar.transform.position, transform.position) <= range)
                OnTotemEnter();

            yield return new WaitForSeconds(0.5f);
        }
    }

    protected override void UpdateTotem()
    {
        base.UpdateTotem();

        if (myChar != null && Vector3.Distance(myChar.transform.position, transform.position) > range)
            OnTotemExit();
    }

    void StunChar()
    {
        myChar.GetComponent<EffectReceiver>().TakeEffect(effectOwner, effectDuration);

        ParticlesManager.Instance.PlayParticle(onRootParticles.name, myChar.transform.position);
    }

    protected override void Dead()
    {
        base.Dead();

        gameObject.SetActive(false);
    }

    protected override void InternalStunOver()
    {
        base.InternalStunOver();
        damageReceiver.AddInvulnerability(Damagetype.All);
        Teleport();
    }

    void Teleport()
    {
        col.enabled = false;
        anim.SetTrigger("Teleport");
    }

    void TeleportForAnim()
    {
        Vector2 aux;
        if (spawneablePosition.isCircle)
        {
            float ang = Random.Range(0, 360);

            Vector3 pos;
            pos.x = spawneablePosition.transform.position.x + spawneablePosition.circleRadius * Random.value * Mathf.Sin(ang * Mathf.Deg2Rad);
            pos.y = spawneablePosition.transform.position.y;
            pos.z = spawneablePosition.transform.position.z + spawneablePosition.circleRadius * Random.value * Mathf.Cos(ang * Mathf.Deg2Rad);

            transform.position = pos;
        }
        else
        {
            aux.x = -(spawneablePosition.cubeArea.x / 2) + spawneablePosition.cubeArea.x * Random.value;
            aux.y = -(spawneablePosition.cubeArea.y / 2) + spawneablePosition.cubeArea.y * Random.value;
            transform.position = spawneablePosition.transform.position + new Vector3(aux.x, 0, aux.y);
        }
        col.enabled = true;
    }

    protected override void InternalTakeDamage()
    {
        if (stuned)
        {
            TakeDamageFeedback(); return;
        }
        InterruptCast();
        Teleport();
    }

    protected override void InmuneFeedback()
    {
        base.InmuneFeedback();
        InterruptCast();
        Teleport();
    }

    protected override void InternalGetStunned()
    {
        base.InternalGetStunned();
        damageReceiver.RemoveInvulnerability(Damagetype.All);
        anim.SetTrigger("Stunned");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }

    protected override bool InternalCondition()
    {
        return true;
    }

    protected override void InternalEndCast()
    {
        feedback.StartGoToFeedback(myChar.transform, (x) => StunChar());
    }
}
