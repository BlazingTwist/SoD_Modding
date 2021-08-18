using System;
using HarmonyLib;
using SoD_BaseMod.config;
using UnityEngine;

namespace SoD_BaseMod {
	[HarmonyPatch(declaringType: typeof(EREel))]
	public static class EREelPatcher {
		[HarmonyPrefix, HarmonyPatch(methodName: nameof(EREel.OnEelHit), argumentTypes: new Type[] { })]
		private static bool OnEelHitPrefix(EREel __instance) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if (hackConfig == null || !hackConfig.eelRoast_infiniteEels) {
				return true;
			}

			SanctuaryManager.pCurPetInstance.UpdateMeter(SanctuaryPetMeterType.HAPPINESS, __instance._PetHappiness);
			if (__instance._EelBlastColors == null || __instance._EelBlastColors.Length == 0 || __instance._EelBlastEffectObj == null) {
				return false;
			}

			int num = UnityEngine.Random.Range(0, __instance._EelBlastColors.Length);
			GameObject gameObject = UnityEngine.Object.Instantiate(__instance._EelBlastEffectObj, __instance._RenderPath.transform.position,
					__instance._EelBlastEffectObj.transform.rotation);
			gameObject.transform.parent = __instance._PrtParent.transform;

			ParticleSystem.MainModule main = gameObject.GetComponent<ParticleSystem>().main;
			main.startColor = __instance._EelBlastColors[num];

			if (__instance._EelHit3DScore == null) {
				return false;
			}

			Vector3 position = SanctuaryManager.pCurPetInstance.GetHeadPosition() + __instance._HappinessTextDragonOffset;
			GameObject gameObject2 = UnityEngine.Object.Instantiate(__instance._EelHit3DScore.gameObject, position,
					__instance._EelHit3DScore.transform.rotation);
			var component = gameObject2.GetComponent<TargetHit3DScore>();
			component.mDisplayScore = (int) __instance._PetHappiness;
			component.mDisplayText = __instance._PetHappinessText._Text;
			gameObject2.transform.parent = __instance._PrtParent.transform;

			return false;
		}
	}
}