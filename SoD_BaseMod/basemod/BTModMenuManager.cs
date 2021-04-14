using UnityEngine;
using UnityEngine.UI;

namespace SoD_BaseMod.basemod
{
	public class BTModMenuManager
	{
		private static GameObject modMenuObject = null;
		private static Text toggleFogButtonText = null;
		private static Text toggleSkyboxButtonText = null;
		private static Text toggleOrthographicButtonText = null;

		public static void Initialize(GameObject modMenuObject) {
			if(BTModMenuManager.modMenuObject == modMenuObject) {
				return;
			}

			BTModMenuManager.modMenuObject = modMenuObject;

			GameObject togglesContainer = BTUIUtils.FindGameObjectAtPath(modMenuObject, "TogglesContainer");
			if(togglesContainer == null) {
				return;
			}

			GameObject toggleFogButton = BTUIUtils.FindGameObjectAtPath(togglesContainer, "ToggleFogContainer/ToggleFogButton");
			GameObject toggleSkyboxButton = BTUIUtils.FindGameObjectAtPath(togglesContainer, "ToggleSkyboxContainer/ToggleSkyboxButton");
			GameObject disableWaterButton = BTUIUtils.FindGameObjectAtPath(togglesContainer, "DisableWaterContainer/DisableWaterButton");
			GameObject toggleOrthographicButton = BTUIUtils.FindGameObjectAtPath(togglesContainer, "ToggleOrthographicContainer/ToggleOrthographicButton");

			if(toggleFogButton != null) {
				toggleFogButton.GetComponent<Button>().onClick.AddListener(ToggleFogClicked);
				GameObject text = BTUIUtils.FindGameObjectAtPath(toggleFogButton, "Text");
				if(text != null) {
					toggleFogButtonText = text.GetComponent<Text>();
				}
			}

			if(toggleSkyboxButton != null) {
				toggleSkyboxButton.GetComponent<Button>().onClick.AddListener(ToggleSkyboxClicked);
				GameObject text = BTUIUtils.FindGameObjectAtPath(toggleSkyboxButton, "Text");
				if(text != null) {
					toggleSkyboxButtonText = text.GetComponent<Text>();
				}
			}

			if(disableWaterButton != null) {
				disableWaterButton.GetComponent<Button>().onClick.AddListener(DisableWaterClicked);
			}

			if(toggleOrthographicButton != null) {
				toggleOrthographicButton.GetComponent<Button>().onClick.AddListener(ToggleOrthographicClicked);
				GameObject text = BTUIUtils.FindGameObjectAtPath(toggleOrthographicButton, "Text");
				if(text != null) {
					toggleOrthographicButtonText = text.GetComponent<Text>();
				}
			}
		}

		private static void ToggleFogClicked() {
			BTDebugCam.ToggleFog();
		}

		private static void ToggleSkyboxClicked() {
			BTDebugCam.ToggleSkybox();
		}

		private static void DisableWaterClicked() {
			BTDebugCam.DisableWater();
		}

		private static void ToggleOrthographicClicked() {
			Camera cam = BTDebugCam.FindMainCamera();
			if(cam != null) {
				BTDebugCam.ToggleOrthographic(cam);
			}
		}

		public static void SetFogText(string text) {
			if(toggleFogButtonText != null) {
				toggleFogButtonText.text = text;
			}
		}

		public static void SetSkyboxText(string text) {
			if(toggleSkyboxButtonText != null) {
				toggleSkyboxButtonText.text = text;
			}
		}

		public static void SetOrthographicText(string text) {
			if(toggleOrthographicButtonText != null) {
				toggleOrthographicButtonText.text = text;
			}
		}
	}
}
