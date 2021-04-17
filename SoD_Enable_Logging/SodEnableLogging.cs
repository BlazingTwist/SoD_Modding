using System.Reflection;
using BepInEx;
using HarmonyLib;
using JetBrains.Annotations;
using SoD_BlazingTwist_Core;
using UnityEngine;

namespace SoD_Enable_Logging
{
	[PublicAPI]
	[BepInPlugin(pluginGuid, pluginName, pluginVersion)]
	class SodEnableLogging : BaseUnityPlugin
	{
		public const string pluginGuid = "blazingtwist.enablelogging";
		public const string pluginName = "BlazingTwist SoD EnableLogging";
		public const string pluginVersion = "1.0.0";

		public static readonly string basePath = Application.dataPath + "/BlazingTwist/";

		public void Awake() {
			/*if(enableServiceCallLogging.Value) {
				filteredServiceCalls = Config.Bind(
					"Logging",
					"FilteredServiceCalls",
					string.Join(", ", new[]{
						WsServiceType.GET_STORE,
						WsServiceType.GET_COMMON_INVENTORY,
						WsServiceType.GET_ALL_RANKS,
						WsServiceType.GET_DETAILED_CHILD_LIST,
						WsServiceType.GET_USER_UPCOMING_MISSION_STATE,
						WsServiceType.GET_USER_ACTIVE_MISSION_STATE
					}.Select(type => type.ToString())),
					"List of the ServiceCalls to ignore when logging.");
			}*/

			Harmony harmony = new Harmony(pluginGuid);
			harmony.PatchAll();
			RuntimePatcherUtils.RunPatchers(Logger, harmony, RuntimePatcherUtils.GetTypesInNamespace(Assembly.GetExecutingAssembly(), "SoD_Enable_Logging.AsmFirstpass"));
		}
	}
}
