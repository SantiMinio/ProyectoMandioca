﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillActive_DamageInRoom : SkillActivas
{
    [SerializeField] private int damagePower;
    [SerializeField] private ParticleSystem feedback;
    [SerializeField] private Damagetype dmgType;

    private CharacterHead _hero;

    protected override void OnOneShotExecute()
    {
        List<EnemyBase> enemies = Main.instance.GetEnemies();

        feedback.transform.position = _hero.transform.position;
        feedback.Play();
        
        foreach (EnemyBase enemy in enemies)
        {
            enemy.TakeDamage(damagePower, Vector3.up, dmgType, _hero);
        }
    }

    protected override void OnBeginSkill()
    {
        _hero = Main.instance.GetChar();
    }
    protected override void OnEndSkill() { }
    protected override void OnUpdateSkill() { }
    protected override void OnStartUse() { }
    protected override void OnStopUse() { }
    protected override void OnUpdateUse() { }
}
