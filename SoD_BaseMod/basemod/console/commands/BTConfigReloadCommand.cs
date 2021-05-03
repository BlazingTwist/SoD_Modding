using System.Collections.Generic;

namespace SoD_BaseMod.basemod.console.commands {
	public class BTConfigReloadCommand {
		public static void Register() {
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "config", "reload" },
					new BTNoArgsInput(),
					"reloads the config file",
					OnExecute
			));
		}

		private static void OnExecute(BTConsoleCommand.BTCommandInput input) {
			BTDebugCamInputManager.ReloadConfig();
			BTConsole.WriteLine("config reloaded.");
		}
	}
}