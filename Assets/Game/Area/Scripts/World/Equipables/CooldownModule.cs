﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CooldownModule : MonoBehaviour
{
    Action<float, float> callback_refreshCooldown = delegate { };
    Action callback_Begin = delegate { };
    Action callback_End = delegate { };
    Action callback_Update = delegate { };

    [SerializeField] float cooldown;
    public float Cooldown { get { return cooldown; } }
    float timer = 0;
    bool anim;

    #region Builder
    public CooldownModule Subscribe_Refresh(Action<float, float> _callback) { callback_refreshCooldown = _callback; return this; }
    public CooldownModule Subscribe_Begin(Action _callback) { callback_Begin = _callback; return this; }
    public CooldownModule Subscribe_End(Action _callback) { callback_End = _callback; return this; }
    public CooldownModule Subscribe_Update(Action _callback) { callback_Update = _callback; return this; }
    #endregion

    public void StartCooldown()
    {
        timer = 0;
        anim = true;
        callback_Begin.Invoke();
    }
    public void StartCooldown(float _newcooldown)
    {
        cooldown = _newcooldown;
        timer = 0;
        anim = true;
        callback_Begin.Invoke();
    }
    
    public void ResetCooldown()
    {
        anim = false;
        timer = 0;
    }
    public void Stop() { anim = false; timer = 0; callback_End.Invoke(); }
    public void Pause() => anim = false;
    public void Resume() => anim = true;

    private void Update()
    {
        if (anim)
        {
            callback_Update.Invoke();

            if (timer < cooldown)
            {
                timer = timer + 1 * Time.deltaTime;
                callback_refreshCooldown.Invoke(timer, cooldown);
            }
            else
            {
                timer = 0;
                anim = false;
                callback_End.Invoke();
            }
        }
    }
}