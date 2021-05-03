﻿using HarmonyLib;
using BlazingTwist_Core;
using System;
using System.Reflection;
using JetBrains.Annotations;
using SoD_BaseMod.basemod;
using SoD_BaseMod.basemod.config;
using SquadTactics;

namespace SoD_BaseMod.asm {
	[UsedImplicitly]
	public class GameManagerPatcher : RuntimePatcher {
		public override void ApplyPatches() {
			Type originalType = typeof(GameManager);
			Type patcherType = typeof(GameManagerPatcher);

			MethodInfo processMouseUpOriginal = AccessTools.Method(originalType, "ProcessMouseUp", new[] { typeof(Node) });

			HarmonyMethod processMouseUpPrefix =
				new HarmonyMethod(patcherType, nameof(ProcessMouseUpPrefix), new[] { typeof(GameManager), typeof(Node) });

			harmony.Patch(processMouseUpOriginal, processMouseUpPrefix);
		}

		private static bool ProcessMouseUpPrefix(GameManager __instance, Node selectedNode) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if (hackConfig == null || !hackConfig.squadTactics_infiniteRange) {
				return true;
			}

			if (selectedNode._CharacterOnNode == null) {
				// do move
				__instance.ShowCharacterMovementRange(true);
				__instance.StartCoroutine(__instance._SelectedCharacter.DoMovement(selectedNode));
			} else {
				// do attack
				selectedNode._CharacterOnNode.TakeStatChange(SquadTactics.Stat.HEALTH, -10_000f);
			}

			return false;
		}
	}
}