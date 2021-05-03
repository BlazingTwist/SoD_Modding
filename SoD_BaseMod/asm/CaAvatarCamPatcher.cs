﻿using HarmonyLib;
using BlazingTwist_Core;
using System;
using System.Reflection;
using JetBrains.Annotations;
using SoD_BaseMod.basemod;

namespace SoD_BaseMod.asm {
	[UsedImplicitly]
	public class CaAvatarCamPatcher : RuntimePatcher {
		public override void ApplyPatches() {
			Type originalType = typeof(CaAvatarCam);
			Type patcherType = typeof(CaAvatarCamPatcher);

			MethodInfo lateUpdateOriginal = AccessTools.Method(originalType, "LateUpdate");

			HarmonyMethod lateUpdatePrefix = new HarmonyMethod(patcherType, nameof(LateUpdatePrefix));

			harmony.Patch(lateUpdateOriginal, lateUpdatePrefix);
		}

		private static bool LateUpdatePrefix() {
			return !BTDebugCam.useDebugCam;
		}
	}
}