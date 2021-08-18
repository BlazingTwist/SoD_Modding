using System.Collections.Generic;

namespace SoD_BaseMod.console {
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
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Player", "setDisplayName" },
					new BTPlayerSetDisplayNameInput(),
					"sets the viking's DisplayName",
					OnExecuteSetDisplayName));
		}

		private static void OnExecutePlayerGetXP(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTPlayerXPGetInput) input;
			UserAchievementInfo achInfo = cmdInput.type == 8
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

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
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

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
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

		private static void OnExecuteSetDisplayName(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTPlayerSetDisplayNameInput) input;
			var request = new SetDisplayNameRequest() {
					DisplayName = cmdInput.name,
					ItemID = 13030,
					StoreID = 93
			};
			WsWebService.SetDisplayName(request, OnCallbackSetDisplayName, cmdInput);
		}

		private static void OnCallbackSetDisplayName(WsServiceType inType, WsServiceEvent inEvent, float inProgress, object inObject, object inUserData) {
			// ReSharper disable once ConvertIfStatementToSwitchStatement
			if (inEvent == WsServiceEvent.ERROR) {
				BTConsole.WriteLine("ERROR: SetDisplayName responded with error!");
				return;
			}

			if (inEvent == WsServiceEvent.COMPLETE) {
				if (inObject == null) {
					BTConsole.WriteLine("ERROR: WebService returned no data!");
					return;
				}

				var result = (SetAvatarResult) inObject;
				if (result.Success) {
					BTConsole.WriteLine("Successfully changed name to: " + result.DisplayName);
					if (AvAvatar.pToolbar != null) {
						var component = AvAvatar.pToolbar.GetComponent<UiToolbar>();
						if (component != null) {
							component.DisplayName();
						}
					}

					AvatarData.SetDisplayName(result.DisplayName);
					UserInfo.pInstance.Username = result.DisplayName;
				} else {
					BTConsole.WriteLine("ERROR: SetDisplayName failed. Status: " + result.StatusCode);
					if (result.Suggestions?.Suggestion != null) {
						foreach (string suggestion in result.Suggestions.Suggestion) {
							BTConsole.WriteLine("\tgot Suggestion: " + suggestion);
						}
					}
				}
			}
		}

		private class BTPlayerSetDisplayNameInput : BTConsoleCommand.BTCommandInput {
			public string name;

			private void SetName(object name, bool isPresent) {
				this.name = (string) name;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"name",
								false,
								"name to use as DisplayName",
								SetName,
								typeof(string)
						)
				};
			}
		}
	}
}