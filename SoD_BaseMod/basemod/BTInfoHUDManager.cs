using UnityEngine;
using UnityEngine.UI;

namespace SoD_BaseMod.basemod
{
	public class BTInfoHUDManager
	{
		private static GameObject infoHUDObject = null;
		private static Text fogInfoText = null;
		private static Text skyboxInfoText = null;
		private static Text orthographicInfoText = null;
		private static Text fovInfoText = null;
		private static Text orthographicSizeInfoText = null;

		public static void Initialize(GameObject infoHUDObject) {
			if(BTInfoHUDManager.infoHUDObject == infoHUDObject) {
				return;
			}

			BTInfoHUDManager.infoHUDObject = infoHUDObject;

			GameObject statusContainerObject = BTUIUtils.FindGameObjectAtPath(infoHUDObject, "StatusContainer");
			if(statusContainerObject == null) {
				return;
			}

			GameObject fogStatusTextObject = BTUIUtils.FindGameObjectAtPath(statusContainerObject, "FogStatusContainer/FogStatusText");
			GameObject skyboxStatusTextObject = BTUIUtils.FindGameObjectAtPath(statusContainerObject, "SkyboxStatusContainer/SkyboxStatusText");
			GameObject orthographicStatusTextObject = BTUIUtils.FindGameObjectAtPath(statusContainerObject, "OrthographicStatusContainer/OrthographicStatusText");
			GameObject fovStatusTextObject = BTUIUtils.FindGameObjectAtPath(statusContainerObject, "FOVStatusContainer/FOVStatusText");
			GameObject orthographicSizeStatusTextObject = BTUIUtils.FindGameObjectAtPath(statusContainerObject, "OrthographicSizeStatusContainer/OrthographicSizeStatusText");

			if(fogStatusTextObject != null) {
				fogInfoText = fogStatusTextObject.GetComponent<Text>();
			}
			if(skyboxStatusTextObject != null) {
				skyboxInfoText = skyboxStatusTextObject.GetComponent<Text>();
			}
			if(orthographicStatusTextObject != null) {
				orthographicInfoText = orthographicStatusTextObject.GetComponent<Text>();
			}
			if(fovStatusTextObject != null) {
				fovInfoText = fovStatusTextObject.GetComponent<Text>();
			}
			if(orthographicSizeStatusTextObject != null) {
				orthographicSizeInfoText = orthographicSizeStatusTextObject.GetComponent<Text>();
			}
		}

		public static void SetFogText(string text) {
			if(fogInfoText != null) {
				fogInfoText.text = text;
			}
		}

		public static void SetSkyboxText(string text) {
			if(skyboxInfoText != null) {
				skyboxInfoText.text = text;
			}
		}

		public static void SetOrthographicText(string text) {
			if(orthographicInfoText != null) {
				orthographicInfoText.text = text;
			}
		}

		public static void SetFOVText(string text) {
			if(fovInfoText != null) {
				fovInfoText.text = text;
			}
		}

		public static void SetOrthographicSizeText(string text) {
			if(orthographicSizeInfoText != null) {
				orthographicSizeInfoText.text = text;
			}
		}
	}
}
