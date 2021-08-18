using System;
using HarmonyLib;

namespace SoD_BaseMod {
	[HarmonyPatch(declaringType: typeof(KAUiGauntletShooterHUD))]
	public static class KAUiGauntletShooterHUDPatcher {
		[HarmonyPostfix, HarmonyPatch(methodName: "ContinueFireBallFrenzy", argumentTypes: new Type[] { })]
		private static void ContinueFireBallFrenzyPostfix(GauntletRailShootManager ___mGameManager) {
			___mGameManager._GauntletController._Pause = BTDebugCam.useDebugCam;
		}
	}
}