using System;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using UnityEngine;

namespace SoD_BaseMod.basemod
{
	public class BTConfigHolder
	{
		public static string basePath = Application.dataPath + "/BlazingTwist/";

		private static readonly string configFileName = "config.cs";
		private static readonly string hackConfigFileName = "hackConfig.cs";
		private static readonly string hackEnableFileName = "enableHacks.7bTRGia50U";
		private static readonly string flightStatsFileName = "flightStatsOverride.cs";

		private static readonly string logFileName = "log.txt";

		private static readonly ILogger logger = Debug.unityLogger;
		public BTConfig config = null;
		public BTHackConfig hackConfig = null;
		public AvAvatarFlyingData flightStats = null;

		public void LogMessage(LogType logType, object message) {
			logger.Log(logType, message);
		}

		public void HandleLog(string logString, string stackTrace, LogType type) {
			if(logString == null) {
				logString = "nullString";
			}
			if(stackTrace == null) {
				stackTrace = "nullString";
			}
			string logTypeString = type.ToString();
			BTLoggerConfigEntry loggerConfigEntry;
			if(config != null && config.loggerConfig.ContainsKey(logTypeString)) {
				loggerConfigEntry = config.loggerConfig[logTypeString];
			} else {
				loggerConfigEntry = new BTLoggerConfigEntry();
			}

			if(!loggerConfigEntry.AnythingToLog()) {
				return;
			}

			if(config != null && config.logMessageFilter.Any(filter => Regex.IsMatch(logString, filter))) {
				return;
			}

			StringBuilder logBuilder = new StringBuilder();
			logBuilder
					.Append("[")
					.Append(logTypeString)
					.Append("]\t[")
					.Append(DateTime.Now.ToString("T"))
					.Append("]");

			if(loggerConfigEntry.logMessage) {
				logBuilder
						.Append("\n")
						.Append(logString);
			}

			if(loggerConfigEntry.logStackTrace) {
				System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
				logBuilder
						.Append("\n")
						.Append(trace.ToString());
			}

			logBuilder.Append("\n");

			using(StreamWriter writer = new StreamWriter((basePath + logFileName).Replace('/', Path.DirectorySeparatorChar), true)) {
				writer.WriteLine(logBuilder.ToString());
			}
		}

		public void LoadConfigs() {
			config = LoadConfigFile<BTConfig>(configFileName, config);
			LoadHackConfig();
			if(hackConfig != null && hackConfig.controls_useFlightStatsOverride) {
				flightStats = LoadConfigFile<AvAvatarFlyingData>(flightStatsFileName, flightStats);
			}
		}

		private bool AreHacksEnabled() {
			return File.Exists((basePath + hackEnableFileName).Replace('/', Path.DirectorySeparatorChar));
		}

		private void LoadHackConfig() {
			if(!AreHacksEnabled()) {
				hackConfig = null;
				return;
			}

			try {
				using(StreamReader reader = File.OpenText((basePath + hackConfigFileName).Replace('/', Path.DirectorySeparatorChar))) {
					hackConfig = BTConfigUtils.LoadConfig<BTHackConfig>(reader);
				}
			} catch(Exception e) {
				LogMessage(LogType.Error, "Encountered an exception during parsing of the hackConfig!\nException: " + e.ToString());
				hackConfig = null;
			}
		}

		private T LoadConfigFile<T>(string fileName, T instance) {
			String filePath = (BTConfigHolder.basePath + fileName).Replace('/', Path.DirectorySeparatorChar);
			if(!File.Exists(filePath)) {
				return default;
			}

			try {
				using(StreamReader reader = File.OpenText(filePath)) {
					if(instance == null) {
						return BTConfigUtils.LoadConfig<T>(reader);
					} else {
						BTConfigUtils.LoadConfig<T>(reader, instance);
						return instance;
					}
				}
			} catch(Exception e) {
				this.LogMessage(LogType.Error, "Encountered an exception during parsing of the file `" + fileName + "`!\nException: " + e.ToString());
				return default;
			}
		}
	}
}
