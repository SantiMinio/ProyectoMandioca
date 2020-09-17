﻿using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
namespace PixelCrushers.SceneStreamer {
    [AddComponentMenu("Scene Streamer/Scene Streamer")]
    public class SceneStreamer : MonoBehaviour {
        [Tooltip("Max number of neighbors to load out from the current scene.")]
        public int maxNeighborDistance = 1;
        [Tooltip("(Failsafe) If scene doesn't load after this many seconds, stop waiting.")]
        public float maxLoadWaitTime = 10f;
        [System.Serializable]
        public class StringEvent : UnityEvent<string> { }
        [System.Serializable]
        public class StringAsyncEvent : UnityEvent<string, AsyncOperation> { }
        public StringAsyncEvent onLoading = new StringAsyncEvent();
        public StringEvent onLoaded = new StringEvent();
        private delegate void InternalLoadedHandler(string sceneName, int distance);
        [Tooltip("Tick to log debug info to the Console window.")]
        public bool debug = false;
        public bool logDebugInfo { get { return debug && Debug.isDebugBuild; } }
        private string m_currentSceneName = null;
        private HashSet<string> m_loaded = new HashSet<string>();
        private HashSet<string> m_loading = new HashSet<string>();
        private HashSet<string> m_near = new HashSet<string>();
        private static object s_lock = new object();
        private static SceneStreamer s_instance = null;
        private static SceneStreamer instance {
            get {
                lock (s_lock) {
                    if (s_instance == null) {
                        s_instance = FindObjectOfType(typeof(SceneStreamer)) as SceneStreamer;
                        if (s_instance == null) s_instance = new GameObject("Scene Loader", typeof(SceneStreamer)).GetComponent<SceneStreamer>();
                    }
                    return s_instance;
                }
            }
            set { s_instance = value; }
        }
        public void Awake() {
            if (s_instance) Destroy(this);
            else { s_instance = this; Object.DontDestroyOnLoad(this.gameObject); }
        }
        public void SetCurrent(string sceneName) {
            if (string.IsNullOrEmpty(sceneName) || string.Equals(sceneName, m_currentSceneName)) return;
            if (logDebugInfo) Debug.Log("Scene Streamer: Setting current scene to " + sceneName + ".");
            StartCoroutine(LoadCurrentScene(sceneName));
        }
        private IEnumerator LoadCurrentScene(string sceneName) {
            m_currentSceneName = sceneName;
            if (!IsLoaded(m_currentSceneName)) Load(sceneName);
            float failsafeTime = Time.realtimeSinceStartup + maxLoadWaitTime;
            while ((m_loading.Count > 0) && (Time.realtimeSinceStartup < failsafeTime)) { yield return null; }
            if (Time.realtimeSinceStartup >= failsafeTime && Debug.isDebugBuild) Debug.LogWarning("Scene Streamer: Timed out waiting to load " + sceneName + ".");
            if (logDebugInfo) Debug.Log("Scene Streamer: Loading " + maxNeighborDistance + " closest neighbors of " + sceneName + ".");
            m_near.Clear();
            LoadNeighbors(sceneName, 0);
            failsafeTime = Time.realtimeSinceStartup + maxLoadWaitTime;
            while ((m_loading.Count > 0) && (Time.realtimeSinceStartup < failsafeTime)) yield return null;
            if (Time.realtimeSinceStartup >= failsafeTime && Debug.isDebugBuild) Debug.LogWarning("Scene Streamer: Timed out waiting to load neighbors of " + sceneName + ".");
            UnloadFarScenes();// Finally unload any scenes not in the near list:
        }
        private void LoadNeighbors(string sceneName, int distance) {
            if (m_near.Contains(sceneName)) return;
            m_near.Add(sceneName);
            if (distance >= maxNeighborDistance) return;
            GameObject scene = GameObject.Find(sceneName);
            NeighboringScenes neighboringScenes = (scene) ? scene.GetComponent<NeighboringScenes>() : null;
            if (!neighboringScenes) neighboringScenes = CreateNeighboringScenesList(scene);
            if (!neighboringScenes) return;
            for (int i = 0; i < neighboringScenes.sceneNames.Length; i++)
            {
                Load(neighboringScenes.sceneNames[i], LoadNeighbors, distance + 1);
            }
        }
        private NeighboringScenes CreateNeighboringScenesList(GameObject scene) {
            if (!scene) return null;
            NeighboringScenes neighboringScenes = scene.AddComponent<NeighboringScenes>();
            HashSet<string> neighbors = new HashSet<string>();
            var sceneEdges = scene.GetComponentsInChildren<SceneEdge>();
            for (int i = 0; i < sceneEdges.Length; i++) neighbors.Add(sceneEdges[i].nextSceneName);
            neighboringScenes.sceneNames = new string[neighbors.Count];
            neighbors.CopyTo(neighboringScenes.sceneNames);
            return neighboringScenes;
        }
        public bool IsLoaded(string sceneName) => m_loaded.Contains(sceneName);
        public void Load(string sceneName) => Load(m_currentSceneName, null, 0);
        private void Load(string sceneName, InternalLoadedHandler loadedHandler, int distance) {
            if (IsLoaded(sceneName)) {
                if (loadedHandler != null) loadedHandler(sceneName, distance);
                return;
            }
            m_loading.Add(sceneName);
            StartCoroutine(LoadAdditiveAsync(sceneName, loadedHandler, distance));
        }
        private IEnumerator LoadAdditiveAsync(string sceneName, InternalLoadedHandler loadedHandler, int distance) {
            if (AlreadyExist(sceneName)) yield break;
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            onLoading.Invoke(sceneName, asyncOperation);
            yield return asyncOperation;
            FinishLoad(sceneName, loadedHandler, distance);
        }
        private IEnumerator LoadAdditive(string sceneName, InternalLoadedHandler loadedHandler, int distance) {
            if (AlreadyExist(sceneName)) yield break;
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            onLoading.Invoke(sceneName, null);
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            FinishLoad(sceneName, loadedHandler, distance);
        }
        private void FinishLoad(string sceneName, InternalLoadedHandler loadedHandler, int distance) {
            GameObject scene = GameObject.Find(sceneName);
            m_loading.Remove(sceneName);
            m_loaded.Add(sceneName);
            onLoaded.Invoke(sceneName);
            if (loadedHandler != null) loadedHandler(sceneName, distance);
        }
        private void UnloadFarScenes() {
            HashSet<string> hash_far = new HashSet<string>(m_loaded);
            hash_far.ExceptWith(m_near);
            foreach (var sceneName in hash_far) Unload(sceneName);
        }
        public void Unload(string sceneName) {
            Destroy(GameObject.Find(sceneName));
            m_loaded.Remove(sceneName);
            SceneManager.UnloadSceneAsync(sceneName);
        }
        public static void SetCurrentScene(string sceneName) => instance.SetCurrent(sceneName);
        public static bool IsSceneLoaded(string sceneName) => instance.IsLoaded(sceneName);
        public static void LoadScene(string sceneName) => instance.Load(sceneName);
        public static void UnloadScene(string sceneName)=>instance.Unload(sceneName);
        bool AlreadyExist(string scene)
        {
            bool exist = false;
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).name == scene) exist = true;
            }
            return exist;
        }
    }
}