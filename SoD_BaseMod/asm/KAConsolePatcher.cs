using HarmonyLib;
using SoD_BaseMod.basemod;
using SoD_BlazingTwist_Core;
using System;
using System.Reflection;

namespace SoD_BaseMod.asm
{
	[HarmonyPatch]
	public class KAConsolePatcher : RuntimePatcher
	{
		public override void ApplyPatches() {
			Type originalType = typeof(KAConsole);
			Type patcherType = typeof(KAConsolePatcher);

			MethodInfo getUnlockedOriginal = AccessTools.PropertyGetter(originalType, "pUnlocked");
			MethodInfo updateOriginal = AccessTools.Method(originalType, "Update", null, null);

			HarmonyMethod getUnlockedPrefix = new HarmonyMethod(AccessTools.Method(patcherType, "GetUnlockedPrefix", new Type[] { typeof(bool).MakeByRefType() }, null));
			HarmonyMethod updatePrefix = new HarmonyMethod(AccessTools.Method(patcherType, "UpdatePrefix", new Type[] { typeof(KAConsole) }, null));

			harmony.Patch(getUnlockedOriginal, getUnlockedPrefix);
			harmony.Patch(updateOriginal, updatePrefix);
		}

		private static bool GetUnlockedPrefix(ref bool __result) {
			__result = true;
			return false;
		}

		private static bool UpdatePrefix(KAConsole __instance) {
			if(BTDebugCamInputManager.IsKeyJustDown("ToggleOfficialConsole")) {
				if(IsConsoleVisibleReverse(__instance)) {
					__instance.CloseConsole();
				} else {
					__instance.ShowConsole();
				}
				return false;
			}
			return true;
		}

		[HarmonyReversePatch]
		[HarmonyPatch(typeof(KAConsole), "IsConsoleVisible")]
		public static bool IsConsoleVisibleReverse(object instance) {
			/* dummy content */
			throw new NotImplementedException("Stub called, reverse patch has not been applied!");
		}
	}
}
