using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HarmonyLib;
using SoD_BaseMod.config;
using UnityEngine;

namespace SoD_BaseMod {
	[HarmonyPatch(declaringType: typeof(SanctuaryPet))]
	public static class SanctuaryPetPatcher {
		private static readonly int EmissiveColor = Shader.PropertyToID("_EmissiveColor");
		private static readonly int EmissiveMap = Shader.PropertyToID("_EmissiveMap");

		[HarmonyPrefix,
		 HarmonyPatch(methodName: nameof(SanctuaryPet.SetMeter), argumentTypes: new[] { typeof(SanctuaryPetMeterInstance), typeof(float), typeof(bool) })]
		private static void SetMeterPrefix(SanctuaryPetMeterInstance ins, ref float val, ref bool forceUpdate, SanctuaryPet __instance) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if (hackConfig == null || !hackConfig.infiniteDragonMeter) {
				return;
			}

			val = SanctuaryData.GetMaxMeter(ins.mMeterTypeInfo._Type, __instance.pData);
			forceUpdate = true;
		}

		[HarmonyPrefix, HarmonyPatch(methodName: nameof(SanctuaryPet.Update), argumentTypes: new Type[] { })]
		private static bool UpdatePrefix() {
			return !BTAnimationPlayerManager.IsActive();
		}

		[HarmonyPostfix, HarmonyPatch(methodName: nameof(SanctuaryPet.UpdateShaders), argumentTypes: new Type[] { })]
		private static void UpdateShadersPostfix(SanctuaryPet __instance, Dictionary<string, SkinnedMeshRenderer> ___mRendererMap) {
			if (__instance.pData?.Colors == null || __instance.pData.Colors.Length < 3) {
				return;
			}

			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if (hackConfig?.disableDragonGlowRegex == null || hackConfig.disableDragonGlow == null) {
				return;
			}

			string regexString = hackConfig.disableDragonGlowRegex;
			bool shouldDisableGlow = hackConfig.disableDragonGlow
					.Select(name => regexString.Replace("${DragonName}", name))
					.Any(regex => Regex.IsMatch(__instance.name, regex, RegexOptions.IgnoreCase));

			if (!shouldDisableGlow) {
				return;
			}

			foreach (Material material in ___mRendererMap.Values.SelectMany(renderer => renderer.materials)) {
				if (material.HasProperty("_EmissiveColor")) {
					material.SetColor(EmissiveColor, new Color(0f, 0f, 0f));
				}

				if (material.HasProperty("_EmissiveMap")) {
					material.SetTexture(EmissiveMap, null);
				}
			}
		}

		[HarmonyPostfix, HarmonyPatch(methodName: nameof(SanctuaryPet.SetSleepParticle), argumentTypes: new[] { typeof(bool), typeof(Transform) })]
		private static void SetSleepParticlePostfix(SanctuaryPet __instance) {
			BTConfig config = BTDebugCamInputManager.GetConfigHolder().config;
			if (config != null && config.disableZZZParticles) {
				__instance._SleepingParticleSystem.Stop();
			}
		}

		[HarmonyPrefix, HarmonyPatch(methodName: "set_" + nameof(SanctuaryPet.pWeaponShotsAvailable), argumentTypes: new[] { typeof(int) })]
		private static bool SetPWeaponShotsAvailablePrefix() {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			return hackConfig == null || !hackConfig.fireball_infiniteShots;
		}
	}
}