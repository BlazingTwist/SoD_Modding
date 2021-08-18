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
	public static class AbilityPatcher {
		private static readonly string loggerName = $"BT_{MethodBase.GetCurrentMethod().DeclaringType?.Name}";
		private static ManualLogSource Logger => _logger ?? (_logger = BepInEx.Logging.Logger.CreateLogSource(loggerName));
		private static ManualLogSource _logger;

		public static void ApplyPatches(Harmony harmony) {
			harmony.Patch(
					original: PatcherUtils.FindIEnumeratorMoveNext(AccessTools.Method(typeof(Ability), nameof(Ability.Activate),
							new[] { typeof(Character), typeof(Character) })),
					transpiler: new HarmonyMethod(SymbolExtensions.GetMethodInfo(() => Activate_Transpiler(null)))
			);
		}

		[UsedImplicitly]
		private static int GetCooldown(int fallback) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if (hackConfig != null && hackConfig.squadTactics_infiniteActions) {
				return 0;
			}
			return fallback;
		}
		
		[UsedImplicitly]
		private static IEnumerable<CodeInstruction> Activate_Transpiler(IEnumerable<CodeInstruction> instructions) {
			List<CodeInstruction> instructionList = instructions.ToList();

			FieldInfo field_mCurrentCooldown = AccessTools.Field(typeof(Ability), "mCurrentCooldown");
			MethodInfo patcherMethod_GetCooldown = SymbolExtensions.GetMethodInfo(() => GetCooldown(0));

			CodeReplacementPatch patch = new CodeReplacementPatch(
					expectedMatches: 1,
					insertInstructionSequence: new List<CodeInstruction> {
							new CodeInstruction(OpCodes.Call, patcherMethod_GetCooldown)
					},
					postfixInstructionSequence: new List<CodeInstruction> {
							new CodeInstruction(OpCodes.Stfld, field_mCurrentCooldown)
					}
			);
			patch.ApplySafe(instructionList, Logger);
			return instructionList;
		}
	}
}