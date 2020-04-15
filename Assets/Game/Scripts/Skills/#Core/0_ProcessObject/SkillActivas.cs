﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillActivas : SkillBase
{
    public float cooldown;
    float time_cooldown;
    bool begincooldown;

    public bool one_time_use = true;
    public float useTime = 5f;
    float timer_use = 0;
    bool beginUse;

    public override void BeginSkill()
    {
        begincooldown = true;
        ui_skill.SetImages(skillinfo.img_avaliable, skillinfo.img_actived);
        ui_skill.Cooldown_ConfigureTime(cooldown);
        base.BeginSkill();
    }
    public override void EndSkill()
    {
        base.EndSkill();
    }

    public void Execute()
    {
        if (!begincooldown)
        {
            begincooldown = true;
            time_cooldown = 0;

            if (one_time_use)
            {
                OnOneShotExecute();
            }
            else
            {
                timer_use = useTime;
                OnStartUse();
                beginUse = true;
            }
        }
    }

    protected abstract void OnStartUse();
    protected abstract void OnStopUse();
    protected abstract void OnUpdateUse();

    internal override void absUpdate()
    {
        base.absUpdate();
        if (beginUse)
        {
            if (timer_use > 0)
            {
                timer_use = timer_use - 1 * Time.deltaTime;
                OnUpdateUse();
                //refresh request
                //aca le paso el current y el maximo
            }
            else
            {

                timer_use = 0;
                beginUse = false;
            }
        }
        if (begincooldown)
        {
            if (time_cooldown < cooldown)
            {
                time_cooldown = time_cooldown + 1 * Time.deltaTime;
                ui_skill.Cooldown_SetValueTime(time_cooldown);
            }
            else
            {
                time_cooldown = 0;
                begincooldown = false;
            }
        }
    }

    protected abstract void OnOneShotExecute();
}
