using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using SoD_BaseMod.basemod;
using SoD_BaseMod.basemod.config;
using BlazingTwist_Core;

namespace SoD_BaseMod.asm {
	[UsedImplicitly]
	public class UiSelectProfilePatcher : RuntimePatcher {
		public override void ApplyPatches() {
			Type originalType = typeof(UiSelectProfile);
			Type patcherType = typeof(UiSelectProfilePatcher);

			MethodInfo initQuickLaunchButtonsOriginal = AccessTools.Method(originalType, "InitQuickLaunchButtons");

			var initQuickLaunchButtonsPostfix =
					new HarmonyMethod(patcherType, nameof(InitQuickLaunchButtonsPostfix), new[] { typeof(UiSelectProfile) });

			harmony.Patch(initQuickLaunchButtonsOriginal, null, initQuickLaunchButtonsPostfix);
		}

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