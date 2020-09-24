﻿using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

/// <summary>
/// aca no voy a hacer getters ni setters... quiero que sea todo lo mas directo posible
/// </summary>
public class LocalSceneHandler : LoadComponent
{
    public SceneData SceneData;
    public string prefabname;

    protected override IEnumerator LoadMe()
    {
        var trigger = GetComponentInChildren<TriggerDispatcher>();
        trigger.SubscribeToEnter(OnEnterToThisScene);

        StartLoad();

        yield return null;
    }

    public void OnEnterToThisScene()
    {
        NewSceneStreamer.instance.LoadScene(SceneData.name, false, true);
    }



    void StartLoad()
    {
        StartCoroutine(ResourcesLoad());
    }

    IEnumerator ResourcesLoad()
    {
        if (string.IsNullOrEmpty(prefabname)) yield break;
        ResourceRequest req = Resources.LoadAsync<GameObject>(prefabname);
        while (req.progress < 0.9f)
        {
            yield return null;
        }
        Instantiate(req.asset, this.transform);
        yield return null;
    }

}
