using System.Reflection;
using BepInEx;
using HarmonyLib;
using JetBrains.Annotations;
using BlazingTwist_Core;
using UnityEngine;

namespace SoD_Enable_Logging
{
	[PublicAPI]
	[BepInPlugin(pluginGuid, pluginName, pluginVersion)]
	public class SodEnableLogging : BaseUnityPlugin
	{
		public const string pluginGuid = "blazingtwist.enablelogging";
		public const string pluginName = "BlazingTwist SoD EnableLogging";
		public const string pluginVersion = "1.0.0";

		public static string basePath;

		public void Awake() {
			basePath = Application.dataPath + "/BlazingTwist/";
		
			var harmony = new Harmony(pluginGuid);
			harmony.PatchAll();
			RuntimePatcherUtils.RunPatchers(Logger, harmony, RuntimePatcherUtils.GetTypesInNamespace(Assembly.GetExecutingAssembly(), "SoD_Enable_Logging.AsmFirstpass"));
		}
	}
}
