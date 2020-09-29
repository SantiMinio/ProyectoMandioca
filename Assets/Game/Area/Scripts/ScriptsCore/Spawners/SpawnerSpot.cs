﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnerSpot
{
    public float radious = 4;
    [SerializeField] float heightSpawn = 20;
    [SerializeField] LayerMask mask = 1 << 21;
    public Transform spawnSpot;

    public void Initialize(Transform _spawnSpot = null, float _radious = -1)
    {
        if (_radious > 0) radious = _radious;

        if(_spawnSpot) spawnSpot = _spawnSpot;
    }

    public PlayObject SpawnPrefab(Vector3 pos, ObjectPool_PlayObject _poolObject, CustomSpawner spawner = null)
    {
        if (_poolObject == null) { Debug.LogWarning("!!!! Aca tira un error, el ObjectPool es nulo y no me deja spawnear la mandragoras secundarias, por ahora lo catcheo para que anden las mandragoras normales"); return null; }
        var newObject = _poolObject.Get();
        newObject.Initialize();
        newObject.On();
        newObject.transform.position = pos;
        newObject.Spawner = spawner;
        newObject.Pool = _poolObject;
        newObject.GetComponent<EnemyBase>()?.SpawnEnemy();
        return newObject;
    }

    public Vector3 GetSurfacePos()
    {
        var pos = GetPosRandom(radious, spawnSpot);
        pos.y += heightSpawn;

        RaycastHit hit;

        if (Physics.Raycast(pos, Vector3.down, out hit, Mathf.Infinity, mask, QueryTriggerInteraction.Ignore))
            pos = hit.point;

        return pos;
    }

    Vector3 GetPosRandom(float radio, Transform t)
    {
        Vector3 min = new Vector3(t.position.x - radio, 0, t.position.z - radio);
        Vector3 max = new Vector3(t.position.x + radio, t.position.y, t.position.z + radio);
        return new Vector3(Random.Range(min.x, max.x), t.position.y, Random.Range(min.z, max.z));
    }
}
