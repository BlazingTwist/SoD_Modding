using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SoD_BaseMod.config;
using UnityEngine;
using UnityEngine.UI;

namespace SoD_BaseMod {
	public class BTCutsceneManager : MonoBehaviour {
		private static GameObject cutscenePlayerObject;
		private static Toggle unlockCameraMovementToggle;
		private static Dropdown cutsceneDropdown;
		private static InputField timestampInputField;
		private static InputField speedInputField;
		private static GameObject playPauseButtonPlayIcon;
		private static GameObject playPauseButtonPauseIcon;
		private static Slider frameSlider;

		private static GameObject currentCutsceneObject;
		private static KAMonoBase cutsceneAnimationContainer;

		private static Camera cameraOverrideCam;
		private static Camera previousUICamera;
		private static Camera previousCameraBeforeOverride;

		private static BTCutsceneManager instance;

		private static BTConfigHolder ConfigHolder => BTDebugCamInputManager.GetConfigHolder();

		private void Awake() {
			if (instance != null) {
				Destroy(instance);
			}

			instance = this;
		}

		private static void DeleteCurrentCutsceneObject() {
			SetAnimationSpeed(0f);
			GoToCutsceneTimestamp(0f);
			if (currentCutsceneObject != null) {
				var animController = currentCutsceneObject.GetComponentInChildren<CoAnimController>();
				if (animController != null) {
					animController.CutSceneDone();
				}

				Destroy(currentCutsceneObject);
				currentCutsceneObject = null;
			}

			cutsceneAnimationContainer = null;
		}

