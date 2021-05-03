using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SoD_BaseMod.basemod;
using SoD_BaseMod.basemod.config;
using BlazingTwist_Core;
using UnityEngine;

namespace SoD_BaseMod.AsmFirstpass {
	[UsedImplicitly]
	public class KAUICursorManagerPatcher : RuntimePatcher {
		private static KAUICursorManagerPatcher instance;

		public override void ApplyPatches() {
			instance = this;

			Type originalType = typeof(KAUICursorManager);
			Type patcherType = typeof(KAUICursorManagerPatcher);

			MethodInfo methodStart = AccessTools.Method(originalType, "Start");
			MethodInfo methodSetCursor = AccessTools.Method(originalType, "SetCursor", new[] { typeof(string), typeof(bool) });

			HarmonyMethod patcherMethod = new HarmonyMethod(patcherType, nameof(CursorAssignmentTranspiler), new[] { typeof(IEnumerable<CodeInstruction>) });

			harmony.Patch(methodStart, null, null, patcherMethod);
			harmony.Patch(methodSetCursor, null, null, patcherMethod);
		}

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

		public static IEnumerable<CodeInstruction> CursorAssignmentTranspiler(IEnumerable<CodeInstruction> instructions) {
			MethodInfo cursorVisibilityModifier = AccessTools.Method(typeof(KAUICursorManagerPatcher), nameof(ApplyCursorVisibility), new[] { typeof(bool) });

			List<CodeInstruction> instructionList = new List<CodeInstruction>(instructions);
			List<CodeInstruction> result = new List<CodeInstruction>();
			int found = 0;
			int instructionCount = instructionList.Count;

			for (int i = 0; i < instructionCount; i++) {
				CodeInstruction inst = instructionList[i];

				if (inst.opcode == OpCodes.Call) {
					MethodInfo method = inst.operand as MethodInfo;
					if (method != null && method.Name.Equals("set_visible") && method.DeclaringType == typeof(Cursor)) {
						CodeInstruction baseInstruction = new CodeInstruction(OpCodes.Call, cursorVisibilityModifier);
						baseInstruction.labels.AddRange(inst.labels);
						inst.labels.Clear();
						result.Add(baseInstruction);
						result.Add(inst);
						found++;
						continue;
					}
				}

				result.Add(inst);
			}
			
			instance?.logger.LogInfo("CursorVisibility transpiler found " + found + " entry-points.");

			return result;
		}
	}
}