using HarmonyLib;
using SoD_BaseMod.basemod;
using SoD_BlazingTwist_Core;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace SoD_BaseMod.asm
{
	public class SanctuaryPetPatcher : RuntimePatcher
	{
		public override void ApplyPatches() {
			Type originalType = typeof(SanctuaryPet);
			Type patcherType = typeof(SanctuaryPetPatcher);

			MethodInfo setMeterOriginal = AccessTools.Method(originalType, "SetMeter", new Type[] { typeof(SanctuaryPetMeterInstance), typeof(float), typeof(bool) }, null);
			MethodInfo updateOriginal = AccessTools.Method(originalType, "Update", null, null);
			MethodInfo updateShadersOriginal = AccessTools.Method(originalType, "UpdateShaders", null, null);
			MethodInfo setSleepParticleOriginal = AccessTools.Method(originalType, "SetSleepParticle", new Type[] { typeof(bool), typeof(Transform) }, null);

			HarmonyMethod setMeterPrefix = new HarmonyMethod(AccessTools.Method(patcherType, "SetMeterPrefix", new Type[] { typeof(SanctuaryPetMeterInstance), typeof(float).MakeByRefType(), typeof(bool).MakeByRefType(), typeof(SanctuaryPet) }, null));
			HarmonyMethod updatePrefix = new HarmonyMethod(AccessTools.Method(patcherType, "UpdatePrefix", null, null));
			HarmonyMethod updateShadersPostfix = new HarmonyMethod(AccessTools.Method(patcherType, "UpdateShadersPostfix", new Type[] { typeof(SanctuaryPet), typeof(Dictionary<string, SkinnedMeshRenderer>) }, null));
			HarmonyMethod setSleepParticlePostfix = new HarmonyMethod(AccessTools.Method(patcherType, "SetSleepParticlePostfix", new Type[] { typeof(SanctuaryPet) }, null));

			harmony.Patch(setMeterOriginal, setMeterPrefix);
			harmony.Patch(updateOriginal, updatePrefix);
			harmony.Patch(updateShadersOriginal, null, updateShadersPostfix);
			harmony.Patch(setSleepParticleOriginal, null, setSleepParticlePostfix);
		}

		private static void SetMeterPrefix(SanctuaryPetMeterInstance ins, ref float val, ref bool forceUpdate, SanctuaryPet __instance) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if(hackConfig != null && hackConfig.infiniteDragonMeter) {
				val = SanctuaryData.GetMaxMeter(ins.mMeterTypeInfo._Type, __instance.pData);
				forceUpdate = true;
			}
		}

		private static bool UpdatePrefix() {
			return !BTAnimationPlayerManager.IsActive();
		}

		private static void UpdateShadersPostfix(SanctuaryPet __instance, Dictionary<string, SkinnedMeshRenderer> ___mRendererMap) {
			if(__instance.pData == null) {
				return;
			}

			if(__instance.pData.Colors == null || __instance.pData.Colors.Length < 3) {
				return;
			}

			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if(hackConfig == null || hackConfig.disableDragonGlowRegex == null || hackConfig.disableDragonGlow == null) {
				return;
			}

			string regexString = hackConfig.disableDragonGlowRegex;
			bool shouldDisableGlow = hackConfig.disableDragonGlow
				.Select(name => regexString.Replace("${DragonName}", name))
				.Any(regex => Regex.IsMatch(__instance.name, regex, RegexOptions.IgnoreCase));

			if(!shouldDisableGlow) {
				return;
			}

			foreach(SkinnedMeshRenderer renderer in ___mRendererMap.Values) {
				foreach(Material material in renderer.materials) {
					if(material.HasProperty("_EmissiveColor")) {
						material.SetColor("_EmissiveColor", new Color(0f, 0f, 0f));
					}
					if(material.HasProperty("_EmissiveMap")) {
						material.SetTexture("_EmissiveMap", null);
					}
				}
			}
		}

		private static void SetSleepParticlePostfix(SanctuaryPet __instance) {
			BTConfig config = BTDebugCamInputManager.GetConfigHolder().config;
			if(config != null && config.disableZZZParticles) {
				__instance._SleepingParticleSystem.Stop();
			}
		}
	}
}
