using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace SoD_BaseMod.Extensions {
	[HarmonyPatch(declaringType: typeof(SanctuaryPet))]
	public static class SanctuaryPet_Extensions {
		public static Dictionary<string, SkinnedMeshRenderer> GetRendererMap(this SanctuaryPet pet) {
			return Traverse
					.Create(pet)
					.Field("mRendererMap")
					.GetValue<Dictionary<string, SkinnedMeshRenderer>>();
		}
	}
}