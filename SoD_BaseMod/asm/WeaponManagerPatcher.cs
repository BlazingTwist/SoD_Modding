using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using SoD_BaseMod.basemod;
using BlazingTwist_Core;

namespace SoD_BaseMod.asm {
	[UsedImplicitly]
	public class WeaponManagerPatcher : RuntimePatcher {
		public override void ApplyPatches() {
			Type originalType = typeof(WeaponManager);
			Type patcherType = typeof(WeaponManagerPatcher);

			MethodInfo originalRangeGetter = AccessTools.PropertyGetter(originalType, "pRange");
			MethodInfo originalCooldownGetter = AccessTools.Method(originalType, "GetCooldown");

			HarmonyMethod patchedRangeGetter = new HarmonyMethod(patcherType, nameof(GetRange), new[] { typeof(WeaponManager), typeof(float).MakeByRefType() });
			HarmonyMethod patchedCooldownGetter = new HarmonyMethod(patcherType, nameof(GetCooldown), new[] { typeof(float).MakeByRefType() });

			harmony.Patch(originalRangeGetter, patchedRangeGetter);
			harmony.Patch(originalCooldownGetter, patchedCooldownGetter);
		}

		private static bool GetRange(WeaponManager __instance, ref float __result) {
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

		private static bool GetCooldown(ref float __result) {
			if (BTDebugCamInputManager.GetConfigHolder().hackConfig == null || !BTDebugCamInputManager.GetConfigHolder().hackConfig.fireball_cooldownOverride) {
				return true;
			}

			__result = 0.1f;
			return false;
		}
	}
}