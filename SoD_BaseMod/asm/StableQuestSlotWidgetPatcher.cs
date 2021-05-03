using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using BlazingTwist_Core;
using UnityEngine;

namespace SoD_BaseMod.asm {
	[UsedImplicitly]
	public class StableQuestSlotWidgetPatcher : RuntimePatcher {
		private static StableQuestSlotWidgetPatcher instance;

		public override void ApplyPatches() {
			instance = this;

			Type originalType = typeof(StableQuestSlotWidget);
			Type patcherType = typeof(StableQuestSlotWidgetPatcher);

			MethodInfo updateOriginal = AccessTools.Method(originalType, "Update");

			var updateTranspiler =
					new HarmonyMethod(patcherType, nameof(UpdateTranspiler), new[] { typeof(IEnumerable<CodeInstruction>) });

			harmony.Patch(updateOriginal, null, null, updateTranspiler);
		}

		private static IEnumerable<CodeInstruction> UpdateTranspiler(IEnumerable<CodeInstruction> instructions) {
			MethodInfo mathClamp = AccessTools.Method(typeof(Mathf), "Clamp", new[] { typeof(int), typeof(int), typeof(int) });

			List<CodeInstruction> instructionList = new List<CodeInstruction>(instructions);
			List<CodeInstruction> result = new List<CodeInstruction>();
			int found = 0;
			int instructionCount = instructionList.Count;

			for (int i = 0; i < instructionCount; i++) {
				CodeInstruction inst = instructionList[i];

				if (inst.opcode == OpCodes.Call && i + 1 < instructionCount) {
					var method = inst.operand as MethodInfo;
					CodeInstruction inst1 = instructionList[i + 1];

					if (method != null && method.DeclaringType == typeof(Mathf) && method.Name.Equals("FloorToInt")
							&& (inst1.opcode == OpCodes.Stloc || inst1.opcode == OpCodes.Stloc_S)) {
						result.Add(inst);
						result.Add(new CodeInstruction(OpCodes.Ldc_I4_0));
						result.Add(new CodeInstruction(OpCodes.Ldarg_0));
						result.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(StableQuestSlotWidget), "mStoryLogs")));
						result.Add(new CodeInstruction(OpCodes.Ldlen));
						result.Add(new CodeInstruction(OpCodes.Ldc_I4_1));
						result.Add(new CodeInstruction(OpCodes.Sub));
						result.Add(new CodeInstruction(OpCodes.Call, mathClamp));
						result.Add(inst1);

						found++;
						i += 1;
						continue;
					}
				}

				result.Add(inst);
			}

			if (found != 1) {
				instance?.logger.LogWarning("Update found " + found + " entry-points, but expected: 1");
			}

			return result;
		}
	}
}