using System;
using HarmonyLib;
using SquadTactics;

namespace SoD_BaseMod {
	[HarmonyPatch(declaringType: typeof(CameraMovement))]
	public static class CameraMovementPatcher {
		[HarmonyPrefix, HarmonyPatch(methodName: "Update", argumentTypes: new Type[] { })]
		private static bool UpdatePrefix() {
			return !BTDebugCam.useDebugCam;
		}

		[HarmonyPrefix, HarmonyPatch(methodName: "LateUpdate", argumentTypes: new Type[] { })]
		private static bool LateUpdatePrefix() {
			return !BTDebugCam.useDebugCam;
		}
	}
}