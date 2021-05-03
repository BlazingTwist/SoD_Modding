using System.Collections.Generic;

namespace SoD_BaseMod.basemod.console.commands {
	public static class BTMysteryChestSpawnAllCommand {
		public static void Register() {
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Mystery", "Chest", "SpawnAll" },
					new BTMysteryChestSpawnAllInput(),
					"enables/disables spawning of all Mystery Chests",
					OnExecute
			));
		}

		private static void OnExecute(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTMysteryChestSpawnAllInput) input;
			if (cmdInput.spawnAll == null) {
				MysteryChestManager._CheatSpawnAll = !MysteryChestManager._CheatSpawnAll;
				BTConsole.WriteLine("MysteryChest spawnAll is now " + MysteryChestManager._CheatSpawnAll);
			} else {
				bool spawnAll = (bool) cmdInput.spawnAll;
				if (spawnAll == MysteryChestManager._CheatSpawnAll) {
					BTConsole.WriteLine("MysteryChest spawnAll is already " + spawnAll);
				} else {
					MysteryChestManager._CheatSpawnAll = spawnAll;
					BTConsole.WriteLine("MysteryChest spawnAll is now " + spawnAll);
				}
			}
		}

		private class BTMysteryChestSpawnAllInput : BTConsoleCommand.BTCommandInput {
			public object spawnAll;

			private void SetSpawnAll(object spawnAll, bool isPresent) {
				this.spawnAll = isPresent ? spawnAll : null;
			}

			protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"spawnAll",
								true,
								"spawnAll, otherwise toggles",
								SetSpawnAll,
								typeof(bool)
						)
				};
			}
		}
	}
}