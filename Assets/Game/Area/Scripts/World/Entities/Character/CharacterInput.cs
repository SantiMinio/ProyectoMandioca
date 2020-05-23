﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using ToolsMandioca.EventClasses;

public class CharacterInput : MonoBehaviour
{
    public enum InputType { Joystick, Mouse, Other }
    public InputType input_type;

    JoystickBasicInput joystickhelper;
    
    [Header("Movement")]
    public UnityEvFloat LeftHorizontal;
    public UnityEvFloat LeftVertical;
    public UnityEvFloat RightHorizontal;
    public UnityEvFloat RightVertical;
    public UnityEvFloat ChangeWeapon;
    public UnityEvent Dash;

    [Header("Defense")]
    public UnityEvent OnBlock;
    public UnityEvent UpBlock;

    [Header("Attack")]
    public UnityEvent OnAttack;
    public UnityEvent OnAttackEnd;

    [Header("Interact")]
    public UnityEvent OnInteractBegin;
    public UnityEvent OnInteractEnd;
    public UnityEvent Back;

    [Header("Test actives")]
    public UnityEvent OnDpad_Up;
    public UnityEvent OnDpad_Down;
    public UnityEvent OnDpad_Left;
    public UnityEvent OnDpad_Right;
    public UnityEvent OnUseActive;

    public EventInt SendIndexAlphanumeric;

    public UnityEvent LockON;
    public UnityEvent NextON;

    private void Awake() => ConfigureJoystickHelper();

    private void Start()
    {
        DevelopTools.UI.Debug_UI_Tools.instance.CreateToogle("Input de Rotacion", false, ChangeRotation);
    }
    private void Update()
    {
        LeftHorizontal.Invoke(Input.GetAxis("Horizontal"));
        LeftVertical.Invoke(Input.GetAxis("Vertical"));
        if (input_type == InputType.Joystick) JoystickInputs();
        else if (input_type == InputType.Mouse) MouseInputs();
        if (Input.GetButtonDown("Dash")) Dash.Invoke();

        if (Input.GetButtonDown("Block")) OnBlock.Invoke();
        if (Input.GetButtonUp("Block")) UpBlock.Invoke();
        
        if (Input.GetButtonDown("Attack")) OnAttack.Invoke();
        if (Input.GetButtonUp("Attack")) OnAttackEnd.Invoke();

        if (Input.GetButtonDown("Interact")) OnInteractBegin.Invoke();
        if (Input.GetButtonUp("Interact")) OnInteractEnd.Invoke();
        ChangeWeapon.Invoke(Input.GetAxis("XBOX360_DPadHorizontal"));

        if (Input.GetKeyDown(KeyCode.Alpha1)) EV_DPAD_UP();
        if (Input.GetKeyDown(KeyCode.Alpha2)) EV_DPAD_LEFT();
        if (Input.GetKeyDown(KeyCode.Alpha3)) EV_DPAD_DOWN();
        if (Input.GetKeyDown(KeyCode.Alpha4)) EV_DPAD_RIGHT();

        if (Input.GetButtonDown("Back")) Back.Invoke();

        if (Input.GetButtonDown("Skill")) OnUseActive.Invoke();

        if (Input.GetButtonDown("LockOn")) LockON.Invoke();

        if (Input.GetKeyDown(KeyCode.Alpha1)) SendIndexAlphanumeric.Invoke(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SendIndexAlphanumeric.Invoke(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SendIndexAlphanumeric.Invoke(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SendIndexAlphanumeric.Invoke(3);

        RefreshHelper();

    }

    public void MouseInputs()
    {
        Vector2 positionOnScreen = Camera.main.WorldToViewportPoint(transform.position);
        Vector2 mouseOnScreen = (Vector2)Camera.main.ScreenToViewportPoint(Input.mousePosition);
        Vector2 mousePos = (mouseOnScreen - positionOnScreen).normalized;
        RightHorizontal.Invoke(mousePos.x);
        RightVertical.Invoke(mousePos.y);
    }

    public void JoystickInputs()
    {
        RightHorizontal.Invoke(Input.GetAxis("RightHorizontal"));
        RightVertical.Invoke(Input.GetAxis("RightVertical"));
    }

    public string ChangeRotation(bool value)
    {
        if (value)
        {
            input_type = InputType.Mouse;
            return "Usa Mouse";
        }
        else
        {
            input_type = InputType.Joystick;
            return "Usa Joystick";
        }
    }

    #region JoystickHelper
    void ConfigureJoystickHelper()
    {
        joystickhelper = new JoystickBasicInput();
        joystickhelper
            .SUBSCRIBE_DPAD_UP(EV_DPAD_UP)
            .SUBSCRIBE_DPAD_DOWN(EV_DPAD_DOWN)
            .SUBSCRIBE_DPAD_RIGHT(EV_DPAD_RIGHT)
            .SUBSCRIBE_DPAD_LEFT(EV_DPAD_LEFT)
            .SUBSCRIBE_LTRIGGER(EV_DPAD_LTRIGGER)
            .SUBSCRIBE_RTRIGGER(EV_DPAD_RTRIGGER);
    }
    void RefreshHelper() => joystickhelper.Refresh();
    void EV_DPAD_UP() { SendIndexAlphanumeric.Invoke(0); }
    void EV_DPAD_DOWN() { SendIndexAlphanumeric.Invoke(3); }
    void EV_DPAD_LEFT() { SendIndexAlphanumeric.Invoke(1);  }
    void EV_DPAD_RIGHT() { SendIndexAlphanumeric.Invoke(2); }
    void EV_DPAD_LTRIGGER() { LockON.Invoke(); }
    void EV_DPAD_RTRIGGER() { NextON.Invoke(); }
    #endregion


    [System.Serializable]
    public class UnityEvFloat : UnityEvent<float> { }
}

