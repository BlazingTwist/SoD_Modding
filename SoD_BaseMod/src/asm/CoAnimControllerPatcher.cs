using System;
using HarmonyLib;

namespace SoD_BaseMod {
	[HarmonyPatch(declaringType: typeof(CoAnimController))]
	public static class CoAnimControllerPatcher {
		[HarmonyPrefix, HarmonyPatch(methodName: "Update", argumentTypes: new Type[] { })]
		private static bool UpdatePrefix() {
			return !BTDebugCam.useDebugCam;
		}
	}
}