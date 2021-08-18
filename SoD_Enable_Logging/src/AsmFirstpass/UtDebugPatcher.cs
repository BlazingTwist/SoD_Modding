using System;
using HarmonyLib;
using SoD_BaseMod;
using SoD_BaseMod.config;
using UnityEngine;

namespace SoD_Enable_Logging.AsmFirstpass {
	[HarmonyPatch(declaringType: typeof(UtDebug))]
	public class UtDebugPatcher {
		private static bool IsUTDebugEnabled() {
			BTConfig config = BTDebugCamInputManager.GetConfigHolder().config;
			return config != null && config.enableUTDebug;
		}

		[HarmonyPrefix, HarmonyPatch(methodName: nameof(UtDebug.Log), argumentTypes: new[] { typeof(object), typeof(int) })]
		private static bool LogPrefix(object message, int priority) {
			if (!IsUTDebugEnabled()) {
				return true;
			}

			Debug.Log(message);
			return false;
		}

		[HarmonyPrefix, HarmonyPatch(methodName: nameof(UtDebug.Log), argumentTypes: new[] { typeof(object), typeof(uint) })]
		private static bool LogPrefix(object message, uint mask) {
			if (!IsUTDebugEnabled()) {
				return true;
			}

			Debug.Log(message);
			return false;
		}

		[HarmonyPrefix, HarmonyPatch(methodName: nameof(UtDebug.LogFastP), argumentTypes: new[] { typeof(int), typeof(object[]) })]
		private static bool LogFastP_Prefix(int priority, params object[] message) {
			if (!IsUTDebugEnabled()) {
				return true;
			}

			Debug.Log(String.Concat(message));
			return false;
		}

		[HarmonyPrefix, HarmonyPatch(methodName: nameof(UtDebug.LogFastM), argumentTypes: new[] { typeof(uint), typeof(object[]) })]
		private static bool LogFastM_Prefix(uint mask, params object[] message) {
			if (!IsUTDebugEnabled()) {
				return true;
			}

			Debug.Log(String.Concat(message));
			return false;
		}

		[HarmonyPrefix, HarmonyPatch(methodName: nameof(UtDebug.LogWarning), argumentTypes: new[] { typeof(object), typeof(int) })]
		private static bool LogWarning_Prefix(object message, int priority) {
			if (!IsUTDebugEnabled()) {
				return true;
			}

			Debug.LogWarning(message);
			return false;
		}

		[HarmonyPrefix, HarmonyPatch(methodName: nameof(UtDebug.LogError), argumentTypes: new[] { typeof(object), typeof(int) })]
		private static bool LogError_Prefix(object message, int priority) {
			if (!IsUTDebugEnabled()) {
				return true;
			}

			Debug.LogError(message);
			return false;
		}
	}
}