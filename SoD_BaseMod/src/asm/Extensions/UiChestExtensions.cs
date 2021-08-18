using System;
using HarmonyLib;
using SquadTactics;

namespace SoD_BaseMod.Extensions {
	[HarmonyPatch(declaringType: typeof(UiChest))]
	public static class UiChestExtensions {
		[HarmonyReversePatch, HarmonyPatch(methodName: "OpenChest", argumentTypes: new Type[] { })]
		public static void OpenChest(this UiChest instance) {
			/* dummy content */
			throw new NotImplementedException("Stub called, reverse patch has not been applied!");
		}
	}
}