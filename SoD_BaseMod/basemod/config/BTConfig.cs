using System.Collections.Generic;
using JetBrains.Annotations;

namespace SoD_BaseMod.basemod.config
{
	[PublicAPI]
	public class BTConfig
	{
		public float fileCheckInterval = 0f;
		public float uiScaleFactor = 0f;

		public bool disableZZZParticles = false;

		public bool animPlayerPlayByDefault = false;
		public float animPlayerTimeStep = 0f;

		public bool cutscenePlayerPlayByDefault = false;
		public float cutscenePlayerTimeStep = 0f;

		// DebugCam Config
		public float cameraSpeed = 0f;
		public float cameraFastSpeed = 0f;
		public float cameraRenderDistance = 0f;
		public float cameraFOV = 0f;
		public float orthographicSize = 0f;

		public Dictionary<string, BTCutsceneConfigEntry> availableCutscenes = null;

		public Dictionary<string, List<string>> inputBinds = null;

		public Dictionary<string, BTLevelConfigEntry> loadLevelBinds = null;

		// Logger Config
		public Dictionary<string, BTLoggerConfigEntry> loggerConfig = null;
		public List<string> logMessageFilter = null;
		public bool enableUTDebug = false;
		public bool logServiceCalls = false;
		public List<WsServiceType> serviceCallFilter = null;

		// Console Config
		public float consoleHeight = 0.3f;
		public int suggestionCount = 5;
		public bool consoleOpenByDefault = false;
		public string consoleDefaultCommand = "help";
		public Dictionary<string, List<string>> commandBinds = null;
		
		// Cursor
		public BTCursorVisibility cursorVisibility = BTCursorVisibility.Default;
	}
}
