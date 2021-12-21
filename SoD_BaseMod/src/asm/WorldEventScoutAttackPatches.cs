using HarmonyLib;
using SoD_BaseMod.console;

namespace SoD_BaseMod {
	[HarmonyPatch(declaringType: typeof(WorldEventScoutAttack))]
	public static class WorldEventScoutAttackPatches {
		[HarmonyPostfix, HarmonyPatch(methodName: "PopulateScore", argumentTypes: new[] { typeof(string[]), typeof(bool) })]
		private static void PopulateScore_Postfix(WorldEventScoutAttack __instance, string[] playersData, bool eventWon) {
			WorldEventManager.WorldEventAchievementRewardInfo[] rewardInfo = new Traverse(__instance)
					.Field("mCurrentRewardInfo")
					.GetValue<WorldEventManager.WorldEventAchievementRewardInfo[]>();
			if (rewardInfo == null) {
				BTConsole.WriteLine("rewardInfo was null!");
				return;
			}

			BTConsole.WriteLine($"  Found {rewardInfo.Length} rewardInfos");
			foreach (WorldEventManager.WorldEventAchievementRewardInfo info in rewardInfo) {
				if (info == null) {
					BTConsole.WriteLine("info was null!");
					continue;
				}
				BTConsole.WriteLine(
						"    "
						+ "_RewardNameText = " + info._RewardNameText + " | "
						+ "_RewardTier = " + info._RewardTier + " | "
						+ "_AchievementID = " + info._AchievementID + " | "
						+ "_AdRewardAchievementID = " + info._AdRewardAchievementID
				);
			}
		}
	}
}