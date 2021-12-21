using System;
using HarmonyLib;

namespace SoD_BaseMod.Extensions {
	[HarmonyPatch(declaringType: typeof(MissionManager))]
	public static class MissionManager_Extensions {
		[HarmonyReversePatch,
		 HarmonyPatch(methodName: "GetUserMissionStaticEventHandler", argumentTypes: new[] {
				 typeof(WsServiceType), typeof(WsServiceEvent), typeof(float), typeof(object), typeof(object)
		 })]
		public static void GetUserMissionStaticEventHandler(this MissionManager __instance,
				WsServiceType inType, WsServiceEvent inEvent, float inProgress, object inObject, object inUserData) {
			/* dummy content */
			throw new NotImplementedException("Stub called, reverse patch has not been applied!");
		}
	}
}