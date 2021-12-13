using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx.Logging;
using BTHarmonyUtils;
using BTHarmonyUtils.TranspilerUtils;
using HarmonyLib;
using JetBrains.Annotations;
using SoD_BaseMod.utils;
using SquadTactics;

namespace SoD_BaseMod {
	[HarmonyPatch]
	public static class AbilityPatcher {
		private static readonly ManualLogSource logger = LoggerUtils.GetLogger();

		[HarmonyTargetMethod, UsedImplicitly]
		private static MethodInfo Find_Activate_MoveNext() {
			return PatcherUtils.FindIEnumeratorMoveNext(AccessTools.Method(typeof(Ability), nameof(Ability.Activate), new[] { typeof(Character), typeof(Character) }));
		}

		[HarmonyTranspiler, UsedImplicitly]
		private static IEnumerable<CodeInstruction> Activate_Transpiler(IEnumerable<CodeInstruction> instructions) {
			List<CodeInstruction> instructionList = instructions.ToList();

			FieldInfo field_mCurrentCooldown = AccessTools.Field(typeof(Ability), "mCurrentCooldown");
			MethodInfo patcherMethod_GetCooldown = SymbolExtensions.GetMethodInfo(() => HackLogic.GetAbilityCooldown(0));

			CodeReplacementPatch patch = new CodeReplacementPatch(
					expectedMatches: 1,
					insertInstructionSequence: new List<CodeInstruction> {
							new CodeInstruction(OpCodes.Call, patcherMethod_GetCooldown)
					},
					postfixInstructionSequence: new List<CodeInstruction> {
							new CodeInstruction(OpCodes.Stfld, field_mCurrentCooldown)
					}
			);
			patch.ApplySafe(instructionList, logger);
			return instructionList;
		}
	}
}