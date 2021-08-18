using System;
using HarmonyLib;

namespace SoD_BaseMod.Extensions {
	[HarmonyPatch(declaringType: typeof(KAConsole))]
	public static class KAConsoleExtensions {
		[HarmonyReversePatch, HarmonyPatch(methodName: "IsConsoleVisible", argumentTypes: new Type[] { })]
		public static bool IsConsoleVisible(this KAConsole __instance) {
			/* dummy content */
			throw new NotImplementedException("Stub called, reverse patch has not been applied!");
		}
	}
}