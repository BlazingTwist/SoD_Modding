using HarmonyLib;
using SoD_BlazingTwist_Core;
using System;
using System.Reflection;
using SoD_BaseMod.basemod;
using SquadTactics;

namespace SoD_BaseMod.asm
{
	[HarmonyPatch]
	public class UIChestPatcher : RuntimePatcher
	{
		public override void ApplyPatches() {
			Type originalType = typeof(UiChest);
			Type patcherType = typeof(UIChestPatcher);

			MethodInfo initChestOriginal = AccessTools.Method(originalType, "InitChest", null, null);

			HarmonyMethod initChestPrefix = new HarmonyMethod(AccessTools.Method(patcherType, "InitChestPrefix", new Type[] { typeof(UiChest) }, null));

			harmony.Patch(initChestOriginal, initChestPrefix);
		}

		public static bool ShouldOpenChest() {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			return hackConfig != null && hackConfig.squadTactics_autochest > 0;
		}

		private static bool InitChestPrefix(UiChest __instance) {
			if(ShouldOpenChest()) {
				OpenChest(__instance);
				return false;
			}
			return true;
		}

		// Transpiler replaced by reverse patching, keeping it around for future reference
		/*private static IEnumerable<CodeInstruction> InitChestTranspiler(IEnumerable<CodeInstruction> instructions) {
			List<CodeInstruction> result = new List<CodeInstruction>();
			bool found = false;
			foreach(CodeInstruction inst in instructions) {
				if(!found) {
					if(inst.opcode == OpCodes.Ldarg_0) {
						result.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(UIChestPatcher), "ShouldOpenChest", null, null)));
						Label label_dontOpen = new Label();
						result.Add(new CodeInstruction(OpCodes.Brfalse_S, label_dontOpen));
						result.Add(new CodeInstruction(OpCodes.Ldarg_0, null));
						result.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(UiChest), "OpenChest", null, null)));
						result.Add(new CodeInstruction(OpCodes.Ret, null));

						// attach branch label to our anchor
						inst.labels.Add(label_dontOpen);

						found = true;
					}
				}
				result.Add(inst);
			}

			if(!found && instance != null) {
				instance.logger.LogWarning("Unable to patch UiChest! Entrypoint not found.");
			}

			return result;
		}*/

		[HarmonyReversePatch]
		[HarmonyPatch(typeof(UiChest), "OpenChest")]
		public static void OpenChest(object instance) {
			/* dummy content */
			throw new NotImplementedException("Stub called, reverse patch has not been applied!");
		}
	}
}
