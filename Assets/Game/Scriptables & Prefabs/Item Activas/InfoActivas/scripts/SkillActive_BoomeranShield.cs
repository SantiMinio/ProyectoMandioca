﻿using System;
using System.Collections;
using System.Collections.Generic;
using Tools.Extensions;
using UnityEditor;
using UnityEngine;

public class SkillActive_BoomeranShield : SkillActivas
{
    [SerializeField] private float throwRange;
    [SerializeField] private float radius;
    [SerializeField] private float spinDuration;
    [SerializeField] private int damage;
    [SerializeField] private float throwSpeed;

    private CharacterHead _hero;
    private GameObject _shield;

    private float timeCount;

    private Vector3 spinPosition;
    private Vector3 startHeroPos;
    private Vector3 startHeroLookDirection;

    [SerializeField] private ParticleSystem sparks;

    [SerializeField] private GameObject auxShield;

    private bool isGoing;
    private bool isSpinning;
    private bool isReturning;

    private float startTime;
    protected override void OnBeginSkill()
    {
        _hero = Main.instance.GetChar();
        _shield = _hero.escudo;
    }

    protected override void OnEndSkill()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnUpdateSkill()
    {
        
    }

    protected override void OnStartUse()
    {
        _hero.ToggleBlock();
        auxShield.SetActive(true);
        auxShield.transform.position = _shield.transform.position;
        _shield.SetActive(false);
        sparks.Play();
        
        spinPosition = auxShield.transform.position + (_hero.GetCharMove().GetRotatorDirection() * throwRange);
        startHeroPos = _shield.transform.position;
        startHeroLookDirection = _hero.GetCharMove().GetRotatorDirection();
        
        startTime = Time.time;
        isGoing = true;
    }

    protected override void OnStopUse()
    {
        _hero.ToggleBlock();
        _shield.SetActive(true);
        auxShield.SetActive(false);
        timeCount = 0;
        sparks.Stop();
    }

    protected override void OnUpdateUse()
    {


       
        //Feedback
        sparks.transform.position = auxShield.transform.position;
        
        //Hago el daño
        var enemiesClose = Extensions.FindInRadius<EnemyBase>(auxShield.transform.position, radius);
        foreach (EnemyBase enemy in enemiesClose)
        {
            enemy.TakeDamage(damage, enemy.transform.position - auxShield.transform.position, Damagetype.normal);
        }


        if (isGoing)
        {
            //viajar hasta la posicion
        
            if (Vector3.Distance(  auxShield.transform.position, spinPosition) > .5f)
            {
                MoveWithLerp(startHeroPos, spinPosition);
            }
            else
            {
                isGoing = false;
                isSpinning = true;
            }
        }

        if (isSpinning)
        {
            timeCount += Time.deltaTime;
            
            //volver
            if (timeCount > spinDuration)
            {
                isSpinning = false;
                isReturning = true;
                startTime = Time.time;
            }
        }

        if (isReturning)
        {
            if (Vector3.Distance(auxShield.transform.position, _hero.transform.position) > .5f)
            {
                MoveWithLerp(auxShield.transform.position, _hero.transform.position);
            }
            else
            {
                isReturning = false;
                OnStopUse();
            }
        }
    }

    
    void MoveWithLerp(Vector3 start,Vector3 end)
    {
        float distCovered = (Time.time - startTime) * throwSpeed;
        
        float fractionOfJourney = distCovered / Vector3.Distance(_hero.transform.position, auxShield.transform.position);
        
        auxShield.transform.position = Vector3.Lerp(start, end, fractionOfJourney);
    }

    protected override void OnOneShotExecute()
    {
        throw new System.NotImplementedException();
    }
}