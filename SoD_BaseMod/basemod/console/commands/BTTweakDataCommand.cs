using System.Collections.Generic;

namespace SoD_BaseMod.basemod.console.commands {
	public static class BTTweakDataCommand {
		public static void Register() {
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "TweakData" },
					new BTTweakDataInput(),
					"shows/hides Input TweakData",
					OnExecute
			));
		}

		private static void OnExecute(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTTweakDataInput) input;
			string showString = cmdInput.show ? "shown" : "hidden";
			KAInput.pInstance.ShowTweak(cmdInput.show);
			BTConsole.WriteLine("Set Input TweakData to: " + showString);
		}

		private class BTTweakDataInput : BTConsoleCommand.BTCommandInput {
			public bool show;

			private void SetShow(object show, bool isPresent) {
				this.show = (bool) show;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"show",
								false,
								"show/hide Input TweakData",
								SetShow,
								typeof(bool)
						)
				};
			}
		}
	}
}