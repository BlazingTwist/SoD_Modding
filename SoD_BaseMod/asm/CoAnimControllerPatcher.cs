using HarmonyLib;
using BlazingTwist_Core;
using System;
using System.Reflection;
using JetBrains.Annotations;
using SoD_BaseMod.basemod;

namespace SoD_BaseMod.asm {
	[UsedImplicitly]
	public class CoAnimControllerPatcher : RuntimePatcher {
		public override void ApplyPatches() {
			Type originalType = typeof(CoAnimController);
			Type patcherType = typeof(CoAnimControllerPatcher);

			MethodInfo updateOriginal = AccessTools.Method(originalType, "Update");

			HarmonyMethod updatePrefix = new HarmonyMethod(patcherType, nameof(UpdatePrefix));

			harmony.Patch(updateOriginal, updatePrefix);
		}

		private static bool UpdatePrefix() {
			return !BTDebugCam.useDebugCam;
		}
	}
}