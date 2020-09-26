﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFloor : PlayObject
{
    DamageData dmgDATA;

    [SerializeField] ParticleSystem telegraphAttack = null;
    //[SerializeField] ParticleSystem fireAttack = null;

    void OnEnable()
    {

      
    }


    IEnumerator SpawnDamageFloor()
    {
        yield return new WaitForSeconds(3);
        
        GetComponent<BoxCollider>().enabled = true;
       
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<CharacterHead>())
        {
            DamageReceiver character = other.gameObject.GetComponent<DamageReceiver>();

            character.TakeDamage(dmgDATA);
        }
    }

    protected override void OnInitialize()
    {

        dmgDATA = GetComponent<DamageData>();
        dmgDATA.SetDamage(5000);

        StartCoroutine(SpawnDamageFloor());
    }

    protected override void OnTurnOn()
    {

    }

    protected override void OnTurnOff()
    {
        Debug.Log("Se apaga");
    }

    protected override void OnUpdate()
    {
        
    }

    protected override void OnFixedUpdate()
    {
        
    }

    protected override void OnPause()
    {
        
    }

    protected override void OnResume()
    {
        
    }
}
