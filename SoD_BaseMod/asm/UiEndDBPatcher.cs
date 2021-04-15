using HarmonyLib;
using SoD_BaseMod.basemod;
using SoD_BlazingTwist_Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace SoD_BaseMod.asm
{
	public class UiEndDBPatcher : RuntimePatcher
	{
		private static UiEndDBPatcher instance;

		public override void ApplyPatches() {
			instance = this;

			Type originalType = typeof(SquadTactics.UiEndDB);
			Type patcherType = typeof(UiEndDBPatcher);

			MethodInfo setRewardsOriginal = AccessTools.Method(originalType, "SetRewards", null, null);

			HarmonyMethod setRewardsTranspiler = new HarmonyMethod(AccessTools.Method(patcherType, "SetRewardsTranspiler", new Type[] { typeof(IEnumerable<CodeInstruction>) }, null));

			harmony.Patch(setRewardsOriginal, null, null, setRewardsTranspiler);
		}

		public static int GetChestCount(int normalCount) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if(hackConfig != null && hackConfig.squadTactics_autochest > 0) {
				return hackConfig.squadTactics_autochest;
			}
			return normalCount;
		}

		private static IEnumerable<CodeInstruction> SetRewardsTranspiler(IEnumerable<CodeInstruction> instructions) {
			MethodInfo chestCountProvider = AccessTools.Method(typeof(UiEndDBPatcher), "GetChestCount", new Type[] { typeof(int) }, null);

			List<CodeInstruction> instructionList = new List<CodeInstruction>(instructions);
			List<CodeInstruction> result = new List<CodeInstruction>();
			int found = 0;
			int instructionCount = instructionList.Count;
			for(int i = 0; i < instructionCount; i++) {
				CodeInstruction inst = instructionList[i];

				if(inst.opcode == OpCodes.Ldarg_0 && (i + 2) < instructionCount) {
					CodeInstruction inst1 = instructionList[i + 1];
					CodeInstruction inst2 = instructionList[i + 2];

					if(inst1.opcode == OpCodes.Ldfld && inst2.opcode == OpCodes.Ldfld) {
						FieldInfo field1 = inst1.operand as FieldInfo;
						FieldInfo field2 = inst2.operand as FieldInfo;

						if(field1 != null && field2 != null
							&& field1.DeclaringType == typeof(SquadTactics.UiEndDB) && field1.Name.Equals("mResultInfo")
							&& field2.DeclaringType == typeof(SquadTactics.UiEndDB.ResultInfo) && field2.Name.Equals("_LockedChests")) {

							found++;
							i += 2;
							result.Add(inst);
							result.Add(inst1);
							result.Add(inst2);
							result.Add(new CodeInstruction(OpCodes.Call, chestCountProvider));
							continue;
						}
					}
				}
				result.Add(inst);
			}

			if(found != 1 && instance != null) {
				instance.logger.LogWarning("SetRewards found " + found + " entrypoints, but expected: 1");
			}

			return result;
		}
	}
}
