using System;
using HarmonyLib;
using SoD_BaseMod.config;
using SoD_BaseMod.Extensions;
using UnityEngine;

namespace SoD_BaseMod {
	[HarmonyPatch(declaringType: typeof(EelRoastManager))]
	public static class EelRoastManagerPatcher {
		[HarmonyPrefix, HarmonyPatch(methodName: "SpawnEels", argumentTypes: new Type[] { })]
		private static bool SpawnEelsPrefix(EelRoastManager __instance) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if (hackConfig == null || !hackConfig.eelRoast_spawnAllEels) {
				return true;
			}

			foreach (EelRoastMarkerInfo eelRoastMarkerInfo in __instance._EelRoastInfos) {
				string randomEelPath = __instance.GetRandomEelPath(eelRoastMarkerInfo);
				if (!string.IsNullOrEmpty(randomEelPath)) {
					string[] array = randomEelPath.Split('/');
					RsResourceManager.LoadAssetFromBundle(
							array[0] + "/" + array[1], array[2], __instance.ResourceEventHandler,
							typeof(GameObject), false, eelRoastMarkerInfo);
				} else {
					UtDebug.Log("Eel Asset path is empty ");
				}
			}

			return false;
		}
	}
}