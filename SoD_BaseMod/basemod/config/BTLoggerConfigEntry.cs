namespace SoD_BaseMod.basemod.config
{
	public class BTLoggerConfigEntry
	{
		public bool logMessage = true;
		public bool logStackTrace = false;

		public bool AnythingToLog() {
			return logMessage;
		}
	}
}
