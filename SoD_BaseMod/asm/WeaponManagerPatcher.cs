using HarmonyLib;
using SoD_BlazingTwist_Core;
using System;
using System.Reflection;
using SoD_BaseMod.basemod;

namespace SoD_BaseMod.asm
{
	public class WeaponManagerPatcher : RuntimePatcher
	{
		public override void ApplyPatches() {
			Type originalType = typeof(WeaponManager);
			Type patcherType = typeof(WeaponManagerPatcher);

			MethodInfo originalRangeGetter = AccessTools.PropertyGetter(originalType, "pRange");
			MethodInfo originalCooldownGetter = AccessTools.Method(originalType, "GetCooldown", null, null);

			HarmonyMethod patchedRangeGetter = new HarmonyMethod(AccessTools.Method(patcherType, "GetRange", new Type[] { typeof(WeaponManager), typeof(float).MakeByRefType() }, null));
			HarmonyMethod patchedCooldownGetter = new HarmonyMethod(AccessTools.Method(patcherType, "GetCooldown", new Type[] { typeof(float).MakeByRefType() }, null));

			harmony.Patch(originalRangeGetter, patchedRangeGetter);
			harmony.Patch(originalCooldownGetter, patchedCooldownGetter);
		}

		private static bool GetRange(WeaponManager __instance, ref float __result) {
			if(__instance.GetCurrentWeapon() == null) {
				return true;
			}
			if(BTDebugCamInputManager.GetConfigHolder().hackConfig != null && BTDebugCamInputManager.GetConfigHolder().hackConfig.fireball_infiniteTargetRange) {
				__result = 10000f;
				return false;
			}
			return true;
		}

		private static bool GetCooldown(ref float __result) {
			if(BTDebugCamInputManager.GetConfigHolder().hackConfig != null && BTDebugCamInputManager.GetConfigHolder().hackConfig.fireball_cooldownOverride) {
				__result = 0.1f;
				return false;
			}
			return true;
		}
	}
}
