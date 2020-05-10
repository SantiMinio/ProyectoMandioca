﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;


namespace DevelopTools
{
    public abstract class SingleObjectPool<T> : MonoBehaviour where T : Component
    {
        /// <summary>
        /// Prefab que se quiere poolear
        /// </summary>
        [Header("Si es un sonido, dejar vacio el prefab")]
        [SerializeField] protected T prefab = null;
        /// <summary>
        /// Instancia del pool
        /// </summary>
        ///
        public static SingleObjectPool<T> Instance { get; private set; }
        /// <summary>
        /// Cola donde se guardan los objetos pooleados
        /// </summary>
        protected Queue<T> objects = new Queue<T>();

        [Header("-----8888888-------")]
        [SerializeField] private int prewarmAmount = 5;

        private void Awake()
        {
            Instance = this;
        }

        protected virtual void Start()
        {
            PreWarm(prewarmAmount);
        }

        /// <summary>
        /// Le pido un objeto del pool y lo prendo.
        /// Si no tengo ninguno, creo uno y lo doy
        /// </summary>
        /// <returns></returns>
        public T Get()
        {
            if (objects.Count == 0)
            {
                AddObject(1);
            }

            var obj = objects.Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        }

        /// <summary>
        /// Crea una cantidad de objetos antes de arrancar
        /// </summary>
        public virtual void PreWarm(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                AddObject(1);
            }
        }

        /// <summary>
        /// Devuelvo el objeto al pool
        /// </summary>
        /// <param name="objectToReturn"></param>
        public void ReturnToPool(T objectToReturn)
        {
            objectToReturn.gameObject.SetActive(false);
            objects.Enqueue(objectToReturn);
        }
        /// <summary>
        /// Creo un objeto del prefab y lo agrego al pool previo apagarlo
        /// </summary>
        /// <param name="amount"></param>
        protected virtual void AddObject(int amount)
        {
            var newObject = GameObject.Instantiate(prefab,transform);
            newObject.gameObject.SetActive(false);
            objects.Enqueue(newObject);
        }
    }    

}
