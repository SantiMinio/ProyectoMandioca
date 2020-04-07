﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FocusOnParryComponent : MonoBehaviour
{
    Action<Vector3, FocusOnParryComponent> listener;

    public void OnBegin()
    {
        Main.instance.GetChar().AddParry(OnExecute);
    }
    public void OnEnd()
    {
        Main.instance.GetChar().RemoveParry(OnExecute);
    }

    public void OnExecute()
    {
       listener.Invoke(Main.instance.GetChar().gameObject.transform.position, this);
    }

    public void Configure(Action<Vector3, FocusOnParryComponent> _listener)
    {
        listener = _listener;
    }
}
