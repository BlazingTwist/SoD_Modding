using System;
using System.Collections.Generic;
using HarmonyLib;
using SoD_BlazingTwist_Core;
using System.Reflection.Emit;
using System.Linq;
using System.Reflection;

namespace SoD_Enable_Cursor.AsmFirstpass
{
	class KAUICursorManagerPatcher : RuntimePatcher
	{
		private static KAUICursorManagerPatcher instance;

		public override void ApplyPatches() {
			instance = this;

			if(SodEnableCursor.GetCursorEnabled() != CursorVisibility.Default) {
				Type originalType = typeof(KAUICursorManager);
				Type patcherType = typeof(KAUICursorManagerPatcher);

				MethodInfo methodStart = AccessTools.Method(originalType, "Start", null, null);
				MethodInfo methodSetCursor = AccessTools.Method(originalType, "SetCursor", new Type[] { typeof(string), typeof(bool) }, null);

				HarmonyMethod patcherMethod = new HarmonyMethod(AccessTools.Method(patcherType, "RemoveCursorAssignments", new Type[] { typeof(IEnumerable<CodeInstruction>) }, null));

				harmony.Patch(methodStart, null, null, patcherMethod);
				harmony.Patch(methodSetCursor, null, null, patcherMethod);
			}
		}

		public static IEnumerable<CodeInstruction> RemoveCursorAssignments(IEnumerable<CodeInstruction> instructions) {
			List<CodeInstruction> instructionList = new List<CodeInstruction>(instructions);

			bool forceCursor = SodEnableCursor.GetCursorEnabled() == CursorVisibility.Force;
			for(int i = 1; i < instructionList.Count; i++) {
				CodeInstruction instruction = instructionList[i];
				if(instruction.opcode != OpCodes.Call) {
					continue;
				}

				MethodInfo callTarget = instruction.operand as MethodInfo;
				if(callTarget == null || !callTarget.Name.Equals("set_visible") || !callTarget.DeclaringType.IsEquivalentTo(typeof(UnityEngine.Cursor))) {
					continue;
				}

				bool found = false;
				for(int j = i - 1; j >= 0; j--) {
					CodeInstruction prevInstruction = instructionList[j];
					if(j == (i - 1)) {
						prevInstruction.opcode = forceCursor ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0;
						prevInstruction.operand = null;
						continue;
					}

					if(prevInstruction.opcode == OpCodes.Nop) {
						continue;
					}

					if(prevInstruction.opcode.StackBehaviourPush == StackBehaviour.Push0) {
						// Super jank condition, but seems to be reliable for now
						found = true;
						break;
					}


					prevInstruction.opcode = OpCodes.Nop;
					prevInstruction.operand = null;
				}

				if(!found) {
					instance.logger.LogWarning("Unable to find start of instructions for Cursor.set_visibility! CIL is probably faulty!");
				}
			}
			return instructionList.AsEnumerable();
		}
	}
}
