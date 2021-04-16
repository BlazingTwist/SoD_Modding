﻿using HarmonyLib;
using SoD_BlazingTwist_Core;
using System;
using System.Reflection;
using JetBrains.Annotations;
using SoD_BaseMod.basemod;

namespace SoD_BaseMod.asm {
	[UsedImplicitly]
	public class GauntletControllerPatcher : RuntimePatcher {
		public override void ApplyPatches() {
			Type originalType = typeof(GauntletController);
			Type patcherType = typeof(GauntletControllerPatcher);

			MethodInfo updateOriginal = AccessTools.Method(originalType, "Update");

			HarmonyMethod updatePrefix = new HarmonyMethod(AccessTools.Method(patcherType, "UpdatePrefix", new[] {typeof(GauntletController)}));

			harmony.Patch(updateOriginal, updatePrefix);
		}

		[UsedImplicitly]
		private static void UpdatePrefix(GauntletController __instance) {
			__instance._Pause = BTDebugCam.useDebugCam;
		}
	}
}