using System;
using System.Collections.Generic;
using SquadTactics;
using UnityEngine;

namespace SoD_BaseMod.basemod.console.commands {
	public static class BTAvatarCommands {
		private static Vector3? previousPosition;

		public static void Register() {
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "avatar", "get", "position" },
					new GetPositionInput(),
					"prints the avatar's current position to the console",
					OnExecuteGetPosition
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "avatar", "teleport", "object" },
					new TeleportToObjectInput(),
					"teleports the avatar to a given object",
					OnExecuteTeleportToObject
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "avatar", "teleport", "position" },
					new TeleportToPositionInput(),
					"teleports the avatar to a given position",
					OnExecuteTeleportToPosition
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "avatar", "teleport", "camera" },
					new BTNoArgsInput(),
					"teleports the avatar to the main-camera",
					OnExecuteTeleportCamera
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "avatar", "teleport", "back" },
					new BTNoArgsInput(),
					"teleports the avatar to the 'previous' position",
					OnExecuteTeleportBack
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "avatar", "clear", "data" },
					new BTNoArgsInput(),
					"clears the avatarData (parts)",
					OnExecuteClearData
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "avatar", "fix" },
					new BTNoArgsInput(),
					"tries to fix the avatar by reloading the model without version/head information",
					OnExecuteFix
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "avatar", "save", "data" },
					new BTNoArgsInput(),
					"calls the AvatarData.Save function",
					OnExecuteSaveData
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "avatar", "show", "userID" },
					new BTNoArgsInput(),
					"prints the userID to console",
					OnExecuteShowUserID
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "avatar", "show", "state" },
					new BTNoArgsInput(),
					"prints information on the avatar's state",
					OnExecuteShowState
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "avatar", "set", "stat" },
					new SetStatInput(),
					"sets the statValue of a worn part",
					OnExecuteSetStat
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "avatar", "set", "speed" },
					new SetSpeedInput(),
					"sets the avatar speed",
					OnExecuteSetSpeed
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "avatar", "update", "flyingData" },
					new BTNoArgsInput(),
					"invokes the avatar's 'OnUpdateAvatar' function",
					OnExecuteUpdateFlyingData
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string>{"avatar", "part", "info"}, 
					new BTNoArgsInput(), 
					"prints info on all equipped avatar parts",
					OnExecuteAvatarPartInfo));
		}

		private static void OnExecuteGetPosition(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (GetPositionInput) input;
			if (AvAvatar.pObject == null) {
				BTConsole.WriteLine("Unable to print position - Avatar not found");
				return;
			}

			Vector3 currentPosition = AvAvatar.GetPosition();
			if (cmdInput.makePreviousPosition) {
				previousPosition = currentPosition;
				BTConsole.WriteLine("Set previousPosition to: " + currentPosition);
			} else {
				BTConsole.WriteLine("Current Avatar Position: " + currentPosition);
			}
		}

		private class GetPositionInput : BTConsoleCommand.BTCommandInput {
			public bool makePreviousPosition;

			private void SetMakePreviousPosition(object makePreviousPosition, bool isPresent) {
				this.makePreviousPosition = isPresent && (bool) makePreviousPosition;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"makePreviousPosition",
								true,
								"if true, will set the previousPosition to the current position, defaults to false",
								SetMakePreviousPosition,
								typeof(bool)
						)
				};
			}
		}

		private static void OnExecuteTeleportToObject(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (TeleportToObjectInput) input;
			if (AvAvatar.pObject == null) {
				BTConsole.WriteLine("Unable to teleport - Avatar not found");
				return;
			}

			GameObject gameObject = GameObject.Find(cmdInput.objectName);
			if (gameObject == null) {
				BTConsole.WriteLine("Unable to teleport - No GameObject of name " + cmdInput.objectName + " found");
				return;
			}

			previousPosition = AvAvatar.GetPosition();
			if (!UtUtilities.FindPosNextToObject(out Vector3 targetPosition, gameObject, cmdInput.distance)) {
				BTConsole.WriteLine(
						"Unable to teleport - No valid position within '" + cmdInput.distance + "' units of '" + cmdInput.objectName + "' found");
				return;
			}

			AvAvatar.TeleportTo(targetPosition);
			AvAvatar.mTransform.LookAt(gameObject.transform);
			BTConsole.WriteLine("Avatar teleported from " + previousPosition + " to " + targetPosition);
		}

		private class TeleportToObjectInput : BTConsoleCommand.BTCommandInput {
			public string objectName;
			public float distance;

			private void SetObjectName(object objectName, bool isPresent) {
				this.objectName = (string) objectName;
			}

			private void SetDistance(object distance, bool isPresent) {
				this.distance = (float) distance;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"object name",
								false,
								"name of the object to teleport to",
								SetObjectName,
								typeof(string)
						),
						new BTConsoleCommand.BTConsoleArgument(
								"distance",
								true,
								"maximum distance from object",
								SetDistance,
								typeof(float)
						)
				};
			}
		}

		private static void OnExecuteTeleportToPosition(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (TeleportToPositionInput) input;
			if (AvAvatar.pObject == null) {
				BTConsole.WriteLine("Unable to teleport - Avatar not found");
				return;
			}

			previousPosition = AvAvatar.GetPosition();
			var targetPosition = new Vector3(cmdInput.x, cmdInput.y, cmdInput.z);
			AvAvatar.TeleportTo(targetPosition);
			BTConsole.WriteLine("Avatar teleported from " + previousPosition + " to " + targetPosition);
		}

		private class TeleportToPositionInput : BTConsoleCommand.BTCommandInput {
			public float x;
			public float y;
			public float z;

			private void SetX(object x, bool isPresent) {
				this.x = (float) x;
			}

			private void SetY(object y, bool isPresent) {
				this.y = (float) y;
			}

			private void SetZ(object z, bool isPresent) {
				this.z = (float) z;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"x",
								true,
								"x value of the target position",
								SetX,
								typeof(float)
						),
						new BTConsoleCommand.BTConsoleArgument(
								"y",
								true,
								"y value of the target position",
								SetY,
								typeof(float)
						),
						new BTConsoleCommand.BTConsoleArgument(
								"z",
								true,
								"z value of the target position",
								SetZ,
								typeof(float)
						)
				};
			}
		}

		private static void OnExecuteTeleportCamera(BTConsoleCommand.BTCommandInput input) {
			Camera mainCamera = BTDebugCam.FindMainCamera();
			if (mainCamera == null) {
				BTConsole.WriteLine("Unable to teleport - mainCamera not found.");
				return;
			}

			previousPosition = AvAvatar.GetPosition();
			Vector3 targetPosition = mainCamera.transform.position;
			AvAvatar.TeleportTo(targetPosition);
			BTConsole.WriteLine("Avatar teleported from " + previousPosition + " to " + targetPosition);
		}

		private static void OnExecuteTeleportBack(BTConsoleCommand.BTCommandInput input) {
			if (previousPosition == null) {
				BTConsole.WriteLine("Unable to teleport - no previous position known. (Only gets set when teleporting using the console commands)");
				return;
			}

			Vector3 targetPosition = previousPosition.Value;
			previousPosition = AvAvatar.GetPosition();
			AvAvatar.TeleportTo(targetPosition);
			BTConsole.WriteLine("Avatar teleported from " + previousPosition + " to " + targetPosition);
		}

		private static void OnExecuteClearData(BTConsoleCommand.BTCommandInput input) {
			AvatarData.Clear();
			BTConsole.WriteLine("Avatar parts cleared");
		}

		private static void OnExecuteFix(BTConsoleCommand.BTCommandInput input) {
			AvatarData.pInstanceInfo.RestorePartData();
			var avatarData = AvatarData.CreateDefault(AvatarData.pInstance.GenderType);
			List<AvatarDataPart> partList = new List<AvatarDataPart>();
			foreach (AvatarDataPart avatarDataPart in avatarData.Part) {
				if (avatarDataPart.PartType == "Version") {
					partList.Add(AvatarData.FindPart(avatarDataPart.PartType));
				} else if (avatarDataPart.PartType == AvatarData.pPartSettings.AVATAR_PART_HEAD) {
					partList.Add(AvatarData.FindPart(avatarDataPart.PartType));
				} else {
					partList.Add(avatarDataPart);
				}
			}

			AvatarData.pInstance.Part = partList.ToArray();
			AvatarData.pInstanceInfo.LoadBundlesAndUpdateAvatar();
			BTConsole.WriteLine("Avatar data reset - excluded version and head");
		}

		private static void OnExecuteSaveData(BTConsoleCommand.BTCommandInput input) {
			AvatarData.Save();
			BTConsole.WriteLine("Avatar data reset - excluded version and head");
		}

		private static void OnExecuteShowUserID(BTConsoleCommand.BTCommandInput input) {
			if (UserInfo.pInstance != null) {
				BTConsole.WriteLine("UserID: " + UserInfo.pInstance.UserID);
			} else {
				BTConsole.WriteLine("can't print userID - no UserInfo found.");
			}
		}

		private static void OnExecuteShowState(BTConsoleCommand.BTCommandInput input) {
			BTConsole.WriteLine("CurrentState: " + AvAvatar.pState + " | SubState: " + AvAvatar.pSubState);
			BTConsole.WriteLine("Old State: " + AvAvatar.pPrevState);
		}

		private static void OnExecuteSetStat(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (SetStatInput) input;
			if (AvatarData.pInstanceInfo.FindPart(cmdInput.part) == null) {
				BTConsole.WriteLine("Part " + cmdInput.part + " not found");
				return;
			}

			UserItemData itemData = CommonInventoryData.pInstance
					.FindItemByUserInventoryID(AvatarData.pInstanceInfo.GetPartInventoryID(cmdInput.part));
			if (itemData == null) {
				BTConsole.WriteLine("No inventory item found for " + cmdInput.part);
				return;
			}

			string statEffectName = Settings.pInstance.GetStatEffectName(cmdInput.statID);
			if (itemData.ItemStats != null) {
				ItemStat itemStat = Array.Find(itemData.ItemStats, x => x.ItemStatID == cmdInput.statID);
				if (itemStat == null) {
					// stat is not in item -> add it manually
					ItemStat[] itemStats = itemData.ItemStats;
					Array.Resize(ref itemStats, itemData.ItemStats.Length + 1);
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
					BTConsole.WriteLine("Changed '" + statEffectName + "' of part '" + cmdInput.part + "' from '" + previousValue + "' to '" +
							cmdInput.value + "'");
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

		private class SetStatInput : BTConsoleCommand.BTCommandInput {
			public string part;
			public int statID;
			public string value;

			private void SetPart(object part, bool isPresent) {
				this.part = (string) part;
			}

			private void SetStatID(object statID, bool isPresent) {
				this.statID = (int) statID;
			}

			private void SetValue(object value, bool isPresent) {
				this.value = (string) value;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"part",
								false,
								"name of the part to modify stats on",
								SetPart,
								typeof(string)
						),
						new BTConsoleCommand.BTConsoleArgument(
								"statID",
								false,
								"ID of the stat to modify",
								SetStatID,
								typeof(int)
						),
						new BTConsoleCommand.BTConsoleArgument(
								"value",
								false,
								"value the stat shall take",
								SetValue,
								typeof(string)
						)
				};
			}
		}

		private static void OnExecuteSetSpeed(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (SetSpeedInput) input;
			if (AvAvatar.pObject == null) {
				BTConsole.WriteLine("error - Avatar not found, can't set speed");
				return;
			}

			var component = AvAvatar.pObject.GetComponent<AvAvatarController>();
			if (component == null) {
				BTConsole.WriteLine("error - AvatarController not found, can't set speed");
				return;
			}

			float previousSpeed = component.pCurrentStateData._MaxForwardSpeed;
			component.pCurrentStateData._MaxForwardSpeed = cmdInput.speed;
			BTConsole.WriteLine("changed avatar speed from '" + previousSpeed + "' to '" + cmdInput.speed + "'");
		}

		private class SetSpeedInput : BTConsoleCommand.BTCommandInput {
			public float speed;

			private void SetSpeed(object speed, bool isPresent) {
				this.speed = (float) speed;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"speed",
								false,
								"target avatar speed",
								SetSpeed,
								typeof(float)
						)
				};
			}
		}

		private static void OnExecuteUpdateFlyingData(BTConsoleCommand.BTCommandInput input) {
			AvAvatar.pObject.GetComponent<AvAvatarController>().OnUpdateAvatar();
			BTConsole.WriteLine("FlyingData updated.");
		}

		private static void OnExecuteAvatarPartInfo(BTConsoleCommand.BTCommandInput input) {
			foreach (AvatarDataPart avatarPart in AvatarData.pInstanceInfo.mInstance.Part) {
				BTConsole.WriteLine($"partType: '{avatarPart.PartType}', itemID: '{avatarPart.ItemId}', inventoryID: '{avatarPart.UserInventoryId}'");
			}
		}
	}
}