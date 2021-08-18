using System.Linq;
using HarmonyLib;
using SoD_BaseMod.config;
using SquadTactics;

namespace SoD_BaseMod {
	[HarmonyPatch(declaringType: typeof(GameManager))]
	public static class GameManagerPatcher {
		[HarmonyPrefix, HarmonyPatch(methodName: "ProcessMouseUp", argumentTypes: new[] { typeof(Node) })]
		private static bool ProcessMouseUpPrefix(GameManager __instance, Node selectedNode) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if (hackConfig == null) {
				return true;
			}

			if (hackConfig.squadTactics_clickToKill && selectedNode._CharacterOnNode != null) {
				selectedNode._CharacterOnNode.TakeStatChange(SquadTactics.Stat.HEALTH, -10_000f);
				return false;
			}
			
			return true;
		}

		[HarmonyPrefix, HarmonyPatch(methodName: nameof(GameManager.SelectCharacter), argumentTypes: new[] { typeof(Character) })]
		private static void SelectCharacter_Prefix(GameManager __instance, Character character) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if (hackConfig != null && hackConfig.squadTactics_infiniteRange) {
				foreach (StStat movementStat in __instance.pAllPlayerUnits.Select(playerUnit => playerUnit.pCharacterData._Stats._Movement)) {
					movementStat._Limits.Max = 100;
					movementStat.pCurrentValue = movementStat._Limits.Max;
				}
			}
		}
	}
}