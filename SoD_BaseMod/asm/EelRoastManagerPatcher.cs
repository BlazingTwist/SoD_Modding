using HarmonyLib;
using SoD_BaseMod.basemod;
using SoD_BlazingTwist_Core;
using System;
using System.Reflection;
using UnityEngine;

namespace SoD_BaseMod.asm
{
	[HarmonyPatch]
	public static class EelRoastManagerExtension
	{
		[HarmonyReversePatch]
		[HarmonyPatch(typeof(EelRoastManager), "ResourceEventHandler")]
		public static void ResourceEventHandlerReverse(this EelRoastManager __instance, string inURL, RsResourceLoadEvent inEvent, float inProgress, object inObject, object inUserData) {
			/* dummy content */
			throw new NotImplementedException("Stub called, reverse patch has not been applied!");
		}
	}

	[HarmonyPatch]
	public class EelRoastManagerPatcher : RuntimePatcher
	{
		public override void ApplyPatches() {
			Type originalType = typeof(EelRoastManager);
			Type patcherType = typeof(EelRoastManagerPatcher);

			MethodInfo spawnEelsOriginal = AccessTools.Method(originalType, "SpawnEels", null, null);

			HarmonyMethod spawnEelsPrefix = new HarmonyMethod(AccessTools.Method(patcherType, "SpawnEelsPrefix", null, null));

			harmony.Patch(spawnEelsOriginal, spawnEelsPrefix);
		}

		private static bool SpawnEelsPrefix(EelRoastManager __instance) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if(hackConfig != null && hackConfig.eelRoast_spawnAllEels) {
				foreach(EelRoastMarkerInfo eelRoastMarkerInfo in __instance._EelRoastInfos) {
					string randomEelPath = GetRandomEelPathReverse(__instance, eelRoastMarkerInfo);
					if(!string.IsNullOrEmpty(randomEelPath)) {
						string[] array = randomEelPath.Split(new char[]
						{
						'/'
						});
						RsResourceManager.LoadAssetFromBundle(array[0] + "/" + array[1], array[2], new RsResourceEventHandler(__instance.ResourceEventHandlerReverse), typeof(GameObject), false, eelRoastMarkerInfo);
					} else {
						UtDebug.Log("Eel Asset path is empty ");
					}
				}
				return false;
			}
			return true;
		}

		[HarmonyReversePatch]
		[HarmonyPatch(typeof(EelRoastManager), "GetRandomEelPath")]
		public static string GetRandomEelPathReverse(object instance, EelRoastMarkerInfo eelRoastMarkerInfo) {
			/* dummy content */
			throw new NotImplementedException("Stub called, reverse patch has not been applied!");
		}
	}
}
