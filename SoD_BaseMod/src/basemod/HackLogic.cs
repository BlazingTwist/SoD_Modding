using JetBrains.Annotations;
using SoD_BaseMod.config;
using SoD_BaseMod.Extensions;
using UnityEngine;

namespace SoD_BaseMod {
	public static class HackLogic {
		[UsedImplicitly]
		public static int GetAbilityCooldown(int defaultCooldown) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if (hackConfig != null && hackConfig.squadTactics_infiniteActions) {
				return 0;
			}
			return defaultCooldown;
		}

		[UsedImplicitly]
		public static bool GetHasMoveAction(bool fallback) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if (hackConfig != null && hackConfig.squadTactics_infiniteMoves) {
				return true;
			}
			return fallback;
		}

		[UsedImplicitly]
		public static bool GetHasAbilityAction(bool fallback) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if (hackConfig != null && hackConfig.squadTactics_infiniteActions) {
				return true;
			}
			return fallback;
		}

		[UsedImplicitly]
		public static float GetFastMovementFactor() {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if (hackConfig != null && hackConfig.controls_fastMovementFactor > 0 && BTDebugCamInputManager.IsKeyDown("DebugCamFastMovement")) {
				return hackConfig.controls_fastMovementFactor;
			}

			return 1f;
		}

		[UsedImplicitly]
		public static float GetTurnFactor(float fallback) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if (hackConfig != null && hackConfig.controls_useSpeedHacks) {
				return 1f;
			}

			return fallback;
		}

		[UsedImplicitly]
		public static void ApplySpeedModifiers(ref float maxSpeed, ref float acceleration) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if (hackConfig != null && hackConfig.controls_useSpeedHacks) {
				maxSpeed *= 10f;
				acceleration *= 3f;
			}
		}

		[UsedImplicitly]
		public static float ApplyWingsuitSpeedModifier(float speed) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if (hackConfig != null && hackConfig.controls_useSpeedHacks) {
				return speed * 5f;
			}

			return speed;
		}

		public static void ModifyHorizontalInput(ref float horizontalInput) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if (hackConfig != null && hackConfig.controls_useSpeedHacks) {
				horizontalInput *= 2f;
			}
		}

		/// <summary>Executes the spawnEelHack</summary>
		/// <returns>true if spawnEelHack is enabled</returns>
		public static bool HandleSpawnEels(EelRoastManager eelRoastManager) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if (hackConfig == null || !hackConfig.eelRoast_spawnAllEels) {
				return false;
			}

			foreach (EelRoastMarkerInfo eelRoastMarkerInfo in eelRoastManager._EelRoastInfos) {
				string randomEelPath = eelRoastManager.GetRandomEelPath(eelRoastMarkerInfo);
				if (!string.IsNullOrEmpty(randomEelPath)) {
					string[] array = randomEelPath.Split('/');
					RsResourceManager.LoadAssetFromBundle(
							array[0] + "/" + array[1], array[2], eelRoastManager.ResourceEventHandler,
							typeof(GameObject), false, eelRoastMarkerInfo);
				} else {
					UtDebug.Log("Eel Asset path is empty ");
				}
			}
			return true;
		}
		
		
	}
}