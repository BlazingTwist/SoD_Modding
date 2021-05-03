using HarmonyLib;
using SoD_BaseMod.basemod;
using BlazingTwist_Core;
using System;
using System.Reflection;
using JetBrains.Annotations;

namespace SoD_BaseMod.asm {
	[UsedImplicitly]
	public class KAUiGauntletShooterHUDPatcher : RuntimePatcher {
		public override void ApplyPatches() {
			Type originalType = typeof(KAUiGauntletShooterHUD);
			Type patcherType = typeof(KAUiGauntletShooterHUDPatcher);

			MethodInfo continueFireBallFrenzyOriginal = AccessTools.Method(originalType, "ContinueFireBallFrenzy");

			HarmonyMethod continueFireBallFrenzyPostfix =
					new HarmonyMethod(patcherType, nameof(ContinueFireBallFrenzyPostfix), new[] { typeof(GauntletRailShootManager) });

			harmony.Patch(continueFireBallFrenzyOriginal, continueFireBallFrenzyPostfix);
		}

		private static void ContinueFireBallFrenzyPostfix(GauntletRailShootManager ___mGameManager) {
			___mGameManager._GauntletController._Pause = BTDebugCam.useDebugCam;
		}
	}
}