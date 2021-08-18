using System;
using HarmonyLib;
using SoD_BaseMod.config;

namespace SoD_BaseMod {
	[HarmonyPatch(declaringType: typeof(UiSelectProfile))]
	public static class UiSelectProfilePatcher {
		[HarmonyPostfix, HarmonyPatch(methodName: "InitQuickLaunchButtons", argumentTypes: new Type[] { })]
		private static void InitQuickLaunchButtonsPostfix(UiSelectProfile __instance) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if (hackConfig == null || !hackConfig.unlockStartButtons) {
				return;
			}

			foreach (QuickLaunchBtnInfo buttonInfo in __instance._QuickLaunchBtnInfo) {
				buttonInfo._LockedButton.SetVisibility(false);
				buttonInfo._UnLockedButton.SetVisibility(true);
			}
		}
	}
}