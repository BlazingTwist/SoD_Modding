using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using SoD_BaseMod.basemod;
using SoD_BaseMod.basemod.config;
using BlazingTwist_Core;
using SquadTactics;

namespace SoD_BaseMod.asm {
	[UsedImplicitly]
	public class CharacterPatcher : RuntimePatcher {
		public override void ApplyPatches() {
			Type originalType = typeof(Character);
			Type patcherType = typeof(CharacterPatcher);

			MethodInfo doMovementOriginal = AccessTools.Method(originalType, "DoMovement", new[] {typeof(Node)});
			MethodInfo useAbilityOriginal = AccessTools.Method(originalType, "UseAbility", new[] {typeof(Character)});

			var doMovementPostfix = new HarmonyMethod(patcherType, nameof(DoMovementPostfix), new[] {typeof(Character)});
			var useAbilityPostfix = new HarmonyMethod(patcherType, nameof(UseAbilityPostfix), new[] {typeof(Character)});

			harmony.Patch(doMovementOriginal, null, doMovementPostfix);
			harmony.Patch(useAbilityOriginal, null, useAbilityPostfix);
		}

		private static void DoMovementPostfix(Character __instance) {
			if (!__instance._HasMoveAction) {
				BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
				if (hackConfig != null && hackConfig.squadTactics_infiniteMoves) {
					__instance._HasMoveAction = true;
				}
			}
		}

		private static void UseAbilityPostfix(Character __instance) {
			if (!__instance._HasAbilityAction) {
				BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
				if (hackConfig != null && hackConfig.squadTactics_infiniteActions) {
					__instance._HasAbilityAction = true;
				}
			}
		}
	}
}