		public static void Initialize(GameObject cutscenePlayerObject) {
			if (BTCutsceneManager.cutscenePlayerObject == cutscenePlayerObject) {
				BTConfigHolder.LogMessage(LogType.Error, "BTCutsceneManager got initialized with null cutscenePlayerWindow!");
				return;
			}

			BTCutsceneManager.cutscenePlayerObject = cutscenePlayerObject;

			GameObject closeButtonObject = BTUIUtils.FindGameObjectAtPath(cutscenePlayerObject, "Title/CloseButton");
			if (closeButtonObject != null) {
				closeButtonObject.GetComponent<Button>().onClick.AddListener(OnCloseButtonClicked);
			} else {
				BTConfigHolder.LogMessage(LogType.Error, "BTCutsceneManager could not initialize UIComponent Title/CloseButton from bundle");
			}

			GameObject contentContainerObject = BTUIUtils.FindGameObjectAtPath(cutscenePlayerObject, "ContentContainer");
			if (contentContainerObject == null) {
				BTConfigHolder.LogMessage(LogType.Error, "BTCutsceneManager could not initialize UIComponent ContentContainer from bundle");
			}

			GameObject unlockCameraMovementObject = BTUIUtils.FindGameObjectAtPath(contentContainerObject, "FreeCameraTickboxContainer/Toggle");
			if (unlockCameraMovementObject != null) {
				unlockCameraMovementToggle = unlockCameraMovementObject.GetComponent<Toggle>();
			} else {
				BTConfigHolder.LogMessage(LogType.Error, "BTCutsceneManager could not initialize UIComponent FreeCameraTickboxContainer/Toggle from bundle");
			}

			GameObject cutsceneDropdownObject = BTUIUtils.FindGameObjectAtPath(contentContainerObject, "CutsceneSelectorContainer/Dropdown");
			if (cutsceneDropdownObject != null) {
				cutsceneDropdown = cutsceneDropdownObject.GetComponent<Dropdown>();
				cutsceneDropdown.onValueChanged.AddListener(OnCutsceneSelectionChanged);
			} else {
				BTConfigHolder.LogMessage(LogType.Error, "BTCutsceneManager could not initialize UIComponent CutsceneSelectorContainer/Dropdown from bundle");
			}

			GameObject timestampInputfieldObject = BTUIUtils.FindGameObjectAtPath(contentContainerObject, "TimestampInputContainer/InputField");
			if (timestampInputfieldObject != null) {
				timestampInputField = timestampInputfieldObject.GetComponent<InputField>();
				timestampInputField.onEndEdit.AddListener(OnTimestampInputChanged);
			} else {
				BTConfigHolder.LogMessage(LogType.Error, "BTCutsceneManager could not initialize UIComponent TimestampInputContainer/InputField from bundle");
			}

			GameObject currentSpeedInputfieldObject = BTUIUtils.FindGameObjectAtPath(contentContainerObject, "CurrentSpeedInputContainer/InputField");
			if (currentSpeedInputfieldObject != null) {
				speedInputField = currentSpeedInputfieldObject.GetComponent<InputField>();
				speedInputField.onEndEdit.AddListener(OnSpeedInputChanged);
			} else {
				BTConfigHolder.LogMessage(LogType.Error,
						"BTCutsceneManager could not initialize UIComponent CurrentSpeedInputContainer/InputField from bundle");
			}

			GameObject cutsceneControllerContainerObject = BTUIUtils.FindGameObjectAtPath(contentContainerObject, "CutsceneControllerContainer");
			if (cutsceneControllerContainerObject == null) {
				BTConfigHolder.LogMessage(LogType.Error, "BTCutsceneManager could not initialize UIComponent CutsceneControllerContainer from bundle");
			}

			GameObject playPauseButtonObject = BTUIUtils.FindGameObjectAtPath(cutsceneControllerContainerObject, "PlayPauseButton");
			playPauseButtonPlayIcon = BTUIUtils.FindGameObjectAtPath(playPauseButtonObject, "PlayImage");
			playPauseButtonPauseIcon = BTUIUtils.FindGameObjectAtPath(playPauseButtonObject, "PauseImage");
			if (playPauseButtonObject != null) {
				playPauseButtonObject.GetComponent<Button>().onClick.AddListener(OnPlayPauseButtonClicked);
			} else {
				BTConfigHolder.LogMessage(LogType.Error, "BTCutsceneManager could not initialize UIComponent PlayPauseButton from bundle");
			}

			GameObject previousFrameButtonObject = BTUIUtils.FindGameObjectAtPath(cutsceneControllerContainerObject, "PreviousFrameButton");
			if (previousFrameButtonObject != null) {
				previousFrameButtonObject.GetComponent<Button>().onClick.AddListener(OnPreviousFrameButtonClicked);
			} else {
				BTConfigHolder.LogMessage(LogType.Error, "BTCutsceneManager could not initialize UIComponent PreviousFrameButton from bundle");
			}

			GameObject nextFrameButtonObject = BTUIUtils.FindGameObjectAtPath(cutsceneControllerContainerObject, "NextFrameButton");
			if (nextFrameButtonObject != null) {
				nextFrameButtonObject.GetComponent<Button>().onClick.AddListener(OnNextFrameButtonClicked);
			} else {
				BTConfigHolder.LogMessage(LogType.Error, "BTCutsceneManager could not initialize UIComponent NextFrameButton from bundle");
			}

			GameObject frameSliderObject = BTUIUtils.FindGameObjectAtPath(cutsceneControllerContainerObject, "Slider");
			if (frameSliderObject != null) {
				frameSlider = frameSliderObject.GetComponent<Slider>();
				frameSlider.onValueChanged.AddListener(OnFrameSliderValueChanged);
				frameSlider.minValue = 0f;
				frameSlider.maxValue = 1f;
			} else {
				BTConfigHolder.LogMessage(LogType.Error, "BTCutsceneManager could not initialize UIComponent Slider from bundle");
			}
		}

		/*============================================================================
		OnClickListeners */

		private static void OnCloseButtonClicked() {
			ChangeWindowVisibility(false);
		}

		private static void OnCutsceneSelectionChanged(int selectedOption) {
			instance.StartCoroutine(DoDelayedCutsceneLoad());
		}

		private static IEnumerator DoDelayedCutsceneLoad() {
			SetAnimationSpeed(0f);
			GoToCutsceneTimestamp(0f);
			yield return new WaitForSecondsRealtime(0.1f);
			LoadCutscene(GetSelectedCutscene());
		}

		private static void OnTimestampInputChanged(string inputValue) {
			if (frameSlider == null) {
				return;
			}

			GoToCutsceneTimestamp(Mathf.Clamp(float.Parse(inputValue, CultureInfo.InvariantCulture), 0f, frameSlider.maxValue));
		}

		private static void OnSpeedInputChanged(string inputValue) {
			SetAnimationSpeed(Mathf.Max(0f, float.Parse(inputValue, CultureInfo.InvariantCulture)));
		}

		private static void OnPlayPauseButtonClicked() {
			TogglePlayCutscene();
		}

