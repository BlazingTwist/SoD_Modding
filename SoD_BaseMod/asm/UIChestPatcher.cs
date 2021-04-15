using HarmonyLib;
using SoD_BlazingTwist_Core;
using System;
using System.Reflection;
using SoD_BaseMod.basemod;
using SquadTactics;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace SoD_BaseMod.asm
{
	public class UIChestPatcher : RuntimePatcher
	{
		private static UIChestPatcher instance;

		public override void ApplyPatches() {
			instance = this;
			
			Type originalType = typeof(UiChest);
			Type patcherType = typeof(UIChestPatcher);

			MethodInfo initChestOriginal = AccessTools.Method(originalType, "InitChest", null, null);
			
			HarmonyMethod initChestTranspiler = new HarmonyMethod(AccessTools.Method(patcherType, "InitChestTranspiler", null, null));

			harmony.Patch(initChestOriginal, null, null, initChestTranspiler);
		}

		public static bool ShouldOpenChest() {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			return hackConfig != null && hackConfig.squadTactics_autochest > 0;
		}

		private static IEnumerable<CodeInstruction> InitChestTranspiler(IEnumerable<CodeInstruction> instructions) {
			bool found = false;
			foreach(CodeInstruction inst in instructions) {
				if(!found) {
					if(inst.opcode == OpCodes.Ldarg_0) {
						yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(UIChestPatcher), "ShouldOpenChest", null, null));
						Label hackDisabledLabel = new Label();
						yield return new CodeInstruction(OpCodes.Brfalse_S, hackDisabledLabel);
						yield return new CodeInstruction(OpCodes.Ldarg_0, null);
						yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(UiChest), "OpenChest", null, null));
						yield return new CodeInstruction(OpCodes.Ret, null);

						// attach branch label to our anchor
						inst.labels.Add(hackDisabledLabel);

						found = true;
					}
				}
				yield return inst;
			}

			if(!found && instance != null) {
				instance.logger.LogWarning("Unable to patch UiChest! Entrypoint not found.");
			}
		}
	}
}
