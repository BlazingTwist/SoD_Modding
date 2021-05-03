using System.Collections.Generic;
using UnityEngine;

namespace SoD_BaseMod.basemod.console.commands {
	public static class BTQualityCommand {
		public static void Register() {
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Quality" },
					new BTQualityCommandInput(),
					"modifies the (graphics) quality settings",
					OnExecute
			));
		}

		private static void OnExecute(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTQualityCommandInput) input;
			if (cmdInput.increase) {
				QualitySettings.IncreaseLevel(cmdInput.applyExpensiveChanges);
			} else {
				QualitySettings.DecreaseLevel(cmdInput.applyExpensiveChanges);
			}

			BTConsole.WriteLine("Quality setting set to: " + QualitySettings.names[QualitySettings.GetQualityLevel()]);
			BTConsole.WriteLine("Applied expensive changes: " + cmdInput.applyExpensiveChanges);
		}

		private class BTQualityCommandInput : BTConsoleCommand.BTCommandInput {
			public bool increase;
			public bool applyExpensiveChanges;

			private void SetIncrease(object increase, bool isPresent) {
				this.increase = (bool) increase;
			}

			private void SetApplyExpensiveChanges(object applyExpensiveChanges, bool isPresent) {
				this.applyExpensiveChanges = (bool) applyExpensiveChanges;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"increase",
								false,
								"true = increase quality | false = decrease quality",
								SetIncrease,
								typeof(bool)
						),
						new BTConsoleCommand.BTConsoleArgument(
								"applyExpensiveChanges",
								true,
								"whether to apply changes that might take longer to compute or not",
								SetApplyExpensiveChanges,
								typeof(bool)
						)
				};
			}
		}
	}
}