		private static void OnPreviousFrameButtonClicked() {
			if (frameSlider == null) {
				return;
			}

			float frameStep = ConfigHolder?.config?.cutscenePlayerTimeStep ?? 1f / 30f;
			GoToCutsceneTimestamp(Mathf.Clamp(frameSlider.value - frameStep, 0f, frameSlider.maxValue));
		}

		private static void OnNextFrameButtonClicked() {
			if (frameSlider == null) {
				return;
			}

			float frameStep = ConfigHolder?.config?.cutscenePlayerTimeStep ?? 1f / 30f;
			GoToCutsceneTimestamp(Mathf.Clamp(frameSlider.value + frameStep, 0f, frameSlider.maxValue));
		}

		private static void OnFrameSliderValueChanged(float dragValue) {
			GoToCutsceneTimestamp(dragValue);
		}

		/* OnClickListeners
		============================================================================*/

		/*============================================================================
		Access to UI Content */

		private static void ChangeWindowVisibility(bool visibility) {
			cutscenePlayerObject.SetActive(visibility);
		}

		private static void ToggleWindowVisibility() {
			ChangeWindowVisibility(!cutscenePlayerObject.activeInHierarchy);
		}

		private static void ShowPlayPauseIcon(bool showPlay) {
			if (playPauseButtonPauseIcon == null || playPauseButtonPlayIcon == null) {
				return;
			}

			playPauseButtonPlayIcon.SetActive(showPlay);
			playPauseButtonPauseIcon.SetActive(!showPlay);
		}

		private static void SetSliderValue(float value, bool notify) {
			if (frameSlider == null) {
				return;
			}

			if (notify) {
				frameSlider.value = value;
			} else {
				frameSlider.SetValueWithoutNotify(value);
			}
		}

		private static void SetTimestampInputText(string text, bool notify, bool force) {
			if (timestampInputField == null
					|| timestampInputField.isFocused && !force) {
				return;
			}

			if (notify) {
				timestampInputField.text = text;
			} else {
				timestampInputField.SetTextWithoutNotify(text);
			}
		}

		private static void SetSpeedInputText(string text, bool notify, bool force) {
			if (speedInputField == null 
					|| speedInputField.isFocused && !force) {
				return;
			}

			if (notify) {
				speedInputField.text = text;
			} else {
				speedInputField.SetTextWithoutNotify(text);
			}
		}

		private static void UpdateProgressUI() {
			if (cutsceneAnimationContainer == null || cutsceneAnimationContainer.animation == null) {
				return;
			}

			AnimationState firstAnimState = cutsceneAnimationContainer.animation.Cast<AnimationState>().FirstOrDefault();

			if (!(firstAnimState is null)) {
				SetSliderValue(firstAnimState.time, false);
				SetTimestampInputText(firstAnimState.time.ToString(CultureInfo.InvariantCulture), false, false);
				SetSpeedInputText(firstAnimState.speed.ToString(CultureInfo.InvariantCulture), false, false);
			}
		}

		/* Access to UI Content
		============================================================================*/

		/*============================================================================
		Access from Keybinds */

		private void LateUpdate() {
			if (cutscenePlayerObject == null) {
				return;
			}

			UpdateAvailableCutscenes();
			UpdateProgressUI();

			if (BTDebugCamInputManager.IsKeyJustDown("ToggleCutscenePlayer")) {
				ToggleWindowVisibility();
			}

			if (BTDebugCamInputManager.IsKeyJustDown("ToggleCutscenePlayerAnimation")) {
				TogglePlayCutscene();
			}

			if (cutscenePlayerObject.activeInHierarchy && unlockCameraMovementToggle.isOn
					|| currentCutsceneObject != null && cameraOverrideCam != null) {
				if (cameraOverrideCam == null) {
					// ReSharper disable once Unity.PerformanceCriticalCodeCameraMain
					previousCameraBeforeOverride = Camera.main;
					previousUICamera = Camera.current;
					if (!(previousCameraBeforeOverride is null)) {
						previousCameraBeforeOverride.enabled = false;
						previousUICamera.enabled = false;
						cameraOverrideCam = gameObject.AddComponent<Camera>();
						cameraOverrideCam.cullingMask = previousCameraBeforeOverride.cullingMask;
					}

					BTDebugCam.SetMainCamera(cameraOverrideCam);
				}

				foreach (Camera cam in FindObjectsOfType<Camera>()) {
					if (cam != cameraOverrideCam) {
						cam.enabled = false;
					}
				}
			} else {
				if (cameraOverrideCam != null) {
					BTDebugCam.RemoveMainCamera(cameraOverrideCam);
					Destroy(cameraOverrideCam);
					cameraOverrideCam = null;
					previousCameraBeforeOverride.enabled = true;
					previousCameraBeforeOverride = null;
					previousUICamera.enabled = true;
					previousUICamera = null;
				}
			}
		}

