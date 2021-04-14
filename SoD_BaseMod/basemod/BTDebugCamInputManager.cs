using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SoD_BaseMod.basemod
{
	public class BTDebugCamInputManager : MonoBehaviour
	{
		private static readonly BTConfigHolder configHolder = new BTConfigHolder();

		private static float nextConfigCheckIn = 0f;
		private static GameObject targetParent = null;

		private static Canvas canvasComponent = null;
		private static float initialCanvasScale = 0f;
		private static GameObject canvasObject = null;
		private static GameObject modMenuObject = null;
		private static GameObject animPlayerObject = null;
		private static GameObject infoHUDObject = null;
		private static GameObject cutscenePlayerObject = null;

		private static BTConsole console = null;

		public static BTConfigHolder GetConfigHolder() {
			return configHolder;
		}

		public static GameObject GetInfoHUDWindow() {
			return infoHUDObject;
		}

		public static GameObject GetModMenuWindow() {
			return modMenuObject;
		}

		public static GameObject GetAnimPlayerWindow() {
			return animPlayerObject;
		}

		public static void AttachToScene() {
			if(targetParent != null) {
				//already loaded
				return;
			}

			configHolder.LoadConfigs();
			Application.logMessageReceived += configHolder.HandleLog;
			try {
				targetParent = new GameObject("DebugCamGameObject");
				targetParent.AddComponent<BTDebugCamInputManager>();
				targetParent.AddComponent<BTCutsceneManager>();
				console = targetParent.AddComponent<BTConsole>();
				UnityEngine.Object.DontDestroyOnLoad(targetParent);
			} catch(Exception e) {
				configHolder.LogMessage(LogType.Error, "Attaching inputManager failed due to error!\nError: " + e.ToString());
			}

			PrepareUI();
		}

		private static void PrepareUI() {
			if(canvasObject != null) {
				return;
			}

			AssetBundle uiBundle = null;
			try {
				string bundlePath = (BTConfigHolder.basePath + "btuiBundle").Replace('/', Path.DirectorySeparatorChar);
				uiBundle = AssetBundle.LoadFromFile(bundlePath);
				if(uiBundle == null) {
					configHolder.LogMessage(LogType.Error, "failed to load bundleFile at path: " + bundlePath);
					return;
				}

				canvasObject = uiBundle.LoadAsset<GameObject>("Canvas");
				if(canvasObject == null) {
					configHolder.LogMessage(LogType.Error, "failed to load CanvasObject from bundle");
					return;
				}

				canvasObject = UnityEngine.Object.Instantiate<GameObject>(canvasObject);
				if(canvasObject == null) {
					configHolder.LogMessage(LogType.Error, "failed to instantiate CanvasObject");
					return;
				}

				UnityEngine.Object.DontDestroyOnLoad(canvasObject);
				canvasComponent = canvasObject.GetComponent<Canvas>();
				if(canvasComponent == null) {
					configHolder.LogMessage(LogType.Error, "failed to obtain Canvas component!");
					return;
				}
				initialCanvasScale = canvasComponent.scaleFactor;

				modMenuObject = BTUIUtils.PrepareWindow(canvasObject, "ModMenuWindow");
				if(modMenuObject == null) {
					configHolder.LogMessage(LogType.Error, "failed to load modMenuWindow from bundle");
					return;
				}
				BTModMenuManager.Initialize(modMenuObject);

				animPlayerObject = BTUIUtils.PrepareWindow(canvasObject, "AnimPlayerWindow");
				if(animPlayerObject == null) {
					configHolder.LogMessage(LogType.Error, "failed to load animPlayerWindow from bundle");
					return;
				}
				BTAnimationPlayerManager.Initialize(animPlayerObject);

				infoHUDObject = BTUIUtils.PrepareWindow(canvasObject, "InfoHUD");
				if(infoHUDObject == null) {
					configHolder.LogMessage(LogType.Error, "failed to load infoHudWindow from bundle");
					return;
				}
				BTInfoHUDManager.Initialize(infoHUDObject);

				cutscenePlayerObject = BTUIUtils.PrepareWindow(canvasObject, "CutscenePlayerWindow");
				if(cutscenePlayerObject == null) {
					configHolder.LogMessage(LogType.Error, "failed to load cutscenePlayerWindow from bundle");
					return;
				}
				BTCutsceneManager.Initialize(cutscenePlayerObject);
			} catch(Exception e) {
				configHolder.LogMessage(LogType.Error, "caught exception while loading UI: " + e.ToString());
			} finally {
				if(uiBundle != null) {
					uiBundle.Unload(false);
				}
			}
		}

		private void LateUpdate() {
			nextConfigCheckIn -= Time.deltaTime;

			if(nextConfigCheckIn <= 0) {
				ReloadConfig();
			}

			if(IsKeyJustDown("ToggleModMenu")) {
				modMenuObject.SetActive(!modMenuObject.activeSelf);
			}

			if(IsKeyJustDown("ToggleInfoHUD")) {
				infoHUDObject.SetActive(!infoHUDObject.activeSelf);
			}

			if(IsKeyJustDown("PauseAllAnimations")) {
				PauseAllAnimations(true);
			}

			if(IsKeyJustDown("PlayAllAnimations")) {
				PauseAllAnimations(false);
			}

			DoLoadLevelCheck();
			BTDebugCam.Update();
			BTAnimationPlayerManager.Update();
		}

		public static void ReloadConfig() {
			configHolder.LoadConfigs();
			OnConfigUpdate();
			if(configHolder.config != null) {
				nextConfigCheckIn = configHolder.config.fileCheckInterval;
			} else {
				//try loading again in 30 seconds
				nextConfigCheckIn = 30f;
			}
		}

		private static void OnConfigUpdate() {
			if(configHolder.config != null) {
				canvasComponent.scaleFactor = initialCanvasScale * configHolder.config.uiScaleFactor;
			}
		}

		private static void PauseAllAnimations(bool doPause) {
			Animation[] animations = UnityEngine.Object.FindObjectsOfType<Animation>();
			foreach(Animation animation in animations) {
				animation.enabled = !doPause;
			}

			AudioSource[] audioSources = UnityEngine.Object.FindObjectsOfType<AudioSource>();
			foreach(AudioSource audioSource in audioSources) {
				if(doPause) {
					audioSource.Pause();
				} else {
					audioSource.UnPause();
				}
			}

			Time.timeScale = doPause ? 0f : 1f;
		}

		private static void DoLoadLevelCheck() {
			if(configHolder == null || configHolder.config == null || configHolder.config.loadLevelBinds == null) {
				return;
			}

			Dictionary<string, BTLevelConfigEntry> levelBinds = configHolder.config.loadLevelBinds;
			foreach(KeyValuePair<string, BTLevelConfigEntry> kvp in levelBinds) {
				if(kvp.Value.hotkey == null) {
					continue;
				}
				if(!AreKeysJustDown(kvp.Value.hotkey)) {
					continue;
				}

				configHolder.LogMessage(LogType.Log, "loading level: " + kvp.Key);
				if(kvp.Value.isLocalFile) {
					LoadLocalLevel(kvp.Key);
				} else {
					RsResourceManager.LoadLevel(kvp.Key);
				}

				//can only load one level at a time, so might as well stop here.
				break;
			}
		}

		private static void LoadLocalLevel(string bundlePath) {
			AssetBundle bundle = null;
			try {
				bundle = AssetBundle.LoadFromFile(bundlePath);
				if(bundle == null) {
					configHolder.LogMessage(LogType.Error, "failed to load bundleFile at path: " + bundlePath);
					return;
				}

				if(!bundle.isStreamedSceneAssetBundle) {
					configHolder.LogMessage(LogType.Error, "failed to load bundleFile-Scene, incompatible format!");
					return;
				}

				string[] scenePaths = bundle.GetAllScenePaths();
				if(scenePaths.Length <= 0) {
					configHolder.LogMessage(LogType.Error, "failed to load bundleFile-Scene, no scenes found in bundle!");
					return;
				}

				configHolder.LogMessage(LogType.Log, "bundle loaded, available scenes: " + string.Join(", ", scenePaths));
				RsResourceManager.LoadLevel(Path.GetFileNameWithoutExtension(scenePaths[0]));
				//SceneManager.LoadScene(Path.GetFileNameWithoutExtension(scenePaths[0]));
			} catch(Exception e) {
				configHolder.LogMessage(LogType.Error, "caught exception while loading bundleFile-Level: " + e.ToString());
			} finally {
				if(bundle != null) {
					bundle.Unload(false);
				}
			}
		}

		public static bool IsKeyDown(string bindName) {
			if(configHolder == null) {
				return false;
			}

			if(configHolder.hackConfig != null && configHolder.hackConfig.inputBinds != null) {
				if(configHolder.hackConfig.inputBinds.ContainsKey(bindName)) {
					if(AreKeysDown(configHolder.hackConfig.inputBinds[bindName])) {
						return true;
					}
				}
			}

			if(configHolder.config != null && configHolder.config.inputBinds != null) {
				if(configHolder.config.inputBinds.ContainsKey(bindName)) {
					if(AreKeysDown(configHolder.config.inputBinds[bindName])) {
						return true;
					}
				}
			}

			return false;
		}

		public static bool IsKeyJustDown(string bindName) {
			if(configHolder == null) {
				return false;
			}

			if(configHolder.hackConfig != null && configHolder.hackConfig.inputBinds != null) {
				if(configHolder.hackConfig.inputBinds.ContainsKey(bindName)) {
					if(AreKeysJustDown(configHolder.hackConfig.inputBinds[bindName])) {
						return true;
					}
				}
			}

			if(configHolder.config != null && configHolder.config.inputBinds != null) {
				if(configHolder.config.inputBinds.ContainsKey(bindName)) {
					if(AreKeysJustDown(configHolder.config.inputBinds[bindName])) {
						return true;
					}
				}
			}

			return false;
		}

		public static bool AreKeysDown(List<string> keys) {
			bool onlyNullOrWhitespace = true;
			foreach(string key in keys) {
				if(String.IsNullOrWhiteSpace(key)) {
					continue;
				}
				onlyNullOrWhitespace = false;
				if(!IsCustomKeyDown(key)) {
					return false;
				}
			}
			return !onlyNullOrWhitespace;
		}

		public static bool AreKeysJustDown(List<string> keys) {
			if(!AreKeysDown(keys)) {
				return false;
			}
			foreach(string key in keys) {
				if(String.IsNullOrWhiteSpace(key)) {
					continue;
				}
				if(IsCustomKeyJustDown(key)) {
					return true;
				}
			}
			return false;
		}

		public static bool IsCustomKeyDown(string key) {
			if(IsNotBindKey(key)) {
				return !Input.GetKey(NotBindToKeyCode(key));
			} else {
				return Input.GetKey(key);
			}
		}

		public static bool IsCustomKeyJustDown(string key) {
			if(IsNotBindKey(key)) {
				return Input.GetKeyUp(NotBindToKeyCode(key));
			} else {
				return Input.GetKeyDown(key);
			}
		}

		private static bool IsNotBindKey(string key) {
			if(String.IsNullOrWhiteSpace(key)) {
				return false;
			}
			return key.StartsWith("!") && key.Length > 1;
		}

		private static string NotBindToKeyCode(string key) {
			return key.Substring(1);
		}
	}
}
