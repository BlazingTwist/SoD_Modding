using HarmonyLib;
using SoD_BlazingTwist_Core;
using System;
using System.Reflection;
using SoD_BaseMod.basemod;

namespace SoD_BaseMod.asm
{
	public class GauntletControllerPatcher : RuntimePatcher
	{
		public override void ApplyPatches() {
			Type originalType = typeof(GauntletController);
			Type patcherType = typeof(GauntletControllerPatcher);

			MethodInfo updateOriginal = AccessTools.Method(originalType, "Update", null, null);

			HarmonyMethod updatePrefix = new HarmonyMethod(AccessTools.Method(patcherType, "UpdatePrefix", new Type[] { typeof(GauntletController) }, null));

			harmony.Patch(updateOriginal, updatePrefix);
		}

		private static void UpdatePrefix(GauntletController __instance) {
			__instance._Pause = BTDebugCam.useDebugCam;
		}
	}
}
