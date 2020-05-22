﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoop : MonoBehaviour
{
    public static GameLoop instance; private void Awake() => instance = this;
    
    Checkpoint_Manager checkpointmanager;
    public void SubscribeCheckpoint(Checkpoint_Manager checkpointmanager) => this.checkpointmanager = checkpointmanager;

    private void Start()
    {
        Main.instance.eventManager.SubscribeToEvent(GameEvents.ON_PLAYER_DEATH, CharacterIsDeath);
    }

    public void StartGame()
    {
        /*
         
        aca le tengo que quitar las funciones al Main
        y ponerselas aca... luego desde el main llamar acá
         
         */

    }
    /*
    play()
    pause()
    stop()
    respaw()
    */
    public void CharacterIsDeath()
    {
        //character can´t receive damage
        //character deactivate
        //anim death
        //anim transition enter
        //calculate
        //anim transition exit
        //anim resurrect
        //character anim can´t receive damage
        //character activate
        //bla bla bla bla bla

        Invoke("CharacterResurrect", 0.5f);
    }

    public void CharacterResurrect()
    {
        Main.instance.GetChar().Life.Heal_AllHealth();
        checkpointmanager.SpawnChar();
    }
}
