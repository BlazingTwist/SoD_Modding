using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using BlazingTwistConfigTools.blazingtwist.config;
using SoD_BaseMod.config;
using SoD_BaseMod.config.toggleScript;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace SoD_BaseMod {
	public class BTConfigHolder {
		public static readonly string basePath = Application.dataPath + "/BlazingTwist/";

		private const string configFileName = "config.cs";
		private const string hackConfigFileName = "hackConfig.cs";
		private const string hackEnableFileName = "enableHacks.7bTRGia50U";
		private const string flightStatsFileName = "flightStatsOverride.cs";
		private const string toggleScriptDirName = "toggleScripts/";

		private const string logFileName = "log.txt";

		private static readonly ILogger logger = Debug.unityLogger;
		public BTConfig config;
		public BTHackConfig hackConfig;
		public AvAvatarFlyingData flightStats;

		public static void LogMessage(LogType logType, object message) {
			logger.Log(logType, message);
		}

		public void HandleLog(string logString, string stackTrace, LogType type) {
			if (logString == null) {
				logString = "nullString";
			}

			if (stackTrace == null) {
				stackTrace = "nullString";
			}

			string logTypeString = type.ToString();
			BTLoggerConfigEntry loggerConfigEntry;
			if (config != null && config.loggerConfig.ContainsKey(logTypeString)) {
				loggerConfigEntry = config.loggerConfig[logTypeString];
			} else {
				loggerConfigEntry = new BTLoggerConfigEntry();
			}

			if (!loggerConfigEntry.AnythingToLog()) {
				return;
			}

			if (config != null && config.logMessageFilter.Any(filter => Regex.IsMatch(logString, filter))) {
				return;
			}

			var logBuilder = new StringBuilder();
			logBuilder
					.Append("[")
					.Append(logTypeString)
					.Append("]\t[")
					.Append(DateTime.Now.ToString("T"))
					.Append("]");

			if (loggerConfigEntry.logMessage) {
				logBuilder
						.Append("\n")
						.Append(logString);
			}

			if (loggerConfigEntry.logStackTrace) {
				logBuilder.Append("\nProvided Trace: ").Append(stackTrace);
				var trace = new StackTrace();
				logBuilder.Append("\n").Append(trace);
			}

			logBuilder.Append("\n");

			using (var writer = new StreamWriter((basePath + logFileName).Replace('/', Path.DirectorySeparatorChar), true)) {
				writer.WriteLine(logBuilder.ToString());
			}
		}

		public void LoadConfigs() {
			config = BTConfigUtils.LoadConfigFile(basePath + configFileName, config);
			LoadHackConfig();
			if (hackConfig != null && hackConfig.controls_useFlightStatsOverride) {
				flightStats = BTConfigUtils.LoadConfigFile(basePath + flightStatsFileName, flightStats);
			}

			string toggleScriptDirPath = (basePath + toggleScriptDirName).Replace('/', Path.DirectorySeparatorChar);
			if (!Directory.Exists(toggleScriptDirPath)) {
				Directory.CreateDirectory(toggleScriptDirPath);
			}
		}

		private static bool AreHacksEnabled() {
			return File.Exists((basePath + hackEnableFileName).Replace('/', Path.DirectorySeparatorChar));
		}

		private void LoadHackConfig() {
			if (!AreHacksEnabled()) {
				hackConfig = null;
				return;
			}

			try {
				using (StreamReader reader = File.OpenText((basePath + hackConfigFileName).Replace('/', Path.DirectorySeparatorChar))) {
					hackConfig = BTConfigTools.LoadConfig<BTHackConfig>(reader);
				}
			} catch (Exception e) {
				LogMessage(LogType.Error, "Encountered an exception during parsing of the hackConfig!\nException: " + e);
				hackConfig = null;
			}
		}

		public static List<BTToggleScriptEntry> LoadToggleScript(string scriptName) {
			string toggleScriptPath = (basePath + toggleScriptDirName + scriptName).Replace('/', Path.DirectorySeparatorChar);
			if (!File.Exists(toggleScriptPath)) {
				throw new FileNotFoundException($"Unable to find toggleScript at '{toggleScriptPath}'");
			}
			List<BTToggleScriptEntry> result = new List<BTToggleScriptEntry>();
			using (StreamReader reader = new StreamReader(toggleScriptPath)) {
				DataContractSerializer serializer = new DataContractSerializer(typeof(BTToggleScriptEntry));
				string line;
				while ((line = reader.ReadLine()) != null) {
					using (MemoryStream stringStream = new MemoryStream(Encoding.UTF8.GetBytes(line))) {
						BTToggleScriptEntry entry = (BTToggleScriptEntry) serializer.ReadObject(stringStream);
						result.Add(entry);
					}
				}
			}
			return result;
		}
	}
}