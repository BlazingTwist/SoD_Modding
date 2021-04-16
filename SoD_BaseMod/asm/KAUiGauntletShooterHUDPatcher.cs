using HarmonyLib;
using SoD_BaseMod.basemod;
using SoD_BlazingTwist_Core;
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
				new HarmonyMethod(AccessTools.Method(patcherType, "ContinueFireBallFrenzyPostfix", new[] {typeof(GauntletRailShootManager)}));

			harmony.Patch(continueFireBallFrenzyOriginal, continueFireBallFrenzyPostfix);
		}

		[UsedImplicitly]
		private static void ContinueFireBallFrenzyPostfix(GauntletRailShootManager ___mGameManager) {
			___mGameManager._GauntletController._Pause = BTDebugCam.useDebugCam;
		}
	}
}