using SoD_BaseMod.basemod.console.commands;

namespace SoD_BaseMod.basemod.console {
	public static class BTCommandUtils {
		public static void RegisterAll() {
			BTAchievementCommands.Register();
			BTAssetBundleCommands.Register();
			BTAvatarCommands.Register();
			BTBuildInfoCommand.Register();
			BTCheckPasswordCommand.Register();
			BTClsCommand.Register();
			BTCogsCommands.Register();
			BTCoinsAddCommand.Register();
			BTConfigReloadCommand.Register();
			BTConsumableCommands.Register();
			BTDailyCommands.Register();
			BTDebugCommands.Register();
			BTDeletePlayerPrefCommand.Register();
			BTFieldGuideUnlockCommand.Register();
			BTFishCommands.Register();
			BTFrameRateCommands.Register();
			BTGlowUICommand.Register();
			BTGPUStatsCommand.Register();
			BTHelpCommand.Register();
			BTIncredibleMachineCommands.Register();
			BTInventoryCommands.Register();
			BTJoystickSetupCommand.Register();
			BTLabCommands.Register();
			BTLevelCommands.Register();
			BTMemProfilerCommand.Register();
			BTMissionCommands.Register();
			BTMMOCommands.Register();
			BTMysteryChestSpawnAllCommand.Register();
			BTPetCommands.Register();
			BTPlayerCommands.Register();
			BTQualityCommand.Register();
			BTServerCommands.Register();
			BTShowFlySpeedDataCommand.Register();
			BTTaskCommands.Register();
			BTTutorialCommands.Register();
			BTTweakDataCommand.Register();
		}
	}
}