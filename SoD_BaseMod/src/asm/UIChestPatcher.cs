using System;
using HarmonyLib;
using SoD_BaseMod.config;
using SoD_BaseMod.Extensions;
using SquadTactics;

namespace SoD_BaseMod {
	[HarmonyPatch(declaringType: typeof(UiChest))]
	public static class UIChestPatcher {
		[HarmonyPrefix, HarmonyPatch(methodName: nameof(UiChest.InitChest), argumentTypes: new Type[] { })]
		private static bool InitChestPrefix(UiChest __instance) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			bool shouldOpenChest = hackConfig != null && hackConfig.squadTactics_autochest > 0;
			if (shouldOpenChest) {
				__instance.OpenChest();
				return false;
			}
			return true;
		}
	}
}