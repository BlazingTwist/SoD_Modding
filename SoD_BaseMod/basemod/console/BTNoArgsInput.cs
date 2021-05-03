using System.Collections.Generic;

namespace SoD_BaseMod.basemod.console {
	public class BTNoArgsInput : BTConsoleCommand.BTCommandInput {
		protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
			return new List<BTConsoleCommand.BTConsoleArgument>();
		}
	}
}