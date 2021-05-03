using System;
using JetBrains.Annotations;
using SoD_BaseMod.basemod;
using SoD_BaseMod.basemod.config;
using BlazingTwist_Core;
using UnityEngine;

namespace SoD_Enable_Logging.AsmFirstpass {
	[UsedImplicitly]
	public class UtDebugPatcher : RuntimePatcher {
		public override void ApplyPatches() {
			Type originalType = typeof(UtDebug);
			Type patcherType = typeof(UtDebugPatcher);

			PatchPrefixMethod(originalType, patcherType, nameof(Log), new[] { typeof(object), typeof(int) });
			PatchPrefixMethod(originalType, patcherType, nameof(Log), new[] { typeof(object), typeof(uint) });
			PatchPrefixMethod(originalType, patcherType, nameof(LogFastP), new[] { typeof(int), typeof(object[]) });
			PatchPrefixMethod(originalType, patcherType, nameof(LogFastM), new[] { typeof(uint), typeof(object[]) });
			PatchPrefixMethod(originalType, patcherType, nameof(LogWarning), new[] { typeof(object), typeof(int) });
			PatchPrefixMethod(originalType, patcherType, nameof(LogError), new[] { typeof(object), typeof(int) });
		}

		private static bool IsUTDebugEnabled() {
			BTConfig config = BTDebugCamInputManager.GetConfigHolder().config;
			return config != null && config.enableUTDebug;
		}

		private static bool Log(object message, int priority) {
			if (!IsUTDebugEnabled()) {
				return true;
			}

			Debug.Log(message);
			return false;
		}

		private static bool Log(object message, uint mask) {
			if (!IsUTDebugEnabled()) {
				return true;
			}

			Debug.Log(message);
			return false;
		}

		private static bool LogFastP(int priority, params object[] message) {
			if (!IsUTDebugEnabled()) {
				return true;
			}

			Debug.Log(String.Concat(message));
			return false;
		}

		private static bool LogFastM(uint mask, params object[] message) {
			if (!IsUTDebugEnabled()) {
				return true;
			}

			Debug.Log(String.Concat(message));
			return false;
		}

		private static bool LogWarning(object message, int priority) {
			if (!IsUTDebugEnabled()) {
				return true;
			}

			Debug.LogWarning(message);
			return false;
		}

		private static bool LogError(object message, int priority) {
			if (!IsUTDebugEnabled()) {
				return true;
			}

			Debug.LogError(message);
			return false;
		}
	}
}