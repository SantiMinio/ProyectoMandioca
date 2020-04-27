﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GenericSword : Weapon
{
    public GenericSword(float dmg, float r, string n, float angle) : base(dmg, r, n, angle)
    {
        
    }

    bool oneshotSucsesfull;

    public override EntityBase Attack(Transform pos, float damage)
    {
        EntityBase entity = null;

        var entities = Physics.OverlapSphere(pos.position, range)
            .Where(x => x.GetComponent<EntityBase>())
            .Where(x => x.GetComponent<EntityBase>() != Main.instance.GetChar())
            .ToList();

        
        foreach (var v in entities)
        {
            Debug.Log("entity " + v.gameObject);
        }

        for (int i = 0; i < entities.Count; i++)
        {
            Debug.Log(entities[i].gameObject.name);

            Vector3 dir = entities[i].transform.position - pos.position;
            float angle = Vector3.Angle(pos.forward, dir);

            var current = entities[i].GetComponent<EntityBase>();

            if (dir.magnitude <= range && angle < base.angle)
            {
                var attackResult = current.TakeDamage(
                        (int) damage, 
                        Main.instance.GetChar().transform.position, 
                        Damagetype.parriable, 
                        _head);

                AttackResult?.Invoke(attackResult);

                if (attackResult == Attack_Result.sucessful)
                {
                    oneshotSucsesfull = true;
                }

                Debug.Log("Attack result: " + attackResult.ToString());

            }
        }

        if (oneshotSucsesfull)
        {
            oneshotSucsesfull = false;
            Main.instance.Vibrate();
            Main.instance.CameraShake();
        }

        return entity;
    }
}
