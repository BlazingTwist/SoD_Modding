using System;
using HarmonyLib;

namespace SoD_BaseMod.Extensions {
	[HarmonyPatch(declaringType: typeof(EelRoastManager))]
	public static class EelRoastManagerExtensions {
		[HarmonyReversePatch,
		 HarmonyPatch(methodName: "ResourceEventHandler",
				 argumentTypes: new[] { typeof(string), typeof(RsResourceLoadEvent), typeof(float), typeof(object), typeof(object) })]
		public static void ResourceEventHandler(this EelRoastManager __instance, string inURL, RsResourceLoadEvent inEvent, float inProgress,
				object inObject, object inUserData) {
			/* dummy content */
			throw new NotImplementedException("Stub called, reverse patch has not been applied!");
		}

		[HarmonyReversePatch,
		 HarmonyPatch(methodName: "GetRandomEelPath", argumentTypes: new[] { typeof(EelRoastMarkerInfo) })]
		public static string GetRandomEelPath(this EelRoastManager __instance, EelRoastMarkerInfo eelRoastMarkerInfo) {
			/* dummy content */
			throw new NotImplementedException("Stub called, reverse patch has not been applied!");
		}
	}
}