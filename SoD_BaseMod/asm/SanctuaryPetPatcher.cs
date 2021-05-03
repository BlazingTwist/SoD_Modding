using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using HarmonyLib;
using JetBrains.Annotations;
using SoD_BaseMod.basemod;
using SoD_BaseMod.basemod.config;
using BlazingTwist_Core;
using UnityEngine;

namespace SoD_BaseMod.asm {
	[UsedImplicitly]
	public class SanctuaryPetPatcher : RuntimePatcher {
		private static readonly int EmissiveColor = Shader.PropertyToID("_EmissiveColor");
		private static readonly int EmissiveMap = Shader.PropertyToID("_EmissiveMap");

		public override void ApplyPatches() {
			Type originalType = typeof(SanctuaryPet);
			Type patcherType = typeof(SanctuaryPetPatcher);

			MethodInfo setMeterOriginal = AccessTools.Method(originalType, "SetMeter",
					new[] { typeof(SanctuaryPetMeterInstance), typeof(float), typeof(bool) });
			MethodInfo updateOriginal = AccessTools.Method(originalType, "Update");
			MethodInfo updateShadersOriginal = AccessTools.Method(originalType, "UpdateShaders");
			MethodInfo setSleepParticleOriginal = AccessTools.Method(originalType, "SetSleepParticle", new[] { typeof(bool), typeof(Transform) });
			MethodInfo setPWeaponShotsAvailableOriginal = AccessTools.PropertySetter(originalType, "pWeaponShotsAvailable");

			var setMeterPrefix = new HarmonyMethod(patcherType, nameof(SetMeterPrefix),
					new[] { typeof(SanctuaryPetMeterInstance), typeof(float).MakeByRefType(), typeof(bool).MakeByRefType(), typeof(SanctuaryPet) });
			var updatePrefix = new HarmonyMethod(patcherType, nameof(UpdatePrefix));
			var updateShadersPostfix = new HarmonyMethod(patcherType, nameof(UpdateShadersPostfix),
					new[] { typeof(SanctuaryPet), typeof(Dictionary<string, SkinnedMeshRenderer>) });
			var setSleepParticlePostfix = new HarmonyMethod(patcherType, nameof(SetSleepParticlePostfix), new[] { typeof(SanctuaryPet) });
			var setPWeaponShotsAvailablePrefix = new HarmonyMethod(patcherType, nameof(SetPWeaponShotsAvailablePrefix));

			harmony.Patch(setMeterOriginal, setMeterPrefix);
			harmony.Patch(updateOriginal, updatePrefix);
			harmony.Patch(updateShadersOriginal, null, updateShadersPostfix);
			harmony.Patch(setSleepParticleOriginal, null, setSleepParticlePostfix);
			harmony.Patch(setPWeaponShotsAvailableOriginal, setPWeaponShotsAvailablePrefix);
		}

		private static void SetMeterPrefix(SanctuaryPetMeterInstance ins, ref float val, ref bool forceUpdate, SanctuaryPet __instance) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if (hackConfig == null || !hackConfig.infiniteDragonMeter) {
				return;
			}

			val = SanctuaryData.GetMaxMeter(ins.mMeterTypeInfo._Type, __instance.pData);
			forceUpdate = true;
		}

		private static bool UpdatePrefix() {
			return !BTAnimationPlayerManager.IsActive();
		}

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

		private static void SetSleepParticlePostfix(SanctuaryPet __instance) {
			BTConfig config = BTDebugCamInputManager.GetConfigHolder().config;
			if (config != null && config.disableZZZParticles) {
				__instance._SleepingParticleSystem.Stop();
			}
		}

		private static bool SetPWeaponShotsAvailablePrefix() {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			return hackConfig == null || !hackConfig.fireball_infiniteShots;
		}
	}
}