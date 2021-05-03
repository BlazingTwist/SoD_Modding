using System.Collections.Generic;

namespace SoD_BaseMod.basemod.console.commands {
	public static class BTHelpCommand {
		public static void Register() {
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "help" },
					new BTNoArgsInput(),
					"list all available commands",
					OnExecute
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "help", "mdFormat" },
					new BTNoArgsInput(),
					"prints all commands in a md-table format",
					OnExecuteMDFormat
			));
		}

		private static void OnExecute(BTConsoleCommand.BTCommandInput input) {
			BTConsole.HelpAll();
		}

		private static void OnExecuteMDFormat(BTConsoleCommand.BTCommandInput input) {
			BTConsole.BuildMDHelpTable();
		}
	}
}