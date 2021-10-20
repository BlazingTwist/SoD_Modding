using System;
using System.Collections.Generic;
using System.Globalization;
using SoD_BaseMod.config.toggleScript;
using SoD_BaseMod.console;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SoD_BaseMod {
	public static class BTDebugCam {
		private static Material prevSkybox;
		private static readonly List<int> toggledUIElements = new List<int>();

		public static bool useDebugCam;
		private static Vector2 mMousePrev;

		private static float camRotationX;
		private static float camRotationY;

		private static Vector3 initialPosition;
		private static Quaternion initialRotation;
		private static float initialFOV;
		private static float initialOrthographicSize;
		private static float initialFarClipPlane;
		private static float initialDetailObjectDistance;
		private static float initialTreeDistance;
		private static float initialTreeBillboardDistance;

		private static Camera overrideCamera;

		private static BTConfigHolder ConfigHolder => BTDebugCamInputManager.GetConfigHolder();

		public static void SetMainCamera(Camera targetCam) {
			overrideCamera = targetCam;
		}

		public static void RemoveMainCamera(Camera targetCam) {
			if (targetCam == overrideCamera) {
				overrideCamera = null;
			}
		}

		public static Camera FindMainCamera() {
			if (overrideCamera != null) {
				return overrideCamera;
			}

			// ReSharper disable once Unity.PerformanceCriticalCodeCameraMain
			Camera camera = Camera.main;
			if (camera == null) {
				var aquasCam = Object.FindObjectOfType<AQUAS_Camera>();
				if (aquasCam != null) {
					camera = aquasCam.GetComponent<Camera>();
				}
			}

			return camera;
		}

		private static void ResetFOV(Camera camera) {
			if (ConfigHolder == null) {
				return;
			}

			if (useDebugCam) {
				SetFOV(camera, ConfigHolder.config.cameraFOV);
				SetOrthographicSize(camera, ConfigHolder.config.orthographicSize);
			} else {
				SetFOV(camera, initialFOV);
				SetOrthographicSize(camera, initialOrthographicSize);
			}
		}

		private static void ToggleDebugCam(Camera camera) {
			if (ConfigHolder == null) {
				return;
			}

			useDebugCam = !useDebugCam;
			if (useDebugCam) {
				//store current values
				Transform transform = camera.transform;
				Vector3 position = transform.position;
				Quaternion rotation = transform.rotation;
				initialPosition = new Vector3(position.x, position.y, position.z);
				initialRotation = new Quaternion(rotation.x, rotation.y, rotation.z, rotation.w);
				initialFOV = camera.fieldOfView;
				initialOrthographicSize = camera.orthographicSize;
				initialFarClipPlane = camera.farClipPlane;

				//set new values
				ResetFOV(camera);
				camera.farClipPlane = ConfigHolder.config.cameraRenderDistance;

				//set terrain values
				var terrain = (Terrain) Object.FindObjectOfType(typeof(Terrain));
				if (terrain != null) {
					initialDetailObjectDistance = terrain.detailObjectDistance;
					initialTreeDistance = terrain.treeDistance;
					initialTreeBillboardDistance = terrain.treeBillboardDistance;
					terrain.detailObjectDistance = ConfigHolder.config.cameraRenderDistance;
					terrain.treeDistance = ConfigHolder.config.cameraRenderDistance;
					terrain.treeBillboardDistance = ConfigHolder.config.cameraRenderDistance;
					float[] newHeightError = new float[terrain.terrainData.GetMaximumHeightError().Length];
					for (int i = 0; i < newHeightError.Length; i++) {
						newHeightError[i] = 1f;
					}

					terrain.terrainData.OverrideMaximumHeightError(newHeightError);
				}
			} else {
				//restore old values
				Transform transform = camera.transform;
				transform.position = initialPosition;
				transform.rotation = initialRotation;
				SetFOV(camera, initialFOV);
				SetOrthographicSize(camera, initialOrthographicSize);
				camera.farClipPlane = initialFarClipPlane;

				//restore terrain
				var terrain = (Terrain) Object.FindObjectOfType(typeof(Terrain));
				if (terrain != null) {
					terrain.detailObjectDistance = initialDetailObjectDistance;
					terrain.treeDistance = initialTreeDistance;
					terrain.treeBillboardDistance = initialTreeBillboardDistance;
					terrain.terrainData.OverrideMaximumHeightError(terrain.terrainData.GetMaximumHeightError());
				}
			}
		}

		private static void DeleteLookedAtObject(Component camera) {
			if (Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit raycastHit)) {
				GameObject deleteObject = raycastHit.transform.gameObject;
				deleteObject.SetActive(false);
				string xmlString = new BTToggleScriptEntry(deleteObject).Serialize();
				string logString = $"Disabled object: {xmlString}";
				BTConsole.WriteLine(logString);
				Debug.Log(logString);
			}
		}

		private static void HandleCameraInput(Camera camera) {
			if (ConfigHolder == null) {
				return;
			}

			if (camera == null) {
				return;
			}

			if (BTDebugCamInputManager.IsKeyJustDown("ToggleOrthographicMode")) {
				ToggleOrthographic(camera);
			}

			if (BTDebugCamInputManager.IsKeyDown("DebugCamFovReset")) {
				ResetFOV(camera);
			}

			if (BTDebugCamInputManager.IsKeyJustDown("DebugCamToggle")) {
				ToggleDebugCam(camera);
			}

			if (useDebugCam) {
				if (BTDebugCamInputManager.IsKeyJustDown("DeleteLookedAtObject")) {
					DeleteLookedAtObject(camera);
				}

				var movementInput = new Vector3(0f, 0f, 0f);
				float movementSpeed = BTDebugCamInputManager.IsKeyDown("DebugCamFastMovement")
						? ConfigHolder.config.cameraFastSpeed
						: ConfigHolder.config.cameraSpeed;

				float forwardInput = 0f;
				float rightInput = 0f;
				if (BTDebugCamInputManager.IsKeyDown("DebugCamForward")) {
					forwardInput += movementSpeed;
				}

				if (BTDebugCamInputManager.IsKeyDown("DebugCamBack")) {
					forwardInput -= movementSpeed;
				}

				if (BTDebugCamInputManager.IsKeyDown("DebugCamRight")) {
					rightInput += movementSpeed;
				}

				if (BTDebugCamInputManager.IsKeyDown("DebugCamLeft")) {
					rightInput -= movementSpeed;
				}

				Transform cameraTransform = camera.transform;
				if (forwardInput != 0f) {
					Vector3 cameraUp = cameraTransform.up;
					Vector3 forward = Math.Abs(camRotationY - 90f) < float.Epsilon
							? cameraUp
							: Math.Abs(camRotationY + 90f) < float.Epsilon
									? -cameraUp
									: cameraTransform.forward;
					forward.y = 0f;
					movementInput += Vector3.Normalize(forward) * forwardInput;
				}

				if (rightInput != 0f) {
					Vector3 right = cameraTransform.right;
					right.y = 0f;
					movementInput += Vector3.Normalize(right) * rightInput;
				}

				if (BTDebugCamInputManager.IsKeyDown("DebugCamUp")) {
					movementInput.y += movementSpeed;
				}

				if (BTDebugCamInputManager.IsKeyDown("DebugCamDown")) {
					movementInput.y -= movementSpeed;
				}

				cameraTransform.position += movementInput;

				//mouse input;
				camRotationX += KAInput.GetAxis("CameraRotationX") * 0.2f;
				camRotationX += KAInput.GetAxis("CameraRotationY") * 0.2f;
				if (KAInput.GetMouseButton(1)) {
					camRotationX += (Input.mousePosition.x - mMousePrev.x) * 0.2f;
					camRotationY -= (Input.mousePosition.y - mMousePrev.y) * 0.2f;
				}

				if (camRotationX > 180f) {
					camRotationX -= 360f;
				} else if (camRotationX < -180f) {
					camRotationX += 360f;
				}

				if (camRotationY > 90f) {
					camRotationY = 90f;
				} else if (camRotationY < -90f) {
					camRotationY = -90f;
				}

				Quaternion rotation = Quaternion.Euler(camRotationY, camRotationX, 0f);
				cameraTransform.rotation = rotation;
				mMousePrev = Input.mousePosition;

				float scrollInput = Input.mouseScrollDelta.y;
				if (camera.orthographic) {
					SetOrthographicSize(camera, camera.orthographicSize - 2f * scrollInput);
				} else {
					SetFOV(camera, camera.fieldOfView - scrollInput);
				}
			}
		}

		public static void Update() {
			if (BTDebugCamInputManager.IsKeyJustDown("DisableWater")) {
				DisableWater();
			}

			if (BTDebugCamInputManager.IsKeyJustDown("ToggleFog")) {
				ToggleFog();
			}

			if (BTDebugCamInputManager.IsKeyJustDown("ToggleSkybox")) {
				ToggleSkybox();
			}

			if (BTDebugCamInputManager.IsKeyJustDown("ToggleUIElements")) {
				ToggleUIElements();
			}

			HandleCameraInput(FindMainCamera());
		}

		public static void DisableWater() {
			GameObject gameObject;
			while ((gameObject = GameObject.Find("Water")) != null) {
				gameObject.SetActive(false);
			}
		}

		public static void ToggleFog() {
			RenderSettings.fog = !RenderSettings.fog;

			BTInfoHUDManager.SetFogText(RenderSettings.fog.ToString());
			BTModMenuManager.SetFogText(RenderSettings.fog ? "enabled" : "disabled");
		}

		public static void ToggleSkybox() {
			if (RenderSettings.skybox != null) {
				prevSkybox = RenderSettings.skybox;
				RenderSettings.skybox = null;
				BTInfoHUDManager.SetSkyboxText("False");
				BTModMenuManager.SetSkyboxText("disabled");
			} else {
				RenderSettings.skybox = prevSkybox;
				BTInfoHUDManager.SetSkyboxText("True");
				BTModMenuManager.SetSkyboxText("enabled");
			}
		}

		private static void ToggleUIElements() {
			KAWidget[] elements = Resources.FindObjectsOfTypeAll<KAWidget>();
			if (toggledUIElements.Count == 0) {
				foreach (KAWidget widget in elements) {
					if (widget != null && widget.GetVisibility()) {
						toggledUIElements.Add(widget.GetInstanceID());
						widget.SetVisibility(false);
					}
				}
			} else {
				foreach (KAWidget widget in elements) {
					if (widget != null && toggledUIElements.Contains(widget.GetInstanceID())) {
						widget.SetVisibility(true);
					}
				}

				toggledUIElements.Clear();
			}
		}

		public static void ToggleOrthographic(Camera camera) {
			camera.orthographic = !camera.orthographic;

			BTInfoHUDManager.SetOrthographicText(camera.orthographic.ToString());
			BTModMenuManager.SetOrthographicText(camera.orthographic ? "enabled" : "disabled");
		}

		private static void SetFOV(Camera camera, float fov) {
			camera.fieldOfView = fov;
			BTInfoHUDManager.SetFOVText(fov.ToString(CultureInfo.InvariantCulture));
		}

		private static void SetOrthographicSize(Camera camera, float orthographicSize) {
			camera.orthographicSize = orthographicSize;
			BTInfoHUDManager.SetOrthographicSizeText(orthographicSize.ToString(CultureInfo.InvariantCulture));
		}
	}
}