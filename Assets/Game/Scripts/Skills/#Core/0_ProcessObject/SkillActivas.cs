﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillActivas : SkillBase
{
    public float cooldown;
    float time_cooldown;
    bool begincooldown;
    public override void BeginSkill()
    {
        base.BeginSkill();
        ui_skill.SetImages(skillinfo.img_avaliable, skillinfo.img_actived);
        ui_skill.Cooldown_ConfigureTime(cooldown);
    }
    public override void EndSkill()
    {
        base.EndSkill();
    }

    public void Execute()
    {
        begincooldown = true;
    }

    internal override void absUpdate()
    {
        base.absUpdate();
        if (begincooldown)
        {
            if (time_cooldown > cooldown)
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

    protected abstract void OnExecute();
}