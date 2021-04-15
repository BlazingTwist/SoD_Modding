﻿using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using BepInEx.Logging;

namespace SoD_BlazingTwist_Core
{
	public static class RuntimePatcherUtils
	{
		public static void RunPatchers(ManualLogSource logger, Harmony harmony, Type[] types) {
			foreach(Type type in types) {
				RuntimePatcher patcher = Activator.CreateInstance(type) as RuntimePatcher;
				if(patcher != null) {
					logger.LogInfo("Patcher started: " + type.FullName);
					patcher.Initialize(logger, harmony);
					patcher.ApplyPatches();
					logger.LogInfo("Patcher finished: " + type.FullName);
				} else {
					logger.LogWarning("Potential namespace-conflict! Found non-pather class: " + type.FullName);
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