		/* Access from Keybinds
		============================================================================*/

		/*============================================================================
		Config Management */

		private static void UpdateAvailableCutscenes() {
			if (ConfigHolder.config?.availableCutscenes == null) {
				BTConfigHolder.LogMessage(LogType.Error, "could not find availableCutscenes in config!");
				return;
			}

			if (cutsceneDropdown == null) {
				BTConfigHolder.LogMessage(LogType.Error, "could not update available cutscenes, missing cutsceneDropdown from bundle.");
				return;
			}

			List<string> cutsceneOptions = new List<string> {
					"None" //require first `None` element to avoid having to load cutscenes on startup
			};
			cutsceneOptions.AddRange(ConfigHolder.config.availableCutscenes.Select(kvp => kvp.Key));

			if (cutsceneOptions.Count != cutsceneDropdown.options.Count
					|| cutsceneDropdown.options.Any(entry => !cutsceneOptions.Contains(entry.text))) {
				cutsceneDropdown.ClearOptions();
				cutsceneDropdown.AddOptions(cutsceneOptions);
				cutsceneDropdown.value = 0; // set cutscene to None and send unload-Notify
			}
		}

		private static BTCutsceneConfigEntry GetSelectedCutscene() {
			if (cutsceneDropdown == null || cutsceneDropdown.options.Count <= 1 || ConfigHolder.config?.availableCutscenes == null ||
					ConfigHolder.config.availableCutscenes.Count == 0) {
				return null;
			}

			return cutsceneDropdown.value == 0
					? null
					: ConfigHolder.config.availableCutscenes[cutsceneDropdown.options[cutsceneDropdown.value].text];
		}

		/* ConfigManagement
		============================================================================*/

		/*============================================================================
		Cutscene Control */

		private static void LoadCutscene(BTCutsceneConfigEntry targetCutscene) {
			DeleteCurrentCutsceneObject();

			if (targetCutscene == null) {
				return;
			}

			if (targetCutscene.isLocalFile) {
				AssetBundle assetBundle = AssetBundle.LoadFromFile(targetCutscene.resourcePath);
				try {
					if (assetBundle == null) {
						BTConfigHolder.LogMessage(LogType.Log, "Unable to load cutsceneBundle from path: " + targetCutscene.resourcePath);
						return;
					}

					var uninstantiatedCutsceneObject = assetBundle.LoadAsset<GameObject>(targetCutscene.assetName);
					if (uninstantiatedCutsceneObject == null) {
						BTConfigHolder.LogMessage(LogType.Log, "Unable to find GameObject (" + targetCutscene.assetName + ") in loaded assetBundle");
						return;
					}

					CutsceneLoadDone(uninstantiatedCutsceneObject);
				} finally {
					if (assetBundle != null) {
						assetBundle.Unload(false);
					}
				}
			} else {
				RsResourceManager.LoadAssetFromBundle(
						targetCutscene.resourcePath,
						targetCutscene.assetName,
						CutsceneLoadEvent,
						typeof(GameObject),
						false,
						false
				);
			}
		}

		private static void CutsceneLoadEvent(string inURL, RsResourceLoadEvent inLoadEvent, float inProgress, object inObject, object inUserData) {
			if (inLoadEvent != RsResourceLoadEvent.COMPLETE) {
				return;
			}

			KAUICursorManager.SetDefaultCursor("Arrow");
			if (inObject == null || (bool) inUserData) {
				BTConfigHolder.LogMessage(LogType.Warning, "CutsceneLoadEvent yielded null object! (that's bad)");
				return;
			}

			CutsceneLoadDone((GameObject) inObject);
		}

