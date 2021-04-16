using KA.Framework;
using KnowledgeAdventure.Multiplayer;
using KnowledgeAdventure.Multiplayer.Utility;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using SquadTactics;
using UnityEngine;

namespace SoD_BaseMod.basemod
{
	public class BTCommands
	{
		public static void RegisterAll() {
			BTCommands.BTAchievementClanSet.Register();
			BTCommands.BTAchievementSet.Register();
			BTCommands.BTAchievementSetWeb.Register();
			BTCommands.BTAvatarCommands.Register();
			BTCommands.BTBuildInfo.Register();
			BTCommands.BTCls.Register();
			BTCommands.BTCogsLoad.Register();
			BTCommands.BTCogsUnlock.Register();
			BTCommands.BTCoinsAdd.Register();
			BTCommands.BTConsumableAdd.Register();
			BTCommands.BTConsumableChart.Register();
			BTCommands.BTConsumableList.Register();
			BTCommands.BTDailyBonus.Register();
			BTCommands.BTDailyQuest.Register();
			BTCommands.BTDebugDeletePlayerPrefs.Register();
			BTCommands.BTDebugFix.Register();
			BTCommands.BTDebugInfo.Register();
			BTCommands.BTDebugMask.Register();
			BTCommands.BTDebugMemDump.Register();
			BTCommands.BTDebugMemWarn.Register();
			BTCommands.BTDebugParticles.Register();
			BTCommands.BTDebugSceneObjects.Register();
			BTCommands.BTDebugUnload.Register();
			BTCommands.BTDeletePlayerPrefCommand.Register();
			BTCommands.BTFieldGuideUnlock.Register();
			BTCommands.BTFishPoleID.Register();
			BTCommands.BTFishRank.Register();
			BTCommands.BTFishRodPower.Register();
			BTCommands.BTFishWeigth.Register();
			BTCommands.BTFrameRate.Register();
			BTCommands.BTFrameRateDegrade.Register();
			BTCommands.BTFrameRateRefresh.Register();
			BTCommands.BTGlowUI.Register();
			BTCommands.BTGPUStats.Register();
			BTCommands.BTHelpCommand.Register();
			BTCommands.BTIncredibleMachineLoad.Register();
			BTCommands.BTIncredibleMachineUnlock.Register();
			BTCommands.BTInventoryCommands.Register();
			BTCommands.BTJoystickSetup.Register();
			BTCommands.BTLabExperimentGet.Register();
			BTCommands.BTLabExperimentSet.Register();
			BTCommands.BTLevelGet.Register();
			BTCommands.BTLevelLoad.Register();
			BTCommands.BTMemProfiler.Register();
			BTCommands.BTMissionCommands.Register();
			BTCommands.BTMMOInfo.Register();
			BTCommands.BTMMOUsersShow.Register();
			BTCommands.BTMysteryChestSpawnAll.Register();
			BTCommands.BTPetCreate.Register();
			BTCommands.BTPetRelease.Register();
			BTCommands.BTPetState.Register();
			BTCommands.BTPlayerXPAdd.Register();
			BTCommands.BTPlayerXPGet.Register();
			BTCommands.BTQualityCommand.Register();
			BTCommands.BTServerTimeAdd.Register();
			BTCommands.BTServerTimeGet.Register();
			BTCommands.BTServerTimeReset.Register();
			BTCommands.BTShowFlySpeedData.Register();
			BTCommands.BTTaskCommands.Register();
			BTCommands.BTTutorialComplete.Register();
			BTCommands.BTTutorialReset.Register();
			BTCommands.BTTweakData.Register();
			BTCommands.BTAssetBundleList.Register();
			BTCommands.BTAssetBundleLoad.Register();
			BTCommands.BTConfigReload.Register();
			BTCommands.BTPetMeter.Register();
			BTCommands.BTPetAge.Register();
			BTCommands.BTCheckPass.Register();
		}

		public class BTNoArgsInput : BTConsoleCommand.BTCommandInput
		{
			protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument>();
			}
		}

		public class BTHelpCommand
		{
			public static void Register() {
				BTConsole.AddCommand(new BTConsoleCommand(
					new List<string>() { "help" },
					new BTNoArgsInput(),
					"list all available commands",
					OnExecute
				));
				BTConsole.AddCommand(new BTConsoleCommand(
					new List<string>() { "help", "mdFormat" },
					new BTNoArgsInput(),
					"prints all commands in a md-table format",
					OnExecuteMDFormat
				));
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTConsole.HelpAll();
			}

			public static void OnExecuteMDFormat(BTConsoleCommand.BTCommandInput input) {
				BTConsole.BuildMDHelpTable();
			}
		}

