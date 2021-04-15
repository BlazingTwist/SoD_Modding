using HarmonyLib;
using SoD_BaseMod.basemod;
using SoD_BlazingTwist_Core;
using System;
using System.Reflection;

namespace SoD_BaseMod.asm
{
	public class KAUiGauntletShooterHUDPatcher : RuntimePatcher
	{
		public override void ApplyPatches() {
			Type originalType = typeof(KAUiGauntletShooterHUD);
			Type patcherType = typeof(KAUiGauntletShooterHUDPatcher);

			MethodInfo continueFireBallFrenzyOriginal = AccessTools.Method(originalType, "ContinueFireBallFrenzy", null, null);

			HarmonyMethod continueFireBallFrenzyPostfix = new HarmonyMethod(AccessTools.Method(patcherType, "ContinueFireBallFrenzyPostfix", new Type[] { typeof(GauntletRailShootManager) }, null));

			harmony.Patch(continueFireBallFrenzyOriginal, continueFireBallFrenzyPostfix);
		}

		private static void ContinueFireBallFrenzyPostfix(GauntletRailShootManager ___mGameManager) {
			___mGameManager._GauntletController._Pause = BTDebugCam.useDebugCam;
		}
	}
}
