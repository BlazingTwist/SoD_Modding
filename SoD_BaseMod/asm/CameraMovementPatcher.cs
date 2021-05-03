using HarmonyLib;
using BlazingTwist_Core;
using System;
using System.Reflection;
using JetBrains.Annotations;
using SoD_BaseMod.basemod;

namespace SoD_BaseMod.asm {
	[UsedImplicitly]
	public class CameraMovementPatcher : RuntimePatcher {
		public override void ApplyPatches() {
			Type originalType = typeof(SquadTactics.CameraMovement);
			Type patcherType = typeof(CameraMovementPatcher);

			MethodInfo originalUpdate = AccessTools.Method(originalType, "Update");
			MethodInfo originalLateUpdate = AccessTools.Method(originalType, "LateUpdate");

			HarmonyMethod prefixPatch = new HarmonyMethod(patcherType, nameof(ShouldRunPrefix));

			harmony.Patch(originalUpdate, prefixPatch);
			harmony.Patch(originalLateUpdate, prefixPatch);
		}

		private static bool ShouldRunPrefix() {
			return !BTDebugCam.useDebugCam;
		}
	}
}