using System.Collections.Generic;
using UnityEngine;

namespace SoD_BaseMod.console {
	public static class BTGlowUICommand {
		public static void Register() {
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "GlowUI" },
					new BTNoArgsInput(),
					"opens the GlowUI (clientSide glow effects)",
					OnExecute
			));
		}

		private static void OnExecute(BTConsoleCommand.BTCommandInput input) {
			var glowConsole = Object.FindObjectOfType<GlowConsole>();
			GameObject glowUI = glowConsole._GlowSettingsUi;
			Object.Instantiate(glowUI, Vector3.zero, Quaternion.identity);
			BTConsole.WriteLine("GlowUI opened.");
		}
	}
}