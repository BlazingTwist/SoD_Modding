﻿using System;
using System.Reflection;
using HarmonyLib;
using SoD_BaseMod.basemod;
using SoD_BaseMod.basemod.config;
using BlazingTwist_Core;
using SquadTactics;

namespace SoD_BaseMod.asm {
	[HarmonyPatch]
	public class UIChestPatcher : RuntimePatcher {
		public override void ApplyPatches() {
			Type originalType = typeof(UiChest);
			Type patcherType = typeof(UIChestPatcher);

			MethodInfo initChestOriginal = AccessTools.Method(originalType, "InitChest");

			var initChestPrefix = new HarmonyMethod(patcherType, nameof(InitChestPrefix), new[] { typeof(UiChest) });

			harmony.Patch(initChestOriginal, initChestPrefix);
		}

		private static bool ShouldOpenChest() {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			return hackConfig != null && hackConfig.squadTactics_autochest > 0;
		}

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