using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using SoD_BlazingTwist_Core;
using System;
using System.Reflection;

namespace SoD_Enable_Cursor
{
	[BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class SodEnableCursor : BaseUnityPlugin
    {
		public const string pluginGuid = "blazingtwist.enablecursor";
		public const string pluginName = "BlazingTwist SoD EnableCursor";
		public const string pluginVersion = "1.0.0";

		private static ManualLogSource logger;
		private static ConfigEntry<string> cursorVisibility;

		public static CursorVisibility GetCursorEnabled() {
			if(cursorVisibility == null) {
				return CursorVisibility.Force;
			}
			CursorVisibility visibility;
			if(!Enum.TryParse(cursorVisibility.Value, true, out visibility)){
				if(logger != null) {
					logger.LogError("invalid cursorVisibility in config: " + cursorVisibility.Value);
				}
				return CursorVisibility.Force;
			}
			return visibility;
		}

		public void Awake() {
			logger = Logger;

			CursorVisibility[] visibilities = (CursorVisibility[])Enum.GetValues(typeof(CursorVisibility));
			cursorVisibility = Config.Bind(
				"Cursor",
				"CursorVisibility",
				"FORCE",
				"Show the system cursor while in-game, allowed values: [" + String.Join(", ", visibilities) + "]");

			Harmony harmony = new Harmony(pluginGuid);
			RuntimePatcherUtils.RunPatchers(Logger, harmony, RuntimePatcherUtils.GetTypesInNamespace(Assembly.GetExecutingAssembly(), "SoD_Enable_Cursor.AsmFirstpass"));
		}
	}
}
