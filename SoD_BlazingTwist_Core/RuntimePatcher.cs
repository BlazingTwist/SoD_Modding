using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Reflection;
using JetBrains.Annotations;

namespace SoD_BlazingTwist_Core {
	public abstract class RuntimePatcher {
		[PublicAPI] protected ManualLogSource logger;
		[PublicAPI] protected Harmony harmony;

		public void Initialize(ManualLogSource logger, Harmony harmony) {
			this.logger = logger;
			this.harmony = harmony;
		}

		public abstract void ApplyPatches();

		[PublicAPI]
		public void PatchPrefixMethod(Type originalType, Type patcherType, String methodName, Type[] argumentTypes = null, Type[] genericTypes = null) {
			MethodInfo original = AccessTools.Method(originalType, methodName, argumentTypes, genericTypes);
			MethodInfo patch = AccessTools.Method(patcherType, methodName, argumentTypes, genericTypes);
			harmony.Patch(original, new HarmonyMethod(patch));
		}

		[PublicAPI]
		public void PatchPostfixMethod(Type originalType, Type patcherType, String methodName, Type[] argumentTypes = null, Type[] genericTypes = null) {
			MethodInfo original = AccessTools.Method(originalType, methodName, argumentTypes, genericTypes);
			MethodInfo patch = AccessTools.Method(patcherType, methodName, argumentTypes, genericTypes);
			harmony.Patch(original, null, new HarmonyMethod(patch));
		}
	}
}