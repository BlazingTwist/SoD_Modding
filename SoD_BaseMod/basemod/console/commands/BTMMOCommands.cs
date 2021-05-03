using System;
using System.Collections.Generic;
using System.Reflection;
using KnowledgeAdventure.Multiplayer;

namespace SoD_BaseMod.basemod.console.commands {
	public static class BTMMOCommands {
		public static void Register() {
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "MMO", "Users" },
					new BTMMOUsersShowInput(),
					"shows / hides the MMO userList",
					OnExecuteMMOUsers
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "MMO", "Info" },
					new BTNoArgsInput(),
					"prints info on the current MMO state",
					OnExecuteMMOInfo
			));
		}

		private static void OnExecuteMMOUsers(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTMMOUsersShowInput) input;
			if (cmdInput.show) {
				MMOUserList.Show();
				BTConsole.WriteLine("UserList is shown.");
			} else {
				MMOUserList.Hide();
				BTConsole.WriteLine("UserList is hidden.");
			}
		}

		private class BTMMOUsersShowInput : BTConsoleCommand.BTCommandInput {
			public bool show;

			private void SetShow(object show, bool isPresent) {
				this.show = (bool) show;
			}

			protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"show",
								false,
								"show/hide the MMO userList",
								SetShow,
								typeof(bool)
						)
				};
			}
		}

		private static void OnExecuteMMOInfo(BTConsoleCommand.BTCommandInput input) {
			// Version data
			Version version = Assembly.GetAssembly(typeof(IMMOClient)).GetName().Version;
			BTConsole.WriteLine("Version - DLL version is " + version);

			if (MainStreetMMOClient.pInstance != null) {
				// Server Data
				BTConsole.WriteLine("Server - Connected to " + MainStreetMMOClient.pInstance.GetMMOServerIP());

				// State Data
				string text = "State - State is " + MainStreetMMOClient.pInstance.pState;
				if (MainStreetMMOClient.pInstance.pAllDeactivated) {
					text += ".  All Deactivated";
				}

				BTConsole.WriteLine(text);
				if (MainStreetMMOClient.pInstance.pState == MMOClientState.IN_ROOM) {
					BTConsole.WriteLine("State - Zone is " + MainStreetMMOClient.pInstance.pZone + ", Room is " + MainStreetMMOClient.pInstance.pRoomName);
				}
			} else {
				BTConsole.WriteLine("error - MMO instance is null or not ready");
			}

			// still State data
			if (GauntletMMOClient.pInstance != null) {
				BTConsole.WriteLine("State - Gauntlet MMO State is " + GauntletMMOClient.pInstance.pMMOState);
			}
		}
	}
}