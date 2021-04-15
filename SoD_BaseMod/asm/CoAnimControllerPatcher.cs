using HarmonyLib;
using SoD_BlazingTwist_Core;
using System;
using System.Reflection;
using SoD_BaseMod.basemod;

namespace SoD_BaseMod.asm
{
	public class CoAnimControllerPatcher : RuntimePatcher
	{
		public override void ApplyPatches() {
			Type originalType = typeof(CoAnimController);
			Type patcherType = typeof(CoAnimControllerPatcher);

			MethodInfo updateOriginal = AccessTools.Method(originalType, "Update", null, null);

			HarmonyMethod updatePrefix = new HarmonyMethod(AccessTools.Method(patcherType, "UpdatePrefix", null, null));

			harmony.Patch(updateOriginal, updatePrefix);
		}

		private static bool UpdatePrefix() {
			return !BTDebugCam.useDebugCam;
		}
	}
}
