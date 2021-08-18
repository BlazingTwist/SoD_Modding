using System.Collections.Generic;

namespace SoD_BaseMod.console {
	public static class BTCheckPasswordCommand {
		public static void Register() {
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "check", "console", "pass" },
					new BTCheckConsolePassInput(),
					"compare your password to the stored one",
					OnExecuteCheckConsolePass
			));
		}

		private static void OnExecuteCheckConsolePass(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTCheckConsolePassInput) input;
			BTConsole.WriteLine("Stored Password Hash: " + ProductConfig.pInstance.ConsolePassword);
			string hash = WsMD5Hash.GetMd5Hash(cmdInput.password);
			BTConsole.WriteLine("Passed Password Hash: " + hash + " (" + cmdInput.password + ")");
			bool matches = ProductConfig.pInstance.ConsolePassword == hash;
			BTConsole.WriteLine("matches? " + (matches ? "YES" : "NO"));
		}

		private class BTCheckConsolePassInput : BTConsoleCommand.BTCommandInput {
			public string password;

			private void SetPassword(object password, bool isPresent) {
				this.password = (string) password;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"password",
								false,
								"plaintext password to check",
								SetPassword,
								typeof(string)
						)
				};
			}
		}
	}
}