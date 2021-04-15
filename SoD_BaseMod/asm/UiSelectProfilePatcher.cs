using HarmonyLib;
using SoD_BaseMod.basemod;
using SoD_BlazingTwist_Core;
using System;
using System.Reflection;

namespace SoD_BaseMod.asm
{
	public class UiSelectProfilePatcher : RuntimePatcher
	{
		public override void ApplyPatches() {
			Type originalType = typeof(UiSelectProfile);
			Type patcherType = typeof(UiSelectProfilePatcher);

			MethodInfo initQuickLaunchButtonsOriginal = AccessTools.Method(originalType, "InitQuickLaunchButtons", null, null);

			HarmonyMethod initQuickLaunchButtonsPostfix = new HarmonyMethod(AccessTools.Method(patcherType, "InitQuickLaunchButtonsPostfix", new Type[] { typeof(UiSelectProfile) }, null));

			harmony.Patch(initQuickLaunchButtonsOriginal, null, initQuickLaunchButtonsPostfix);
		}

		private static void InitQuickLaunchButtonsPostfix(UiSelectProfile __instance) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if(hackConfig == null || !hackConfig.unlockStartButtons) {
				return;
			}

			foreach(QuickLaunchBtnInfo buttonInfo in __instance._QuickLaunchBtnInfo) {
				buttonInfo._LockedButton.SetVisibility(false);
				buttonInfo._LockedButton.SetVisibility(true);
			}
		}
	}
}
