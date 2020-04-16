﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageHere : Interact_Receptor
{
    public Item[] items;
    public Transform trans;

    public int cant;
    public GameObject model;

    Vector3 getPosRandom(int radio, Transform t)
    {
        Vector3 min = new Vector3(t.position.x - radio, t.position.y - radio, t.position.z - radio);
        Vector3 max = new Vector3(t.position.x + radio, t.position.y + radio, t.position.z + radio);
        return new Vector3(Random.Range(min.x, max.x), 0, Random.Range(min.z, max.z));
    }

    public override void Execute()
    {
        foreach (var i in items)
        {
            Main.instance.SpawnItem(i, getPosRandom(2, trans));
        }

        for (int i = 0; i < cant; i++)
        {
            GameObject go = Instantiate(model);
            go.transform.position = getPosRandom(10, this.transform);

            if (go.GetComponent<DummyEnemy>()) {

                var enem = go.GetComponent<DummyEnemy>();
            }
        }
    }
}
