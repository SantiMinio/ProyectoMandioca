﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToolsMandioca.Extensions;

public class SkillActive_FreezeRange : SkillActivas
{
    [SerializeField] private float range = 10;
    [SerializeField] private int freezeDuration = 5;

    [SerializeField] private ParticleSystem freezeNova = null;
    [SerializeField] private ParticleSystem freezeSmoke = null;
    [SerializeField] private Transform particleContainer = null;
    [SerializeField] private IceShard_particleObjectPool shardPool = null;
    [SerializeField] private AudioClip freeze_Sound;

    private List<ParticleSystem> shards = new List<ParticleSystem>();
    List<ParticleSystem> ps = new List<ParticleSystem>();

    private CharacterHead _hero;
    private const string freezeSound = "freeze";

    [SerializeField] Atenea atenea;
    protected override void OnBeginSkill()
    {
        _hero = Main.instance.GetChar();
        AudioManager.instance.GetSoundPool(freezeSound, freeze_Sound);
    }

    protected override void OnOneShotExecute()
    {
        AudioManager.instance.PlaySound(freezeSound);
        particleContainer.position = _hero.transform.position;
        var auraMain = freezeNova.main;
        //auraMain.duration = freezeDuration;//no se puede setear la duracion en play
        auraMain.startSize = range;
        freezeSmoke.Play();
        freezeNova.Play();

        atenea.gameObject.SetActive(true);
        atenea.GoToHero();
        atenea.Anim_Freeze();
        
        
        List<EnemyBase> enemies = Extensions.FindInRadius<EnemyBase>(_hero.transform, range);

        
        foreach (EnemyBase enemy in enemies)
        {
            {
                var shard = shardPool.Get();
                shard.transform.position = enemy.transform.position;
                shards.Add(shard);
                
                enemy.OnFreeze();
            }
        }
        
        Invoke("GuardarShardsParticles", 2);
    }

    void GuardarShardsParticles()
    {
        foreach (var shard in shards)
        {
            shardPool.ReturnToPool(shard);
        }
    }

   
    protected override void OnEndSkill() { }
    protected override void OnUpdateSkill() { }
    protected override void OnStartUse() { }
    protected override void OnStopUse() { }
    protected override void OnUpdateUse() { }
}