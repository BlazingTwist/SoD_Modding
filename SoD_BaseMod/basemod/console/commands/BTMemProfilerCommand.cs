using System.Collections.Generic;
using UnityEngine;

namespace SoD_BaseMod.basemod.console.commands {
	public static class BTMemProfilerCommand {
		public static void Register() {
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "MemProfiler" },
					new BTNoArgsInput(),
					"Opens the MemProfiler",
					OnExecute
			));
		}

		private static void OnExecute(BTConsoleCommand.BTCommandInput input) {
			if (Object.FindObjectOfType(typeof(BTMemoryProfilerContainer)) == null) {
				// ReSharper disable once ObjectCreationAsStatement
				new GameObject("Memory Profiler", typeof(BTMemoryProfilerContainer));
				BTConsole.WriteLine("Attached MemProfiler.");
			} else {
				BTConsole.WriteLine("MemProfiler is already attached.");
			}
		}
	}
}