using System;
using HarmonyLib;

namespace SoD_BaseMod.Extensions {
	[HarmonyPatch(declaringType: typeof(UiAvatarControls))]
	public static class UiAvatarControlsExtensions {
		[HarmonyReversePatch, HarmonyPatch(methodName: "Fire", argumentTypes: new Type[] { })]
		public static void Fire(this UiAvatarControls __instance) {
			/* dummy content */
			throw new NotImplementedException("Stub called, reverse patch has not been applied!");
		}
		
		[HarmonyReversePatch, HarmonyPatch(methodName: "EnableAvatarHideButton", argumentTypes: new[] { typeof(bool) })]
		public static void EnableAvatarHideButton(this UiAvatarControls __instance, bool hide) {
			/* dummy content */
			throw new NotImplementedException("Stub called, reverse patch has not been applied!");
		}

		[HarmonyReversePatch, HarmonyPatch(methodName: "EnableAvatarShowButton", argumentTypes: new[] { typeof(bool) })]
		public static void EnableAvatarShowButton(this UiAvatarControls __instance, bool hide) {
			/* dummy content */
			throw new NotImplementedException("Stub called, reverse patch has not been applied!");
		}
	}
}