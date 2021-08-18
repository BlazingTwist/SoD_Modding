using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx.Logging;
using BTHarmonyUtils;
using BTHarmonyUtils.TranspilerUtils;
using HarmonyLib;
using JetBrains.Annotations;
using SoD_BaseMod.config;
using SquadTactics;

namespace SoD_BaseMod {
	public static class CharacterPatcher {
		private static readonly string loggerName = $"BT_{MethodBase.GetCurrentMethod().DeclaringType?.Name}";
		private static ManualLogSource Logger => _logger ?? (_logger = BepInEx.Logging.Logger.CreateLogSource(loggerName));
		private static ManualLogSource _logger;

		public static void ApplyPatches(Harmony harmony) {
			harmony.Patch(
					original: PatcherUtils.FindIEnumeratorMoveNext(AccessTools.Method(typeof(Character), nameof(Character.DoMovement), new[] { typeof(Node) })),
					transpiler: new HarmonyMethod(SymbolExtensions.GetMethodInfo(() => DoMovement_Transpiler(null)))
			);
			harmony.Patch(
					original: PatcherUtils.FindIEnumeratorMoveNext(AccessTools.Method(typeof(Character), nameof(Character.UseAbility),
							new[] { typeof(Character) })),
					transpiler: new HarmonyMethod(SymbolExtensions.GetMethodInfo(() => UseAbility_Transpiler(null)))
			);
		}

		[UsedImplicitly]
		public static bool GetHasMoveAction(bool fallback) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if (hackConfig != null && hackConfig.squadTactics_infiniteMoves) {
				return true;
			}
			return fallback;
		}

		[UsedImplicitly]
		public static bool GetHasAbilityAction(bool fallback) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if (hackConfig != null && hackConfig.squadTactics_infiniteActions) {
				return true;
			}
			return fallback;
		}

		[UsedImplicitly]
		private static IEnumerable<CodeInstruction> DoMovement_Transpiler(IEnumerable<CodeInstruction> instructions) {
			List<CodeInstruction> instructionList = instructions.ToList();

			FieldInfo field__HasMoveAction = AccessTools.Field(typeof(Character), nameof(Character._HasMoveAction));
			MethodInfo patcherMethod_GetHasMoveAction = SymbolExtensions.GetMethodInfo(() => GetHasMoveAction(false));

			CodeReplacementPatch patch = new CodeReplacementPatch(
					expectedMatches: 1,
					insertInstructionSequence: new List<CodeInstruction> {
							new CodeInstruction(OpCodes.Call, patcherMethod_GetHasMoveAction)
					},
					postfixInstructionSequence: new List<CodeInstruction> {
							new CodeInstruction(OpCodes.Stfld, field__HasMoveAction)
					}
			);
			patch.ApplySafe(instructionList, Logger);
			return instructionList;
		}

		[UsedImplicitly]
		private static IEnumerable<CodeInstruction> UseAbility_Transpiler(IEnumerable<CodeInstruction> instructions) {
			List<CodeInstruction> instructionList = instructions.ToList();

			FieldInfo field__HasAbilityAction = AccessTools.Field(typeof(Character), nameof(Character._HasAbilityAction));
			MethodInfo patcherMethod_GetHasAbilityAction = SymbolExtensions.GetMethodInfo(() => GetHasAbilityAction(false));

			CodeReplacementPatch patch = new CodeReplacementPatch(
					expectedMatches: 1,
					insertInstructionSequence: new List<CodeInstruction> {
							new CodeInstruction(OpCodes.Call, patcherMethod_GetHasAbilityAction)
					},
					postfixInstructionSequence: new List<CodeInstruction> {
							new CodeInstruction(OpCodes.Stfld, field__HasAbilityAction)
					}
			);
			patch.ApplySafe(instructionList, Logger);
			return instructionList;
		}
	}
}