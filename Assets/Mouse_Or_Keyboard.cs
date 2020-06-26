﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Mouse_Or_Keyboard : MonoBehaviour
{

    [SerializeField] DefaultMenuAnim buttons_joystick_panel;
    [SerializeField] DefaultMenuAnim buttons_Keyboard_panel;

    [Header("Si esta activo usa el input del mouse")]
    public bool _activeRotation;

    public string sceneGym = "Gym";
    public string sceneBlocking = "TerrainTestGonzaSinChar";

    public void SwitchControls()
    {
        if (_activeRotation)
        {
            buttons_joystick_panel.Open();
            buttons_Keyboard_panel.Close();
            _activeRotation = false;
        }
        else 
        {
            buttons_joystick_panel.Close();
            buttons_Keyboard_panel.Open();
            _activeRotation = true;
        }
    }

    public void JoystickButon()
    {
        _activeRotation = false;
    }
    public void KeyBoardButon()
    {
        _activeRotation = true;
    }
    public void goToGym()
    {
        CharacterInput inputs = Main.instance.GetChar().getInput;
        inputs.ChangeRotation(_activeRotation);
        LoadSceneHandler.instance.LoadAScene(sceneGym);
        gameObject.SetActive(false);
    }
    public void goToBlockinNuevo()
    {
        CharacterInput inputs = Main.instance.GetChar().getInput;
        inputs.ChangeRotation(_activeRotation);
        LoadSceneHandler.instance.LoadAScene(sceneBlocking);
        gameObject.SetActive(false);
    }
}
