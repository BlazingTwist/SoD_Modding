using System;
using HarmonyLib;
using SoD_BaseMod.config;
using SoD_BaseMod.Extensions;
using UnityEngine;

namespace SoD_BaseMod {
	[HarmonyPatch(declaringType: typeof(UiAvatarControls))]
	public static class UiAvatarControlsPatcher {
		
		[HarmonyPostfix, HarmonyPatch(methodName: nameof(UiAvatarControls.GetWeaponCooldown), argumentTypes: new Type[] { })]
		private static void GetWeaponCooldownPostfix(ref float __result) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if (hackConfig != null && hackConfig.fireball_cooldownOverride) {
				__result = 0.1f;
			}
		}

		private static BTVisibilitySetting previousHideButtonVisibility = BTVisibilitySetting.Default;

		[HarmonyPostfix, HarmonyPatch(methodName: "Update", argumentTypes: new Type[] { })]
		private static void UpdatePostfix(UiAvatarControls __instance, bool ___mFireBtnReady, bool ___mEnableFireOnButtonDown,
				bool ___mEnableFireOnButtonUp, AvAvatarController ___mAVController) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if (hackConfig != null && hackConfig.fireball_autoFireOnHold && AvAvatar.pInputEnabled && ___mFireBtnReady) {
				if (___mEnableFireOnButtonDown && KAInput.GetButton("DragonFire")
						|| ___mEnableFireOnButtonUp && KAInput.GetButtonUp("DragonFire")) {
					__instance.Fire();
				}
			}

			BTVisibilitySetting visibility = BTDebugCamInputManager.GetConfigHolder().config.avatarButtonVisibility;
			if (visibility != previousHideButtonVisibility) {
				previousHideButtonVisibility = visibility;
				if (visibility == BTVisibilitySetting.Hide) {
					__instance.EnableAvatarHideButton(false);
					__instance.EnableAvatarShowButton(false);
					___mAVController.AvatarHidden = false;
					___mAVController.EnableRenderer(true);
				} else if (___mAVController != null) {
					bool hidden = ___mAVController.AvatarHidden;
					if (visibility == BTVisibilitySetting.Force) {
						__instance.EnableAvatarHideButton(!hidden);
						__instance.EnableAvatarShowButton(hidden);
					} else if (visibility == BTVisibilitySetting.Default) {
						__instance.HideAvatar(hidden);
					}
				}
			}
		}

		[HarmonyPrefix, HarmonyPatch(methodName: nameof(UiAvatarControls.HideAvatar), argumentTypes: new[] { typeof(bool) })]
		private static bool HideAvatarPrefix(UiAvatarControls __instance, bool hide) {
			BTVisibilitySetting visibility = BTDebugCamInputManager.GetConfigHolder().config.avatarButtonVisibility;
			return visibility != BTVisibilitySetting.Hide;
		}

		[HarmonyPostfix, HarmonyPatch(methodName: nameof(UiAvatarControls.HideAvatar), argumentTypes: new[] { typeof(bool) })]
		private static void HideAvatarPostfix(UiAvatarControls __instance, bool hide) {
			BTVisibilitySetting visibility = BTDebugCamInputManager.GetConfigHolder().config.avatarButtonVisibility;
			if (visibility == BTVisibilitySetting.Default) {
				return;
			}

			if (visibility == BTVisibilitySetting.Force) {
				if (FUEManager.IsInputEnabled("ToggleAvatar")) {
					__instance.EnableAvatarHideButton(!hide);
				}
			}
		}
		
		[HarmonyPrefix, HarmonyPatch(methodName: nameof(UiAvatarControls.ShowAvatarToggleButton), argumentTypes: new[] { typeof(bool) })]
		private static bool ShowAvatarToggleButtonPrefix(UiAvatarControls __instance, bool show) {
			BTVisibilitySetting visibility = BTDebugCamInputManager.GetConfigHolder().config.avatarButtonVisibility;
			switch (visibility) {
				case BTVisibilitySetting.Default:
					return true;
				case BTVisibilitySetting.Force:
					return false;
				case BTVisibilitySetting.Hide:
					return false;
				default:
					BTConfigHolder.LogMessage(LogType.Error, $"ShowAvatarToggleButtonPrefix: Unknown ButtonVisibility '{visibility}'");
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}