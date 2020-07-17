﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class CombatDirectorElement : MonoBehaviour, ICombatDirector
{
    [Header("TEMP:/Combat director")]
    //[SerializeField, Range(0.5f, 15)] float distancePos = 1.5f;
    bool withPos;

    CombatDirector director;
    EntityBase owner = null;
    EntityBase target;
    bool combat;

    [SerializeField] UnityEvent CanAttack = null;

    void SubscribeMeToInitializer()
    {
        //aca me subscribo a los sensores de inicializacion
        //tanto para el del proximidad
        //como el del habitacion
    }

    public void Initialize(EntityBase entityBase)
    {
        director = Main.instance.GetCombatDirector();
        director.AddNewTarget(entityBase);

    }
    public void IAmReady(Action _toAttackCallback)
    {
        combat = true;
        director.PrepareToAttack(this, Main.instance.GetChar());
    }
    public void IAmNotReady()
    {
        combat = false;
        director.DeleteToPrepare(this, Main.instance.GetChar());
    }
    public void AttackRelease()
    {
        director.AttackRelease(this, Main.instance.GetChar());
    }
    public void EnterCombat()
    {
        director.AddToList(this, Main.instance.GetChar());
        combat = true;
    }
    public void ExitCombat()
    {
        director.DeadEntity(this, Main.instance.GetChar());
        target = null;
        combat = false;
    }

    #region ICombatDirector Functions
    public void SetTarget(EntityBase entity) { target = entity; }
    public EntityBase CurrentTarget() => target;
    public Vector3 CurrentPos() => owner.transform.position;
    public void ToAttack()
    {
        CanAttack.Invoke();
    }

    public bool IsInPos() => withPos;
    public void SetBool(bool isPos) => withPos = isPos;
    public void ResetCombat()
    {
        target = null;
        combat = false;
        SetBool(false);
    }
    #endregion
}

