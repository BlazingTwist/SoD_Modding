using System;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using BepInEx.Configuration;

namespace SoD_BepInEx_Runtime
{
	[BepInPlugin(pluginGuid, pluginName, pluginVersion)]
	class BlazingTwistSodMod : BaseUnityPlugin
	{
		public const string pluginGuid = "blazingtwist.sodmod";
		public const string pluginName = "BlazingTwist SoD Modifications";
		public const string pluginVersion = "1.0.0";

		public static string basePath = Application.dataPath + "/BlazingTwist/";

		private static ConfigEntry<bool> enableServiceCallLogging;
		private static ConfigEntry<string> filteredServiceCalls;

		public static bool IsServiceCallLoggingEnabled() {
			if(enableServiceCallLogging == null) {
				return false;
			}
			return enableServiceCallLogging.Value;
		}

		public static WsServiceType[] GetFilteredServiceCalls() {
			if(filteredServiceCalls == null) {
				return null;
			}
			return filteredServiceCalls.Value
				.Split(',')
				.Select(name => {
					Enum.TryParse(name.Trim(), true, out WsServiceType type);
					return type;
				}).ToArray();
		}

		public void Awake() {
			enableServiceCallLogging = Config.Bind("Logging", "EnableServiceCallLogging", false, "Will log the contents of all ServiceCalls (WARNING: this might log millions of lines!)");
			if(enableServiceCallLogging.Value) {
				filteredServiceCalls = Config.Bind(
					"Logging",
					"FilteredServiceCalls",
					string.Join(", ", new WsServiceType[]{
						WsServiceType.GET_STORE,
						WsServiceType.GET_COMMON_INVENTORY,
						WsServiceType.GET_ALL_RANKS,
						WsServiceType.GET_DETAILED_CHILD_LIST,
						WsServiceType.GET_USER_UPCOMING_MISSION_STATE,
						WsServiceType.GET_USER_ACTIVE_MISSION_STATE
					}.Select(type => type.ToString())),
					"List of the ServiceCalls to ignore when logging.");
			}

			Harmony harmony = new Harmony(pluginGuid);
			RunPatchers(GetTypesInNamespace("SoD_BepInEx_Runtime.AsmFirstpass"), harmony);
			RunPatchers(GetTypesInNamespace("SoD_BepInEx_Runtime.Asm"), harmony);
		}

		private void RunPatchers(Type[] types, Harmony harmony) {
			foreach(Type type in types) {
				Logger.LogInfo("Starting Patcher: " + type.FullName);
				RuntimePatcher patcher = Activator.CreateInstance(type) as RuntimePatcher;
				patcher.Initialize(Logger, harmony);
				patcher.ApplyPatches();
			}
		}

		private Type[] GetTypesInNamespace(string nameSpace) {
			return Assembly.GetExecutingAssembly().GetTypes()
				.Where(type => String.Equals(type.Namespace, nameSpace, StringComparison.OrdinalIgnoreCase))
				.ToArray();
		}
	}
}
