using System;
using HarmonyLib;

namespace SoD_BaseMod {
	[HarmonyPatch(declaringType: typeof(CaAvatarCam))]
	public static class CaAvatarCamPatcher {
		[HarmonyPrefix, HarmonyPatch(methodName: "LateUpdate", argumentTypes: new Type[] { })]
		private static bool LateUpdatePrefix() {
			return !BTDebugCam.useDebugCam;
		}
	}
}