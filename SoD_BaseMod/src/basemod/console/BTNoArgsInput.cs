using System.Collections.Generic;

namespace SoD_BaseMod.console {
	public class BTNoArgsInput : BTConsoleCommand.BTCommandInput {
		protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
			return new List<BTConsoleCommand.BTConsoleArgument>();
		}
	}
}