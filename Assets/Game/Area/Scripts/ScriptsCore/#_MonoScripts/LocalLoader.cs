﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class LocalLoader : MonoBehaviour
{
    public List<LoadComponent> loadCOmponents;
    public int MaxCount { get => loadCOmponents.Count; }

    public IEnumerator Load()
    {
        yield return null;
    }
}