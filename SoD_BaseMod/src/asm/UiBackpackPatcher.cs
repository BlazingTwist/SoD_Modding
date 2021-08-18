using HarmonyLib;
using SoD_BaseMod.Extensions;

namespace SoD_BaseMod {
	[HarmonyPatch(declaringType: typeof(UiBackpack))]
	public static class UiBackpackPatcher {
		[HarmonyPostfix, HarmonyPatch(methodName: nameof(UiBackpack.SelectItem), argumentTypes: new[] { typeof(KAWidget) })]
		private static void SelectItemPostfix(UiBackpack __instance, UserItemData ___mSelectedUserItemData, KAWidget item) {
			ItemData selectedItem = ___mSelectedUserItemData.Item;
			var kAUISelectItemData = (KAUISelectItemData) item.GetUserData();
			if (selectedItem.HasCategory(424)) { // DragonSkin category
				__instance.ApplyOnPet(kAUISelectItemData._UserItemData);
			}
		}
	}
}