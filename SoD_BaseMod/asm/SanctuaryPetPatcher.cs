using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using HarmonyLib;
using JetBrains.Annotations;
using SoD_BaseMod.basemod;
using SoD_BlazingTwist_Core;
using UnityEngine;

namespace SoD_BaseMod.asm {
	[UsedImplicitly]
	public class SanctuaryPetPatcher : RuntimePatcher {
		public override void ApplyPatches() {
			Type originalType = typeof(SanctuaryPet);
			Type patcherType = typeof(SanctuaryPetPatcher);

			MethodInfo setMeterOriginal = AccessTools.Method(originalType, "SetMeter",
				new[] {typeof(SanctuaryPetMeterInstance), typeof(float), typeof(bool)});
			MethodInfo updateOriginal = AccessTools.Method(originalType, "Update");
			MethodInfo updateShadersOriginal = AccessTools.Method(originalType, "UpdateShaders");
			MethodInfo setSleepParticleOriginal = AccessTools.Method(originalType, "SetSleepParticle", new[] {typeof(bool), typeof(Transform)});
			MethodInfo setPWeaponShotsAvailableOriginal = AccessTools.PropertySetter(originalType, "pWeaponShotsAvailable");

			HarmonyMethod setMeterPrefix = new HarmonyMethod(AccessTools.Method(patcherType, "SetMeterPrefix",
				new[] {typeof(SanctuaryPetMeterInstance), typeof(float).MakeByRefType(), typeof(bool).MakeByRefType(), typeof(SanctuaryPet)}));
			HarmonyMethod updatePrefix = new HarmonyMethod(AccessTools.Method(patcherType, "UpdatePrefix"));
			HarmonyMethod updateShadersPostfix = new HarmonyMethod(AccessTools.Method(patcherType, "UpdateShadersPostfix",
				new[] {typeof(SanctuaryPet), typeof(Dictionary<string, SkinnedMeshRenderer>)}));
			HarmonyMethod setSleepParticlePostfix =
				new HarmonyMethod(AccessTools.Method(patcherType, "SetSleepParticlePostfix", new[] {typeof(SanctuaryPet)}));
			HarmonyMethod setPWeaponShotsAvailablePrefix = new HarmonyMethod(AccessTools.Method(patcherType, "SetPWeaponShotsAvailablePrefix"));

			harmony.Patch(setMeterOriginal, setMeterPrefix);
			harmony.Patch(updateOriginal, updatePrefix);
			harmony.Patch(updateShadersOriginal, null, updateShadersPostfix);
			harmony.Patch(setSleepParticleOriginal, null, setSleepParticlePostfix);
			harmony.Patch(setPWeaponShotsAvailableOriginal, setPWeaponShotsAvailablePrefix);
		}

		[UsedImplicitly]
		private static void SetMeterPrefix(SanctuaryPetMeterInstance ins, ref float val, ref bool forceUpdate, SanctuaryPet __instance) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if (hackConfig == null || !hackConfig.infiniteDragonMeter) {
				return;
			}

			val = SanctuaryData.GetMaxMeter(ins.mMeterTypeInfo._Type, __instance.pData);
			forceUpdate = true;
		}

		[UsedImplicitly]
		private static bool UpdatePrefix() {
			return !BTAnimationPlayerManager.IsActive();
		}

		[UsedImplicitly]
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

			foreach (SkinnedMeshRenderer renderer in ___mRendererMap.Values) {
				foreach (Material material in renderer.materials) {
					if (material.HasProperty("_EmissiveColor")) {
						material.SetColor("_EmissiveColor", new Color(0f, 0f, 0f));
					}

					if (material.HasProperty("_EmissiveMap")) {
						material.SetTexture("_EmissiveMap", null);
					}
				}
			}
		}

		[UsedImplicitly]
		private static void SetSleepParticlePostfix(SanctuaryPet __instance) {
			BTConfig config = BTDebugCamInputManager.GetConfigHolder().config;
			if (config != null && config.disableZZZParticles) {
				__instance._SleepingParticleSystem.Stop();
			}
		}

		[UsedImplicitly]
		private static bool SetPWeaponShotsAvailablePrefix() {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			return hackConfig == null || !hackConfig.fireball_infiniteShots;
		}
	}
}