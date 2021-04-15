using HarmonyLib;
using SoD_BlazingTwist_Core;
using System;
using System.Reflection;
using SoD_BaseMod.basemod;
using SquadTactics;

namespace SoD_BaseMod.asm
{
	public class GameManagerPatcher : RuntimePatcher
	{
		public override void ApplyPatches() {
			Type originalType = typeof(GameManager);
			Type patcherType = typeof(GameManagerPatcher);

			MethodInfo processMouseUpOriginal = AccessTools.Method(originalType, "ProcessMouseUp", new Type[] { typeof(Node) }, null);
			
			HarmonyMethod processMouseUpPrefix = new HarmonyMethod(AccessTools.Method(patcherType, "ProcessMouseUpPrefix", new Type[] { typeof(GameManager), typeof(Node) }, null));

			harmony.Patch(processMouseUpOriginal, processMouseUpPrefix);
		}

		private static bool ProcessMouseUpPrefix(GameManager __instance, Node selectedNode) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if(hackConfig != null && hackConfig.squadTactics_infiniteRange) {
				if(selectedNode._CharacterOnNode == null) {
					// do move
					__instance.ShowCharacterMovementRange(true);
					__instance.StartCoroutine(__instance._SelectedCharacter.DoMovement(selectedNode));
				} else {
					// do attack
					selectedNode._CharacterOnNode.TakeStatChange(SquadTactics.Stat.HEALTH, -10_000f, null, false, false);
				}
				return false;
			}
			return true;
		}
	}
}
