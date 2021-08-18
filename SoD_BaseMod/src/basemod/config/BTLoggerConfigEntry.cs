using JetBrains.Annotations;

namespace SoD_BaseMod.config {
	[PublicAPI]
	public class BTLoggerConfigEntry {
		public bool logMessage = true;
		public bool logStackTrace;

		public bool AnythingToLog() {
			return logMessage;
		}
	}
}