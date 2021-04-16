using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using SoD_BaseMod.basemod;
using SoD_BlazingTwist_Core;
using SquadTactics;

namespace SoD_BaseMod.asm {
	[HarmonyPatch]
	public class UIChestPatcher : RuntimePatcher {
		public override void ApplyPatches() {
			Type originalType = typeof(UiChest);
			Type patcherType = typeof(UIChestPatcher);

			MethodInfo initChestOriginal = AccessTools.Method(originalType, "InitChest");

			HarmonyMethod initChestPrefix = new HarmonyMethod(AccessTools.Method(patcherType, "InitChestPrefix", new[] {typeof(UiChest)}));

			harmony.Patch(initChestOriginal, initChestPrefix);
		}

		public static bool ShouldOpenChest() {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			return hackConfig != null && hackConfig.squadTactics_autochest > 0;
		}

		[UsedImplicitly]
		private static bool InitChestPrefix(UiChest __instance) {
			if (!ShouldOpenChest()) {
				return true;
			}
			
			OpenChest(__instance);
			return false;
		}

		[HarmonyReversePatch]
		[HarmonyPatch(typeof(UiChest), "OpenChest")]
		public static void OpenChest(object instance) {
			/* dummy content */
			throw new NotImplementedException("Stub called, reverse patch has not been applied!");
		}
	}
}