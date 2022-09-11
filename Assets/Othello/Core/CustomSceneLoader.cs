using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RangerRPG.Core {
	public class CustomSceneLoader : SingletonBehaviour<CustomSceneLoader> {
		private static List<string> _scenesList;
		private static string _loadingScene = "";
		private static AsyncOperation _asyncLoad;
		private static string _sceneToLoad = "";
		
		public override void Awake() {
			base.Awake();
			if (Instance == null || Instance == this) {
				DontDestroyOnLoad(gameObject);	
			}
			
			_scenesList = new List<string>();
			for (var i = 1; i < SceneManager.sceneCountInBuildSettings; i++) {
				var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
				var lastSlash = scenePath.LastIndexOf("/", StringComparison.Ordinal);
				_scenesList.Add(scenePath.Substring(lastSlash + 1,
					scenePath.LastIndexOf(".", StringComparison.Ordinal) - lastSlash - 1));
			}
			// Log.I($"{scenesList[0]}");

		}
		
		public static void RegisterLoadingScene(string name) {
			if (!_scenesList.Contains(name)) {
				Debug.Log($"[Scene Loader] invalid name provided for the load scene: {name}");
				return;
			}
			_loadingScene = name;
		}

		public static void LoadScene(string sceneName, bool loadAsync = false) {
			if (!_scenesList.Contains(sceneName)) {
				Debug.Log($"[SceneLoader] invalid name provided for loading the scene: {sceneName}");
				return;
			}
			if (_asyncLoad!= null && !_asyncLoad.isDone) {
				Debug.Log($"cannot load scene, another scene being loaded: {_sceneToLoad}");
				return;
			}
			
			// Loading the existing scene
			if (!loadAsync || _loadingScene.Equals("")) {
				SceneManager.LoadScene(sceneName);
				return;
			}
			
			// Load the loading scene
			_sceneToLoad = sceneName;
			SceneManager.LoadScene(_loadingScene);
			Instance.StartCoroutine(LoadSceneAsync());
		}

		public static IEnumerator LoadSceneAsync() {
			yield return null;

			_asyncLoad = SceneManager.LoadSceneAsync(_sceneToLoad);	
			while (!_asyncLoad.isDone) {
				yield return null;
			}
			_asyncLoad = null;
		}

		public static float GetProgress() {
			return _asyncLoad?.progress ?? 1f;
		}

		public static string GetCurrentScene() {
			return _sceneToLoad;
		}

	}
}