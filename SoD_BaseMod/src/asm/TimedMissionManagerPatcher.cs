using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx.Logging;
using BTHarmonyUtils.TranspilerUtils;
using HarmonyLib;
using SoD_BaseMod.config;
using SoD_BaseMod.utils;

namespace SoD_BaseMod {
	[HarmonyPatch(declaringType: typeof(TimedMissionManager))]
	public static class TimedMissionManagerPatcher {
		private static readonly ManualLogSource logger = LoggerUtils.GetLogger();

		[HarmonyPostfix, HarmonyPatch(methodName: "UpdateSlotStates", argumentTypes: new Type[] { })]
		private static void UpdateSlotStatesPostfix(List<TimedMissionSlotData> ___mTimedMissionSlotList) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if (hackConfig == null || !hackConfig.stableMission_instantCompletion) {
				return;
			}

			foreach (TimedMissionSlotData missionData in ___mTimedMissionSlotList.Where(missionData => missionData != null)) {
				missionData.pCoolDownDuration = 0;
				if (missionData.pMission != null) {
					missionData.pMission.Duration = 0;
				}
			}
		}

		[HarmonyPrefix, HarmonyPatch(methodName: nameof(TimedMissionManager.GetNextMission), argumentTypes: new[] { typeof(TimedMissionSlotData) })]
		private static void GetNextMissionPrefix(ref int ___mCheatPreferredMissionID) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if (hackConfig == null) {
				return;
			}

			___mCheatPreferredMissionID = hackConfig.stableMission_forceMissionID;
		}

		[HarmonyPostfix, HarmonyPatch(methodName: nameof(TimedMissionManager.GetNextMission), argumentTypes: new[] { typeof(TimedMissionSlotData) })]
		private static void GetNextMissionPostfix(TimedMission __result) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if (hackConfig == null || !hackConfig.stableMission_instantCompletion) {
				return;
			}

			__result.Duration = 0;
		}

		[HarmonyPrefix, HarmonyPatch(methodName: "IsMissionValid", argumentTypes: new[] { typeof(TimedMission) })]
		private static bool IsMissionValidPrefix(TimedMission mission, out bool __result) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			__result = hackConfig != null && hackConfig.stableMission_forceMissionID == mission.MissionID;
			return !__result;
		}

		[HarmonyPrefix, HarmonyPatch(methodName: nameof(TimedMissionManager.GetWinProbability), argumentTypes: new[] { typeof(int) })]
		private static bool GetWinProbabilityPrefix(ref float __result) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if (hackConfig == null || !hackConfig.stableMission_forceWin) {
				return true;
			}

			__result = 100;
			return false;
		}

		[HarmonyTranspiler,
		 HarmonyPatch(methodName: nameof(TimedMissionManager.CompleteMission),
				 argumentTypes: new[] { typeof(TimedMissionSlotData), typeof(TimedMissionCompletion) })]
		private static IEnumerable<CodeInstruction> CompleteMissionTranspiler(IEnumerable<CodeInstruction> instructions) {
			List<CodeInstruction> instructionList = new List<CodeInstruction>(instructions);

			MethodInfo method_SetAchievementByEntityIDs =
					SymbolExtensions.GetMethodInfo(() => WsWebService.SetAchievementByEntityIDs(0, null, null, null, null));

			MethodInfo patcherMethod_SetAchievementByEntityIDsHook =
					SymbolExtensions.GetMethodInfo(() => SetAchievementByEntityIDsHook(0, null, null, null, null));

			CodeReplacementPatch patch = new CodeReplacementPatch(
					expectedMatches: 1,
					targetInstructionSequence: new List<CodeInstruction> {
							new CodeInstruction(OpCodes.Call, method_SetAchievementByEntityIDs)
					},
					insertInstructionSequence: new List<CodeInstruction> {
							new CodeInstruction(OpCodes.Call, patcherMethod_SetAchievementByEntityIDsHook)
					}
			);
			patch.ApplySafe(instructionList, logger);
			return instructionList;
		}

		private static void SetAchievementByEntityIDsHook(int achievementId, Guid?[] petIDs, string inGroupID, WsServiceEventHandler inCallback,
				object inUserData) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if (hackConfig == null || hackConfig.stableMission_triggerRewardCount <= 0) {
				WsWebService.SetAchievementByEntityIDs(achievementId, petIDs, inGroupID, inCallback, inUserData);
				return;
			}

			for (int i = hackConfig.stableMission_triggerRewardCount; i > 0; i--) {
				WsWebService.SetAchievementByEntityIDs(achievementId, petIDs, inGroupID, inCallback, inUserData);
			}
		}
	}
}