using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SoD_BaseMod {
	public class BTAnimationPlayerManager : MonoBehaviour {
		private static readonly Dictionary<string, AnimationState> animationDict = new Dictionary<string, AnimationState>();
		private static GameObject animationPlayerObject;
		private static Dropdown animationDropdown;
		private static InputField frameInputField;
		private static InputField speedInputField;
		private static Slider frameSlider;

		private static GameObject playPauseButtonPlayIcon;
		private static GameObject playPauseButtonPauseIcon;

		private static BTConfigHolder ConfigHolder => BTDebugCamInputManager.GetConfigHolder();

		public static void Initialize(GameObject animationPlayerObject) {
			if (BTAnimationPlayerManager.animationPlayerObject == animationPlayerObject) {
				return;
			}

			BTAnimationPlayerManager.animationPlayerObject = animationPlayerObject;

			GameObject animationLoadButton = BTUIUtils.FindGameObjectAtPath(animationPlayerObject, "LoadAnimsButtonContainer/LoadAnimsButton");
			if (animationLoadButton != null) {
				animationLoadButton.GetComponent<Button>().onClick.AddListener(FindAnimationsClicked);
			}

			GameObject animDropdownObject = BTUIUtils.FindGameObjectAtPath(animationPlayerObject, "AnimSelectorContainer/Dropdown");
			if (animDropdownObject != null) {
				animationDropdown = animDropdownObject.GetComponent<Dropdown>();
				animationDropdown.onValueChanged.AddListener(OnSelectedAnimationChanged);
			}

			GameObject frameInputFieldObject = BTUIUtils.FindGameObjectAtPath(animationPlayerObject, "AnimPlayer/FrameSelector/InputField");
			if (frameInputFieldObject != null) {
				frameInputField = frameInputFieldObject.GetComponent<InputField>();
				frameInputField.onEndEdit.AddListener(OnSelectedFrameChanged);
			}

			GameObject speedInputFieldObject = BTUIUtils.FindGameObjectAtPath(animationPlayerObject, "AnimPlayer/SpeedSelector/InputField");
			if (speedInputFieldObject != null) {
				speedInputField = speedInputFieldObject.GetComponent<InputField>();
				speedInputField.onEndEdit.AddListener(OnSelectedSpeedChanged);
			}

			GameObject frameStepper = BTUIUtils.FindGameObjectAtPath(animationPlayerObject, "AnimPlayer/FrameStepper");
			GameObject playPauseButtonObject = BTUIUtils.FindGameObjectAtPath(frameStepper, "PlayPauseButton");
			playPauseButtonPlayIcon = BTUIUtils.FindGameObjectAtPath(playPauseButtonObject, "PlayImage");
			playPauseButtonPauseIcon = BTUIUtils.FindGameObjectAtPath(playPauseButtonObject, "PauseImage");
			if (playPauseButtonObject != null) {
				playPauseButtonObject.GetComponent<Button>().onClick.AddListener(PlayPauseAnimation);
			}

			GameObject previousFrameButtonObject = BTUIUtils.FindGameObjectAtPath(frameStepper, "PreviousFrameButton");
			if (previousFrameButtonObject != null) {
				previousFrameButtonObject.GetComponent<Button>().onClick.AddListener(OnPreviousFrameButtonClicked);
			}

			GameObject sliderObject = BTUIUtils.FindGameObjectAtPath(frameStepper, "Slider");
			if (sliderObject != null) {
				frameSlider = sliderObject.GetComponent<Slider>();
				frameSlider.onValueChanged.AddListener(OnFrameSliderDragged);
				frameSlider.minValue = 0f;
				frameSlider.maxValue = 1f;
			}

			GameObject nextFrameButtonObject = BTUIUtils.FindGameObjectAtPath(frameStepper, "NextFrameButton");
			if (nextFrameButtonObject != null) {
				nextFrameButtonObject.GetComponent<Button>().onClick.AddListener(OnNextFrameButtonClicked);
			}
		}

		/*============================================================================
		OnClickListeners */

		private static void FindAnimationsClicked() {
			StoreAnimationStates();

			if (animationDropdown == null) {
				return;
			}

			SanctuaryPet pet = SanctuaryManager.pCurPetInstance;
			if (pet == null || pet.animation == null) {
				return;
			}

			animationDropdown.ClearOptions();
			List<string> animOptions = (from AnimationState state in pet.animation select state.name).ToList();
			animationDropdown.AddOptions(animOptions);
			ChangeAnimation(animOptions[0]);
		}

		private static void OnSelectedAnimationChanged(int selectedOption) {
			ChangeAnimation(GetSelectedAnimation());
		}

		private static void OnSelectedFrameChanged(string inputValue) {
			if (frameSlider == null) {
				return;
			}

			GoToAnimationTimestamp(Mathf.Clamp(float.Parse(inputValue, CultureInfo.InvariantCulture), 0f, frameSlider.maxValue));
		}

		private static void OnSelectedSpeedChanged(string inputValue) {
			SetAnimSpeed(Mathf.Max(0f, float.Parse(inputValue, CultureInfo.InvariantCulture)));
		}

		private static void OnPreviousFrameButtonClicked() {
			if (frameSlider == null) {
				return;
			}

			float frameStep = ConfigHolder?.config?.animPlayerTimeStep ?? 1f / 30f;
			GoToAnimationTimestamp(Mathf.Clamp(frameSlider.value - frameStep, 0f, frameSlider.maxValue));
		}

		private static void OnNextFrameButtonClicked() {
			if (frameSlider == null) {
				return;
			}

			float frameStep = ConfigHolder?.config?.animPlayerTimeStep ?? 1f / 30f;
			GoToAnimationTimestamp(Mathf.Clamp(frameSlider.value + frameStep, 0f, frameSlider.maxValue));
		}

		private static void OnFrameSliderDragged(float dragValue) {
			GoToAnimationTimestamp(dragValue);
		}

		/* OnClickListeners
		============================================================================*/

		/*============================================================================
		Access to UI Content */

		private static void ToggleAnimPlayer() {
			animationPlayerObject.SetActive(!animationPlayerObject.activeInHierarchy);
		}

		private static void ShowPauseIcon() {
			if (playPauseButtonPlayIcon == null || playPauseButtonPauseIcon == null) {
				return;
			}

			playPauseButtonPlayIcon.SetActive(false);
			playPauseButtonPauseIcon.SetActive(true);
		}

		private static void ShowPlayIcon() {
			if (playPauseButtonPlayIcon == null || playPauseButtonPauseIcon == null) {
				return;
			}

			playPauseButtonPlayIcon.SetActive(true);
			playPauseButtonPauseIcon.SetActive(false);
		}

		private static string GetSelectedAnimation() {
			if (animationDropdown == null || animationDropdown.options.Count == 0) {
				return null;
			}

			return animationDropdown.options[animationDropdown.value].text;
		}

		private static void SetSliderValue(float val) {
			if (frameSlider == null) {
				return;
			}

			frameSlider.SetValueWithoutNotify(val);
		}

		private static void SetFrameInputText(string text) {
			if (frameInputField == null) {
				return;
			}

			frameInputField.SetTextWithoutNotify(text);
		}

		private static void SetSpeedInputText(string text) {
			if (speedInputField == null) {
				return;
			}

			speedInputField.SetTextWithoutNotify(text);
		}

		private static void UpdateAnimUI() {
			SanctuaryPet pet = SanctuaryManager.pCurPetInstance;
			string targetAnimation = GetSelectedAnimation();
			if (pet == null || targetAnimation == null || pet.animation[targetAnimation] == null) {
				return;
			}

			AnimationState animState = pet.animation[targetAnimation];
			float animProgress = animState.time;
			if (animProgress > animState.length) {
				animProgress %= animState.length;
			}

			SetSliderValue(animProgress);

			if (frameInputField != null && !frameInputField.isFocused) {
				SetFrameInputText(animProgress.ToString(CultureInfo.InvariantCulture));
			}

			if (speedInputField != null && !speedInputField.isFocused) {
				SetSpeedInputText(animState.speed.ToString(CultureInfo.InvariantCulture));
			}
		}

		/* Access to UI Content
		============================================================================*/

		/*============================================================================
		Update */

		public static bool IsActive() {
			return animationPlayerObject != null && animationPlayerObject.activeInHierarchy;
		}

		// ReSharper disable once Unity.IncorrectMethodSignature
		public static void Update() {
			if (animationPlayerObject == null) {
				return;
			}

			if (BTDebugCamInputManager.IsKeyJustDown("ToggleAnimPlayer")) {
				ToggleAnimPlayer();
			}

			if (animationPlayerObject.activeInHierarchy) {
				if (BTDebugCamInputManager.IsKeyJustDown("ToggleAnimPlayerAnimation")) {
					PlayPauseAnimation();
				}

				UpdateAnimUI();
			} else {
				if (animationDict.Count != 0) {
					RestoreAnimationStates();
				}
			}
		}

		/* Update
		============================================================================*/

		/*============================================================================
		Animation Control */

		private static void StoreAnimationStates() {
			SanctuaryPet pet = SanctuaryManager.pCurPetInstance;
			if (pet == null || pet.animation == null) {
				return;
			}

			if (animationDict.Count != 0) {
				RestoreAnimationStates();
			}

			foreach (AnimationState state in pet.animation) {
				var result = new AnimationState();
				AssignAnimationState(state, result);
				animationDict[state.name] = result;
			}
		}

		private static void AssignAnimationState(AnimationState source, AnimationState target) {
			if (source == null || target == null) {
				return;
			}

			target.blendMode = source.blendMode;
			target.enabled = source.enabled;
			target.layer = source.layer;
			target.name = source.name;
			target.normalizedSpeed = source.normalizedSpeed;
			target.normalizedTime = source.normalizedTime;
			target.speed = source.speed;
			target.time = source.time;
			target.weight = source.weight;
			target.wrapMode = source.wrapMode;
		}

		private static void RestoreAnimationStates() {
			SanctuaryPet pet = SanctuaryManager.pCurPetInstance;
			if (pet == null || pet.animation == null) {
				return;
			}

			foreach (KeyValuePair<string, AnimationState> kvp in animationDict) {
				AssignAnimationState(kvp.Value, pet.animation[kvp.Key]);
			}

			animationDict.Clear();
		}

		private static void PlayPauseAnimation() {
			SanctuaryPet pet = SanctuaryManager.pCurPetInstance;
			if (pet == null) {
				return;
			}

			string targetAnimation = GetSelectedAnimation();
			AnimationState animState = pet.animation[targetAnimation];
			if (animState == null) {
				return;
			}

			if (animState.speed == 0f) {
				animState.speed = 1f;
				ShowPauseIcon();
			} else {
				animState.speed = 0f;
				ShowPlayIcon();
			}

			PlayAnim(targetAnimation);
		}

		private static void PlayAnim(string name) {
			SanctuaryPet pet = SanctuaryManager.pCurPetInstance;
			if (pet == null) {
				return;
			}

			if (pet.animation[name] == null) {
				return;
			}

			pet.animation[name].wrapMode = WrapMode.Loop;
			pet.animation.Play(name, PlayMode.StopAll);
		}

		private static void GoToAnimationTimestamp(float time) {
			SanctuaryPet pet = SanctuaryManager.pCurPetInstance;
			if (pet == null) {
				return;
			}

			string targetAnimation = GetSelectedAnimation();
			AnimationState animState = pet.animation[targetAnimation];
			if (animState == null) {
				return;
			}

			animState.time = time;
			PlayAnim(targetAnimation);
		}

		private static void SetAnimSpeed(float speed) {
			SanctuaryPet pet = SanctuaryManager.pCurPetInstance;
			string targetAnimation = GetSelectedAnimation();
			if (pet == null || targetAnimation == null || pet.animation[targetAnimation] == null) {
				return;
			}

			pet.animation[targetAnimation].speed = speed;
			PlayAnim(targetAnimation);

			if (speed > 0f) {
				ShowPauseIcon();
			} else {
				ShowPlayIcon();
			}
		}

		private static void ChangeAnimation(string animationName) {
			SanctuaryPet pet = SanctuaryManager.pCurPetInstance;
			if (pet == null) {
				return;
			}

			AnimationState animState = pet.animation[animationName];
			if (animState == null) {
				return;
			}

			animState.time = 0f;
			frameSlider.maxValue = animState.length;
			if (ConfigHolder?.config != null && ConfigHolder.config.animPlayerPlayByDefault) {
				//if explicitly stated so, play the animation
				animState.speed = 1f;
				ShowPauseIcon();
			} else {
				//otherwise pause it
				animState.speed = 0f;
				ShowPlayIcon();
			}

			PlayAnim(animationName);
		}

		/* Animation Control
		============================================================================*/
	}
}