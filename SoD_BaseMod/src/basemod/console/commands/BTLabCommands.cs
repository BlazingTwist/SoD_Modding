using System.Collections.Generic;

namespace SoD_BaseMod.console {
	public static class BTLabCommands {
		public static void Register() {
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Lab", "Experiment", "Get" },
					new BTNoArgsInput(),
					"Gets the active lab experiment",
					OnExecuteLabExperimentGet
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Lab", "Experiment", "Set" },
					new BTLabExperimentSetInput(),
					"Sets the active lab experiment (for this session only)",
					OnExecuteLabExperimentSet
			));
		}

		private static void OnExecuteLabExperimentGet(BTConsoleCommand.BTCommandInput input) {
			int expID = ScientificExperiment.pActiveExperimentID;
			bool isNatural = !ScientificExperiment.pUseExperimentCheat;
			BTConsole.WriteLine("Active Experiment ID: " + expID + " | isNatural: " + isNatural);
		}

		private static void OnExecuteLabExperimentSet(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTLabExperimentSetInput) input;
			int previousID = ScientificExperiment.pActiveExperimentID;
			ScientificExperiment.pActiveExperimentID = cmdInput.experimentID;
			ScientificExperiment.pUseExperimentCheat = !cmdInput.isNatural;
			BTConsole.WriteLine("Changed Experiment ID from: " + previousID + " | to: " + cmdInput.experimentID);
		}

		private class BTLabExperimentSetInput : BTConsoleCommand.BTCommandInput {
			public int experimentID;
			public bool isNatural;

			private void SetExperimentID(object experimentID, bool isPresent) {
				this.experimentID = (int) experimentID;
			}

			private void SetIsNatural(object isNatural, bool isPresent) {
				this.isNatural = (bool) isNatural;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"experimentID",
								false,
								"ID of the experiment",
								SetExperimentID,
								typeof(int)
						),
						new BTConsoleCommand.BTConsoleArgument(
								"isNatural",
								true,
								"set to true when resetting to a known natural ID",
								SetIsNatural,
								typeof(bool)
						)
				};
			}
		}
	}
}