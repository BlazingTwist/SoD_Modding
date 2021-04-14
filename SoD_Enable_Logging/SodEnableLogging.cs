using System;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using SoD_BlazingTwist_Core;
using UnityEngine;

namespace SoD_Enable_Logging
{
	[BepInPlugin(pluginGuid, pluginName, pluginVersion)]
	class SodEnableLogging : BaseUnityPlugin
	{
		public const string pluginGuid = "blazingtwist.enablelogging";
		public const string pluginName = "BlazingTwist SoD EnableLogging";
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
			RuntimePatcherUtils.RunPatchers(Logger, harmony, RuntimePatcherUtils.GetTypesInNamespace("SoD_Enable_Logging.AsmFirstpass"));
			RuntimePatcherUtils.RunPatchers(Logger, harmony, RuntimePatcherUtils.GetTypesInNamespace("SoD_Enable_Logging.Asm"));
		}
	}
}
