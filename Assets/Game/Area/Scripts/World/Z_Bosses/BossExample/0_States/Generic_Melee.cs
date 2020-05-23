﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generic_Melee : MonoStateBase
{
    public ParticleSystem partCircle;

    protected override void OnBegin() 
    {
        Get_Anim_Event_Subscriber.SubscribeMeTo(AnimEventLabel.Boss_StopLookAt, Get_Behaviours.followBehaviour.StopLookAt);
        Get_Anim_Event_Subscriber.SubscribeMeTo(AnimEventLabel.Boss_EndLoadHeavyAttack, OnFeedbackLoadHeavyAttack);
        Get_Anim_Event_Subscriber.SubscribeMeTo(AnimEventLabel.Boss_HitTheFloor, HitTheFloor);
        Get_Anim_Event_Subscriber.SubscribeMeTo(AnimEventLabel.Boss_Close_Melee_End, EndHitTheFloor);
        Get_Behaviours.followBehaviour.StartLookAt();
    }
    protected override void OnExit()
    { 
        Get_Behaviours.followBehaviour.StopLookAt();
        Get_Anim_Event_Subscriber.EraseSubscriptions();
    }
    protected override void OnUpdate() { }
    protected override void OnOneAwake() { }

    void OnFeedbackLoadHeavyAttack()
    {
        partCircle.Play();
    }
    void HitTheFloor()
    {
        Get_Behaviours.activateDamageHitTheFloor.Activate();
    }
    void EndHitTheFloor()
    {
        Get_Behaviours.activateDamageHitTheFloor.Deactivate();
    }
}