using System.Collections.Generic;

namespace SoD_BaseMod.console {
	public static class BTFrameRateCommands {
		public static void Register() {
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "FrameRate" },
					new BTFrameRateInput(),
					"shows / hides the framerate ui",
					OnExecuteFrameRate
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "FrameRate", "degrade" },
					new BTFrameRateDegradeInput(),
					"enables/disables the auto-degrade feature of the FPS UI",
					OnExecuteFrameRateDegrade
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "FrameRate", "refresh" },
					new BTNoArgsInput(),
					"recomputes FrameRate stats",
					OnExecuteFrameRateRefresh
			));
		}

		private static void OnExecuteFrameRate(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTFrameRateInput) input;
			if (cmdInput.show == null) {
				GrFPS._Display = !GrFPS._Display;
				string showString = GrFPS._Display ? "shown" : "hidden";
				BTConsole.WriteLine("Frame Rate is now " + showString);
			} else {
				bool show = (bool) cmdInput.show;
				string showString = show ? "shown" : "hidden";
				if (GrFPS._Display == show) {
					BTConsole.WriteLine("Frame Rate is already " + showString);
				} else {
					GrFPS._Display = show;
					BTConsole.WriteLine("Frame Rate is now " + showString);
				}
			}
		}

		private class BTFrameRateInput : BTConsoleCommand.BTCommandInput {
			public object show;

			private void SetShow(object show, bool isPresent) {
				this.show = isPresent ? show : null;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"show",
								true,
								"show/hide the FrameRate UI - otherwise toggles",
								SetShow,
								typeof(bool)
						)
				};
			}
		}

		private static void OnExecuteFrameRateDegrade(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTFrameRateDegradeInput) input;
			if (cmdInput.enable == null) {
				GrFPS._AutoDegradeActive = !GrFPS._AutoDegradeActive;
				string enableString = GrFPS._AutoDegradeActive ? "enabled" : "disabled";
				BTConsole.WriteLine("AutoDegrade is now " + enableString);
			} else {
				bool enable = (bool) cmdInput.enable;
				string enableString = enable ? "enabled" : "disabled";
				if (GrFPS._AutoDegradeActive == enable) {
					BTConsole.WriteLine("AutoDegrade is already " + enableString);
				} else {
					GrFPS._AutoDegradeActive = enable;
					BTConsole.WriteLine("AutoDegrade is now " + enableString);
				}
			}
		}

		private class BTFrameRateDegradeInput : BTConsoleCommand.BTCommandInput {
			public object enable;

			private void SetEnable(object enable, bool isPresent) {
				this.enable = isPresent ? enable : null;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"enable",
								true,
								"enable/disable autoDegrade - otherwise toggles",
								SetEnable,
								typeof(bool)
						)
				};
			}
		}

		private static void OnExecuteFrameRateRefresh(BTConsoleCommand.BTCommandInput input) {
			UiDebugInfo.Instance.ComputeStats();
			BTConsole.WriteLine("refreshing stats");
		}
	}
}