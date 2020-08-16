﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotemSpawner : Totem
{
    [SerializeField] CustomSpawner spawner;

    protected override void InternalEndCast()
    {
        for (int i = 0; i < spawner.waveAmount; i++)
        {
            Vector3 pos = spawner.GetSurfacePos();

            feedback.StartGoToFeedback(pos, (x) => spawner.SpawnPrefab(pos));
        }
    }

    protected override bool InternalCondition()
    {
        if (spawner.ReachMaxSpawn())
        {
            StartCoroutine(CheckMaxSpawn());
            return false;
        }
        else return true;
    }

    IEnumerator CheckMaxSpawn()
    {
        yield return new WaitForSeconds(1);

        OnStartCast();
    }

    protected override void TakeDamage()
    {

    }
}