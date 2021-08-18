using BepInEx;
using HarmonyLib;
using JetBrains.Annotations;

namespace SoD_BaseMod {
	[PublicAPI]
	[BepInPlugin(pluginGuid, pluginName, pluginVersion)]
	public class SodBaseMod : BaseUnityPlugin {
		public const string pluginGuid = "blazingtwist.basemod";
		public const string pluginName = "BlazingTwist SoD BaseMod";
		public const string pluginVersion = "1.0.0";

		public void Awake() {
			BTDebugCamInputManager.AttachToScene();

			var harmony = new Harmony(pluginGuid);
			harmony.PatchAll(GetType().Assembly);
			CharacterPatcher.ApplyPatches(harmony);
		}
	}
}