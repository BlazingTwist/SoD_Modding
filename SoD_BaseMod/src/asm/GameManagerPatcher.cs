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

			if (hackConfig.squadTactics_infiniteRange) {
				__instance.ShowCharacterMovementRange(true);
				__instance.StartCoroutine(selectedNode._CharacterOnNode == null
						? __instance._SelectedCharacter.DoMovement(selectedNode)
						: __instance._SelectedCharacter.DoMovePlusAbility(selectedNode._CharacterOnNode));
				return false;
			}

			return true;
		}
	}
}