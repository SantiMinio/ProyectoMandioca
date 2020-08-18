﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools.Extensions;
using UnityEngine.Events;

public class NPC_Dialog : Interactable
{
    public string nombre_NPC;
    public DialogueTree[] dialogues;
    public DialogueTree currentDialoge;
    public UnityEvent OnBeginMission;

    private void Awake()
    {
        if(dialogues.Length > 0)
            currentDialoge = dialogues[0];
    }

    public override void OnEnter(WalkingEntity entity)
    {
        
        WorldItemInfo.instance.Show(pointToMessage.position, nombre_NPC, "", "hablar", false, false);
        
    }
    public override void OnExecute(WalkingEntity collector)
    {
        if(currentDialoge) DialogueManager.instance.StartDialogue(currentDialoge);
        WorldItemInfo.instance.Hide();
    }
    public override void OnExit()
    {
        WorldItemInfo.instance.Hide();
    }

    public void GoToFase(int newfase)
    {
        currentDialoge = dialogues[newfase];
    }

}