		public class BTCls
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "cls" },
					new BTNoArgsInput(),
					"clears console output",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTConsole.ClearConsole();
			}
		}

		public class BTDeletePlayerPrefCommand
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "DeletePlayerPrefs" },
					new BTDeletePlayerPrefCommandInput(),
					"deletes a key from the player prefs",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTDeletePlayerPrefCommandInput cmdInput = (BTDeletePlayerPrefCommandInput)input;
				if(PlayerPrefs.HasKey(cmdInput.keyName)) {
					PlayerPrefs.DeleteKey(cmdInput.keyName);
					BTConsole.WriteLine("Deleted " + cmdInput.keyName + " from player prefs.");
				} else {
					BTConsole.WriteLine(cmdInput.keyName + " is not present in player prefs.");
				}
			}

			public class BTDeletePlayerPrefCommandInput : BTConsoleCommand.BTCommandInput
			{
				public string keyName = null;

				private void SetKeyName(object keyName, bool isPresent) {
					this.keyName = (string)keyName;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"keyname",
						false,
						"key(name) of the playerPref",
						this.SetKeyName,
						typeof(string)
					)
				};
				}
			}
		}

		public class BTDebugMask
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "DebugMask" },
					new BTDebugMaskInput(),
					"adds or clears a UtDebug._Mask",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTDebugMaskInput cmdInput = (BTDebugMaskInput)input;
				if(cmdInput.add) {
					UtDebug._Mask |= cmdInput.mask;
					BTConsole.WriteLine("DebugMask " + cmdInput.mask + " added.");
				} else {
					UtDebug._Mask &= ~cmdInput.mask;
					BTConsole.WriteLine("DebugMask " + cmdInput.mask + " cleared.");
				}
				Log._Mask = UtDebug._Mask;
				BTConsole.WriteLine("DebugMask is " + UtDebug._Mask);
			}

			public class BTDebugMaskInput : BTConsoleCommand.BTCommandInput
			{
				public uint mask;
				public bool add;

				private void SetMask(object mask, bool isPresent) {
					this.mask = (uint)mask;
				}

				private void SetAdd(object add, bool isPresent) {
					this.add = (bool)add;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"mask",
						false,
						"number of the render-mask to add/clear",
						this.SetMask,
						typeof(uint)
					),
					new BTConsoleCommand.BTConsoleArgument(
						"add/clear",
						false,
						"true = add | false = clear",
						this.SetAdd,
						typeof(bool)
					)
				};
				}
			}
		}

		public class BTBuildInfo
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "BuildInfo" },
					new BTNoArgsInput(),
					"prints build information to console",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTConsole.WriteLine("Unity Project ID - " + Application.cloudProjectId);
				string changeListNumber = ProductSettings.pInstance.GetChangelistNumber();
				string formattedCLNumber = (String.IsNullOrEmpty(changeListNumber) ? "not found" : changeListNumber);
				BTConsole.WriteLine("Application.version - " + Application.version);
				BTConsole.WriteLine("Application.unityVersion - " + Application.unityVersion);
				BTConsole.WriteLine("ChangeList Number - " + formattedCLNumber);
				BTConsole.WriteLine("Version - " + ProductConfig.pProductVersion);
				BTConsole.WriteLine("Environment - " + ProductConfig.GetEnvironmentForBundles().ToString());
				BTConsole.WriteLine("Current Platform - " + UtPlatform.GetPlatformName());
			}
		}

		public class BTAvatarCommands
		{
			public static Vector3? previousPosition = null;

			public static void Register() {
				BTConsole.AddCommand(new BTConsoleCommand(
					new List<string>() { "avatar", "get", "position" },
					new GetPositionInput(),
					"prints the avatar's current position to the console",
					OnExecuteGetPosition
				));
				BTConsole.AddCommand(new BTConsoleCommand(
					new List<string>() { "avatar", "teleport", "object" },
					new TeleportToObjectInput(),
					"teleports the avatar to a given object",
					OnExecuteTeleportToObject
				));
				BTConsole.AddCommand(new BTConsoleCommand(
					new List<string>() { "avatar", "teleport", "position" },
					new TeleportToPositionInput(),
					"teleports the avatar to a given position",
					OnExecuteTeleportToPosition
				));
				BTConsole.AddCommand(new BTConsoleCommand(
					new List<string>() { "avatar", "teleport", "camera" },
					new BTNoArgsInput(),
					"teleports the avatar to the main-camera",
					OnExecuteTeleportCamera
				));
				BTConsole.AddCommand(new BTConsoleCommand(
					new List<string>() { "avatar", "teleport", "back" },
					new BTNoArgsInput(),
					"teleports the avatar to the 'previous' position",
					OnExecuteTeleportBack
				));
				BTConsole.AddCommand(new BTConsoleCommand(
					new List<string>() { "avatar", "clear", "data" },
					new BTNoArgsInput(),
					"clears the avatarData (parts)",
					OnExecuteClearData
				));
				BTConsole.AddCommand(new BTConsoleCommand(
					new List<string>() { "avatar", "fix" },
					new BTNoArgsInput(),
					"tries to fix the avatar by reloading the model without version/head information",
					OnExecuteFix
				));
				BTConsole.AddCommand(new BTConsoleCommand(
					new List<string>() { "avatar", "save", "data" },
					new BTNoArgsInput(),
					"calls the AvatarData.Save function",
					OnExecuteSaveData
				));
				BTConsole.AddCommand(new BTConsoleCommand(
					new List<string>() { "avatar", "show", "userID" },
					new BTNoArgsInput(),
					"prints the userID to console",
					OnExecuteShowUserID
				));
				BTConsole.AddCommand(new BTConsoleCommand(
					new List<string>() { "avatar", "show", "state" },
					new BTNoArgsInput(),
					"prints information on the avatar's state",
					OnExecuteShowState
				));
				BTConsole.AddCommand(new BTConsoleCommand(
					new List<string>() { "avatar", "set", "stat" },
					new SetStatInput(),
					"sets the statValue of a worn part",
					OnExecuteSetStat
				));
				BTConsole.AddCommand(new BTConsoleCommand(
					new List<string>() { "avatar", "set", "speed" },
					new SetSpeedInput(),
					"sets the avatar speed",
					OnExecuteSetSpeed
				));
				BTConsole.AddCommand(new BTConsoleCommand(
					new List<string>() { "avatar", "update", "flyingData" },
					new BTNoArgsInput(),
					"invokes the avatar's 'OnUpdateAvatar' function",
					OnExecuteUpdateFlyingData
				));
			}

			public static void OnExecuteGetPosition(BTConsoleCommand.BTCommandInput input) {
				GetPositionInput cmdInput = (GetPositionInput)input;
				if(AvAvatar.pObject == null) {
					BTConsole.WriteLine("Unable to print position - Avatar not found");
					return;
				}
				Vector3 currentPosition = AvAvatar.GetPosition();
				if(cmdInput.makePreviousPosition) {
					previousPosition = currentPosition;
					BTConsole.WriteLine("Set previousPosition to: " + currentPosition.ToString());
				} else {
					BTConsole.WriteLine("Current Avatar Position: " + currentPosition.ToString());
				}
			}

			public class GetPositionInput : BTConsoleCommand.BTCommandInput
			{
				public bool makePreviousPosition;

				private void SetMakePreviousPosition(object makePreviousPosition, bool isPresent) {
					this.makePreviousPosition = isPresent ? (bool)makePreviousPosition : false;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"makePreviousPosition",
						true,
						"if true, will set the previousPosition to the current position, defaults to false",
						this.SetMakePreviousPosition,
						typeof(bool)
					)
				};
				}
			}

			public static void OnExecuteTeleportToObject(BTConsoleCommand.BTCommandInput input) {
				TeleportToObjectInput cmdInput = (TeleportToObjectInput)input;
				if(AvAvatar.pObject == null) {
					BTConsole.WriteLine("Unable to teleport - Avatar not found");
					return;
				}
				GameObject gameObject = GameObject.Find(cmdInput.objectName);
				if(gameObject == null) {
					BTConsole.WriteLine("Unable to teleport - No GameObject of name " + cmdInput.objectName + " found");
					return;
				}
				previousPosition = AvAvatar.GetPosition();
				if(!UtUtilities.FindPosNextToObject(out Vector3 targetPosition, gameObject, cmdInput.distance)) {
					BTConsole.WriteLine("Unable to teleport - No valid position within '" + cmdInput.distance + "' units of '" + cmdInput.objectName + "' found");
					return;
				}
				AvAvatar.TeleportTo(targetPosition);
				AvAvatar.mTransform.LookAt(gameObject.transform);
				BTConsole.WriteLine("Avatar teleported from " + previousPosition.ToString() + " to " + targetPosition.ToString());
			}

			public class TeleportToObjectInput : BTConsoleCommand.BTCommandInput
			{
				public string objectName;
				public float distance;

				private void SetObjectName(object objectName, bool isPresent) {
					this.objectName = (string)objectName;
				}

				private void SetDistance(object distance, bool isPresent) {
					this.distance = (float)distance;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"object name",
						false,
						"name of the object to teleport to",
						this.SetObjectName,
						typeof(string)
					),
					new BTConsoleCommand.BTConsoleArgument(
						"distance",
						true,
						"maximum distance from object",
						this.SetDistance,
						typeof(float)
					)
				};
				}
			}

			public static void OnExecuteTeleportToPosition(BTConsoleCommand.BTCommandInput input) {
				TeleportToPositionInput cmdInput = (TeleportToPositionInput)input;
				if(AvAvatar.pObject == null) {
					BTConsole.WriteLine("Unable to teleport - Avatar not found");
					return;
				}
				previousPosition = AvAvatar.GetPosition();
				Vector3 targetPosition = new Vector3(cmdInput.x, cmdInput.y, cmdInput.z);
				AvAvatar.TeleportTo(targetPosition);
				BTConsole.WriteLine("Avatar teleported from " + previousPosition.ToString() + " to " + targetPosition.ToString());
			}

			public class TeleportToPositionInput : BTConsoleCommand.BTCommandInput
			{
				public float x;
				public float y;
				public float z;

				private void SetX(object x, bool isPresent) {
					this.x = (float)x;
				}

				private void SetY(object y, bool isPresent) {
					this.y = (float)y;
				}

				private void SetZ(object z, bool isPresent) {
					this.z = (float)z;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"x",
						true,
						"x value of the target position",
						this.SetX,
						typeof(float)
					),
					new BTConsoleCommand.BTConsoleArgument(
						"y",
						true,
						"y value of the target position",
						this.SetY,
						typeof(float)
					),
					new BTConsoleCommand.BTConsoleArgument(
						"z",
						true,
						"z value of the target position",
						this.SetZ,
						typeof(float)
					)
				};
				}
			}

			public static void OnExecuteTeleportCamera(BTConsoleCommand.BTCommandInput input) {
				Camera mainCamera = BTDebugCam.FindMainCamera();
				if(mainCamera == null) {
					BTConsole.WriteLine("Unable to teleport - mainCamera not found.");
					return;
				}
				previousPosition = AvAvatar.GetPosition();
				Vector3 targetPosition = mainCamera.transform.position;
				AvAvatar.TeleportTo(targetPosition);
				BTConsole.WriteLine("Avatar teleported from " + previousPosition.ToString() + " to " + targetPosition.ToString());
			}

			public static void OnExecuteTeleportBack(BTConsoleCommand.BTCommandInput input) {
				if(previousPosition == null) {
					BTConsole.WriteLine("Unable to teleport - no previous position known. (Only gets set when teleporting using the console commands)");
					return;
				}
				Vector3 targetPosition = previousPosition.Value;
				previousPosition = AvAvatar.GetPosition();
				AvAvatar.TeleportTo(targetPosition);
				BTConsole.WriteLine("Avatar teleported from " + previousPosition.ToString() + " to " + targetPosition.ToString());
			}

			public static void OnExecuteClearData(BTConsoleCommand.BTCommandInput input) {
				AvatarData.Clear();
				BTConsole.WriteLine("Avatar parts cleared");
			}

			public static void OnExecuteFix(BTConsoleCommand.BTCommandInput input) {
				AvatarData.pInstanceInfo.RestorePartData();
				AvatarData avatarData = AvatarData.CreateDefault(AvatarData.pInstance.GenderType);
				List<AvatarDataPart> partList = new List<AvatarDataPart>();
				foreach(AvatarDataPart avatarDataPart in avatarData.Part) {
					if(avatarDataPart.PartType == "Version") {
						partList.Add(AvatarData.FindPart(avatarDataPart.PartType));
					} else if(avatarDataPart.PartType == AvatarData.pPartSettings.AVATAR_PART_HEAD) {
						partList.Add(AvatarData.FindPart(avatarDataPart.PartType));
					} else {
						partList.Add(avatarDataPart);
					}
				}
				AvatarData.pInstance.Part = partList.ToArray();
				AvatarData.pInstanceInfo.LoadBundlesAndUpdateAvatar();
				BTConsole.WriteLine("Avatar data reset - excluded version and head");
			}

			public static void OnExecuteSaveData(BTConsoleCommand.BTCommandInput input) {
				AvatarData.Save();
				BTConsole.WriteLine("Avatar data reset - excluded version and head");
			}

			public static void OnExecuteShowUserID(BTConsoleCommand.BTCommandInput input) {
				if(UserInfo.pInstance != null) {
					BTConsole.WriteLine("UserID: " + UserInfo.pInstance.UserID);
				} else {
					BTConsole.WriteLine("can't print userID - no UserInfo found.");
				}
			}

			public static void OnExecuteShowState(BTConsoleCommand.BTCommandInput input) {
				BTConsole.WriteLine("CurrentState: " + AvAvatar.pState + " | SubState: " + AvAvatar.pSubState);
				BTConsole.WriteLine("Old State: " + AvAvatar.pPrevState);
			}

			public static void OnExecuteSetStat(BTConsoleCommand.BTCommandInput input) {
				SetStatInput cmdInput = (SetStatInput)input;
				if(AvatarData.pInstanceInfo.FindPart(cmdInput.part) == null) {
					BTConsole.WriteLine("Part " + cmdInput.part + " not found");
					return;
				}
				UserItemData itemData = CommonInventoryData.pInstance
						.FindItemByUserInventoryID(AvatarData.pInstanceInfo.GetPartInventoryID(cmdInput.part));
				if(itemData == null) {
					BTConsole.WriteLine("No inventory item found for " + cmdInput.part);
					return;
				}
				string statEffectName = Settings.pInstance.GetStatEffectName(cmdInput.statID);
				if(itemData.ItemStats != null) {
					ItemStat itemStat = Array.Find<ItemStat>(itemData.ItemStats, (ItemStat x) => x.ItemStatID == cmdInput.statID);
					if(itemStat == null) {
						// stat is not in item -> add it manually
						ItemStat[] itemStats = itemData.ItemStats;
						Array.Resize<ItemStat>(ref itemStats, itemData.ItemStats.Length + 1);
						itemStat = new ItemStat {
							ItemStatID = cmdInput.statID,
							Value = cmdInput.value
						};
						itemStats[itemStats.Length - 1] = itemStat;
						itemData.ItemStats = itemStats;
						BTConsole.WriteLine("Added '" + statEffectName + "' of value '" + cmdInput.value + "' to part '" + cmdInput.part + "'");
					} else {
						string previousValue = itemStat.Value;
						itemStat.Value = cmdInput.value;
						BTConsole.WriteLine("Changed '" + statEffectName + "' of part '" + cmdInput.part + "' from '" + previousValue + "' to '" + cmdInput.value + "'");
					}
				} else {
					itemData.ItemStats = new ItemStat[1];
					itemData.ItemStats[0] = new ItemStat {
						ItemStatID = cmdInput.statID,
						Value = cmdInput.value
					};
					BTConsole.WriteLine("Added '" + statEffectName + "' of value '" + cmdInput.value + "' to part '" + cmdInput.part + "'");
				}
			}

			public class SetStatInput : BTConsoleCommand.BTCommandInput
			{
				public string part;
				public int statID;
				public string value;

				private void SetPart(object part, bool isPresent) {
					this.part = (string)part;
				}

				private void SetStatID(object statID, bool isPresent) {
					this.statID = (int)statID;
				}

				private void SetValue(object value, bool isPresent) {
					this.value = (string)value;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"part",
						false,
						"name of the part to modify stats on",
						this.SetPart,
						typeof(string)
					),
					new BTConsoleCommand.BTConsoleArgument(
						"statID",
						false,
						"ID of the stat to modify",
						this.SetStatID,
						typeof(int)
					),
					new BTConsoleCommand.BTConsoleArgument(
						"value",
						false,
						"value the stat shall take",
						this.SetValue,
						typeof(string)
					)
				};
				}
			}

			public static void OnExecuteSetSpeed(BTConsoleCommand.BTCommandInput input) {
				SetSpeedInput cmdInput = (SetSpeedInput)input;
				if(AvAvatar.pObject == null) {
					BTConsole.WriteLine("error - Avatar not found, can't set speed");
					return;
				}
				AvAvatarController component = AvAvatar.pObject.GetComponent<AvAvatarController>();
				if(component == null) {
					BTConsole.WriteLine("error - AvatarController not found, can't set speed");
					return;
				}
				float previousSpeed = component.pCurrentStateData._MaxForwardSpeed;
				component.pCurrentStateData._MaxForwardSpeed = cmdInput.speed;
				BTConsole.WriteLine("changed avatar speed from '" + previousSpeed + "' to '" + cmdInput.speed + "'");
			}

			public class SetSpeedInput : BTConsoleCommand.BTCommandInput
			{
				public float speed;

				private void SetSpeed(object speed, bool isPresent) {
					this.speed = (float)speed;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"speed",
						false,
						"target avatar speed",
						this.SetSpeed,
						typeof(float)
					)
				};
				}
			}

			public static void OnExecuteUpdateFlyingData(BTConsoleCommand.BTCommandInput input) {
				AvAvatar.pObject.GetComponent<AvAvatarController>().OnUpdateAvatar();
				BTConsole.WriteLine("FlyingData updated.");
			}
		}

		public class BTJoystickSetup
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "joyaim" },
					new BTJoystickSetupInput(),
					"enables / disables AvAvatarController.mAimControlMode",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTJoystickSetupInput cmdInput = (BTJoystickSetupInput)input;
				string enableText = cmdInput.enable ? "enabled" : "disabled";
				if(cmdInput.enable == AvAvatarController.mAimControlMode) {
					BTConsole.WriteLine("joyaim is already " + enableText);
				} else {
					AvAvatarController.mAimControlMode = cmdInput.enable;
					BTConsole.WriteLine("joyaim is now " + enableText);
				}
			}

			public class BTJoystickSetupInput : BTConsoleCommand.BTCommandInput
			{
				public bool enable;

				private void SetEnable(object enable, bool isPresent) {
					this.enable = (bool)enable;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"enable",
						false,
						"enable/disable joyaim",
						this.SetEnable,
						typeof(bool)
					)
				};
				}
			}
		}

		public class BTTweakData
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "TweakData" },
					new BTTweakDataInput(),
					"shows/hides Input TweakData",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTTweakDataInput cmdInput = (BTTweakDataInput)input;
				string showString = cmdInput.show ? "shown" : "hidden";
				KAInput.pInstance.ShowTweak(cmdInput.show);
				BTConsole.WriteLine("Set Input TweakData to: " + showString);
			}

			public class BTTweakDataInput : BTConsoleCommand.BTCommandInput
			{
				public bool show;

				private void SetShow(object show, bool isPresent) {
					this.show = (bool)show;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"show",
						false,
						"show/hide Input TweakData",
						this.SetShow,
						typeof(bool)
					)
				};
				}
			}
		}

		public class BTShowFlySpeedData
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "speed", "data" },
					new BTShowFlySpeedDataInput(),
					"shows / hides the Speed Data UI",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTShowFlySpeedDataInput cmdInput = (BTShowFlySpeedDataInput)input;
				if(AvAvatar.pObject == null) {
					BTConsole.WriteLine("can't show Speed Data - Avatar not found");
					return;
				}
				AvAvatarController avatarController = AvAvatar.pObject.GetComponent<AvAvatarController>();
				if(avatarController == null) {
					BTConsole.WriteLine("can't show Speed Data - AvatarController not found");
					return;
				}
				if(cmdInput.show == null) {
					avatarController.mDisplayFlyingData = !avatarController.mDisplayFlyingData;
					string showString = avatarController.mDisplayFlyingData ? "shown" : "hidden";
					BTConsole.WriteLine("Speed Data is now " + showString);
				} else {
					bool show = (bool)cmdInput.show;
					string showString = show ? "shown" : "hidden";
					if(avatarController.mDisplayFlyingData == show) {
						BTConsole.WriteLine("Speed Data is already " + showString);
					} else {
						avatarController.mDisplayFlyingData = show;
						BTConsole.WriteLine("Speed Data is now " + showString);
					}
				}
			}

			public class BTShowFlySpeedDataInput : BTConsoleCommand.BTCommandInput
			{
				public object show;

				private void SetShow(object show, bool isPresent) {
					this.show = isPresent ? show : null;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"show",
						true,
						"show/hide the Speed Data UI - otherwise toggles",
						this.SetShow,
						typeof(bool)
					)
				};
				}
			}
		}

		public class BTFrameRate
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "FrameRate" },
					new BTFrameRateInput(),
					"shows / hides the framerate ui",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTFrameRateInput cmdInput = (BTFrameRateInput)input;
				if(cmdInput.show == null) {
					GrFPS._Display = !GrFPS._Display;
					string showString = GrFPS._Display ? "shown" : "hidden";
					BTConsole.WriteLine("Frame Rate is now " + showString);
				} else {
					bool show = (bool)cmdInput.show;
					string showString = show ? "shown" : "hidden";
					if(GrFPS._Display == show) {
						BTConsole.WriteLine("Frame Rate is already " + showString);
					} else {
						GrFPS._Display = show;
						BTConsole.WriteLine("Frame Rate is now " + showString);
					}
				}
			}

			public class BTFrameRateInput : BTConsoleCommand.BTCommandInput
			{
				public object show;

				private void SetShow(object show, bool isPresent) {
					this.show = isPresent ? show : null;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"show",
						true,
						"show/hide the FrameRate UI - otherwise toggles",
						this.SetShow,
						typeof(bool)
					)
				};
				}
			}
		}

		public class BTFrameRateRefresh
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "FrameRate", "refresh" },
					new BTNoArgsInput(),
					"recomputes FrameRate stats",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				UiDebugInfo.Instance.ComputeStats();
				BTConsole.WriteLine("refreshing stats");
			}
		}

		public class BTFrameRateDegrade
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "FrameRate", "degrade" },
					new BTFrameRateDegradeInput(),
					"enables/disables the auto-degrade feature of the FPS UI",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTFrameRateDegradeInput cmdInput = (BTFrameRateDegradeInput)input;
				if(cmdInput.enable == null) {
					GrFPS._AutoDegradeActive = !GrFPS._AutoDegradeActive;
					string enableString = GrFPS._AutoDegradeActive ? "enabled" : "disabled";
					BTConsole.WriteLine("AutoDegrade is now " + enableString);
				} else {
					bool enable = (bool)cmdInput.enable;
					string enableString = enable ? "enabled" : "disabled";
					if(GrFPS._AutoDegradeActive == enable) {
						BTConsole.WriteLine("AutoDegrade is already " + enableString);
					} else {
						GrFPS._AutoDegradeActive = enable;
						BTConsole.WriteLine("AutoDegrade is now " + enableString);
					}
				}
			}

			public class BTFrameRateDegradeInput : BTConsoleCommand.BTCommandInput
			{
				public object enable;

				private void SetEnable(object enable, bool isPresent) {
					this.enable = isPresent ? enable : null;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"enable",
						true,
						"enable/disable autoDegrade - otherwise toggles",
						this.SetEnable,
						typeof(bool)
					)
				};
				}
			}
		}

		public class BTMemProfiler
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "MemProfiler" },
					new BTNoArgsInput(),
					"Opens the MemProfiler",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				if(UnityEngine.Object.FindObjectOfType(typeof(BTMemoryProfilerContainer)) == null) {
					new GameObject("Memory Profiler", new Type[] { typeof(BTMemoryProfilerContainer) });
					BTConsole.WriteLine("Attached MemProfiler.");
				} else {
					BTConsole.WriteLine("MemProfiler is already attached.");
				}
			}
		}

		public class BTGPUStats
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "GPU", "stats" },
					new BTNoArgsInput(),
					"Prints GPU stats to console",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTConsole.WriteLine("GPU name - " + SystemInfo.graphicsDeviceName);
				BTConsole.WriteLine("  Memory - " + SystemInfo.graphicsMemorySize);
			}
		}

		public class BTQualityCommand
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "Quality" },
					new BTQualityCommandInput(),
					"modifies the (graphics) quality settings",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTQualityCommandInput cmdInput = (BTQualityCommandInput)input;
				if(cmdInput.increase) {
					QualitySettings.IncreaseLevel(cmdInput.applyExpensiveChanges);
				} else {
					QualitySettings.DecreaseLevel(cmdInput.applyExpensiveChanges);
				}
				BTConsole.WriteLine("Quality setting set to: " + QualitySettings.names[QualitySettings.GetQualityLevel()]);
				BTConsole.WriteLine("Applied expensive changes: " + cmdInput.applyExpensiveChanges.ToString());
			}

			public class BTQualityCommandInput : BTConsoleCommand.BTCommandInput
			{
				public bool increase;
				public bool applyExpensiveChanges;

				private void SetIncrease(object increase, bool isPresent) {
					this.increase = (bool)increase;
				}

				private void SetApplyExpensiveChanges(object applyExpensiveChanges, bool isPresent) {
					this.applyExpensiveChanges = (bool)applyExpensiveChanges;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"increase",
						false,
						"true = increase quality | false = decrease quality",
						this.SetIncrease,
						typeof(bool)
					),
					new BTConsoleCommand.BTConsoleArgument(
						"applyExpensiveChanges",
						true,
						"whether to apply changes that might take longer to compute or not",
						this.SetApplyExpensiveChanges,
						typeof(bool)
					)
				};
				}
			}
		}

		public class BTLevelLoad
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "Level", "load" },
					new BTLevelLoadInput(),
					"load a specified level",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTLevelLoadInput cmdInput = (BTLevelLoadInput)input;
				BTConsole.WriteLine("Loading level: " + cmdInput.level);
				RsResourceManager.LoadLevel(cmdInput.level);
			}

			public class BTLevelLoadInput : BTConsoleCommand.BTCommandInput
			{
				public string level;

				private void SetLevel(object level, bool isPresent) {
					this.level = (string)level;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"level",
						false,
						"name of the level to load",
						this.SetLevel,
						typeof(string)
					)
				};
				}
			}
		}

		public class BTLevelGet
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "Level", "get" },
					new BTNoArgsInput(),
					"prints the current level name (name of the active scene)",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTConsole.WriteLine("Current Level: " + RsResourceManager.pCurrentLevel);
			}
		}

		public class BTCogsLoad
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "Cogs", "load" },
					new BTNoArgsInput(),
					"loads into the 'cogs' minigame",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTConsole.WriteLine("Loading Cogs Minigame...");
				RsResourceManager.LoadAssetFromBundle(GameConfig.GetKeyData("CogsAsset"), new RsResourceEventHandler(OnCogsBundleReady), typeof(GameObject), false, null);
				KAUICursorManager.SetDefaultCursor("Loading", true);
			}

			private static void OnCogsBundleReady(string inURL, RsResourceLoadEvent inEvent, float inProgress, object inObject, object inUserData) {
				if(inEvent == RsResourceLoadEvent.COMPLETE) {
					if(inObject == null) {
						BTConsole.WriteLine("Cogs Asset not found in the bundle!");
					} else {
						BTConsole.WriteLine("Asset found, instantiating...");
						UnityEngine.Object.Instantiate<GameObject>((GameObject)inObject);
					}
					KAUICursorManager.SetDefaultCursor("Arrow", true);
				} else if(inEvent == RsResourceLoadEvent.ERROR) {
					BTConsole.WriteLine("Failed to download the cogs asset bundle!");
					KAUICursorManager.SetDefaultCursor("Arrow", true);
				}
			}
		}

		public class BTCogsUnlock
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "Cogs", "unlock" },
					new BTNoArgsInput(),
					"unlocks all levels in the 'cogs' minigame",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTConsole.WriteLine("Unlocking Cogs Minigame Levels");
				CogsLevelManager.UnlockAllLevels();
			}
		}

		public class BTIncredibleMachineLoad
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "IncredibleMachine", "load" },
					new BTNoArgsInput(),
					"loads into the 'IncredibleMachine' minigame",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTConsole.WriteLine("Loading IncredibleMachine Minigame...");
				RsResourceManager.LoadAssetFromBundle(GameConfig.GetKeyData("IncredibleMachineAsset"), new RsResourceEventHandler(OnIncredibleMachineBundleReady), typeof(GameObject), false, null);
				KAUICursorManager.SetDefaultCursor("Loading", true);
			}

			private static void OnIncredibleMachineBundleReady(string inURL, RsResourceLoadEvent inEvent, float inProgress, object inObject, object inUserData) {
				if(inEvent == RsResourceLoadEvent.COMPLETE) {
					if(inObject == null) {
						BTConsole.WriteLine("IncredibleMachine Asset not found in the bundle!");
					} else {
						BTConsole.WriteLine("Asset found, instantiating...");
						GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>((GameObject)inObject);
						gameObject.name = gameObject.name.Replace("(Clone)", "");
					}
					KAUICursorManager.SetDefaultCursor("Arrow", true);
				} else if(inEvent == RsResourceLoadEvent.ERROR) {
					BTConsole.WriteLine("Failed to download the IncredibleMachine asset bundle!");
					KAUICursorManager.SetDefaultCursor("Arrow", true);
				}
			}
		}

		public class BTIncredibleMachineUnlock
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "IncredibleMachine", "unlock" },
					new BTNoArgsInput(),
					"unlocks all levels in the 'IncredibleMachine' minigame",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTConsole.WriteLine("Unlocking IncredibleMachine Minigame Levels");
				CTLevelManager.UnlockAllLevels();
			}
		}

		public class BTConsumableList
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "Consumable", "list" },
					new BTNoArgsInput(),
					"prints all available consumables",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				if(!ConsumableData.pIsReady) {
					BTConsole.WriteLine("Can't print - ConsumableData is not ready");
					return;
				}
				GameObject gameObject = GameObject.Find("PfUiConsumable");
				if(gameObject == null) {
					BTConsole.WriteLine("Can't print - PfUiConsumable not found");
					return;
				}
				UiConsumable component = gameObject.GetComponent<UiConsumable>();
				ConsumableType consumableType = ConsumableData.GetConsumableTypeByGame(component.pGameName, "Game");
				if(consumableType == null) {
					BTConsole.WriteLine("Can't print - consumableType not found");
					return;
				}
				BTConsole.WriteLine("Consumables:");
				foreach(Consumable consumable in consumableType.Consumables) {
					BTConsole.WriteLine("\t" + consumable.name);
				}
			}
		}

		public class BTConsumableAdd
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "Consumable", "add" },
					new BTConsumableAddInput(),
					"adds a consumable to the game",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTConsumableAddInput cmdInput = (BTConsumableAddInput)input;
				if(!ConsumableData.pIsReady) {
					BTConsole.WriteLine("Can't add - ConsumableData is not ready");
					return;
				}
				GameObject gameObject = GameObject.Find("PfUiConsumable");
				if(gameObject == null) {
					BTConsole.WriteLine("Can't add - PfUiConsumable not found");
					return;
				}
				UiConsumable component = gameObject.GetComponent<UiConsumable>();
				ConsumableType consumableType = ConsumableData.GetConsumableTypeByGame(component.pGameName, "Game");
				if(consumableType == null) {
					BTConsole.WriteLine("Can't add - consumableType not found");
					return;
				}
				foreach(Consumable consumable in consumableType.Consumables) {
					if(String.Equals(consumable.name, cmdInput.consumable, StringComparison.OrdinalIgnoreCase)) {
						BTConsole.WriteLine("Added consumable: " + consumable.name);
						component.RegisterConsumable(consumable, true);
						return;
					}
				}
				BTConsole.WriteLine("Can't add - consumable of name '" + cmdInput.consumable + "' not found");
			}

			public class BTConsumableAddInput : BTConsoleCommand.BTCommandInput
			{
				public string consumable;

				private void SetConsumable(object consumable, bool isPresent) {
					this.consumable = (string)consumable;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"consumable",
						false,
						"name of the consumable to add",
						this.SetConsumable,
						typeof(string)
					)
				};
				}
			}
		}

		public class BTConsumableChart
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "Consumable", "chart" },
					new BTConsumableChartInput(),
					"shows/hides the probability charts of consumables",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTConsumableChartInput cmdInput = (BTConsumableChartInput)input;
				LevelManager levelManager = UnityEngine.Object.FindObjectOfType<LevelManager>();
				if(levelManager == null) {
					BTConsole.WriteLine("Can't show/hide chart - LevelManager not found");
					return;
				}
				string showString = cmdInput.show ? "shown" : "hidden";
				if(cmdInput.show == levelManager._ShowProbabilityChart) {
					BTConsole.WriteLine("Probability charts are already " + showString);
				} else {
					levelManager._ShowProbabilityChart = cmdInput.show;
					BTConsole.WriteLine("Probability charts are now " + showString);
				}
			}

			public class BTConsumableChartInput : BTConsoleCommand.BTCommandInput
			{
				public bool show;

				private void SetShow(object show, bool isPresent) {
					this.show = (bool)show;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"show",
						false,
						"show/hide probability charts",
						this.SetShow,
						typeof(bool)
					)
				};
				}
			}
		}

		public class BTDailyBonus
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "DailyBonus" },
					new BTDailyBonusInput(),
					"sets the dailyBonus acquired after the next restart",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			private static int targetDay = 0;

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTDailyBonusInput cmdInput = (BTDailyBonusInput)input;
				if(!ServerTime.pIsReady) {
					BTConsole.WriteLine("error - ServerTime did not start yet");
					return;
				}
				targetDay = Mathf.Max(1, cmdInput.day);
				BTConsole.WriteLine("Requesting pairdata 1216...");
				PairData.Load(1216, new PairDataEventHandler(OnPairDataReady), null, false, ParentData.pInstance.pUserInfo.UserID);
			}

			private static void OnPairDataReady(bool success, PairData inData, object inUserData) {
				if(targetDay >= 5) {
					inData.SetValue("LP", UtUtilities.GetPSTTimeFromUTC(ServerTime.pCurrentTime).AddDays(-2.0).Date.ToString(UtUtilities.GetCultureInfo("en-US")));
				} else {
					inData.SetValue("LP", UtUtilities.GetPSTTimeFromUTC(ServerTime.pCurrentTime).AddDays(-1.0).Date.ToString(UtUtilities.GetCultureInfo("en-US")));
				}
				inData.SetValue("LPC", targetDay.ToString());
				PairData.Save(1216, ParentData.pInstance.pUserInfo.UserID);
				BTConsole.WriteLine("Pairdata has been modified, reward '" + targetDay + "' will be awarded after the next restart");
			}

			public class BTDailyBonusInput : BTConsoleCommand.BTCommandInput
			{
				public int day;

				private void SetDay(object day, bool isPresent) {
					this.day = isPresent ? (int)day : 5;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"day",
						true,
						"target reward day, defaults to 5",
						this.SetDay,
						typeof(int)
					)
				};
				}
			}
		}

		public class BTDailyQuest
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "DailyQuest", "show" },
					new BTNoArgsInput(),
					"tries opening the DailyQuest UI",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				string inBundleURL = "RS_DATA/PfUiDailyQuestDO.unity3d/PfUiDailyQuestDO";
				KAUICursorManager.SetDefaultCursor("Loading", true);
				AvAvatar.pState = AvAvatarState.PAUSED;
				AvAvatar.SetUIActive(false);
				BTConsole.WriteLine("loading UI bundle...");
				RsResourceManager.LoadAssetFromBundle(inBundleURL, new RsResourceEventHandler(OnDailyQuestLoaded), typeof(GameObject), false, null);
			}

			private static void OnDailyQuestLoaded(string inURL, RsResourceLoadEvent inEvent, float inProgress, object inObject, object inUserData) {
				if(inEvent == RsResourceLoadEvent.COMPLETE) {
					BTConsole.WriteLine("Bundle loaded, UI should open now.");
					AvAvatar.pState = AvAvatarState.PAUSED;
					AvAvatar.SetUIActive(false);
					UiDailyQuests.pMissionGroup = MissionManager.pInstance._DailyMissions[0]._GroupID;
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>((GameObject)inObject);
					gameObject.name = "PfUiDailyQuestDO";
					KAUICursorManager.SetDefaultCursor("Arrow", true);
					gameObject.GetComponent<UiDailyQuests>().pOnUiClosed = new Action(OnUiClosed);
				} else if(inEvent == RsResourceLoadEvent.ERROR) {
					BTConsole.WriteLine("error - unknown error from the webserver, can't open ui");
					AvAvatar.pState = AvAvatarState.IDLE;
					AvAvatar.SetUIActive(true);
					KAUICursorManager.SetDefaultCursor("Arrow", true);
				}
			}

			private static void OnUiClosed() {
				AvAvatar.pState = AvAvatarState.IDLE;
				AvAvatar.SetUIActive(true);
			}
		}

		public class BTDebugUnload
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "Debug", "unload" },
					new BTNoArgsInput(),
					"unloads all unused assets",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTConsole.WriteLine("unloading unused assets...");
				Resources.UnloadUnusedAssets();
			}
		}

		public class BTDebugFix
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "Debug", "fix" },
					new BTNoArgsInput(),
					"resets avatar-state and UI - tries to clear softlocks",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTConsole.WriteLine("resetting avatar-state and ui");
				AvAvatar.pState = AvAvatarState.IDLE;
				AvAvatar.SetUIActive(true);
				if(KAUI._GlobalExclusiveUI != null) {
					KAUI.RemoveExclusive(KAUI._GlobalExclusiveUI);
					UnityEngine.Object.Destroy(KAUI._GlobalExclusiveUI.gameObject);
				}
			}
		}

		public class BTDebugInfo
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "Debug", "info" },
					new BTDebugInfoInput(),
					"shows/hides debug info ui",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTDebugInfoInput cmdInput = (BTDebugInfoInput)input;
				GameObject debugUI = GameObject.Find("PfUiDebugInfo");
				if(debugUI == null) {
					BTConsole.WriteLine("error - Can't find debug UI");
					return;
				}
				debugUI.SendMessage("SetVisibility", cmdInput.show);
				BTConsole.WriteLine("Debug UI is now " + (cmdInput.show ? "shown" : "hidden"));
			}

			public class BTDebugInfoInput : BTConsoleCommand.BTCommandInput
			{
				public bool show;

				private void SetShow(object show, bool isPresent) {
					this.show = (bool)show;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"show",
						false,
						"show/hide debug info ui",
						this.SetShow,
						typeof(bool)
					)
				};
				}
			}
		}

		public class BTDebugDeletePlayerPrefs
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "Debug", "delete", "player", "prefs" },
					new BTNoArgsInput(),
					"deletes ALL player prefs",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				PlayerPrefs.DeleteAll();
				BTConsole.WriteLine("Deleted all Player Prefs");
			}
		}

		public class BTDebugMemDump
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "Debug", "memDump" },
					new BTDebugMemDumpInput(),
					"Dumps the memory to a file",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTDebugMemDumpInput cmdInput = (BTDebugMemDumpInput)input;
				string outputFileName = MemDump.WriteToFile(cmdInput.fileName);
				BTConsole.WriteLine("Dumping Memory to file " + outputFileName);
			}

			public class BTDebugMemDumpInput : BTConsoleCommand.BTCommandInput
			{
				public string fileName;

				private void SetFileName(object fileName, bool isPresent) {
					this.fileName = isPresent ? "_" + (string)fileName : "";
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"fileName",
						true,
						"name of the file to dump to",
						this.SetFileName,
						typeof(string)
					)
				};
				}
			}
		}

		public class BTDebugMemWarn
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "Debug", "memWarn" },
					new BTNoArgsInput(),
					"triggers a low memory warning - forces memory cleanup, lowers rendered playercount...",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				MemoryManager.pInstance.gameObject.SendMessage("OnReceivedMemoryWarning", "dbgMsg");
				BTConsole.WriteLine("Triggered memory warning");
			}
		}

		public class BTDebugSceneObjects
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "Debug", "SceneObjects" },
					new BTNoArgsInput(),
					"toggles (delete/create) sceneObjects",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				GameObject sceneObjectsObject = GameObject.Find("SceneObjects");
				if(sceneObjectsObject != null) {
					UnityEngine.Object.Destroy(sceneObjectsObject);
					BTConsole.WriteLine("Destroyed SceneObjects");
				} else {
					new GameObject("SceneObjects").AddComponent<SceneObjects>();
					BTConsole.WriteLine("Created SceneObjects");
				}
			}
		}

		public class BTDebugParticles
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "Debug", "particles" },
					new BTDebugParticlesInput(),
					"enables/disables all particleSystems",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTDebugParticlesInput cmdInput = (BTDebugParticlesInput)input;
				foreach(ParticleSystem particleSystem in Resources.FindObjectsOfTypeAll(typeof(ParticleSystem)) as ParticleSystem[]) {
					if(cmdInput.enable) {
						particleSystem.Play();
					} else {
						particleSystem.Stop();
					}
				}
			}

			public class BTDebugParticlesInput : BTConsoleCommand.BTCommandInput
			{
				public bool enable;

				private void SetEnable(object enable, bool isPresent) {
					this.enable = (bool)enable;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"enable",
						false,
						"enable/disable all particle systems",
						this.SetEnable,
						typeof(bool)
					)
				};
				}
			}
		}

		public class BTFieldGuideUnlock
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "FieldGuide", "unlock" },
					new BTFieldGuideUnlockInput(),
					"unlocks/relocks the field guide",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTFieldGuideUnlockInput cmdInput = (BTFieldGuideUnlockInput)input;
				string unlockString = cmdInput.unlock ? "unlocked" : "locked";
				if(FieldGuideData.pUnlocked == cmdInput.unlock) {
					BTConsole.WriteLine("FieldGuide is already " + unlockString);
				} else {
					FieldGuideData.pUnlocked = cmdInput.unlock;
					BTConsole.WriteLine("FieldGuide is now " + unlockString);
				}
			}

			public class BTFieldGuideUnlockInput : BTConsoleCommand.BTCommandInput
			{
				public bool unlock;

				private void SetUnlock(object unlock, bool isPresent) {
					this.unlock = (bool)unlock;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"unlock",
						false,
						"unlock/relock field guide",
						this.SetUnlock,
						typeof(bool)
					)
				};
				}
			}
		}

		public class BTFishWeigth
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "Fish", "weight" },
					new BTFishWeigthInput(),
					"sets the fish weight for the next fish",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTFishWeigthInput cmdInput = (BTFishWeigthInput)input;
				FishingZone._CheatFishWeight = cmdInput.weight;
				BTConsole.WriteLine("Next fish will have weight: " + cmdInput.weight);
			}

			public class BTFishWeigthInput : BTConsoleCommand.BTCommandInput
			{
				public float weight;

				private void SetWeigth(object weigth, bool isPresent) {
					this.weight = (float)weigth;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"weight",
						false,
						"weight of the fish, must be > 0",
						this.SetWeigth,
						typeof(float)
					)
				};
				}
			}
		}

		public class BTFishRank
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "Fish", "rank" },
					new BTFishRankInput(),
					"sets the fish rank for the next fish",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTFishRankInput cmdInput = (BTFishRankInput)input;
				FishingZone._CheatFishRank = cmdInput.rank;
				BTConsole.WriteLine("Next fish will have rank: " + cmdInput.rank);
			}

			public class BTFishRankInput : BTConsoleCommand.BTCommandInput
			{
				public int rank;

				private void SetRank(object rank, bool isPresent) {
					this.rank = (int)rank;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"rank",
						false,
						"rank of the fish",
						this.SetRank,
						typeof(int)
					)
				};
				}
			}
		}

		public class BTFishRodPower
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "Fish", "rodpower" },
					new BTFishRodPowerInput(),
					"sets the rodPower for the next fishing attempt",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTFishRodPowerInput cmdInput = (BTFishRodPowerInput)input;
				FishingZone._CheatRodPower = cmdInput.rodPower;
				BTConsole.WriteLine("Next fishing attempt will have rodPower: " + cmdInput.rodPower);
			}

			public class BTFishRodPowerInput : BTConsoleCommand.BTCommandInput
			{
				public float rodPower;

				private void SetRodPower(object rodPower, bool isPresent) {
					this.rodPower = (float)rodPower;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"rodPower",
						false,
						"rodPower for the next fishing attempt",
						this.SetRodPower,
						typeof(float)
					)
				};
				}
			}
		}

		public class BTFishPoleID
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "Fish", "poleID" },
					new BTFishPoleIDInput(),
					"sets the fishing pole id for the next fishing attempt",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTFishPoleIDInput cmdInput = (BTFishPoleIDInput)input;
				FishingZone._CheatPoleID = cmdInput.poleID;
				BTConsole.WriteLine("Next fishing attempt will have poleID: " + cmdInput.poleID);
			}

			public class BTFishPoleIDInput : BTConsoleCommand.BTCommandInput
			{
				public int poleID;

				private void SetPoleID(object poleID, bool isPresent) {
					this.poleID = (int)poleID;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"poleID",
						false,
						"fishing pole id for the next fishing attempt",
						this.SetPoleID,
						typeof(int)
					)
				};
				}
			}
		}

		public class BTGlowUI
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "GlowUI" },
					new BTNoArgsInput(),
					"opens the GlowUI (clientSide glow effects)",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				GlowConsole glowConsole = UnityEngine.Object.FindObjectOfType<GlowConsole>();
				GameObject glowUI = glowConsole._GlowSettingsUi;
				UnityEngine.Object.Instantiate<GameObject>(glowUI, Vector3.zero, Quaternion.identity);
				BTConsole.WriteLine("GlowUI opened.");
			}
		}

		public class BTInventoryCommands
		{
			public static void Register() {
				BTConsole.AddCommand(new BTConsoleCommand(
					new List<string>() { "Inventory", "clear" },
					new BTNoArgsInput(),
					"clears the avatar's inventory",
					OnExecuteInventoryClear
				));
				BTConsole.AddCommand(new BTConsoleCommand(
					new List<string>() { "Inventory", "save" },
					new BTNoArgsInput(),
					"saves the avatar's inventory",
					OnExecuteInventorySave
				));
				BTConsole.AddCommand(new BTConsoleCommand(
					new List<string>() { "Inventory", "remove" },
					new BTInventoryRemoveInput(),
					"removes an Item from the Inventory",
					OnExecuteInventoryRemove
				));
				BTConsole.AddCommand(new BTConsoleCommand(
					new List<string>() { "Inventory", "add" },
					new BTInventoryAddInput(),
					"adds an Item to the Inventory",
					OnExecuteInventoryAdd
				));
				BTConsole.AddCommand(new BTConsoleCommand(
					new List<string>() { "Inventory", "add", "range" },
					new BTInventoryAddRangeInput(),
					"adds one item of each itemID in the specified range to the inventory",
					OnExecuteInventoryAddRange
				));
				BTConsole.AddCommand(new BTConsoleCommand(
					new List<string>() { "Inventory", "add", "battle", "item" },
					new BTInventoryAddBattleItemInput(),
					"adds 'BattleItems' to the inventory",
					OnExecuteInventoryAddBattleItem
				));
				BTConsole.AddCommand(new BTConsoleCommand(
					new List<string>() { "Inventory", "ItemID" },
					new BTInventoryShowIDInput(),
					"shows / hides ItemIDs in the Inventory",
					OnExecuteInventoryItemID
				));
				BTConsole.AddCommand(new BTConsoleCommand(
					new List<string>() { "Inventory", "dumpItemData" },
					new BTInventoryDumpItemDataInput(),
					"tries to load the specified item IDs and dumps their data to the logFile (as errors)",
					OnExecuteInventoryDumpItemData
				));
			}

			public static void OnExecuteInventoryClear(BTConsoleCommand.BTCommandInput input) {
				CommonInventoryData.pInstance.Clear();
				BTConsole.WriteLine("Inventory cleared.");
			}

			public static void OnExecuteInventorySave(BTConsoleCommand.BTCommandInput input) {
				CommonInventoryData.pInstance.Save(
					new InventorySaveEventHandler(InventorySaveCallback),
					null,
					null);
			}

			public static void OnExecuteInventoryRemove(BTConsoleCommand.BTCommandInput input) {
				BTInventoryRemoveInput cmdInput = (BTInventoryRemoveInput)input;
				CommonInventoryData.pInstance.RemoveItem(
					cmdInput.itemID,
					cmdInput.updateServer,
					cmdInput.quantity,
					null);
				BTConsole.WriteLine("Removed ItemID: " + cmdInput.itemID + " | Quantity: " + cmdInput.quantity);
			}

			public static void OnExecuteInventoryAdd(BTConsoleCommand.BTCommandInput input) {
				BTInventoryAddInput cmdInput = (BTInventoryAddInput)input;
				CommonInventoryData.pInstance.AddItem(
					cmdInput.itemID,
					cmdInput.updateServer,
					new InventoryItemDataEventHandler(InventoryItemDataCallback),
					null,
					cmdInput.quantity);
			}

			public static void OnExecuteInventoryAddRange(BTConsoleCommand.BTCommandInput input) {
				BTInventoryAddRangeInput cmdInput = (BTInventoryAddRangeInput)input;
				bool isReverse = (cmdInput.idStart > cmdInput.idEnd);
				int start = isReverse ? cmdInput.idEnd : cmdInput.idStart;
				int end = isReverse ? cmdInput.idStart : cmdInput.idEnd;
				for(int itemID = start; itemID <= end; itemID++) {
					CommonInventoryData.pInstance.AddItem(
						itemID,
						cmdInput.updateServer,
						new InventoryItemDataEventHandler(InventoryItemDataCallback),
						null);
				}
			}

			public static void OnExecuteInventoryAddBattleItem(BTConsoleCommand.BTCommandInput input) {
				BTInventoryAddBattleItemInput cmdInput = (BTInventoryAddBattleItemInput)input;
				AddBattleItemsRequest request = new AddBattleItemsRequest();
				List<BattleItemTierMap> list = new List<BattleItemTierMap>();
				BattleItemTierMap battleItemTierMap = new BattleItemTierMap {
					ItemID = cmdInput.itemID,
					Quantity = new int?(cmdInput.quantity)
				};
				if(cmdInput.itemTier != 0) {
					battleItemTierMap.Tier = new ItemTier?((ItemTier)cmdInput.itemTier);
				}
				list.Add(battleItemTierMap);
				request.BattleItemTierMaps = list;
				WsWebService.BattleReadyItems(
					request,
					new WsServiceEventHandler(AddBattleItemCallback),
					null);
			}

			public static void OnExecuteInventoryItemID(BTConsoleCommand.BTCommandInput input) {
				BTInventoryShowIDInput cmdInput = (BTInventoryShowIDInput)input;
				if(cmdInput.show == null) {
					CommonInventoryData.pShowItemID = !CommonInventoryData.pShowItemID;
					string showString = CommonInventoryData.pShowItemID ? "shown" : "hidden";
					BTConsole.WriteLine("ItemIDs are now " + showString);
				} else {
					bool show = (bool)cmdInput.show;
					string showString = show ? "shown" : "hidden";
					if(CommonInventoryData.pShowItemID == show) {
						BTConsole.WriteLine("ItemIDs are already " + showString);
					} else {
						CommonInventoryData.pShowItemID = show;
						BTConsole.WriteLine("ItemIDs are now " + showString);
					}
				}
			}

			public static void OnExecuteInventoryDumpItemData(BTConsoleCommand.BTCommandInput input) {
				BTInventoryDumpItemDataInput cmdInput = (BTInventoryDumpItemDataInput)input;

				bool isReverse = (cmdInput.idStart > cmdInput.idEnd);
				int start = isReverse ? cmdInput.idEnd : cmdInput.idStart;
				int end = isReverse ? cmdInput.idStart : cmdInput.idEnd;

				ItemDataEventHandler callback = new ItemDataEventHandler(InventoryDumpItemDataCallback);
				BTInventoryDumpItemDataProgress progress = new BTInventoryDumpItemDataProgress(callback, start, end, cmdInput.batchSize);
				progress.RequestNextBatch();
			}

			public static void InventoryDumpItemDataCallback(int itemID, ItemData itemData, object inUserData) {
				if(inUserData == null) {
					BTConsole.WriteLine("  ERROR - callback for ItemDump went missing, cannot dump!");
					return;
				}
				BTInventoryDumpItemDataProgress progress = (BTInventoryDumpItemDataProgress)inUserData;
				if(itemData == null || itemData.ItemID != itemID) {
					BTConsole.WriteLine("  WARNING - itemID: " + itemID + " yielded no ItemData");
					progress.AddLoadedItem(itemID, null);
				} else {
					progress.AddLoadedItem(itemID, itemData);
				}
			}

			public class BTInventoryDumpItemDataProgress
			{
				public Dictionary<int, ItemData> loadedItemData;
				public ItemDataEventHandler callback;
				public int startID;
				public int endID;
				public int batchSize;

				private int batchProgress = 0;

				public BTInventoryDumpItemDataProgress(ItemDataEventHandler callback, int startID, int endID, int batchSize) {
					loadedItemData = new Dictionary<int, ItemData>();
					this.callback = callback;
					this.startID = startID;
					this.endID = endID;
					this.batchSize = batchSize;
				}

				public void RequestNextBatch() {
					int loadedItemDataCount = loadedItemData.Count;
					int requestsRemainingCount = (endID - startID + 1) - loadedItemDataCount;
					int batchStartID = startID + loadedItemDataCount;
					int batchEndID;
					if(requestsRemainingCount < batchSize) {
						batchEndID = endID;
					} else {
						batchEndID = batchStartID + batchSize - 1;
					}
					BTConsole.WriteLine(" requesting IDs " + batchStartID + " - " + batchEndID);
					this.batchProgress = 0;
					for(int i = batchStartID; i <= batchEndID; i++) {
						ItemData.Load(i, callback, this);
					}
				}

				public void AddLoadedItem(int itemID, ItemData itemData) {
					loadedItemData[itemID] = itemData;
					batchProgress++;
					if(AllItemsLoaded()) {
						OnAllItemsLoaded();
					} else if(batchProgress >= batchSize) {
						RequestNextBatch();
					}
				}

				public bool AllItemsLoaded() {
					return loadedItemData.Count >= (endID - startID + 1);
				}

				public void OnAllItemsLoaded() {
					BTConsole.WriteLine("All requested items loaded!");
					StringBuilder resultBuilder = new StringBuilder();
					resultBuilder.Append("ItemID")
						.Append("\t").Append("ItemName")
						.Append("\t").Append("AssetName")
						.Append("\t").Append("IconName")
						.Append("\t").Append("Rollover.DialogName")
						.Append("\t").Append("Rollover.Bundle")
						.Append("\t").Append("Description")
						.Append("\t").Append("Geometry2")
						.Append("\t").Append("TextureCount")
						.Append("\t").Append("Texture.TextureName")
						.Append("\t").Append("Texture.TextureTypeName")
						.Append("\t").Append("Texture.OffsetX")
						.Append("\t").Append("Texture.OffsetY")
						.Append("\t").Append("CategoryCount")
						.Append("\t").Append("Category.CategoryID")
						.Append("\t").Append("Category.CategoryName")
						.Append("\t").Append("Category.IconName")
						.Append("\t").Append("RelationshipCount")
						.Append("\t").Append("Relationship.Type")
						.Append("\t").Append("Relationship.ItemID")
						.Append("\t").Append("Relationship.Weight")
						.Append("\t").Append("Relationship.Quantity")
						.Append("\t").Append("RankID")
						.Append("\t").Append("Locked")
						.Append("\t").Append("Cost")
						.Append("\t").Append("Uses")
						.Append("\t").Append("InventoryMax")
						.Append("\t").Append("CreativePoints");
					for(int i = startID; i <= endID; i++) {
						if(!loadedItemData.ContainsKey(i)) {
							BTConsole.WriteLine("ERROR - itemData for ID: " + i + " was never loaded!");
							continue;
						}
						ItemData itemData = loadedItemData[i];
						if(itemData == null) {
							continue;
						}
						resultBuilder.Append("\n");
						resultBuilder.Append(itemData.ItemID);
						resultBuilder.Append("\t").Append(ReplaceNewline(itemData.ItemName));
						resultBuilder.Append("\t").Append(ReplaceNewline(itemData.AssetName));
						resultBuilder.Append("\t").Append(ReplaceNewline(itemData.IconName));
						ItemDataRollover rollover = itemData.Rollover;
						if(rollover == null) {
							resultBuilder.Append("\t\t");
						} else {
							resultBuilder.Append("\t").Append(ReplaceNewline(rollover.DialogName));
							resultBuilder.Append("\t").Append(ReplaceNewline(rollover.Bundle));
						}
						resultBuilder.Append("\t").Append(ReplaceNewline(itemData.Description));
						resultBuilder.Append("\t").Append(ReplaceNewline(itemData.Geometry2));
						if(itemData.Texture == null) {
							resultBuilder.Append("\t").Append("0");
							resultBuilder.Append("\t\t\t\t");
						} else {
							resultBuilder.Append("\t").Append(itemData.Texture.Length);
							string textureNames = "";
							string textureTypeNames = "";
							string offsetX = "";
							string offsetY = "";
							for(int index = 0; index < itemData.Texture.Length; index++) {
								ItemDataTexture texture = itemData.Texture[index];
								if(index == 0) {
									textureNames += texture.TextureName;
									textureTypeNames += texture.TextureTypeName;
									offsetX += (texture.OffsetX == null) ? "null" : texture.OffsetX.Value.ToString();
									offsetY += (texture.OffsetY == null) ? "null" : texture.OffsetY.Value.ToString();
								} else {
									textureNames += (", " + texture.TextureName);
									textureTypeNames += (", " + texture.TextureTypeName);
									offsetX += (", " + ((texture.OffsetX == null) ? "null" : texture.OffsetX.Value.ToString()));
									offsetY += (", " + ((texture.OffsetY == null) ? "null" : texture.OffsetY.Value.ToString()));
								}
							}
							resultBuilder.Append("\t").Append(ReplaceNewline(textureNames));
							resultBuilder.Append("\t").Append(ReplaceNewline(textureTypeNames));
							resultBuilder.Append("\t").Append(ReplaceNewline(offsetX));
							resultBuilder.Append("\t").Append(ReplaceNewline(offsetY));
						}
						if(itemData.Category == null) {
							resultBuilder.Append("\t").Append("0");
							resultBuilder.Append("\t\t\t");
						} else {
							resultBuilder.Append("\t").Append(itemData.Category.Length);
							string categoryIDs = "";
							string categoryNames = "";
							string iconNames = "";
							for(int index = 0; index < itemData.Category.Length; index++) {
								ItemDataCategory category = itemData.Category[index];
								if(index == 0) {
									categoryIDs += category.CategoryId;
									categoryNames += category.CategoryName;
									iconNames += category.IconName;
								} else {
									categoryIDs += (", " + category.CategoryId);
									categoryNames += (", " + category.CategoryName);
									iconNames += (", " + category.IconName);
								}
							}
							resultBuilder.Append("\t").Append(ReplaceNewline(categoryIDs));
							resultBuilder.Append("\t").Append(ReplaceNewline(categoryNames));
							resultBuilder.Append("\t").Append(ReplaceNewline(iconNames));
						}
						if(itemData.Relationship == null) {
							resultBuilder.Append("\t").Append("0");
							resultBuilder.Append("\t\t");
						} else {
							resultBuilder.Append("\t").Append(itemData.Relationship.Length);
							string relationshipTypes = "";
							string relationshipItemIDs = "";
							string relationshipWeigths = "";
							string relationshipQuantities = "";
							for(int index = 0; index < itemData.Relationship.Length; index++) {
								ItemDataRelationship relationship = itemData.Relationship[index];
								if(index == 0) {
									relationshipTypes += relationship.Type;
									relationshipItemIDs += relationship.ItemId;
									relationshipWeigths += relationship.Weight;
									relationshipQuantities += relationship.Quantity;
								} else {
									relationshipTypes += (", " + relationship.Type);
									relationshipItemIDs += (", " + relationship.ItemId);
									relationshipWeigths += (", " + relationship.Weight);
									relationshipQuantities += (", " + relationship.Quantity);
								}
							}
							resultBuilder.Append("\t").Append(ReplaceNewline(relationshipTypes));
							resultBuilder.Append("\t").Append(ReplaceNewline(relationshipItemIDs));
							resultBuilder.Append("\t").Append(ReplaceNewline(relationshipWeigths));
							resultBuilder.Append("\t").Append(ReplaceNewline(relationshipQuantities));
						}
						resultBuilder.Append("\t").Append(itemData.RankId);
						resultBuilder.Append("\t").Append(itemData.Locked);
						resultBuilder.Append("\t").Append(itemData.Cost);
						resultBuilder.Append("\t").Append(itemData.Uses);
						resultBuilder.Append("\t").Append(itemData.InventoryMax);
						resultBuilder.Append("\t").Append(itemData.CreativePoints);
					}
					BTConsole.WriteLine(resultBuilder.ToString());
					Debug.LogError(resultBuilder.ToString());
				}
			}

			private static string ReplaceNewline(string input) {
				if(input == null) {
					return "";
				}
				return input.Replace("\n", "\\n");
			}

			public static void InventorySaveCallback(bool success, object inUserData) {
				if(success) {
					BTConsole.WriteLine("CommonInventory Save successful");
					return;
				}
				BTConsole.WriteLine("CommonInventory Save failed");
			}

			public static void InventoryItemDataCallback(UserItemData dataItem, object inUserData) {
				if(dataItem != null) {
					BTConsole.WriteLine(string.Concat(new object[]
						{
					"Item [",
					dataItem.Item.ItemName,
					"] added quantity = ",
					dataItem.Quantity
						}));
				} else {
					BTConsole.WriteLine("Inventory returned no itemData, request failed?");
				}
			}

			public static void AddBattleItemCallback(WsServiceType inType, WsServiceEvent inEvent, float inProgress, object inObject, object inUserData) {
				KAUICursorManager.SetDefaultCursor("Arrow", true);
				if(inEvent == WsServiceEvent.ERROR) {
					BTConsole.WriteLine("error - AddBattleItem request failed!");
					return;
				}
				if(inEvent == WsServiceEvent.COMPLETE) {
					if(inObject == null) {
						BTConsole.WriteLine("error - AddBattleItem returned no data");
						return;
					}
					AddBattleItemsResponse response = (AddBattleItemsResponse)inObject;
					if(response.InventoryItemStatsMaps.Count <= 0) {
						BTConsole.WriteLine("error - AddBattleItem returned no items");
						return;
					}
					foreach(InventoryItemStatsMap statsMap in response.InventoryItemStatsMaps) {
						UserItemData userItemData = new UserItemData {
							UserInventoryID = statsMap.CommonInventoryID,
							Item = statsMap.Item
						};
						userItemData.Item.ItemStatsMap = statsMap.ItemStatsMap;
						userItemData.ItemStats = statsMap.ItemStatsMap.ItemStats;
						userItemData.ItemTier = new ItemTier?(statsMap.ItemStatsMap.ItemTier);
						userItemData.ItemID = statsMap.ItemStatsMap.ItemID;
						userItemData.Quantity = 1;
						CommonInventoryData.pInstance.AddToCategories(userItemData);
						BTConsole.WriteLine("AddBattleItem - added Item, ID: " + userItemData.ItemID);
					}
				}
			}

			public class BTInventoryRemoveInput : BTConsoleCommand.BTCommandInput
			{
				public int itemID;
				public int quantity;
				public bool updateServer;

				private void SetItemID(object itemID, bool isPresent) {
					this.itemID = (int)itemID;
				}

				private void SetQuantity(object quantity, bool isPresent) {
					this.quantity = isPresent ? (int)quantity : 1;
				}

				private void SetUpdateServer(object updateServer, bool isPresent) {
					this.updateServer = (bool)updateServer;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"itemID",
						false,
						"ID of the item to remove",
						this.SetItemID,
						typeof(int)
					),
					new BTConsoleCommand.BTConsoleArgument(
						"quantity",
						true,
						"amount of items to remove, defaults to 1",
						this.SetQuantity,
						typeof(int)
					),
					new BTConsoleCommand.BTConsoleArgument(
						"updateServer",
						true,
						"whether to sync the item(s) with the server or not",
						this.SetUpdateServer,
						typeof(bool)
					)
				};
				}
			}

			public class BTInventoryAddInput : BTConsoleCommand.BTCommandInput
			{
				public int itemID;
				public int quantity;
				public bool updateServer;

				private void SetItemID(object itemID, bool isPresent) {
					this.itemID = (int)itemID;
				}

				private void SetQuantity(object quantity, bool isPresent) {
					this.quantity = isPresent ? (int)quantity : 1;
				}

				private void SetUpdateServer(object updateServer, bool isPresent) {
					this.updateServer = (bool)updateServer;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"itemID",
						false,
						"ID of the item to add",
						this.SetItemID,
						typeof(int)
					),
					new BTConsoleCommand.BTConsoleArgument(
						"quantity",
						true,
						"amount of items to add, defaults to 1",
						this.SetQuantity,
						typeof(int)
					),
					new BTConsoleCommand.BTConsoleArgument(
						"updateServer",
						true,
						"whether to sync the item(s) with the server or not",
						this.SetUpdateServer,
						typeof(bool)
					)
				};
				}
			}

			public class BTInventoryAddRangeInput : BTConsoleCommand.BTCommandInput
			{
				public int idStart;
				public int idEnd;
				public bool updateServer;

				private void SetIDStart(object idStart, bool isPresent) {
					this.idStart = (int)idStart;
				}

				private void SetIDEnd(object idEnd, bool isPresent) {
					this.idEnd = isPresent ? (int)idEnd : 1;
				}

				private void SetUpdateServer(object updateServer, bool isPresent) {
					this.updateServer = (bool)updateServer;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"idStart",
						false,
						"first ID to add",
						this.SetIDStart,
						typeof(int)
					),
					new BTConsoleCommand.BTConsoleArgument(
						"idEnd",
						false,
						"last ID to add",
						this.SetIDEnd,
						typeof(int)
					),
					new BTConsoleCommand.BTConsoleArgument(
						"updateServer",
						true,
						"whether to sync the item(s) with the server or not",
						this.SetUpdateServer,
						typeof(bool)
					)
				};
				}
			}

			public class BTInventoryAddBattleItemInput : BTConsoleCommand.BTCommandInput
			{
				public int itemID;
				public int quantity;
				public int itemTier;

				private void SetItemID(object itemID, bool isPresent) {
					this.itemID = (int)itemID;
				}

				private void SetQuantity(object quantity, bool isPresent) {
					this.quantity = isPresent ? (int)quantity : 1;
				}

				private void SetItemTier(object itemTier, bool isPresent) {
					this.itemTier = (int)itemTier;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"itemID",
						false,
						"ID of the item to add",
						this.SetItemID,
						typeof(int)
					),
					new BTConsoleCommand.BTConsoleArgument(
						"quantity",
						true,
						"amount of items to add, defaults to 1",
						this.SetQuantity,
						typeof(int)
					),
					new BTConsoleCommand.BTConsoleArgument(
						"tier",
						true,
						"tier number of the item (1-4)",
						this.SetItemTier,
						typeof(int)
					)
				};
				}
			}

			public class BTInventoryShowIDInput : BTConsoleCommand.BTCommandInput
			{
				public object show;

				private void SetShow(object show, bool isPresent) {
					this.show = isPresent ? show : null;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"show",
						true,
						"show/hide the ItemIDs - otherwise toggles",
						this.SetShow,
						typeof(bool)
					)
				};
				}
			}

			public class BTInventoryDumpItemDataInput : BTConsoleCommand.BTCommandInput
			{
				public int idStart;
				public int idEnd;
				public int batchSize;

				private void SetIDStart(object idStart, bool isPresent) {
					this.idStart = (int)idStart;
				}

				private void SetIDEnd(object idEnd, bool isPresent) {
					this.idEnd = (int)idEnd;
				}

				private void SetBatchSize(object batchSize, bool isPresent) {
					this.batchSize = isPresent ? (int)batchSize : 100;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"idStart",
						false,
						"first ID to check",
						this.SetIDStart,
						typeof(int)
					),
					new BTConsoleCommand.BTConsoleArgument(
						"idEnd",
						false,
						"last ID to check",
						this.SetIDEnd,
						typeof(int)
					),
					new BTConsoleCommand.BTConsoleArgument(
						"batchSize",
						true,
						"amount of simultaneous server calls to send, defaults to 100",
						this.SetBatchSize,
						typeof(int)
					)
				};
				}
			}
		}

		public class BTLabExperimentGet
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "Lab", "Experiment", "Get" },
					new BTNoArgsInput(),
					"Gets the active lab experiment",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				int expID = ScientificExperiment.pActiveExperimentID;
				bool isNatural = !ScientificExperiment.pUseExperimentCheat;
				BTConsole.WriteLine("Active Experiment ID: " + expID + " | isNatural: " + isNatural);
			}
		}

		public class BTLabExperimentSet
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "Lab", "Experiment", "Set" },
					new BTLabExperimentSetInput(),
					"Sets the active lab experiment (for this session only)",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTLabExperimentSetInput cmdInput = (BTLabExperimentSetInput)input;
				int previousID = ScientificExperiment.pActiveExperimentID;
				ScientificExperiment.pActiveExperimentID = cmdInput.experimentID;
				ScientificExperiment.pUseExperimentCheat = !cmdInput.isNatural;
				BTConsole.WriteLine("Changed Experiment ID from: " + previousID + " | to: " + cmdInput.experimentID);
			}

			public class BTLabExperimentSetInput : BTConsoleCommand.BTCommandInput
			{
				public int experimentID;
				public bool isNatural;

				private void SetExperimentID(object experimentID, bool isPresent) {
					this.experimentID = (int)experimentID;
				}

				private void SetIsNatural(object isNatural, bool isPresent) {
					this.isNatural = (bool)isNatural;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"experimentID",
						false,
						"ID of the experiment",
						this.SetExperimentID,
						typeof(int)
					),
					new BTConsoleCommand.BTConsoleArgument(
						"isNatural",
						true,
						"set to true when resetting to a known natural ID",
						this.SetIsNatural,
						typeof(bool)
					)
				};
				}
			}
		}

		public class BTTaskCommands
		{
			public static void Register() {
				BTConsole.AddCommand(new BTConsoleCommand(
					new List<string>() { "Task", "Complete" },
					new CompleteInput(),
					"Completes an active Task",
					OnExecuteComplete
				));
				BTConsole.AddCommand(new BTConsoleCommand(
					new List<string>() { "Task", "Console" },
					new ConsoleShowInput(),
					"shows / hides the Task Console",
					OnExecuteConsoleShow
				));
			}

			public static void OnExecuteComplete(BTConsoleCommand.BTCommandInput input) {
				CompleteInput cmdInput = (CompleteInput)input;
				if(MissionManager.pInstance == null) {
					BTConsole.WriteLine("error - MissionManager not ready");
					return;
				}
				Task task = MissionManager.pInstance.pActiveTasks.Find((Task t) => t.TaskID == cmdInput.taskID);
				if(task == null) {
					BTConsole.WriteLine("error - No active Task found for ID: " + cmdInput.taskID);
					return;
				}
				task.Completed++;
				BTConsole.WriteLine("Task " + cmdInput.taskID + " completed.");
			}

			public class CompleteInput : BTConsoleCommand.BTCommandInput
			{
				public int taskID;

				private void SetTaskID(object taskID, bool isPresent) {
					this.taskID = (int)taskID;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"TaskID",
						false,
						"ID of the task",
						this.SetTaskID,
						typeof(int)
					)
				};
				}
			}

			public static void OnExecuteConsoleShow(BTConsoleCommand.BTCommandInput input) {
				ConsoleShowInput cmdInput = (ConsoleShowInput)input;
				TaskStatusConsole taskConsole = KAConsole.mObject.GetComponent<TaskStatusConsole>();
				if(taskConsole == null) {
					BTConsole.WriteLine("Unable to find Task Console.");
					return;
				}
				if(cmdInput.show) {
					taskConsole.Show();
					BTConsole.WriteLine("Task Console is shown.");
				} else {
					taskConsole.Close();
					BTConsole.WriteLine("Task Console is closed.");
				}
			}

			public class ConsoleShowInput : BTConsoleCommand.BTCommandInput
			{
				public bool show;

				private void SetShow(object show, bool isPresent) {
					this.show = (bool)show;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"show",
						false,
						"show/hide the Task Console",
						this.SetShow,
						typeof(bool)
					)
				};
				}
			}
		}

		public class BTMissionCommands
		{
			public static void Register() {
				BTConsole.AddCommand(new BTConsoleCommand(
					new List<string>() { "Mission", "Init" },
					new BTNoArgsInput(),
					"initializes the MissionManager",
					OnExecuteInit
				));
				BTConsole.AddCommand(new BTConsoleCommand(
					new List<string>() { "Mission", "Reset" },
					new BTNoArgsInput(),
					"resets the MissionManager",
					OnExecuteReset
				));
				BTConsole.AddCommand(new BTConsoleCommand(
					new List<string>() { "Mission", "Save" },
					new SaveInput(),
					"enables/disables mission saving",
					OnExecuteSave
				));
				BTConsole.AddCommand(new BTConsoleCommand(
					new List<string>() { "Mission", "Fail" },
					new FailInput(),
					"enables/disables automatic mission failing",
					OnExecuteFail
				));
				BTConsole.AddCommand(new BTConsoleCommand(
					new List<string>() { "Mission", "SyncDB" },
					new SyncDBInput(),
					"enables/disables database synchronisation",
					OnExecuteSyncDB
				));
				BTConsole.AddCommand(new BTConsoleCommand(
					new List<string>() { "Mission", "Unlock" },
					new UnlockInput(),
					"when enabled, missions ignore all locking-constraints (e.g. winter-missions become available during summer)",
					OnExecuteUnlock
				));
				BTConsole.AddCommand(new BTConsoleCommand(
					new List<string>() { "Mission", "Accept" },
					new AcceptInput(),
					"Accepts a Mission",
					OnExecuteAccept
				));
			}

			public static void OnExecuteInit(BTConsoleCommand.BTCommandInput input) {
				if(MissionManager.pInstance == null) {
					BTConsole.WriteLine("error - no Instance of MissionManager found.");
					return;
				}
				if(MissionManager.pIsReady) {
					BTConsole.WriteLine("MissionManager is already initialized.");
					return;
				}
				MissionManager.Init();
				BTConsole.WriteLine("MissionManager initialized.");
			}

			public static void OnExecuteReset(BTConsoleCommand.BTCommandInput input) {
				if(MissionManager.pInstance == null) {
					BTConsole.WriteLine("error - no Instance of MissionManager found.");
					return;
				}
				MissionManager.Reset();
				BTConsole.WriteLine("Missions reset.");
			}

			public static void OnExecuteSave(BTConsoleCommand.BTCommandInput input) {
				SaveInput cmdInput = (SaveInput)input;
				if(MissionManager.pInstance == null) {
					BTConsole.WriteLine("error - no Instance of MissionManager found.");
					return;
				}
				bool previousValue = Mission.pSave;
				bool newValue = cmdInput.save;
				if(previousValue == newValue) {
					BTConsole.WriteLine("MissionSaving already was: " + newValue);
				} else {
					Mission.pSave = newValue;
					BTConsole.WriteLine("Changed MissionSaving from: " + previousValue + " | to: " + newValue);
				}
			}

			public class SaveInput : BTConsoleCommand.BTCommandInput
			{
				public bool save;

				private void SetSave(object save, bool isPresent) {
					this.save = isPresent ? (bool)save : true;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>() {
					new BTConsoleCommand.BTConsoleArgument(
						"save",
						true,
						"enable mission saving or not - defaults to true",
						SetSave,
						typeof(bool)
					)
				};
				}
			}

			public static void OnExecuteFail(BTConsoleCommand.BTCommandInput input) {
				FailInput cmdInput = (FailInput)input;
				if(MissionManager.pInstance == null) {
					BTConsole.WriteLine("error - no Instance of MissionManager found.");
					return;
				}
				bool previousValue = Mission.pFail;
				bool newValue = cmdInput.fail;
				if(previousValue == newValue) {
					BTConsole.WriteLine("MissionFailing already was: " + newValue);
				} else {
					Mission.pFail = newValue;
					BTConsole.WriteLine("Changed MissionFailing from: " + previousValue + " | to: " + newValue);
				}
			}

			public class FailInput : BTConsoleCommand.BTCommandInput
			{
				public bool fail;

				private void SetFail(object fail, bool isPresent) {
					this.fail = isPresent ? (bool)fail : false;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>() {
					new BTConsoleCommand.BTConsoleArgument(
						"fail",
						true,
						"enable mission failing or not - defaults to false",
						SetFail,
						typeof(bool)
					)
				};
				}
			}

			public static void OnExecuteSyncDB(BTConsoleCommand.BTCommandInput input) {
				SyncDBInput cmdInput = (SyncDBInput)input;
				if(MissionManager.pInstance == null) {
					BTConsole.WriteLine("error - no Instance of MissionManager found.");
					return;
				}
				bool previousValue = Mission.pSyncDB;
				bool newValue = cmdInput.syncDB;
				if(previousValue == newValue) {
					BTConsole.WriteLine("SyncDB already was: " + newValue);
				} else {
					Mission.pSyncDB = newValue;
					BTConsole.WriteLine("Changed SyncDB from: " + previousValue + " | to: " + newValue);
				}
			}

			public class SyncDBInput : BTConsoleCommand.BTCommandInput
			{
				public bool syncDB;

				private void SetSyncDB(object syncDB, bool isPresent) {
					this.syncDB = isPresent ? (bool)syncDB : true;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>() {
					new BTConsoleCommand.BTConsoleArgument(
						"syncDB",
						true,
						"enable database synchronisation or not - defaults to true",
						SetSyncDB,
						typeof(bool)
					)
				};
				}
			}

			public static void OnExecuteUnlock(BTConsoleCommand.BTCommandInput input) {
				UnlockInput cmdInput = (UnlockInput)input;
				if(MissionManager.pInstance == null) {
					BTConsole.WriteLine("error - no Instance of MissionManager found.");
					return;
				}
				bool previousValue = !Mission.pLocked;
				bool newValue = cmdInput.unlock;
				if(previousValue == newValue) {
					BTConsole.WriteLine("Unlock already was: " + newValue);
				} else {
					Mission.pLocked = !newValue;
					BTConsole.WriteLine("Changed Unlock from: " + previousValue + " | to: " + newValue);
				}
			}

			public class UnlockInput : BTConsoleCommand.BTCommandInput
			{
				public bool unlock;

				private void SetUnlock(object unlock, bool isPresent) {
					this.unlock = isPresent ? (bool)unlock : false;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>() {
					new BTConsoleCommand.BTConsoleArgument(
						"unlock",
						true,
						"ignore mission constraints or not - defaults to false",
						SetUnlock,
						typeof(bool)
					)
				};
				}
			}

			public static void OnExecuteAccept(BTConsoleCommand.BTCommandInput input) {
				AcceptInput cmdInput = (AcceptInput)input;
				if(MissionManager.pInstance == null) {
					BTConsole.WriteLine("error - no Instance of MissionManager found.");
					return;
				}
				Mission mission = MissionManager.pInstance.GetMission(cmdInput.missionID);
				if(mission == null) {
					BTConsole.WriteLine("unable to accept mission - no mission of id '" + cmdInput.missionID + "' found.");
					return;
				}
				if(mission.pStaticDataReady) {
					BTConsole.WriteLine("Mission '" + cmdInput.missionID + "' found and ready - accepting Mission...");
					MissionManager.pInstance.AcceptMission(
						cmdInput.missionID,
						new AcceptMissionCallback(AcceptMissionCallback)
					);
				} else {
					BTConsole.WriteLine("Mission '" + cmdInput.missionID + "' found but not ready - loading MissionData...");
					MissionManager.pInstance.LoadMissionData(
						-1,
						new MissionStaticLoadCallback(OnLoadAcceptMissionData),
						cmdInput.missionID,
						false
					);
				}
			}

			public class AcceptInput : BTConsoleCommand.BTCommandInput
			{
				public int missionID;

				private void SetMissionID(object missionID, bool isPresent) {
					this.missionID = isPresent ? (int)missionID : -1;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>() {
					new BTConsoleCommand.BTConsoleArgument(
						"missionID",
						false,
						"missionID to accept",
						SetMissionID,
						typeof(int)
					)
				};
				}
			}

			public static void OnLoadAcceptMissionData(List<Mission> missions) {
				if(missions == null || missions.Count <= 0) {
					BTConsole.WriteLine("unable to load missionData - no missions found.");
					return;
				}
				Mission mission = missions[0];
				BTConsole.WriteLine("MissionData loaded for id '" + mission.MissionID + "' - accepting Mission...");
				MissionManager.pInstance.AcceptMission(
					mission.MissionID,
					new AcceptMissionCallback(AcceptMissionCallback)
				);
			}

			public static void AcceptMissionCallback(bool success, Mission mission) {
				if(mission == null) {
					BTConsole.WriteLine("error - received AcceptMission callback for `null` mission");
					return;
				}
				if(!success) {
					BTConsole.WriteLine("error - AcceptMissionCallback was unsuccessful for mission '" + mission.MissionID + "'");
					return;
				}
				Mission rootMission = MissionManager.pInstance.GetRootMission(mission);
				MarkPrerequisiteMissionsComplete(rootMission);
				if(rootMission.pData.Hidden) {
					BTConsole.WriteLine("warning - rootMissionData was hidden, unable to set active task");
					return;
				}
				List<Task> list = new List<Task>();
				MissionManager.pInstance.GetNextTask(mission, ref list);
				if(list.Count <= 0) {
					BTConsole.WriteLine("warning - MissionManager found no next task, unable to set active task");
					return;
				}
				MissionManagerDO.SetCurrentActiveTask(list[0].TaskID, true);
			}

			public static void MarkPrerequisiteMissionsComplete(Mission mission) {
				List<int> prerequisites = mission.MissionRule.GetPrerequisites<int>(PrerequisiteRequiredType.Mission);
				for(int i = 0; i < prerequisites.Count; i++) {
					Mission prerequisiteMission = MissionManager.pInstance.GetMission(prerequisites[i]);
					if(prerequisiteMission != null) {
						prerequisiteMission.Completed = 1;
						MarkPrerequisiteMissionsComplete(prerequisiteMission);
					}
				}
			}
		}

		public class BTMMOUsersShow
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "MMO", "Users" },
					new BTMMOUsersShowInput(),
					"shows / hides the MMO userList",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTMMOUsersShowInput cmdInput = (BTMMOUsersShowInput)input;
				if(cmdInput.show) {
					MMOUserList.Show();
					BTConsole.WriteLine("UserList is shown.");
				} else {
					MMOUserList.Hide();
					BTConsole.WriteLine("UserList is hidden.");
				}
			}

			public class BTMMOUsersShowInput : BTConsoleCommand.BTCommandInput
			{
				public bool show;

				private void SetShow(object show, bool isPresent) {
					this.show = (bool)show;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"show",
						false,
						"show/hide the MMO userList",
						this.SetShow,
						typeof(bool)
					)
				};
				}
			}
		}

		public class BTMMOInfo
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "MMO", "Info" },
					new BTNoArgsInput(),
					"prints info on the current MMO state",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				// Version data
				Version version = Assembly.GetAssembly(typeof(IMMOClient)).GetName().Version;
				BTConsole.WriteLine("Version - DLL version is " + version);

				if(MainStreetMMOClient.pInstance != null) {
					// Server Data
					BTConsole.WriteLine("Server - Connected to " + MainStreetMMOClient.pInstance.GetMMOServerIP());

					// State Data
					string text = "State - State is " + MainStreetMMOClient.pInstance.pState.ToString();
					if(MainStreetMMOClient.pInstance.pAllDeactivated) {
						text += ".  All Deactivated";
					}
					BTConsole.WriteLine(text);
					if(MainStreetMMOClient.pInstance.pState == MMOClientState.IN_ROOM) {
						BTConsole.WriteLine("State - Zone is " + MainStreetMMOClient.pInstance.pZone + ", Room is " + MainStreetMMOClient.pInstance.pRoomName);
					}
				} else {
					BTConsole.WriteLine("error - MMO instance is null or not ready");
				}

				// still State data
				if(GauntletMMOClient.pInstance != null) {
					BTConsole.WriteLine("State - Gauntlet MMO State is " + GauntletMMOClient.pInstance.pMMOState.ToString());
				}
			}
		}

		public class BTCoinsAdd
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "Coins", "add" },
					new BTCoinsAddInput(),
					"adds coins and (supposedly) syncs with the server",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTCoinsAddInput cmdInput = (BTCoinsAddInput)input;
				if(!Money.pIsReady) {
					BTConsole.WriteLine("error - Money object isn't ready.");
					return;
				}
				Money.AddMoney(cmdInput.amount, true);
				BTConsole.WriteLine("Added " + cmdInput.amount + " Coins.");
			}

			public class BTCoinsAddInput : BTConsoleCommand.BTCommandInput
			{
				public int amount;

				private void SetAmount(object amount, bool isPresent) {
					this.amount = (int)amount;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"amount",
						false,
						"amount of coins to add",
						this.SetAmount,
						typeof(int)
					)
				};
				}
			}
		}

		public class BTMysteryChestSpawnAll
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "Mystery", "Chest", "SpawnAll" },
					new BTMysteryChestSpawnAllInput(),
					"enables/disables spawning of all Mystery Chests",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTMysteryChestSpawnAllInput cmdInput = (BTMysteryChestSpawnAllInput)input;
				if(cmdInput.spawnAll == null) {
					MysteryChestManager._CheatSpawnAll = !MysteryChestManager._CheatSpawnAll;
					BTConsole.WriteLine("MysteryChest spawnall is now " + MysteryChestManager._CheatSpawnAll);
				} else {
					bool spawnAll = (bool)cmdInput.spawnAll;
					if(spawnAll == MysteryChestManager._CheatSpawnAll) {
						BTConsole.WriteLine("MysteryChest spawnall is already " + spawnAll);
					} else {
						MysteryChestManager._CheatSpawnAll = spawnAll;
						BTConsole.WriteLine("MysteryChest spawnall is now " + spawnAll);
					}
				}
			}

			public class BTMysteryChestSpawnAllInput : BTConsoleCommand.BTCommandInput
			{
				public object spawnAll;

				private void SetSpawnAll(object spawnAll, bool isPresent) {
					this.spawnAll = isPresent ? spawnAll : null;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"spawnAll",
						true,
						"spawnAll, otherwise toggles",
						this.SetSpawnAll,
						typeof(bool)
					)
				};
				}
			}
		}

		public class BTPlayerXPGet
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "Player", "getXP" },
					new BTPlayerXPGetInput(),
					"prints rank points of specified type",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTPlayerXPGetInput cmdInput = (BTPlayerXPGetInput)input;
				UserAchievementInfo achInfo = (cmdInput.type == 8)
					? PetRankData.GetUserAchievementInfo(SanctuaryManager.pCurPetData)
					: UserRankData.GetUserAchievementInfoByType(cmdInput.type);
				if(achInfo == null) {
					BTConsole.WriteLine("No Data found for type: " + cmdInput.type);
					return;
				}
				string xpInfoString = achInfo.AchievementPointTotal.Value.ToString();
				string rankInfoString = achInfo.RankID.ToString();
				BTConsole.WriteLine("XP = " + xpInfoString + " | Rank = " + rankInfoString);
			}

			public class BTPlayerXPGetInput : BTConsoleCommand.BTCommandInput
			{
				public int type;

				private void SetType(object type, bool isPresent) {
					this.type = (int)type;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"type",
						false,
						"pointTypeID to print (8 is pet xp)",
						this.SetType,
						typeof(int)
					)
				};
				}
			}
		}

		public class BTPlayerXPAdd
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "Player", "addXP" },
					new BTPlayerXPAddInput(),
					"adds rank points of specified type",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTPlayerXPAddInput cmdInput = (BTPlayerXPAddInput)input;
				UserAchievementInfo achInfo;
				if(cmdInput.type == 8) {
					PetRankData.AddPoints(SanctuaryManager.pCurPetData, cmdInput.amount);
					achInfo = PetRankData.GetUserAchievementInfo(SanctuaryManager.pCurPetData);
				} else {
					UserRankData.AddPoints(cmdInput.type, cmdInput.amount);
					achInfo = UserRankData.GetUserAchievementInfoByType(cmdInput.type);
				}
				if(achInfo == null) {
					BTConsole.WriteLine("No Data found for type: " + cmdInput.type);
					return;
				}
				string xpInfoString = achInfo.AchievementPointTotal.Value.ToString();
				string rankInfoString = achInfo.RankID.ToString();
				BTConsole.WriteLine("Added " + cmdInput.amount + " points of type " + cmdInput.type + " | XP = " + xpInfoString + " | Rank = " + rankInfoString);
			}

			public class BTPlayerXPAddInput : BTConsoleCommand.BTCommandInput
			{
				public int type;
				public int amount;

				private void SetType(object type, bool isPresent) {
					this.type = (int)type;
				}

				private void SetAmount(object amount, bool isPresent) {
					this.amount = (int)amount;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"type",
						false,
						"pointTypeID to add",
						this.SetType,
						typeof(int)
					),
					new BTConsoleCommand.BTConsoleArgument(
						"amount",
						false,
						"amount of points to add",
						this.SetAmount,
						typeof(int)
					)
				};
				}
			}
		}

		public class BTPetState
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "Pet", "State" },
					new BTNoArgsInput(),
					"prints debug info of the active pet",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				if(SanctuaryManager.pCurPetInstance == null) {
					BTConsole.WriteLine("error - No Pet found");
					return;
				}
				BTConsole.WriteLine(SanctuaryManager.pCurPetData.GetDebugString());
			}
		}

		public class BTPetCreate
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "Pet", "Get" },
					new BTPetCreateInput(),
					"creates a pet of specified type",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTPetCreateInput cmdInput = (BTPetCreateInput)input;

				SanctuaryPetTypeInfo petTypeInfo = SanctuaryData.FindSanctuaryPetTypeInfo(cmdInput.type);
				if(petTypeInfo == null) {
					BTConsole.WriteLine("No PetTypeInfo found for type: " + cmdInput.type);
					return;
				}
				string prefab = petTypeInfo._AgeData[RaisedPetData.GetAgeIndex(cmdInput.age)]._PetResList[0]._Prefab;
				RaisedPetData raisedPetData = RaisedPetData.CreateCustomizedPetData(petTypeInfo._TypeID, cmdInput.age, prefab, cmdInput.gender, null, true);
				raisedPetData.pNoSave = false;
				raisedPetData.Name = petTypeInfo._Name;
				UnityEngine.Object.Destroy(SanctuaryManager.pCurPetInstance);
				SanctuaryManager.pCurPetInstance = null;
				SanctuaryManager.pCurPetData = raisedPetData;
				SanctuaryManager.CreatePet(raisedPetData, Vector3.zero, Quaternion.identity, SanctuaryManager.pInstance.gameObject, "Full", false);
				SanctuaryManager.pInstance.pSetFollowAvatar = false;
				SanctuaryManager.pCurrentPetType = cmdInput.type;
				SanctuaryManager.pInstance.pCreateInstance = false;
				RaisedPetData.UpdateActivePet(cmdInput.type, raisedPetData);
				BTConsole.WriteLine("Created temporary pet of type: " + cmdInput.type + " | age: " + cmdInput.age + " | gender: " + cmdInput.gender);
			}

			public class BTPetCreateInput : BTConsoleCommand.BTCommandInput
			{
				public int type;
				public RaisedPetStage age;
				public Gender gender;

				private void SetType(object type, bool isPresent) {
					this.type = isPresent ? (int)type : 11;
				}

				private void SetAge(object age, bool isPresent) {
					this.age = isPresent ? (RaisedPetStage)age : RaisedPetStage.BABY;
				}

				private void SetGender(object gender, bool isPresent) {
					this.gender = isPresent ? (Gender)gender : Gender.Male;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"type",
						true,
						"petTypeID of the pet to create, defaults to 11",
						this.SetType,
						typeof(int)
					),
					new BTConsoleCommand.BTConsoleArgument(
						"age",
						true,
						"age to provide to the pet creator, defaults to 'BABY', valid input {BABY, CHILD, TEEN, ADULT, TITAN}",
						this.SetAge,
						typeof(RaisedPetStage)
					),
					new BTConsoleCommand.BTConsoleArgument(
						"gender",
						true,
						"gender to provide to the pet creator, defaults to 'Male', valid input {Male, Female}",
						this.SetGender,
						typeof(Gender)
					)
				};
				}
			}
		}

		public class BTPetRelease
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "Pet", "Release" },
					new BTNoArgsInput(),
					"releases the active pet",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				SanctuaryManager.pCurPetData.ReleasePet(null);
				if(SanctuaryManager.pCurPetInstance != null) {
					UnityEngine.Object.Destroy(SanctuaryManager.pCurPetInstance.gameObject);
				}
				SanctuaryManager.pCurPetInstance = null;
				BTConsole.WriteLine("Pet Released.");
			}
		}

		public class BTServerTimeGet
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "ServerTime", "get" },
					new BTNoArgsInput(),
					"prints the current ServerTime",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTConsole.WriteLine("Current Server Time: " + ServerTime.pCurrentTime);
			}
		}

		public class BTServerTimeReset
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "ServerTime", "reset" },
					new BTNoArgsInput(),
					"resets the ServerTime and re-enables timeHack prevention",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				if(TimeHackPrevent.pInstance != null) {
					TimeHackPrevent.pInstance.gameObject.SetActive(true);
				}
				ServerTime.ResetOffsetTime();
				BTConsole.WriteLine("Reset Server Time - Current Server Time is " + ServerTime.pCurrentTime);
			}
		}

		public class BTServerTimeAdd
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "ServerTime", "add" },
					new BTServerTimeAddInput(),
					"adds a time offset to the serverTime and disables timeHack prevention",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTServerTimeAddInput cmdInput = (BTServerTimeAddInput)input;
				if(TimeHackPrevent.pInstance != null) {
					TimeHackPrevent.pInstance.gameObject.SetActive(false);
				}
				ServerTime.AddOffsetTime(cmdInput.duration);
				BTConsole.WriteLine("Offset Server Time by: " + cmdInput.duration);
				BTConsole.WriteLine("Server Time now is " + ServerTime.pCurrentTime.ToString());
			}

			public class BTServerTimeAddInput : BTConsoleCommand.BTCommandInput
			{
				public string duration;

				private void SetDuration(object duration, bool isPresent) {
					this.duration = (string)duration;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"duration",
						false,
						"formatted time to offset Servertime with",
						this.SetDuration,
						typeof(string)
					)
				};
				}
			}
		}

		public class BTTutorialComplete
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "Tutorial", "complete" },
					new BTTutorialCompleteInput(),
					"completes the specified tutorial",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTTutorialCompleteInput cmdInput = (BTTutorialCompleteInput)input;
				if(ProductData.AddTutorial(cmdInput.name)) {
					BTConsole.WriteLine("Tutorial " + cmdInput.name + " marked as done.");
				} else {
					BTConsole.WriteLine("Tutorial " + cmdInput.name + " is already completed or does not exist.");
				}
			}

			public class BTTutorialCompleteInput : BTConsoleCommand.BTCommandInput
			{
				public string name;

				private void SetName(object name, bool isPresent) {
					this.name = (string)name;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"name",
						false,
						"name of the tutorial to complete",
						this.SetName,
						typeof(string)
					)
				};
				}
			}
		}

		public class BTTutorialReset
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "Tutorial", "reset" },
					new BTTutorialResetInput(),
					"resets all or the specified tutorial",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTTutorialResetInput cmdInput = (BTTutorialResetInput)input;
				bool result = ProductData.ResetTutorial(cmdInput.name);
				if(cmdInput.name == null) {
					if(result) {
						BTConsole.WriteLine("Tutorial play status reset.");
					} else {
						BTConsole.WriteLine("Could not reset Tutorials");
					}
				} else {
					if(result) {
						BTConsole.WriteLine("Tutorial " + cmdInput.name + " reset (not saved)");
					} else {
						BTConsole.WriteLine("Could not Reset Tutorial " + cmdInput.name);
					}
				}
			}

			public class BTTutorialResetInput : BTConsoleCommand.BTCommandInput
			{
				public string name;

				private void SetName(object name, bool isPresent) {
					this.name = (string)name;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"name",
						true,
						"name of the tutorial to reset",
						this.SetName,
						typeof(string)
					)
				};
				}
			}
		}

		public class BTAchievementSet
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "Achievement", "set" },
					new BTAchievementSetInput(),
					"sets an achievement",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTAchievementSetInput cmdInput = (BTAchievementSetInput)input;
				if(cmdInput.points == null) {
					UserAchievementTask.Set(cmdInput.taskID, "", false, 0, null, null);
					BTConsole.WriteLine("completing achievementTaskID: " + cmdInput.taskID);
				} else {
					int points = (int)cmdInput.points;
					AchievementTask task = new AchievementTask(cmdInput.taskID, "", 0, points, 1);
					UserAchievementTask.Set(new AchievementTask[] { task });
					BTConsole.WriteLine("setting achievementTaskID: " + cmdInput.taskID + " | to amount: " + points);
				}
			}

			public class BTAchievementSetInput : BTConsoleCommand.BTCommandInput
			{
				public int taskID;
				public object points;

				private void SetTaskID(object taskID, bool isPresent) {
					this.taskID = (int)taskID;
				}

				private void SetPoints(object points, bool isPresent) {
					this.points = isPresent ? points : null;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"taskID",
						false,
						"taskID of the achievement to set",
						this.SetTaskID,
						typeof(int)
					),
					new BTConsoleCommand.BTConsoleArgument(
						"points",
						true,
						"points to set the achievement to, optional for pointless achievements",
						this.SetPoints,
						typeof(int)
					)
				};
				}
			}
		}

		public class BTAchievementSetWeb
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "Achievement", "set", "web" },
					new BTAchievementSetWebInput(),
					"sets an achievement using the webserver (and gets reward)",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTAchievementSetWebInput cmdInput = (BTAchievementSetWebInput)input;
				WsWebService.SetAchievementAndGetReward(cmdInput.taskID, "", new WsServiceEventHandler(ServiceEventHandler), null);
				BTConsole.WriteLine("SetAchievementAndGetReward: " + cmdInput.taskID);
			}

			public static void ServiceEventHandler(WsServiceType inType, WsServiceEvent inEvent, float inProgress, object inObject, object inUserData) {
				if(inEvent != WsServiceEvent.COMPLETE) {
					return;
				}
				if(inObject != null) {
					AchievementReward[] array = (AchievementReward[])inObject;
					if(array != null) {
						GameUtilities.AddRewards(array, false, false);
					}
				}
			}

			public class BTAchievementSetWebInput : BTConsoleCommand.BTCommandInput
			{
				public int taskID;

				private void SetTaskID(object taskID, bool isPresent) {
					this.taskID = (int)taskID;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"taskID",
						false,
						"taskID of the achievement to set",
						this.SetTaskID,
						typeof(int)
					)
				};
				}
			}
		}

		public class BTAchievementClanSet
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "Achievement", "clan", "set" },
					new BTAchievementClanSetInput(),
					"sets a clan achievement",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTAchievementClanSetInput cmdInput = (BTAchievementClanSetInput)input;
				UserAchievementTask.Set(UserProfile.pProfileData.Groups[0].GroupID, cmdInput.taskID, 2, "", false, 0, null, null);
				BTConsole.WriteLine("Setting Clan Achievement " + cmdInput.taskID);
			}

			public class BTAchievementClanSetInput : BTConsoleCommand.BTCommandInput
			{
				public int taskID;

				private void SetTaskID(object taskID, bool isPresent) {
					this.taskID = (int)taskID;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"taskID",
						false,
						"taskID of the achievement to set",
						this.SetTaskID,
						typeof(int)
					)
				};
				}
			}
		}

		public class BTAssetBundleList
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "AssetBundle", "list" },
					new BTLoadAssetInput(),
					"lists all the assets in a local AssetBundle",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTLoadAssetInput cmdInput = (BTLoadAssetInput)input;
				AssetBundle assetBundle = AssetBundle.LoadFromFile(cmdInput.assetPath);
				try {
					if(assetBundle == null) {
						BTConsole.WriteLine("Unable to load assetBundle at path: " + cmdInput.assetPath);
						return;
					}
					BTConsole.WriteLine("AssetNames: {'" + String.Join("', '", assetBundle.GetAllAssetNames()) + "'}");
					BTConsole.WriteLine("ScenePaths: {'" + String.Join("', '", assetBundle.GetAllScenePaths()) + "'}");
				} catch(Exception e) {
					BTConsole.WriteLine("AssetBundle list failed - " + e.ToString());
				} finally {
					if(assetBundle != null) {
						assetBundle.Unload(true);
					}
				}
			}

			public class BTLoadAssetInput : BTConsoleCommand.BTCommandInput
			{
				public string assetPath;

				private void SetAssetPath(object assetPath, bool isPresent) {
					this.assetPath = (string)assetPath;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"assetPath",
						false,
						"full path to the assetBundle to load",
						this.SetAssetPath,
						typeof(string)
					)
				};
				}
			}
		}

		public class BTAssetBundleLoad
		{
			public static void Register() {
				BTConsole.AddCommand(new BTConsoleCommand(
					new List<string>() { "AssetBundle", "load", "asset" },
					new BTAssetBundleLoadInput(),
					"loads an asset from a local AssetBundle",
					OnExecuteLoadAsset
				));
				BTConsole.AddCommand(new BTConsoleCommand(
					new List<string>() { "AssetBundle", "load", "scene" },
					new BTAssetBundleLoadSceneInput(),
					"loads a scene from a local AssetBundle",
					OnExecuteLoadScene
				));
				BTConsole.AddCommand(new BTConsoleCommand(
					new List<string>() { "AssetBundle", "unload" },
					new BTNoArgsInput(),
					"unloads all loaded assetBundle assets",
					OnExecuteUnload
				));
			}

			private static readonly List<GameObject> loadedAssets = new List<GameObject>();

			public static void OnExecuteLoadAsset(BTConsoleCommand.BTCommandInput input) {
				BTAssetBundleLoadInput cmdInput = (BTAssetBundleLoadInput)input;
				AssetBundle assetBundle = AssetBundle.LoadFromFile(cmdInput.assetPath);
				try {
					if(assetBundle == null) {
						BTConsole.WriteLine("Unable to load assetBundle at path: " + cmdInput.assetPath);
						return;
					}
					GameObject gameObject = assetBundle.LoadAsset<GameObject>(cmdInput.assetName);
					if(gameObject == null) {
						BTConsole.WriteLine("Unable to find GameObject (" + cmdInput.assetName + ") in loaded assetBundle!");
						return;
					}
					gameObject = UnityEngine.Object.Instantiate<GameObject>(gameObject);
					loadedAssets.Add(gameObject);
					BTConsole.WriteLine("instantiated gameObject.");
				} catch(Exception e) {
					BTConsole.WriteLine("AssetBundle load asset failed - " + e.ToString());
				} finally {
					if(assetBundle != null) {
						assetBundle.Unload(false);
					}
				}
			}

			public static void OnExecuteLoadScene(BTConsoleCommand.BTCommandInput input) {
				BTAssetBundleLoadSceneInput cmdInput = (BTAssetBundleLoadSceneInput)input;
				AssetBundle assetBundle = AssetBundle.LoadFromFile(cmdInput.assetPath);
				try {
					if(assetBundle == null) {
						BTConsole.WriteLine("Unable to load assetBundle at path: " + cmdInput.assetPath);
						return;
					}
					if(!assetBundle.isStreamedSceneAssetBundle) {
						BTConsole.WriteLine("Unable to load scene, bundle has incompatible format!");
						return;
					}
					string[] scenePaths = assetBundle.GetAllScenePaths();
					if(scenePaths.Length == 0) {
						BTConsole.WriteLine("Unable to load scene, no scenes found in the bundle!");
						return;
					}
					if(scenePaths.Length <= cmdInput.sceneIndex || cmdInput.sceneIndex < 0) {
						BTConsole.WriteLine("Index: " + cmdInput.sceneIndex + " | is out of bounds!");
						return;
					}
					BTConsole.WriteLine("loading scene: " + scenePaths[cmdInput.sceneIndex]);
					RsResourceManager.LoadLevel(System.IO.Path.GetFileNameWithoutExtension(scenePaths[cmdInput.sceneIndex]), false);
				} catch(Exception e) {
					BTConsole.WriteLine("AssetBundle load asset failed - " + e.ToString());
				} finally {
					if(assetBundle != null) {
						assetBundle.Unload(false);
					}
				}
			}

			public static void OnExecuteUnload(BTConsoleCommand.BTCommandInput input) {
				try {
					BTConsole.WriteLine("Destroying " + loadedAssets.Count + " assets.");
					foreach(GameObject asset in loadedAssets) {
						UnityEngine.Object.Destroy(asset);
					}
					BTConsole.WriteLine("done.");
				} catch(Exception e) {
					BTConsole.WriteLine("AssetBundle unload failed - " + e.ToString());
				} finally {
					loadedAssets.Clear();
				}
			}

			public class BTAssetBundleLoadInput : BTConsoleCommand.BTCommandInput
			{
				public string assetPath;
				public string assetName;

				private void SetAssetPath(object assetPath, bool isPresent) {
					this.assetPath = (string)assetPath;
				}

				private void SetAssetName(object assetName, bool isPresent) {
					this.assetName = (string)assetName;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"assetPath",
						false,
						"full path to the assetBundle to load",
						this.SetAssetPath,
						typeof(string)
					),
					new BTConsoleCommand.BTConsoleArgument(
						"assetName",
						false,
						"name of the asset to load, must be a GameObject",
						this.SetAssetName,
						typeof(string)
					)
				};
				}
			}

			public class BTAssetBundleLoadSceneInput : BTConsoleCommand.BTCommandInput
			{
				public string assetPath;
				public int sceneIndex;

				private void SetAssetPath(object assetPath, bool isPresent) {
					this.assetPath = (string)assetPath;
				}

				private void SetSceneIndex(object sceneIndex, bool isPresent) {
					this.sceneIndex = isPresent ? (int)sceneIndex : 0;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"assetPath",
						false,
						"full path to the assetBundle to load",
						this.SetAssetPath,
						typeof(string)
					),
					new BTConsoleCommand.BTConsoleArgument(
						"sceneIndex",
						true,
						"index of the scene to load, otherwise 0",
						this.SetSceneIndex,
						typeof(int)
					)
				};
				}
			}
		}

		public class BTConfigReload
		{
			public static void Register() {
				BTConsoleCommand command = new BTConsoleCommand(
					new List<string>() { "config", "reload" },
					new BTNoArgsInput(),
					"reloads the config file",
					OnExecute
				);
				BTConsole.AddCommand(command);
			}

			public static void OnExecute(BTConsoleCommand.BTCommandInput input) {
				BTDebugCamInputManager.ReloadConfig();
				BTConsole.WriteLine("config reloaded.");
			}
		}

		public class BTPetMeter
		{
			public static void Register() {
				BTConsole.AddCommand(new BTConsoleCommand(
					new List<string>() { "pet", "meter", "get" },
					new BTPetMeterGetInput(),
					"prints the meterValue of the active pet",
					OnExecutePetMeterGet
				));
				BTConsole.AddCommand(new BTConsoleCommand(
					new List<string>() { "pet", "meter", "set" },
					new BTPetMeterSetInput(),
					"sets the meterValue of the active pet",
					OnExecutePetMeterSet
				));
			}

			public static void OnExecutePetMeterGet(BTConsoleCommand.BTCommandInput input) {
				BTPetMeterGetInput cmdInput = (BTPetMeterGetInput)input;
				SanctuaryPet activePet = SanctuaryManager.pCurPetInstance;
				if(activePet == null) {
					BTConsole.WriteLine("error - No Pet found");
					return;
				}
				SanctuaryPetMeterInstance meterInstance = activePet.GetPetMeter(cmdInput.meterType);
				if(meterInstance == null) {
					BTConsole.WriteLine("error - No Meter of type '" + cmdInput.meterType + "' found in active pet!");
					return;
				}
				float meterValue = meterInstance.mMeterValData.Value;
				float maxMeterValue = SanctuaryData.GetMaxMeter(cmdInput.meterType, activePet.pData);
				BTConsole.WriteLine("meterValue of meterType: '" + cmdInput.meterType + "' is: '" + meterValue + "' | maxValue: '" + maxMeterValue + "'");
			}

			public static void OnExecutePetMeterSet(BTConsoleCommand.BTCommandInput input) {
				BTPetMeterSetInput cmdInput = (BTPetMeterSetInput)input;
				SanctuaryPet activePet = SanctuaryManager.pCurPetInstance;
				if(activePet == null) {
					BTConsole.WriteLine("error - No Pet found");
					return;
				}
				SanctuaryPetMeterInstance meterInstance = activePet.GetPetMeter(cmdInput.meterType);
				if(meterInstance == null) {
					BTConsole.WriteLine("error - No Meter of type '" + cmdInput.meterType + "' found in active pet!");
					return;
				}
				float meterValue = meterInstance.mMeterValData.Value;
				if(cmdInput.meterValue == BTPetMeterSetInput.MAX_VALUE) {
					cmdInput.meterValue = SanctuaryData.GetMaxMeter(cmdInput.meterType, activePet.pData);
				}
				activePet.SetMeter(cmdInput.meterType, cmdInput.meterValue, cmdInput.forceUpdate);
				float meterValueNew = meterInstance.mMeterValData.Value;
				BTConsole.WriteLine("changed meterValue of meterType: '" + cmdInput.meterType + "' from: '" + meterValue + "' to: '" + cmdInput.meterValue + "' | actualMeterValue: '" + meterValueNew + "'");
			}

			public class BTPetMeterGetInput : BTConsoleCommand.BTCommandInput
			{
				public SanctuaryPetMeterType meterType;

				private void SetMeterType(object meterType, bool isPresent) {
					this.meterType = (SanctuaryPetMeterType)meterType;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"meterType",
						false,
						"meterType to change - {Happiness, Energy, Racing_Energy, Racing_Fire, Health}",
						this.SetMeterType,
						typeof(SanctuaryPetMeterType)
					)
				};
				}
			}

			public class BTPetMeterSetInput : BTConsoleCommand.BTCommandInput
			{
				public SanctuaryPetMeterType meterType;
				public float meterValue;
				public bool forceUpdate;

				public const float MAX_VALUE = -1;

				private void SetMeterType(object meterType, bool isPresent) {
					this.meterType = (SanctuaryPetMeterType)meterType;
				}

				private void SetMeterValue(object meterValue, bool isPresent) {
					this.meterValue = isPresent ? (float)meterValue : MAX_VALUE;
				}

				private void SetForceUpdate(object forceUpdate, bool isPresent) {
					this.forceUpdate = isPresent ? (bool)forceUpdate : false;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"meterType",
						false,
						"meterType to change - {Happiness, Energy, Racing_Energy, Racing_Fire, Health}",
						this.SetMeterType,
						typeof(SanctuaryPetMeterType)
					),
					new BTConsoleCommand.BTConsoleArgument(
						"meterValue",
						true,
						"value to set the meter to, defaults to the maxValue of the meter (" + MAX_VALUE + ")",
						this.SetMeterValue,
						typeof(float)
					),
					new BTConsoleCommand.BTConsoleArgument(
						"forceUpdate",
						true,
						"shouldn't be necessary, but there may be some cases where the meterValue is locked.",
						this.SetForceUpdate,
						typeof(bool)
					)
				};
				}
			}
		}

		public class BTPetAge
		{
			public static void Register() {
				BTConsole.AddCommand(new BTConsoleCommand(
					new List<string>() { "pet", "age", "get" },
					new BTNoArgsInput(),
					"prints the age of the active pet",
					OnExecutePetAgeGet
				));
				BTConsole.AddCommand(new BTConsoleCommand(
					new List<string>() { "pet", "age", "set" },
					new BTPetAgeSetInput(),
					"sets the age of the active pet",
					OnExecutePetAgeSet
				));
			}

			public static void OnExecutePetAgeGet(BTConsoleCommand.BTCommandInput input) {
				SanctuaryPet activePet = SanctuaryManager.pCurPetInstance;
				if(activePet == null) {
					BTConsole.WriteLine("error - No Pet found");
					return;
				}
				RaisedPetStage petStage = RaisedPetData.GetGrowthStage(activePet.pAge);
				BTConsole.WriteLine("Current pet-age is: " + petStage);
			}

			public static void OnExecutePetAgeSet(BTConsoleCommand.BTCommandInput input) {
				BTPetAgeSetInput cmdInput = (BTPetAgeSetInput)input;
				int ageIndex = RaisedPetData.GetAgeIndex(cmdInput.age);
				SanctuaryPet activePet = SanctuaryManager.pCurPetInstance;
				if(activePet == null) {
					BTConsole.WriteLine("error - No Pet found");
					return;
				}
				RaisedPetStage previousStage = RaisedPetData.GetGrowthStage(activePet.pAge);
				bool success = activePet.SetAge(ageIndex, cmdInput.save, cmdInput.resetSkills, null);
				if(!success) {
					BTConsole.WriteLine("error - unable to set pet age (unknown cause)");
					return;
				}
				RaisedPetStage currentStage = RaisedPetData.GetGrowthStage(activePet.pAge);
				BTConsole.WriteLine("changed pet age from: '" + previousStage + "' to: '" + currentStage + "'");
			}

			public class BTPetAgeSetInput : BTConsoleCommand.BTCommandInput
			{
				public RaisedPetStage age;
				public bool save;
				public bool resetSkills;

				private void SetAge(object age, bool isPresent) {
					this.age = (RaisedPetStage)age;
				}

				private void SetSave(object save, bool isPresent) {
					this.save = isPresent ? (bool)save : true;
				}

				private void SetResetSkills(object resetSkills, bool isPresent) {
					this.resetSkills = isPresent ? (bool)resetSkills : true;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"age",
						false,
						"age to apply to the active pet, valid input {BABY, CHILD, TEEN, ADULT, TITAN}",
						this.SetAge,
						typeof(RaisedPetStage)
					),
					new BTConsoleCommand.BTConsoleArgument(
						"save",
						true,
						"default is 'true', I'd recommend leaving it unchanged",
						this.SetSave,
						typeof(bool)
					),
					new BTConsoleCommand.BTConsoleArgument(
						"resetSkills",
						true,
						"default is 'true', I'd recommend leaving it unchanged",
						this.SetResetSkills,
						typeof(bool)
					)
				};
				}
			}
		}

		public class BTCheckPass
		{
			public static void Register() {
				BTConsole.AddCommand(new BTConsoleCommand(
					new List<string>() { "check", "console", "pass" },
					new BTCheckConsolePassInput(),
					"compare your password to the stored one",
					OnExecuteCheckConsolePass
				));
			}

			public static void OnExecuteCheckConsolePass(BTConsoleCommand.BTCommandInput input) {
				BTCheckConsolePassInput cmdInput = (BTCheckConsolePassInput)input;
				BTConsole.WriteLine("Stored Password Hash: " + ProductConfig.pInstance.ConsolePassword);
				string hash = WsMD5Hash.GetMd5Hash(cmdInput.password);
				BTConsole.WriteLine("Passed Password Hash: " + hash + " (" + cmdInput.password + ")");
				bool matches = ProductConfig.pInstance.ConsolePassword == hash;
				BTConsole.WriteLine("matches? " + (matches ? "YES" : "NO"));
			}

			public class BTCheckConsolePassInput : BTConsoleCommand.BTCommandInput
			{
				public string password;

				private void SetPassword(object password, bool isPresent) {
					this.password = (string)password;
				}

				protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
					return new List<BTConsoleCommand.BTConsoleArgument>(){
					new BTConsoleCommand.BTConsoleArgument(
						"password",
						false,
						"plaintext password to check",
						this.SetPassword,
						typeof(string)
					)
				};
				}
			}
		}
	}
}
