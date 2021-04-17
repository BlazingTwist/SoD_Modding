using System.Collections.Generic;
using JetBrains.Annotations;

namespace SoD_BaseMod.basemod.config
{
	[PublicAPI]
	public class BTHackConfig
	{
		public bool unlockStartButtons = false;
		public bool fastOpenInventoryChests = false;

		public bool infiniteDragonMeter = false;

		public bool eelRoast_spawnAllEels = false;
		public bool eelRoast_infiniteEels = false;

		public bool fireball_infiniteShots = false;
		public bool fireball_cooldownOverride = false;
		public bool fireball_infiniteTargetRange = false;
		public bool fireball_autoFireOnHold = false;

		public int squadTactics_autochest = 0;
		public bool squadTactics_infiniteRange = false;
		public bool squadTactics_infiniteMoves = false;
		public bool squadTactics_infiniteActions = false;

		public float controls_fastMovementFactor = 0f;
		public bool controls_useSpeedHacks = false;
		public bool controls_useFlightStatsOverride = false;

		public int stableMission_forceMissionID = -1;
		public bool stableMission_instantCompletion = false;
		public bool stableMission_forceWin = false;
		public int stableMission_triggerRewardCount = 1;

		public string disableDragonGlowRegex = null;
		public List<string> disableDragonGlow = null;

		public Dictionary<string, List<string>> inputBinds = null;
	}
}
