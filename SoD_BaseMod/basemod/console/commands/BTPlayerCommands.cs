using System.Collections.Generic;

namespace SoD_BaseMod.basemod.console.commands {
	public static class BTPlayerCommands {
		public static void Register() {
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Player", "getXP" },
					new BTPlayerXPGetInput(),
					"prints rank points of specified type",
					OnExecutePlayerGetXP
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Player", "addXP" },
					new BTPlayerXPAddInput(),
					"adds rank points of specified type",
					OnExecute
			));
		}

		private static void OnExecutePlayerGetXP(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTPlayerXPGetInput) input;
			UserAchievementInfo achInfo = (cmdInput.type == 8)
					? PetRankData.GetUserAchievementInfo(SanctuaryManager.pCurPetData)
					: UserRankData.GetUserAchievementInfoByType(cmdInput.type);
			if (achInfo == null) {
				BTConsole.WriteLine("No Data found for type: " + cmdInput.type);
				return;
			}

			BTConsole.WriteLine("XP = " + achInfo.AchievementPointTotal + " | Rank = " + achInfo.RankID);
		}

		private class BTPlayerXPGetInput : BTConsoleCommand.BTCommandInput {
			public int type;

			private void SetType(object type, bool isPresent) {
				this.type = (int) type;
			}

			protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"type",
								false,
								"pointTypeID to print (8 is pet xp)",
								SetType,
								typeof(int)
						)
				};
			}
		}

		private static void OnExecute(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTPlayerXPAddInput) input;
			UserAchievementInfo achInfo;
			if (cmdInput.type == 8) {
				PetRankData.AddPoints(SanctuaryManager.pCurPetData, cmdInput.amount);
				achInfo = PetRankData.GetUserAchievementInfo(SanctuaryManager.pCurPetData);
			} else {
				UserRankData.AddPoints(cmdInput.type, cmdInput.amount);
				achInfo = UserRankData.GetUserAchievementInfoByType(cmdInput.type);
			}

			if (achInfo == null) {
				BTConsole.WriteLine("No Data found for type: " + cmdInput.type);
				return;
			}

			BTConsole.WriteLine(
					"Added " + cmdInput.amount + " points of type " + cmdInput.type
					+ " | XP = " + achInfo.AchievementPointTotal
					+ " | Rank = " + achInfo.RankID);
		}

		private class BTPlayerXPAddInput : BTConsoleCommand.BTCommandInput {
			public int type;
			public int amount;

			private void SetType(object type, bool isPresent) {
				this.type = (int) type;
			}

			private void SetAmount(object amount, bool isPresent) {
				this.amount = (int) amount;
			}

			protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"type",
								false,
								"pointTypeID to add",
								SetType,
								typeof(int)
						),
						new BTConsoleCommand.BTConsoleArgument(
								"amount",
								false,
								"amount of points to add",
								SetAmount,
								typeof(int)
						)
				};
			}
		}
	}
}