using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx.Logging;
using BTHarmonyUtils.TranspilerUtils;
using HarmonyLib;
using JetBrains.Annotations;
using SoD_BaseMod.config;
using SoD_BaseMod.utils;
using SquadTactics;

namespace SoD_BaseMod {
	[HarmonyPatch(declaringType: typeof(UiEndDB))]
	public static class UiEndDBPatcher {
		private static readonly ManualLogSource logger = LoggerUtils.GetLogger();

		[UsedImplicitly]
		public static int GetChestCount(int normalCount) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if (hackConfig != null && hackConfig.squadTactics_autochest > 0) {
				return hackConfig.squadTactics_autochest;
			}

			return normalCount;
		}

		[HarmonyTranspiler, HarmonyPatch(methodName: nameof(UiEndDB.SetRewards), argumentTypes: new Type[] { })]
		private static IEnumerable<CodeInstruction> SetRewardsTranspiler(IEnumerable<CodeInstruction> instructions) {
			List<CodeInstruction> instructionList = new List<CodeInstruction>(instructions);

			FieldInfo field_mResultInfo = AccessTools.Field(typeof(UiEndDB), "mResultInfo");
			FieldInfo field__LockedChests = AccessTools.Field(typeof(UiEndDB.ResultInfo), nameof(UiEndDB.ResultInfo._LockedChests));

			MethodInfo patcherMethod_GetChestCount = SymbolExtensions.GetMethodInfo(() => GetChestCount(0));

			// replace
			//  this.mResultInfo._LockedChests
			// with
			//  GetChestCount(this.mResultInfo._LockedChests)
			CodeReplacementPatch patch = new CodeReplacementPatch(
					expectedMatches: 1,
					prefixInstructionSequence: new List<CodeInstruction> {
							new CodeInstruction(OpCodes.Ldarg),
							new CodeInstruction(OpCodes.Ldfld, field_mResultInfo),
							new CodeInstruction(OpCodes.Ldfld, field__LockedChests)
					},
					insertInstructionSequence: new List<CodeInstruction> {
							new CodeInstruction(OpCodes.Call, patcherMethod_GetChestCount)
					}
			);
			patch.ApplySafe(instructionList, logger);
			return instructionList;
		}
	}
}