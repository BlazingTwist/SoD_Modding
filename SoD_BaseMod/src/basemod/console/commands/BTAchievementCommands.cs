using System.Collections.Generic;

namespace SoD_BaseMod.console {
	public static class BTAchievementCommands {
		public static void Register() {
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Achievement", "set" },
					new BTAchievementSetInput(),
					"sets an achievement",
					OnExecuteAchievementSet
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Achievement", "set", "web" },
					new BTAchievementSetWebInput(),
					"sets an achievement using the webserver (and gets reward)",
					OnExecuteAchievementSetWeb
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Achievement", "clan", "set" },
					new BTAchievementClanSetInput(),
					"sets a clan achievement",
					OnExecuteAchievementClanSet
			));
		}

		private static void OnExecuteAchievementSet(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTAchievementSetInput) input;
			if (cmdInput.points == null) {
				UserAchievementTask.Set(cmdInput.taskID);
				BTConsole.WriteLine("completing achievementTaskID: " + cmdInput.taskID);
			} else {
				int points = (int) cmdInput.points;
				var task = new AchievementTask(cmdInput.taskID, "", 0, points);
				UserAchievementTask.Set(task);
				BTConsole.WriteLine("setting achievementTaskID: " + cmdInput.taskID + " | to amount: " + points);
			}
		}

		private class BTAchievementSetInput : BTConsoleCommand.BTCommandInput {
			public int taskID;
			public object points;

			private void SetTaskID(object taskID, bool isPresent) {
				this.taskID = (int) taskID;
			}

			private void SetPoints(object points, bool isPresent) {
				this.points = isPresent ? points : null;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"taskID",
								false,
								"taskID of the achievement to set",
								SetTaskID,
								typeof(int)
						),
						new BTConsoleCommand.BTConsoleArgument(
								"points",
								true,
								"points to set the achievement to, optional for pointless achievements",
								SetPoints,
								typeof(int)
						)
				};
			}
		}

		private static void OnExecuteAchievementSetWeb(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTAchievementSetWebInput) input;
			WsWebService.SetAchievementAndGetReward(cmdInput.taskID, "", ServiceEventHandler, null);
			BTConsole.WriteLine("SetAchievementAndGetReward: " + cmdInput.taskID);
		}

		private static void ServiceEventHandler(WsServiceType inType, WsServiceEvent inEvent, float inProgress, object inObject, object inUserData) {
			if (inEvent != WsServiceEvent.COMPLETE) {
				return;
			}

			AchievementReward[] array = (AchievementReward[]) inObject;
			if (array != null) {
				GameUtilities.AddRewards(array, false, false);
			}
		}

		private class BTAchievementSetWebInput : BTConsoleCommand.BTCommandInput {
			public int taskID;

			private void SetTaskID(object taskID, bool isPresent) {
				this.taskID = (int) taskID;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"taskID",
								false,
								"taskID of the achievement to set",
								SetTaskID,
								typeof(int)
						)
				};
			}
		}

		private static void OnExecuteAchievementClanSet(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTAchievementClanSetInput) input;
			UserAchievementTask.Set(UserProfile.pProfileData.Groups[0].GroupID, cmdInput.taskID, 2);
			BTConsole.WriteLine("Setting Clan Achievement " + cmdInput.taskID);
		}

		private class BTAchievementClanSetInput : BTConsoleCommand.BTCommandInput {
			public int taskID;

			private void SetTaskID(object taskID, bool isPresent) {
				this.taskID = (int) taskID;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"taskID",
								false,
								"taskID of the achievement to set",
								SetTaskID,
								typeof(int)
						)
				};
			}
		}
	}
}