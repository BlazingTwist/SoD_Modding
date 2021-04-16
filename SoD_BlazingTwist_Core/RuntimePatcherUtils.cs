using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using BepInEx.Logging;
using JetBrains.Annotations;

namespace SoD_BlazingTwist_Core
{
	public static class RuntimePatcherUtils
	{
		[PublicAPI]
		public static void RunPatchers(ManualLogSource logger, Harmony harmony, Type[] types) {
			foreach(Type type in types) {
				if(typeof(RuntimePatcher).IsAssignableFrom(type)) {
					if(Activator.CreateInstance(type) is RuntimePatcher patcher) {
						try {
							logger.LogInfo("Patcher started: " + type.FullName);
							patcher.Initialize(logger, harmony);
							patcher.ApplyPatches();
							logger.LogInfo("Patcher finished: " + type.FullName);
						} catch(Exception e) {
							logger.LogError("Patcher threw exception: " + e);
						}
					}
				}
			}
		}

		[PublicAPI]
		public static Type[] GetTypesInNamespace(Assembly assembly, string nameSpace) {
			return assembly.GetTypes()
				.Where(type => String.Equals(type.Namespace, nameSpace, StringComparison.OrdinalIgnoreCase))
				.ToArray();
		}
	}
}
