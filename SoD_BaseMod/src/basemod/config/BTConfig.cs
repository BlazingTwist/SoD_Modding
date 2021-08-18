using System.Collections.Generic;
using JetBrains.Annotations;

namespace SoD_BaseMod.config {
	[PublicAPI]
	public class BTConfig {
		public float fileCheckInterval;
		public float uiScaleFactor;

		public bool disableZZZParticles;

		public bool animPlayerPlayByDefault;
		public float animPlayerTimeStep;

		public bool cutscenePlayerPlayByDefault;
		public float cutscenePlayerTimeStep;

		// DebugCam Config
		public float cameraSpeed;
		public float cameraFastSpeed;
		public float cameraRenderDistance;
		public float cameraFOV;
		public float orthographicSize;

		public Dictionary<string, BTCutsceneConfigEntry> availableCutscenes;

		public Dictionary<string, List<string>> inputBinds;

		public Dictionary<string, BTLevelConfigEntry> loadLevelBinds;

		// Logger Config
		public Dictionary<string, BTLoggerConfigEntry> loggerConfig;
		public List<string> logMessageFilter;
		public bool enableUTDebug;
		public bool logServiceCalls;
		public List<WsServiceType> serviceCallFilter;

		// Console Config
		public float consoleHeight = 0.3f;
		public int suggestionCount = 5;
		public bool consoleOpenByDefault;
		public bool consoleReverseOutput;
		public string consoleDefaultCommand = "help";
		public Dictionary<string, List<string>> commandBinds;

		// Cursor
		public BTCursorVisibility cursorVisibility = BTCursorVisibility.Default;
	}
}