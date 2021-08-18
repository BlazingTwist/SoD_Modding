using System;
using HarmonyLib;

namespace SoD_BaseMod {
	[HarmonyPatch(declaringType: typeof(GauntletController))]
	public static class GauntletControllerPatcher {
		[HarmonyPrefix, HarmonyPatch(methodName: "Update", argumentTypes: new Type[] { })]
		private static void UpdatePrefix(GauntletController __instance) {
			__instance._Pause = BTDebugCam.useDebugCam;
		}
	}
}