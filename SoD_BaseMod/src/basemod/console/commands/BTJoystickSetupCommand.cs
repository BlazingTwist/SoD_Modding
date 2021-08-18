using System.Collections.Generic;

namespace SoD_BaseMod.console {
	public static class BTJoystickSetupCommand {
		public static void Register() {
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "joyAim" },
					new BTJoystickSetupInput(),
					"enables / disables AvAvatarController.mAimControlMode",
					OnExecute
			));
		}

		private static void OnExecute(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTJoystickSetupInput) input;
			string enableText = cmdInput.enable ? "enabled" : "disabled";
			if (cmdInput.enable == AvAvatarController.mAimControlMode) {
				BTConsole.WriteLine("joyAim is already " + enableText);
			} else {
				AvAvatarController.mAimControlMode = cmdInput.enable;
				BTConsole.WriteLine("joyAim is now " + enableText);
			}
		}

		private class BTJoystickSetupInput : BTConsoleCommand.BTCommandInput {
			public bool enable;

			private void SetEnable(object enable, bool isPresent) {
				this.enable = (bool) enable;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"enable",
								false,
								"enable/disable joyAim",
								SetEnable,
								typeof(bool)
						)
				};
			}
		}
	}
}