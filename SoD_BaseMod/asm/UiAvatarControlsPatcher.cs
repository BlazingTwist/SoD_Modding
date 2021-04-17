using System;
using System.Reflection;
using HarmonyLib;
using SoD_BaseMod.basemod;
using SoD_BaseMod.basemod.config;
using SoD_BlazingTwist_Core;

namespace SoD_BaseMod.asm {
	[HarmonyPatch]
	public class UiAvatarControlsPatcher : RuntimePatcher {
		public override void ApplyPatches() {
			Type originalType = typeof(UiAvatarControls);
			Type patcherType = typeof(UiAvatarControlsPatcher);

			MethodInfo showAvatarToggleButtonOriginal = AccessTools.Method(originalType, "ShowAvatarToggleButton", new[] { typeof(bool) });
			MethodInfo getWeaponCooldownOriginal = AccessTools.Method(originalType, "GetWeaponCooldown");
			MethodInfo updateOriginal = AccessTools.Method(originalType, "Update");
			MethodInfo hideAvatarOriginal = AccessTools.Method(originalType, "HideAvatar", new[] { typeof(bool) });

			HarmonyMethod showAvatarToggleButtonPrefix =
					new HarmonyMethod(patcherType, nameof(ShowAvatarToggleButtonPrefix), new[] { typeof(bool).MakeByRefType() });
			HarmonyMethod getWeaponCooldownPostfix =
					new HarmonyMethod(patcherType, nameof(GetWeaponCooldownPostfix), new[] { typeof(float).MakeByRefType() });
			HarmonyMethod updatePostfix = new HarmonyMethod(patcherType, nameof(UpdatePostfix),
					new[] { typeof(UiAvatarControls), typeof(bool), typeof(bool), typeof(bool) });
			HarmonyMethod hideAvatarPostfix =
					new HarmonyMethod(patcherType, nameof(HideAvatarPostfix), new[] { typeof(UiAvatarControls), typeof(bool) });

			harmony.Patch(showAvatarToggleButtonOriginal, showAvatarToggleButtonPrefix);
			harmony.Patch(getWeaponCooldownOriginal, null, getWeaponCooldownPostfix);
			harmony.Patch(updateOriginal, null, updatePostfix);
			harmony.Patch(hideAvatarOriginal, null, hideAvatarPostfix);
		}

		private static void ShowAvatarToggleButtonPrefix(out bool show) {
			show = true;
		}

		private static void GetWeaponCooldownPostfix(ref float __result) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if (hackConfig != null && hackConfig.fireball_cooldownOverride) {
				__result = 0.1f;
			}
		}

		private static void UpdatePostfix(UiAvatarControls __instance, bool ___mFireBtnReady, bool ___mEnableFireOnButtonDown, bool ___mEnableFireOnButtonUp) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if (hackConfig == null || !hackConfig.fireball_autoFireOnHold || !AvAvatar.pInputEnabled || !___mFireBtnReady) {
				return;
			}

			if ((___mEnableFireOnButtonDown && KAInput.GetButton("DragonFire")) || (___mEnableFireOnButtonUp && KAInput.GetButtonUp("DragonFire"))) {
				FireReverse(__instance);
			}
		}

		private static void HideAvatarPostfix(UiAvatarControls __instance, bool hide) {
			if (FUEManager.IsInputEnabled("ToggleAvatar")) {
				EnableAvatarHideButtonReverse(__instance, !hide);
			}
		}

		[HarmonyReversePatch]
		[HarmonyPatch(typeof(UiAvatarControls), "Fire")]
		public static void FireReverse(object instance) {
			/* dummy content */
			throw new NotImplementedException("Stub called, reverse patch has not been applied!");
		}

		[HarmonyReversePatch]
		[HarmonyPatch(typeof(UiAvatarControls), "EnableAvatarHideButton")]
		public static void EnableAvatarHideButtonReverse(object instance, bool hide) {
			/* dummy content */
			throw new NotImplementedException("Stub called, reverse patch has not been applied!");
		}
	}
}