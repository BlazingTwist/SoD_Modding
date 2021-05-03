using HarmonyLib;
using BlazingTwist_Core;
using System;
using System.Reflection;
using SoD_BaseMod.basemod;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using JetBrains.Annotations;
using SoD_BaseMod.basemod.config;

namespace SoD_BaseMod.asm {
	[UsedImplicitly]
	public class TimedMissionManagerPatcher : RuntimePatcher {
		private static TimedMissionManagerPatcher instance;

		public override void ApplyPatches() {
			instance = this;

			Type originalType = typeof(TimedMissionManager);
			Type patcherType = typeof(TimedMissionManagerPatcher);

			MethodInfo updateSlotStatesOriginal = AccessTools.Method(originalType, "UpdateSlotStates");
			MethodInfo getNextMissionOriginal = AccessTools.Method(originalType, "GetNextMission", new[] { typeof(TimedMissionSlotData) });
			MethodInfo isMissionValidOriginal = AccessTools.Method(originalType, "IsMissionValid", new[] { typeof(TimedMission) });
			MethodInfo getWinProbabilityOriginal = AccessTools.Method(originalType, "GetWinProbability", new[] { typeof(int) });
			MethodInfo completeMissionOriginal =
					AccessTools.Method(originalType, "CompleteMission", new[] { typeof(TimedMissionSlotData), typeof(TimedMissionCompletion) });

			var updateSlotStatesPostfix =
					new HarmonyMethod(patcherType, nameof(UpdateSlotStatesPostfix), new[] { typeof(List<TimedMissionSlotData>) });
			var getNextMissionPrefix =
					new HarmonyMethod(patcherType, nameof(GetNextMissionPrefix), new[] { typeof(int).MakeByRefType() });
			var getNextMissionPostfix =
					new HarmonyMethod(patcherType, nameof(GetNextMissionPostfix), new[] { typeof(TimedMission) });
			var isMissionValidPrefix =
					new HarmonyMethod(patcherType, nameof(IsMissionValidPrefix), new[] { typeof(TimedMission), typeof(bool).MakeByRefType() });
			var getWinProbabilityPrefix =
					new HarmonyMethod(patcherType, nameof(GetWinProbabilityPrefix), new[] { typeof(float).MakeByRefType() });
			var completeMissionTranspiler =
					new HarmonyMethod(patcherType, nameof(CompleteMissionTranspiler), new[] { typeof(IEnumerable<CodeInstruction>) });

			harmony.Patch(updateSlotStatesOriginal, null, updateSlotStatesPostfix);
			harmony.Patch(getNextMissionOriginal, getNextMissionPrefix, getNextMissionPostfix);
			harmony.Patch(isMissionValidOriginal, isMissionValidPrefix);
			harmony.Patch(getWinProbabilityOriginal, getWinProbabilityPrefix);
			harmony.Patch(completeMissionOriginal, null, null, completeMissionTranspiler);
		}

		// ReSharper disable once ParameterTypeCanBeEnumerable.Local
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

		private static void GetNextMissionPrefix(ref int ___mCheatPreferredMissionID) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if (hackConfig == null) {
				return;
			}

			___mCheatPreferredMissionID = hackConfig.stableMission_forceMissionID;
		}

		private static void GetNextMissionPostfix(TimedMission __result) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if (hackConfig == null || !hackConfig.stableMission_instantCompletion) {
				return;
			}

			__result.Duration = 0;
		}

		private static bool IsMissionValidPrefix(TimedMission mission, out bool __result) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			__result = hackConfig != null && hackConfig.stableMission_forceMissionID == mission.MissionID;
			return !__result;
		}

		private static bool GetWinProbabilityPrefix(ref float __result) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if (hackConfig == null || !hackConfig.stableMission_forceWin) {
				return true;
			}

			__result = 100;
			return false;
		}

		private static IEnumerable<CodeInstruction> CompleteMissionTranspiler(IEnumerable<CodeInstruction> instructions) {
			MethodInfo setAchievementHook = AccessTools.Method(
					typeof(TimedMissionManagerPatcher), nameof(SetAchievementByEntityIDsHook),
					new[] { typeof(int), typeof(Guid?[]), typeof(string), typeof(WsServiceEventHandler), typeof(object) });

			List<CodeInstruction> instructionList = new List<CodeInstruction>(instructions);
			List<CodeInstruction> result = new List<CodeInstruction>();
			int found = 0;
			int instructionCount = instructionList.Count;

			for (int i = 0; i < instructionCount; i++) {
				CodeInstruction inst = instructionList[i];

				if (inst.opcode == OpCodes.Call) {
					var method = inst.operand as MethodInfo;
					if (method != null && method.DeclaringType == typeof(WsWebService) && method.Name.Equals("SetAchievementByEntityIDs")) {
						inst.operand = setAchievementHook;
						found++;
					}
				}

				result.Add(inst);
			}

			if (found != 1) {
				instance?.logger.LogWarning("CompleteMission found " + found + " entry-points, but expected: 1");
			}

			return result;
		}

		public static void SetAchievementByEntityIDsHook(int achievementId, Guid?[] petIDs, string inGroupID, WsServiceEventHandler inCallback,
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