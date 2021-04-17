using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using SoD_BlazingTwist_Core;

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

	[UsedImplicitly]
	public class UiBackpackPatcher : RuntimePatcher {
		public override void ApplyPatches() {
			Type originalType = typeof(UiBackpack);
			Type patcherType = typeof(UiBackpackPatcher);

			MethodInfo selectItemOriginal = AccessTools.Method(originalType, "SelectItem", new[] { typeof(KAWidget) });

			HarmonyMethod selectItemPostfix =
					new HarmonyMethod(patcherType, nameof(SelectItemPostfix), new[] { typeof(UiBackpack), typeof(UserItemData), typeof(KAWidget) });

			harmony.Patch(selectItemOriginal, null, selectItemPostfix);
		}

		private static void SelectItemPostfix(UiBackpack __instance, UserItemData ___mSelectedUserItemData, KAWidget item) {
			ItemData selectedItem = ___mSelectedUserItemData.Item;
			KAUISelectItemData kAUISelectItemData = (KAUISelectItemData) item.GetUserData();
			if (selectedItem.HasCategory(424)) { // DragonSkin category
				__instance.ApplyOnPetReverse(kAUISelectItemData._UserItemData);
			}
		}
	}
}