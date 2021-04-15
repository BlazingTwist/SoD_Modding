using HarmonyLib;
using SoD_BaseMod.basemod;
using SoD_BlazingTwist_Core;
using System;
using System.Reflection;

namespace SoD_BaseMod.asm
{
	public class ObstacleCourseGamePatcher : RuntimePatcher
	{
		private static int obstacleCourseGameID = 0;
		private static bool timeStopped = false;

		public override void ApplyPatches() {
			Type originalType = typeof(ObstacleCourseGame);
			Type patcherType = typeof(ObstacleCourseGamePatcher);

			MethodInfo updateOriginal = AccessTools.Method(originalType, "Update", null, null);

			HarmonyMethod updatePrefix = new HarmonyMethod(AccessTools.Method(patcherType, "UpdatePrefix", new Type[] { typeof(ObstacleCourseGame), typeof(UiFlightSchoolHUD), typeof(float) }, null));

			harmony.Patch(updateOriginal, updatePrefix);
		}

		private static void UpdatePrefix(ObstacleCourseGame __instance, UiFlightSchoolHUD ___mKAUIHud, float ___mTimeLeft) {
			int instanceID = __instance.GetInstanceID();
			if(obstacleCourseGameID != instanceID) {
				obstacleCourseGameID = instanceID;
				timeStopped = false;
			}
			if(BTDebugCam.useDebugCam) {
				if(!timeStopped) {
					timeStopped = true;
					__instance.StopTimer();
				}
			} else if(timeStopped) {
				timeStopped = false;
				___mKAUIHud.StartTimer(___mTimeLeft);
			}
		}
	}
}
