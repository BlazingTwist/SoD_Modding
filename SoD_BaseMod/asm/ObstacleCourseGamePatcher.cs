using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using SoD_BaseMod.basemod;
using BlazingTwist_Core;

namespace SoD_BaseMod.asm {
	[UsedImplicitly]
	public class ObstacleCourseGamePatcher : RuntimePatcher {
		private static int obstacleCourseGameID;
		private static bool timeStopped;

		public override void ApplyPatches() {
			Type originalType = typeof(ObstacleCourseGame);
			Type patcherType = typeof(ObstacleCourseGamePatcher);

			MethodInfo updateOriginal = AccessTools.Method(originalType, "Update");

			var updatePrefix = new HarmonyMethod(patcherType, nameof(UpdatePrefix),
					new[] { typeof(ObstacleCourseGame), typeof(UiFlightSchoolHUD), typeof(float) });

			harmony.Patch(updateOriginal, updatePrefix);
		}

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