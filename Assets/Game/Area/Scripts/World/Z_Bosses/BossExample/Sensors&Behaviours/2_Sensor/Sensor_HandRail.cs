﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class Sensor_HandRail : MonoBehaviour
{
    [SerializeField] UnityEvent OnExecute = null;

    Func<bool> pred = delegate { return true; };
    public void SetPredicate(Func<bool> _pred ) => pred = _pred;

    public void Hand_Enter()
    {
        Debug.Log("HandRail");
        if (pred.Invoke())
        {
            HandExit();
        }
    }

    void HandExit()
    {
        OnExecute.Invoke();
    }
}
