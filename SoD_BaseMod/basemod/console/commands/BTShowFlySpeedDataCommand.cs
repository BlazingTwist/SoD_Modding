using System.Collections.Generic;

namespace SoD_BaseMod.basemod.console.commands {
	public static class BTShowFlySpeedDataCommand {
		public static void Register() {
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "speed", "data" },
					new BTShowFlySpeedDataInput(),
					"shows / hides the Speed Data UI",
					OnExecute
			));
		}

		private static void OnExecute(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTShowFlySpeedDataInput) input;
			if (AvAvatar.pObject == null) {
				BTConsole.WriteLine("can't show Speed Data - Avatar not found");
				return;
			}

			var avatarController = AvAvatar.pObject.GetComponent<AvAvatarController>();
			if (avatarController == null) {
				BTConsole.WriteLine("can't show Speed Data - AvatarController not found");
				return;
			}

			if (cmdInput.show == null) {
				avatarController.mDisplayFlyingData = !avatarController.mDisplayFlyingData;
				string showString = avatarController.mDisplayFlyingData ? "shown" : "hidden";
				BTConsole.WriteLine("Speed Data is now " + showString);
			} else {
				bool show = (bool) cmdInput.show;
				string showString = show ? "shown" : "hidden";
				if (avatarController.mDisplayFlyingData == show) {
					BTConsole.WriteLine("Speed Data is already " + showString);
				} else {
					avatarController.mDisplayFlyingData = show;
					BTConsole.WriteLine("Speed Data is now " + showString);
				}
			}
		}

		private class BTShowFlySpeedDataInput : BTConsoleCommand.BTCommandInput {
			public object show;

			private void SetShow(object show, bool isPresent) {
				this.show = isPresent ? show : null;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"show",
								true,
								"show/hide the Speed Data UI - otherwise toggles",
								SetShow,
								typeof(bool)
						)
				};
			}
		}
	}
}