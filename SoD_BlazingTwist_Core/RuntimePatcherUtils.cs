using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using BepInEx.Logging;
using JetBrains.Annotations;

namespace SoD_BlazingTwist_Core {
	[PublicAPI]
	public static class RuntimePatcherUtils {
		public static void RunPatchers(ManualLogSource logger, Harmony harmony, Type[] types) {
			foreach (Type type in types) {
				if (typeof(RuntimePatcher).IsAssignableFrom(type)) {
					if (Activator.CreateInstance(type) is RuntimePatcher patcher) {
						try {
							logger.LogInfo("Patcher started: " + type.FullName);
							patcher.Initialize(logger, harmony);
							patcher.ApplyPatches();
							logger.LogInfo("Patcher finished: " + type.FullName);
						} catch (Exception e) {
							logger.LogError("Patcher threw exception: " + e);
						}
					}
				}
			}
		}

		public static Type[] GetTypesInNamespace(Assembly assembly, string nameSpace) {
			return assembly.GetTypes()
					.Where(type => String.Equals(type.Namespace, nameSpace, StringComparison.OrdinalIgnoreCase))
					.ToArray();
		}
	}
}