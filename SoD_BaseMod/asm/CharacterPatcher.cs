using HarmonyLib;
using SoD_BlazingTwist_Core;
using System;
using System.Reflection;
using SoD_BaseMod.basemod;
using SquadTactics;

namespace SoD_BaseMod.asm
{
	public class CharacterPatcher : RuntimePatcher
	{
		public override void ApplyPatches() {
			Type originalType = typeof(Character);
			Type patcherType = typeof(CharacterPatcher);

			MethodInfo doMovementOriginal = AccessTools.Method(originalType, "DoMovement", new Type[] { typeof(Node) }, null);
			MethodInfo useAbilityOriginal = AccessTools.Method(originalType, "UseAbility", new Type[] { typeof(Character) }, null);

			HarmonyMethod doMovementPostfix = new HarmonyMethod(AccessTools.Method(patcherType, "DoMovementPostfix", new Type[] { typeof(Character) }, null));
			HarmonyMethod useAbilityPostfix = new HarmonyMethod(AccessTools.Method(patcherType, "UseAbilityPostfix", new Type[] { typeof(Character) }, null));

			harmony.Patch(doMovementOriginal, null, doMovementPostfix);
			harmony.Patch(useAbilityOriginal, null, useAbilityPostfix);

		}

		private static void DoMovementPostfix(Character __instance) {
			if(!__instance._HasMoveAction) {
				BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
				if(hackConfig != null && hackConfig.squadTactics_infiniteMoves) {
					__instance._HasMoveAction = true;
				}
			}
		}

		private static void UseAbilityPostfix(Character __instance) {
			if(!__instance._HasAbilityAction) {
				BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
				if(hackConfig != null && hackConfig.squadTactics_infiniteActions) {
					__instance._HasAbilityAction = true;
				}
			}
		}
	}
}
