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
	public static class CharacterPatcher {
		private static readonly ManualLogSource logger = LoggerUtils.GetLogger();

		[HarmonyPatch]
		private static class DoMovement_MoveNext_Patches {
			[HarmonyTargetMethod, UsedImplicitly]
			private static MethodInfo Find_DoMovement_MoveNext() {
				return PatcherUtils.FindIEnumeratorMoveNext(AccessTools.Method(typeof(Character), nameof(Character.DoMovement), new[] { typeof(Node) }));
			}
			
			[HarmonyTranspiler, UsedImplicitly]
			private static IEnumerable<CodeInstruction> DoMovement_Transpiler(IEnumerable<CodeInstruction> instructions) {
				List<CodeInstruction> instructionList = instructions.ToList();

				FieldInfo field__HasMoveAction = AccessTools.Field(typeof(Character), nameof(Character._HasMoveAction));
				MethodInfo patcherMethod_GetHasMoveAction = SymbolExtensions.GetMethodInfo(() => HackLogic.GetHasMoveAction(false));

				CodeReplacementPatch patch = new CodeReplacementPatch(
						expectedMatches: 1,
						insertInstructionSequence: new List<CodeInstruction> {
								new CodeInstruction(OpCodes.Call, patcherMethod_GetHasMoveAction)
						},
						postfixInstructionSequence: new List<CodeInstruction> {
								new CodeInstruction(OpCodes.Stfld, field__HasMoveAction)
						}
				);
				patch.ApplySafe(instructionList, logger);
				return instructionList;
			}
		}
		
		[HarmonyPatch]
		private static class UseAbility_MoveNext_Patches {
			[HarmonyTargetMethod, UsedImplicitly]
			private static MethodInfo Find_UseAbility_MoveNext() {
				return PatcherUtils.FindIEnumeratorMoveNext(AccessTools.Method(typeof(Character), nameof(Character.UseAbility), new[] { typeof(Character) }));
			}
			
			[UsedImplicitly]
			private static IEnumerable<CodeInstruction> UseAbility_Transpiler(IEnumerable<CodeInstruction> instructions) {
				List<CodeInstruction> instructionList = instructions.ToList();

				FieldInfo field__HasAbilityAction = AccessTools.Field(typeof(Character), nameof(Character._HasAbilityAction));
				MethodInfo patcherMethod_GetHasAbilityAction = SymbolExtensions.GetMethodInfo(() => HackLogic.GetHasAbilityAction(false));

				CodeReplacementPatch patch = new CodeReplacementPatch(
						expectedMatches: 1,
						insertInstructionSequence: new List<CodeInstruction> {
								new CodeInstruction(OpCodes.Call, patcherMethod_GetHasAbilityAction)
						},
						postfixInstructionSequence: new List<CodeInstruction> {
								new CodeInstruction(OpCodes.Stfld, field__HasAbilityAction)
						}
				);
				patch.ApplySafe(instructionList, logger);
				return instructionList;
			}
		}
	}
}