using System.Collections.Generic;

namespace SoD_BaseMod.basemod.console.commands {
	public static class BTFieldGuideUnlockCommand {
		public static void Register() {
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "FieldGuide", "unlock" },
					new BTFieldGuideUnlockInput(),
					"unlocks/locks the field guide",
					OnExecute
			));
		}

		private static void OnExecute(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTFieldGuideUnlockInput) input;
			string unlockString = cmdInput.unlock ? "unlocked" : "locked";
			if (FieldGuideData.pUnlocked == cmdInput.unlock) {
				BTConsole.WriteLine("FieldGuide is already " + unlockString);
			} else {
				FieldGuideData.pUnlocked = cmdInput.unlock;
				BTConsole.WriteLine("FieldGuide is now " + unlockString);
			}
		}

		private class BTFieldGuideUnlockInput : BTConsoleCommand.BTCommandInput {
			public bool unlock;

			private void SetUnlock(object unlock, bool isPresent) {
				this.unlock = (bool) unlock;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"unlock",
								false,
								"unlock/lock field guide",
								SetUnlock,
								typeof(bool)
						)
				};
			}
		}
	}
}