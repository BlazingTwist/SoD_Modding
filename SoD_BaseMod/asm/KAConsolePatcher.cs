using HarmonyLib;
using SoD_BaseMod.basemod;
using BlazingTwist_Core;
using System;
using System.Reflection;

namespace SoD_BaseMod.asm {
	[HarmonyPatch]
	public class KAConsolePatcher : RuntimePatcher {
		public override void ApplyPatches() {
			Type originalType = typeof(KAConsole);
			Type patcherType = typeof(KAConsolePatcher);

			MethodInfo getUnlockedOriginal = AccessTools.PropertyGetter(originalType, "pUnlocked");
			MethodInfo updateOriginal = AccessTools.Method(originalType, "Update");

			var getUnlockedPrefix = new HarmonyMethod(patcherType, nameof(GetUnlockedPrefix), new[] { typeof(bool).MakeByRefType() });
			var updatePrefix = new HarmonyMethod(patcherType, nameof(UpdatePrefix), new[] { typeof(KAConsole) });

			harmony.Patch(getUnlockedOriginal, getUnlockedPrefix);
			harmony.Patch(updateOriginal, updatePrefix);
		}

		private static bool GetUnlockedPrefix(out bool __result) {
			__result = true;
			return false;
		}

		private static bool UpdatePrefix(KAConsole __instance) {
			if (!BTDebugCamInputManager.IsKeyJustDown("ToggleOfficialConsole")) {
				return true;
			}

			if (IsConsoleVisibleReverse(__instance)) {
				__instance.CloseConsole();
			} else {
				__instance.ShowConsole();
			}

			return false;
		}

		[HarmonyReversePatch]
		[HarmonyPatch(typeof(KAConsole), "IsConsoleVisible")]
		public static bool IsConsoleVisibleReverse(object instance) {
			/* dummy content */
			throw new NotImplementedException("Stub called, reverse patch has not been applied!");
		}
	}
}