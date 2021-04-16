using HarmonyLib;
using SoD_BlazingTwist_Core;
using System;
using System.Reflection;
using SoD_BaseMod.basemod;
using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SoD_BaseMod.asm {
	[HarmonyPatch]
	public static class UiBackpackExtension {
		[HarmonyReversePatch]
		[HarmonyPatch(typeof(UiBackpack), "ApplyOnPet")]
		public static bool ApplyOnPetReverse(this UiBackpack __instance, UserItemData inItemData) {
			/* dummy content */
			throw new NotImplementedException("Stub called, reverse patch has not been applied!");
		}
	}

	public class UiBackpackPatcher : RuntimePatcher {
		public override void ApplyPatches() {
			Type originalType = typeof(UiBackpack);
			Type patcherType = typeof(UiBackpackPatcher);

			MethodInfo selectItemOriginal = AccessTools.Method(originalType, "SelectItem", new Type[] {typeof(KAWidget)}, null);

			HarmonyMethod selectItemPostfix =
				new HarmonyMethod(AccessTools.Method(patcherType, "SelectItemPostfix", new Type[] {typeof(UiBackpack), typeof(UserItemData), typeof(KAWidget)},
				                                     null));

			harmony.Patch(selectItemOriginal, null, selectItemPostfix);
		}

		[UsedImplicitly]
		private static void SelectItemPostfix(UiBackpack __instance, UserItemData ___mSelectedUserItemData, KAWidget item) {
			ItemData selectedItem = ___mSelectedUserItemData.Item;
			KAUISelectItemData kAUISelectItemData = (KAUISelectItemData) item.GetUserData();
			if (selectedItem.HasCategory(424)) { // DragonSkin category
				__instance.ApplyOnPetReverse(kAUISelectItemData._UserItemData);
			}
		}
	}
}