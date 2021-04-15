using HarmonyLib;
using SoD_BlazingTwist_Core;
using System;
using System.Reflection;
using SoD_BaseMod.basemod;

namespace SoD_BaseMod.asm
{
	public class CameraMovementPatcher : RuntimePatcher
	{
		public override void ApplyPatches() {
			Type originalType = typeof(SquadTactics.CameraMovement);
			Type patcherType = typeof(CameraMovementPatcher);

			MethodInfo originalUpdate = AccessTools.Method(originalType, "Update", null, null);
			MethodInfo originalLateUpdate = AccessTools.Method(originalType, "LateUpdate", null, null);

			HarmonyMethod prefixPatch = new HarmonyMethod(AccessTools.Method(patcherType, "ShouldRunPrefix", null, null));

			harmony.Patch(originalUpdate, prefixPatch);
			harmony.Patch(originalLateUpdate, prefixPatch);
		}

		private static bool ShouldRunPrefix() {
			return !BTDebugCam.useDebugCam;
		}
	}
}