		private static void CutsceneLoadDone(GameObject loadedCutsceneObject) {
			try {
				currentCutsceneObject = Instantiate(loadedCutsceneObject);
				if (currentCutsceneObject == null) {
					BTConfigHolder.LogMessage(LogType.Warning, "Instantiation of GameObject failed!");
					return;
				}

				currentCutsceneObject.name = loadedCutsceneObject.name;
				var animationController = currentCutsceneObject.GetComponentInChildren<CoAnimController>();
				cutsceneAnimationContainer = animationController;

				GoToCutsceneTimestamp(0f);

				if (ConfigHolder.config == null || !ConfigHolder.config.cutscenePlayerPlayByDefault) {
					SetAnimationSpeed(0f);
				}

				if (cutsceneAnimationContainer != null && cutsceneAnimationContainer.animation != null && frameSlider != null) {
					foreach (AnimationState animState in cutsceneAnimationContainer.animation) {
						frameSlider.maxValue = animState.length;
						break;
					}

					/*foreach(Animation anim in cutsceneAnimationContainer.transform.GetComponentsInChildren<Animation>()){
						configHolder.LogInfo("animationContainer: " + anim.gameObject.name);
						foreach(AnimationState animState in anim){
							configHolder.LogInfo("\tanimationState: " + animState.name);
							foreach(AnimationEvent animEvent in animState.clip.events){
								configHolder.LogInfo("\t\t" + AnimationEventToString(animEvent));
							}
						}
					}*/

					/*foreach(AQUAS_Camera cam in cutsceneAnimationContainer.transform.GetComponentsInChildren<AQUAS_Camera>()){
						if(cam.gameObject.activeInHierarchy){
							configHolder.LogInfo("active Camera: " + cam.name);
						}else{
							configHolder.LogInfo("inactive Camera: " + cam.name);
						}
					}*/
				}

				if (animationController == null) {
					BTConfigHolder.LogMessage(LogType.Warning, "Could not find AnimationController in loaded cutscene, not sure why (that's bad)");
				} else {
					animationController._MessageObject = instance.gameObject;
					BTConfigHolder.LogMessage(LogType.Log, "Everything seems fine, cutscene should be loaded.");
					animationController.CutSceneStart();
				}
			} catch (Exception e) {
				BTConfigHolder.LogMessage(LogType.Error, "caught unexpected exception: " + e);
			}
		}

		/*private static string AnimationEventToString(AnimationEvent animEvent){
			return "animationEvent: time = " + animEvent.time + " | " + animEvent.functionName + "(int " + animEvent.intParameter + ", float " + animEvent.floatParameter + ", string " + animEvent.stringParameter + ", object " + animEvent.objectReferenceParameter + ")";
		}*/

		private static void GoToCutsceneTimestamp(float time) {
			if (cutsceneAnimationContainer == null || cutsceneAnimationContainer.animation == null) {
				return;
			}

			foreach (AnimationState animState in cutsceneAnimationContainer.animation) {
				animState.time = time;
			}
		}

		private static void SetAnimationSpeed(float speed) {
			if (cutsceneAnimationContainer == null) {
				return;
			}

			/*foreach(AnimationState animState in cutsceneAnimationContainer.animation){
				animState.speed = speed;
			}*/

			foreach (Animation anim in cutsceneAnimationContainer.transform.GetComponentsInChildren<Animation>()) {
				foreach (AnimationState animState in anim) {
					animState.speed = speed;
				}
			}

			ShowPlayPauseIcon(speed == 0f);
		}

		private static void TogglePlayCutscene() {
			if (cutsceneAnimationContainer == null || cutsceneAnimationContainer.animation == null) {
				return;
			}

			bool allSpeedZero = true;
			foreach (Animation anim in cutsceneAnimationContainer.transform.GetComponentsInChildren<Animation>()) {
				if (anim.Cast<AnimationState>().Any(animState => animState.speed != 0f)) {
					allSpeedZero = false;
				}
			}
			/*foreach(AnimationState animState in cutsceneAnimationContainer.animation){
				if(animState.speed != 0f){
					allSpeedZero = false;
					break;
				}
			}*/

			SetAnimationSpeed(allSpeedZero ? 1f : 0f);
		}

		/* Cutscene Control
		============================================================================*/

		//Beware: predefined naming by JS-Games, triggered via SendMessage
		public void OnCutSceneDone() {
			if (currentCutsceneObject == null) {
				BTConfigHolder.LogMessage(LogType.Log, "Received CutSceneDone event, but no reason why, no cutscene was loaded");
				return;
			}

			BTConfigHolder.LogMessage(LogType.Log, "Received CutSceneDone event for loaded cutscene");
			DeleteCurrentCutsceneObject();
		}
	}
}