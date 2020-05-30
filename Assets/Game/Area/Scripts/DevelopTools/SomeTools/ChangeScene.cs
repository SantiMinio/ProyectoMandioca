﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public float timer = 1;
    public string scene = "level_to_change";
    public bool stayHere;

    void Start()
    {
        Invoke("Execute", timer);
    }
    void Execute()
    {
        if (!stayHere)
            LoadSceneHandler.instance.LoadAScene(scene);
    }
}