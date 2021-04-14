namespace SoD_BaseMod.basemod
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
