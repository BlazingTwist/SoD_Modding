using System.Collections.Generic;

namespace SoD_BaseMod.console {
	public static class BTClsCommand {
		public static void Register() {
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "cls" },
					new BTNoArgsInput(),
					"clears console output",
					OnExecute
			));
		}

		private static void OnExecute(BTConsoleCommand.BTCommandInput input) {
			BTConsole.ClearConsole();
		}
	}
}