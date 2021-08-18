using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx.Logging;
using BTHarmonyUtils.TranspilerUtils;
using HarmonyLib;
using JetBrains.Annotations;
using SoD_BaseMod.config;
using UnityEngine;

namespace SoD_BaseMod {
	[HarmonyPatch(declaringType: typeof(KAUICursorManager))]
	public class KAUICursorManagerPatcher {
		private static readonly string loggerName = $"BT_{MethodBase.GetCurrentMethod().DeclaringType?.Name}";
		private static ManualLogSource Logger => _logger ?? (_logger = BepInEx.Logging.Logger.CreateLogSource(loggerName));
		private static ManualLogSource _logger;

		[UsedImplicitly]
		public static bool ApplyCursorVisibility(bool currentValue) {
			BTConfig config = BTDebugCamInputManager.GetConfigHolder().config;
			if (config == null) {
				return currentValue;
			}

			switch (config.cursorVisibility) {
				case BTCursorVisibility.Force:
					return true;
				case BTCursorVisibility.Hide:
					return false;
				case BTCursorVisibility.Default:
				default:
					return currentValue;
			}
		}

		[HarmonyTranspiler, HarmonyPatch(methodName: "Start", argumentTypes: new Type[] { })]
		private static IEnumerable<CodeInstruction> StartTranspiler(IEnumerable<CodeInstruction> instructions) {
			List<CodeInstruction> instructionList = instructions.ToList();
			GetCursorVisibilityPatch().ApplySafe(instructionList, Logger);
			return instructionList;
		}

		[HarmonyTranspiler, HarmonyPatch(methodName: nameof(KAUICursorManager.SetCursor), argumentTypes: new[] { typeof(string), typeof(bool) })]
		private static IEnumerable<CodeInstruction> SetCursorTranspiler(IEnumerable<CodeInstruction> instructions) {
			List<CodeInstruction> instructionList = instructions.ToList();
			GetCursorVisibilityPatch().ApplySafe(instructionList, Logger);
			return instructionList;
		}

		private static CodeReplacementPatch GetCursorVisibilityPatch() {
			return new CodeReplacementPatch(
					expectedMatches: -1,
					insertInstructionSequence: new List<CodeInstruction> {
							new CodeInstruction(OpCodes.Call, SymbolExtensions.GetMethodInfo(() => ApplyCursorVisibility(false)))
					},
					postfixInstructionSequence: new List<CodeInstruction> {
							new CodeInstruction(OpCodes.Call, AccessTools.PropertySetter(typeof(Cursor), nameof(Cursor.visible)))
					}
			);
		}
	}
}