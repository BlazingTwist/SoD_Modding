using HarmonyLib;
using SoD_BlazingTwist_Core;
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

			HarmonyMethod updatePrefix = new HarmonyMethod(AccessTools.Method(patcherType, "UpdatePrefix"));

			harmony.Patch(updateOriginal, updatePrefix);
		}

		[UsedImplicitly]
		private static bool UpdatePrefix() {
			return !BTDebugCam.useDebugCam;
		}
	}
}