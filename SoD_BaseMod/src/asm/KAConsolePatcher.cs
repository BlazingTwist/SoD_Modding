using System;
using HarmonyLib;
using SoD_BaseMod.Extensions;

namespace SoD_BaseMod {
	[HarmonyPatch(declaringType: typeof(KAConsole))]
	public static class KAConsolePatcher {
		[HarmonyPrefix, HarmonyPatch(methodName: "get_" + nameof(KAConsole.pUnlocked), argumentTypes: new Type[] { })]
		private static bool Get_pUnlockedPrefix(out bool __result) {
			__result = true;
			return false;
		}


		[HarmonyPrefix, HarmonyPatch(methodName: "Update", argumentTypes: new Type[] { })]
		private static bool UpdatePrefix(KAConsole __instance) {
			if (!BTDebugCamInputManager.IsKeyJustDown("ToggleOfficialConsole")) {
				return true;
			}

			if (__instance.IsConsoleVisible()) {
				__instance.CloseConsole();
			} else {
				__instance.ShowConsole();
			}

			return false;
		}
	}
}