using System;
using HarmonyLib;

namespace SoD_BaseMod {
	[HarmonyPatch(declaringType: typeof(EelRoastManager))]
	public static class EelRoastManagerPatcher {
		[HarmonyPrefix, HarmonyPatch(methodName: "SpawnEels", argumentTypes: new Type[] { })]
		private static bool SpawnEelsPrefix(EelRoastManager __instance) {
			return !HackLogic.HandleSpawnEels(__instance);
		}
	}
}