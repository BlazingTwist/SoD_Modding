using System.Collections.Generic;

namespace SoD_BaseMod.console {
	public static class BTLevelCommands {
		public static void Register() {
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Level", "load" },
					new BTLevelLoadInput(),
					"load a specified level",
					OnExecuteLevelLoad
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Level", "get" },
					new BTNoArgsInput(),
					"prints the current level name (name of the active scene)",
					OnExecuteLevelGet
			));
		}

		private static void OnExecuteLevelLoad(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTLevelLoadInput) input;
			BTConsole.WriteLine("Loading level: " + cmdInput.level);
			RsResourceManager.LoadLevel(cmdInput.level);
		}

		private class BTLevelLoadInput : BTConsoleCommand.BTCommandInput {
			public string level;

			private void SetLevel(object level, bool isPresent) {
				this.level = (string) level;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"level",
								false,
								"name of the level to load",
								SetLevel,
								typeof(string)
						)
				};
			}
		}
		
		private static void OnExecuteLevelGet(BTConsoleCommand.BTCommandInput input) {
			BTConsole.WriteLine("Current Level: " + RsResourceManager.pCurrentLevel);
		}
	}
}