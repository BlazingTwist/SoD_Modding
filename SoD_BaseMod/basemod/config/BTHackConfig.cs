using System.Collections.Generic;
using JetBrains.Annotations;

namespace SoD_BaseMod.basemod.config {
	[PublicAPI]
	public class BTHackConfig {
		public bool unlockStartButtons;
		public bool fastOpenInventoryChests;

		public bool infiniteDragonMeter;

		public bool eelRoast_spawnAllEels;
		public bool eelRoast_infiniteEels;

		public bool fireball_infiniteShots;
		public bool fireball_cooldownOverride;
		public bool fireball_infiniteTargetRange;
		public bool fireball_autoFireOnHold;

		public int squadTactics_autochest;
		public bool squadTactics_infiniteRange;
		public bool squadTactics_infiniteMoves;
		public bool squadTactics_infiniteActions;

		public float controls_fastMovementFactor;
		public bool controls_useSpeedHacks;
		public bool controls_useFlightStatsOverride;

		public int stableMission_forceMissionID = -1;
		public bool stableMission_instantCompletion;
		public bool stableMission_forceWin;
		public int stableMission_triggerRewardCount = 1;

		public string disableDragonGlowRegex;
		public List<string> disableDragonGlow;

		public Dictionary<string, List<string>> inputBinds;
	}
}