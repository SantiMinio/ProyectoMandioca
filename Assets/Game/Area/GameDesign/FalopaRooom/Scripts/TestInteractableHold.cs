﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestInteractableHold : Interactable
{
    [SerializeField] Image _loadBar;
    private void Start()
    {
        _executeAction += DestroyGameObject;
    }
    public override void OnEnter(WalkingEntity entity)
    {
        WorldItemInfo.instance.Show(transform.position, "Object", "Hold to grab object");
    }

    public override void OnExecute(WalkingEntity collector)
    {
        _executeAction();
    }

    public override void OnExit()
    {
        WorldItemInfo.instance.Hide();
    }
    public override void DelayExecute(float loadTime)
    {
        base.DelayExecute(loadTime);
        float amount = (currentTime / delayTime);
        _loadBar.fillAmount=amount;
    }
    void DestroyGameObject()
    {
        Destroy(this.gameObject);
    }

}