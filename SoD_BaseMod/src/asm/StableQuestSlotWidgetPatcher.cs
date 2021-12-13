using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx.Logging;
using BTHarmonyUtils.TranspilerUtils;
using HarmonyLib;
using SoD_BaseMod.utils;
using UnityEngine;

namespace SoD_BaseMod {
	[HarmonyPatch(declaringType: typeof(StableQuestSlotWidget))]
	public static class StableQuestSlotWidgetPatcher {
		private static readonly ManualLogSource logger = LoggerUtils.GetLogger();

		[HarmonyTranspiler, HarmonyPatch(methodName: "Update", argumentTypes: new Type[] { })]
		private static IEnumerable<CodeInstruction> UpdateTranspiler(IEnumerable<CodeInstruction> instructions) {
			List<CodeInstruction> instructionList = new List<CodeInstruction>(instructions);

			FieldInfo field_mStoryLogs = AccessTools.Field(typeof(StableQuestSlotWidget), "mStoryLogs");
			
			MethodInfo method_FloorToInt = SymbolExtensions.GetMethodInfo(() => Mathf.FloorToInt(0f));
			MethodInfo method_Clamp = SymbolExtensions.GetMethodInfo(() => Mathf.Clamp(0, 0, 0));
			
			// replace
			//  int num2 = Mathf.FloorToInt(<...>)
			// with
			//  int num2 = Mathf.Clamp(Mathf.FloorToInt(<...>), 0, this.mStoryLogs.length - 1)
			CodeReplacementPatch patch = new CodeReplacementPatch(
					expectedMatches: 1,
					prefixInstructionSequence: new List<CodeInstruction> {
							new CodeInstruction(OpCodes.Call, method_FloorToInt)
					},
					insertInstructionSequence: new List<CodeInstruction> {
							new CodeInstruction(OpCodes.Ldc_I4_0),
							new CodeInstruction(OpCodes.Ldarg_0),
							new CodeInstruction(OpCodes.Ldfld, field_mStoryLogs),
							new CodeInstruction(OpCodes.Ldlen),
							new CodeInstruction(OpCodes.Ldc_I4_1),
							new CodeInstruction(OpCodes.Sub),
							new CodeInstruction(OpCodes.Call, method_Clamp)
					},
					postfixInstructionSequence: new List<CodeInstruction> {
							new CodeInstruction(OpCodes.Stloc_S, 5)
					}
			);
			patch.ApplySafe(instructionList, logger);
			return instructionList;
		}
	}
}