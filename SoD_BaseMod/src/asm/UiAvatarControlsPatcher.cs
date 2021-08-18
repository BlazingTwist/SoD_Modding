using System;
using HarmonyLib;
using SoD_BaseMod.config;
using SoD_BaseMod.Extensions;

namespace SoD_BaseMod {
	[HarmonyPatch(declaringType: typeof(UiAvatarControls))]
	public static class UiAvatarControlsPatcher {
		[HarmonyPrefix, HarmonyPatch(methodName: nameof(UiAvatarControls.ShowAvatarToggleButton), argumentTypes: new[] { typeof(bool) })]
		private static void ShowAvatarToggleButtonPrefix(out bool show) {
			show = true;
		}

		[HarmonyPostfix, HarmonyPatch(methodName: nameof(UiAvatarControls.GetWeaponCooldown), argumentTypes: new Type[] { })]
		private static void GetWeaponCooldownPostfix(ref float __result) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if (hackConfig != null && hackConfig.fireball_cooldownOverride) {
				__result = 0.1f;
			}
		}

		[HarmonyPostfix, HarmonyPatch(methodName: "Update", argumentTypes: new Type[] { })]
		private static void UpdatePostfix(UiAvatarControls __instance, bool ___mFireBtnReady, bool ___mEnableFireOnButtonDown, bool ___mEnableFireOnButtonUp) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if (hackConfig == null || !hackConfig.fireball_autoFireOnHold || !AvAvatar.pInputEnabled || !___mFireBtnReady) {
				return;
			}

			if (___mEnableFireOnButtonDown && KAInput.GetButton("DragonFire")
					|| ___mEnableFireOnButtonUp && KAInput.GetButtonUp("DragonFire")) {
				__instance.Fire();
			}
		}

		[HarmonyPostfix, HarmonyPatch(methodName: nameof(UiAvatarControls.HideAvatar), argumentTypes: new[] { typeof(bool) })]
		private static void HideAvatarPostfix(UiAvatarControls __instance, bool hide) {
			if (FUEManager.IsInputEnabled("ToggleAvatar")) {
				__instance.EnableAvatarHideButton(!hide);
			}
		}
	}
}