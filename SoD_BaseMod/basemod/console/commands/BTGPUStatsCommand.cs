using System.Collections.Generic;
using UnityEngine;

namespace SoD_BaseMod.basemod.console.commands {
	public static class BTGPUStatsCommand {
		public static void Register() {
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "GPU", "stats" },
					new BTNoArgsInput(),
					"Prints GPU stats to console",
					OnExecute
			));
		}

		private static void OnExecute(BTConsoleCommand.BTCommandInput input) {
			BTConsole.WriteLine("GPU name - " + SystemInfo.graphicsDeviceName);
			BTConsole.WriteLine("  Memory - " + SystemInfo.graphicsMemorySize);
		}
	}
}