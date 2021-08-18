using System;
using HarmonyLib;

namespace SoD_BaseMod.Extensions {
	[HarmonyPatch(declaringType: typeof(UiBackpack))]
	public static class UiBackpackExtensions {
		[HarmonyReversePatch, HarmonyPatch(methodName: "ApplyOnPet", argumentTypes: new[] { typeof(UserItemData) })]
		public static bool ApplyOnPet(this UiBackpack __instance, UserItemData inItemData) {
			/* dummy content */
			throw new NotImplementedException("Stub called, reverse patch has not been applied!");
		}
	}
}