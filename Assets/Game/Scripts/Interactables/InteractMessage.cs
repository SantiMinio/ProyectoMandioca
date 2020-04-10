﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractMessage : Interactable
{

    //para mensajes muy simples

    public string message = "simple message";

    public override void Execute(WalkingEntity collector)
    {
        //no hago nada con el collector
        //UI_Messages.instancia.ShowMessage(message, 2f);

        Debug.Log("Estoy interactuando");
    }

    public override void Exit()
    {
        if (feedback.Length > 0) foreach (var i in feedback) i.Hide();
    }

    public override void ShowInfo(WalkingEntity entity)
    {
        WorldItemInfo.instance.Show(pointToMessage.position, "Interactuable", "Esto es un interactuable", "interactuar");
        if (feedback.Length > 0) foreach (var i in feedback) i.Show();
    }
}