using HarmonyLib;
using SoD_BlazingTwist_Core;
using System;
using System.Reflection;
using SoD_BaseMod.basemod;

namespace SoD_BaseMod.asm
{
	public class CaAvatarCamPatcher : RuntimePatcher
	{
		public override void ApplyPatches() {
			Type originalType = typeof(CaAvatarCam);
			Type patcherType = typeof(CaAvatarCamPatcher);

			MethodInfo lateUpdateOriginal = AccessTools.Method(originalType, "LateUpdate", null, null);

			HarmonyMethod lateUpdatePrefix = new HarmonyMethod(AccessTools.Method(patcherType, "LateUpdatePrefix", null, null));

			harmony.Patch(lateUpdateOriginal, lateUpdatePrefix);
		}

		private static bool LateUpdatePrefix() {
			return !BTDebugCam.useDebugCam;
		}
	}
}
