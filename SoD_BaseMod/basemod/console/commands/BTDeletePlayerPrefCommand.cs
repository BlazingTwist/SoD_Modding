using System.Collections.Generic;
using UnityEngine;

namespace SoD_BaseMod.basemod.console.commands {
	public static class BTDeletePlayerPrefCommand {
		public static void Register() {
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "DeletePlayerPrefs" },
					new BTDeletePlayerPrefCommandInput(),
					"deletes a key from the player prefs",
					OnExecute
			));
		}

		private static void OnExecute(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTDeletePlayerPrefCommandInput) input;
			if (PlayerPrefs.HasKey(cmdInput.keyName)) {
				PlayerPrefs.DeleteKey(cmdInput.keyName);
				BTConsole.WriteLine("Deleted " + cmdInput.keyName + " from player prefs.");
			} else {
				BTConsole.WriteLine(cmdInput.keyName + " is not present in player prefs.");
			}
		}

		private class BTDeletePlayerPrefCommandInput : BTConsoleCommand.BTCommandInput {
			public string keyName;

			private void SetKeyName(object keyName, bool isPresent) {
				this.keyName = (string) keyName;
			}

			protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"keyName",
								false,
								"key(name) of the playerPref",
								SetKeyName,
								typeof(string)
						)
				};
			}
		}
	}
}