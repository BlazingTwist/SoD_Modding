using System;
using HarmonyLib;

namespace SoD_BaseMod {
	[HarmonyPatch(declaringType: typeof(WeaponManager))]
	public static class WeaponManagerPatcher {
		[HarmonyPrefix, HarmonyPatch(methodName: "get_" + nameof(WeaponManager.pRange), argumentTypes: new Type[] { })]
		private static bool Get_pRange_Prefix(WeaponManager __instance, ref float __result) {
			if (__instance.GetCurrentWeapon() == null) {
				return true;
			}

			if (BTDebugCamInputManager.GetConfigHolder().hackConfig == null ||
					!BTDebugCamInputManager.GetConfigHolder().hackConfig.fireball_infiniteTargetRange) {
				return true;
			}

			__result = 10000f;
			return false;
		}

		[HarmonyPrefix, HarmonyPatch(methodName: nameof(WeaponManager.GetCooldown), argumentTypes: new Type[] { })]
		private static bool GetCooldown_Prefix(ref float __result) {
			if (BTDebugCamInputManager.GetConfigHolder().hackConfig == null || !BTDebugCamInputManager.GetConfigHolder().hackConfig.fireball_cooldownOverride) {
				return true;
			}

			__result = 0.1f;
			return false;
		}
	}
}