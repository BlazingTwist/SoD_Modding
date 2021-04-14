using System;
using UnityEngine;

namespace SoD_BepInEx_Runtime.AsmFirstpass
{
	public class UtDebugPatcher : RuntimePatcher
	{
		public override void ApplyPatches() {
			Type originalType = typeof(UtDebug);
			Type patcherType = typeof(UtDebugPatcher);

			PatchPrefixMethod(originalType, patcherType, "Log", new Type[] { typeof(object), typeof(int) });
			PatchPrefixMethod(originalType, patcherType, "Log", new Type[] { typeof(object), typeof(uint) });
			PatchPrefixMethod(originalType, patcherType, "LogFastP", new Type[] { typeof(int), typeof(object[]) });
			PatchPrefixMethod(originalType, patcherType, "LogFastM", new Type[] { typeof(uint), typeof(object[]) });
			PatchPrefixMethod(originalType, patcherType, "LogWarning", new Type[] { typeof(object), typeof(int) });
			PatchPrefixMethod(originalType, patcherType, "LogError", new Type[] { typeof(object), typeof(int) });

			logger.LogInfo("UtDebug: finished patching");
		}

		public static bool Log(object message, int priority) {
			Debug.Log(message);
			return false;
		}

		public static bool Log(object message, uint mask) {
			Debug.Log(message);
			return false;
		}

		public static bool LogFastP(int priority, params object[] message) {
			Debug.Log(String.Concat(message));
			return false;
		}

		public static bool LogFastM(uint mask, params object[] message) {
			Debug.Log(String.Concat(message));
			return false;
		}

		public static bool LogWarning(object message, int priority) {
			Debug.LogWarning(message);
			return false;
		}

		public static bool LogError(object message, int priority) {
			Debug.LogError(message);
			return false;
		}
	}
}
