using System.Collections.Generic;
using KA.Framework;
using UnityEngine;

namespace SoD_BaseMod.basemod.console.commands {
	public static class BTBuildInfoCommand {
		public static void Register() {
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "BuildInfo" },
					new BTNoArgsInput(),
					"prints build information to console",
					OnExecute
			));
		}

		private static void OnExecute(BTConsoleCommand.BTCommandInput input) {
			BTConsole.WriteLine("Unity Project ID - " + Application.cloudProjectId);
			string changeListNumber = ProductSettings.pInstance.GetChangelistNumber();
			string formattedCLNumber = (string.IsNullOrEmpty(changeListNumber) ? "not found" : changeListNumber);
			BTConsole.WriteLine("Application.version - " + Application.version);
			BTConsole.WriteLine("Application.unityVersion - " + Application.unityVersion);
			BTConsole.WriteLine("ChangeList Number - " + formattedCLNumber);
			BTConsole.WriteLine("Version - " + ProductConfig.pProductVersion);
			BTConsole.WriteLine("Environment - " + ProductConfig.GetEnvironmentForBundles());
			BTConsole.WriteLine("Current Platform - " + UtPlatform.GetPlatformName());
		}
	}
}