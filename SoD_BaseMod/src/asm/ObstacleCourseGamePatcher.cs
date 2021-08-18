using System;
using HarmonyLib;

namespace SoD_BaseMod {
	[HarmonyPatch(declaringType: typeof(ObstacleCourseGame))]
	public static class ObstacleCourseGamePatcher {
		private static int obstacleCourseGameID;
		private static bool timeStopped;

		[HarmonyPrefix, HarmonyPatch(methodName: nameof(ObstacleCourseGame.Update), argumentTypes: new Type[] { })]
		private static void UpdatePrefix(ObstacleCourseGame __instance, UiFlightSchoolHUD ___mKAUIHud, float ___mTimeLeft) {
			int instanceID = __instance.GetInstanceID();
			if (obstacleCourseGameID != instanceID) {
				obstacleCourseGameID = instanceID;
				timeStopped = false;
			}

			if (BTDebugCam.useDebugCam) {
				if (timeStopped) {
					return;
				}

				timeStopped = true;
				__instance.StopTimer();
			} else if (timeStopped) {
				timeStopped = false;
				___mKAUIHud.StartTimer(___mTimeLeft);
			}
		}
	}
}