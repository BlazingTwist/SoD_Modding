using System.Collections.Generic;

namespace SoD_BaseMod.basemod.console.commands {
	public static class BTCoinsAddCommand {
		public static void Register() {
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Coins", "add" },
					new BTCoinsAddInput(),
					"adds coins and (supposedly) syncs with the server",
					OnExecute
			));
		}

		private static void OnExecute(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTCoinsAddInput) input;
			if (!Money.pIsReady) {
				BTConsole.WriteLine("error - Money object isn't ready.");
				return;
			}

			Money.AddMoney(cmdInput.amount, true);
			BTConsole.WriteLine("Added " + cmdInput.amount + " Coins.");
		}

		private class BTCoinsAddInput : BTConsoleCommand.BTCommandInput {
			public int amount;

			private void SetAmount(object amount, bool isPresent) {
				this.amount = (int) amount;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"amount",
								false,
								"amount of coins to add",
								SetAmount,
								typeof(int)
						)
				};
			}
		}
	}
}