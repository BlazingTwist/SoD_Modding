using System.Collections.Generic;

namespace SoD_BaseMod.console {
	public static class BTTutorialCommands {
		public static void Register() {
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Tutorial", "complete" },
					new BTTutorialCompleteInput(),
					"completes the specified tutorial",
					OnExecuteTutorialComplete
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Tutorial", "reset" },
					new BTTutorialResetInput(),
					"resets all or the specified tutorial",
					OnExecuteTutorialReset
			));
		}

		private static void OnExecuteTutorialComplete(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTTutorialCompleteInput) input;
			if (ProductData.AddTutorial(cmdInput.name)) {
				BTConsole.WriteLine("Tutorial " + cmdInput.name + " marked as done.");
			} else {
				BTConsole.WriteLine("Tutorial " + cmdInput.name + " is already completed or does not exist.");
			}
		}

		private class BTTutorialCompleteInput : BTConsoleCommand.BTCommandInput {
			public string name;

			private void SetName(object name, bool isPresent) {
				this.name = (string) name;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"name",
								false,
								"name of the tutorial to complete",
								SetName,
								typeof(string)
						)
				};
			}
		}

		private static void OnExecuteTutorialReset(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTTutorialResetInput) input;
			bool result = ProductData.ResetTutorial(cmdInput.name);
			if (cmdInput.name == null) {
				string message = result
						? "Tutorial play status reset."
						: "Could not reset Tutorials";
				BTConsole.WriteLine(message);
			} else {
				if (result) {
					BTConsole.WriteLine("Tutorial " + cmdInput.name + " reset (not saved)");
				} else {
					BTConsole.WriteLine("Could not Reset Tutorial " + cmdInput.name);
				}
			}
		}

		private class BTTutorialResetInput : BTConsoleCommand.BTCommandInput {
			public string name;

			private void SetName(object name, bool isPresent) {
				this.name = (string) name;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"name",
								true,
								"name of the tutorial to reset",
								SetName,
								typeof(string)
						)
				};
			}
		}
	}
}