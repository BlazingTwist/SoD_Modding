using System.Collections.Generic;
using System.Linq;

namespace SoD_BaseMod.basemod.console.commands {
	public static class BTMissionCommands {
		public static void Register() {
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Mission", "Init" },
					new BTNoArgsInput(),
					"initializes the MissionManager",
					OnExecuteInit
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Mission", "Reset" },
					new BTNoArgsInput(),
					"resets the MissionManager",
					OnExecuteReset
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Mission", "Save" },
					new SaveInput(),
					"enables/disables mission saving",
					OnExecuteSave
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Mission", "Fail" },
					new FailInput(),
					"enables/disables automatic mission failing",
					OnExecuteFail
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Mission", "SyncDB" },
					new SyncDBInput(),
					"enables/disables database synchronisation",
					OnExecuteSyncDB
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Mission", "Unlock" },
					new UnlockInput(),
					"when enabled, missions ignore all locking-constraints (e.g. winter-missions become available during summer)",
					OnExecuteUnlock
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Mission", "Accept" },
					new AcceptInput(),
					"Accepts a Mission",
					OnExecuteAccept
			));
		}

		private static void OnExecuteInit(BTConsoleCommand.BTCommandInput input) {
			if (MissionManager.pInstance == null) {
				BTConsole.WriteLine("error - no Instance of MissionManager found.");
				return;
			}

			if (MissionManager.pIsReady) {
				BTConsole.WriteLine("MissionManager is already initialized.");
				return;
			}

			MissionManager.Init();
			BTConsole.WriteLine("MissionManager initialized.");
		}

		private static void OnExecuteReset(BTConsoleCommand.BTCommandInput input) {
			if (MissionManager.pInstance == null) {
				BTConsole.WriteLine("error - no Instance of MissionManager found.");
				return;
			}

			MissionManager.Reset();
			BTConsole.WriteLine("Missions reset.");
		}

		private static void OnExecuteSave(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (SaveInput) input;
			if (MissionManager.pInstance == null) {
				BTConsole.WriteLine("error - no Instance of MissionManager found.");
				return;
			}

			bool previousValue = Mission.pSave;
			bool newValue = cmdInput.save;
			if (previousValue == newValue) {
				BTConsole.WriteLine("MissionSaving already was: " + newValue);
			} else {
				Mission.pSave = newValue;
				BTConsole.WriteLine("Changed MissionSaving from: " + previousValue + " | to: " + newValue);
			}
		}

		private class SaveInput : BTConsoleCommand.BTCommandInput {
			public bool save;

			private void SetSave(object save, bool isPresent) {
				this.save = !isPresent || (bool) save;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
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

		private static void OnExecuteFail(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (FailInput) input;
			if (MissionManager.pInstance == null) {
				BTConsole.WriteLine("error - no Instance of MissionManager found.");
				return;
			}

			bool previousValue = Mission.pFail;
			bool newValue = cmdInput.fail;
			if (previousValue == newValue) {
				BTConsole.WriteLine("MissionFailing already was: " + newValue);
			} else {
				Mission.pFail = newValue;
				BTConsole.WriteLine("Changed MissionFailing from: " + previousValue + " | to: " + newValue);
			}
		}

		private class FailInput : BTConsoleCommand.BTCommandInput {
			public bool fail;

			private void SetFail(object fail, bool isPresent) {
				this.fail = isPresent && (bool) fail;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
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

		private static void OnExecuteSyncDB(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (SyncDBInput) input;
			if (MissionManager.pInstance == null) {
				BTConsole.WriteLine("error - no Instance of MissionManager found.");
				return;
			}

			bool previousValue = Mission.pSyncDB;
			bool newValue = cmdInput.syncDB;
			if (previousValue == newValue) {
				BTConsole.WriteLine("SyncDB already was: " + newValue);
			} else {
				Mission.pSyncDB = newValue;
				BTConsole.WriteLine("Changed SyncDB from: " + previousValue + " | to: " + newValue);
			}
		}

		private class SyncDBInput : BTConsoleCommand.BTCommandInput {
			public bool syncDB;

			private void SetSyncDB(object syncDB, bool isPresent) {
				this.syncDB = !isPresent || (bool) syncDB;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
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

		private static void OnExecuteUnlock(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (UnlockInput) input;
			if (MissionManager.pInstance == null) {
				BTConsole.WriteLine("error - no Instance of MissionManager found.");
				return;
			}

			bool previousValue = !Mission.pLocked;
			bool newValue = cmdInput.unlock;
			if (previousValue == newValue) {
				BTConsole.WriteLine("Unlock already was: " + newValue);
			} else {
				Mission.pLocked = !newValue;
				BTConsole.WriteLine("Changed Unlock from: " + previousValue + " | to: " + newValue);
			}
		}

		private class UnlockInput : BTConsoleCommand.BTCommandInput {
			public bool unlock;

			private void SetUnlock(object unlock, bool isPresent) {
				this.unlock = isPresent && (bool) unlock;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
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

		private static void OnExecuteAccept(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (AcceptInput) input;
			if (MissionManager.pInstance == null) {
				BTConsole.WriteLine("error - no Instance of MissionManager found.");
				return;
			}

			Mission mission = MissionManager.pInstance.GetMission(cmdInput.missionID);
			if (mission == null) {
				BTConsole.WriteLine("unable to accept mission - no mission of id '" + cmdInput.missionID + "' found.");
				return;
			}

			if (mission.pStaticDataReady) {
				BTConsole.WriteLine("Mission '" + cmdInput.missionID + "' found and ready - accepting Mission...");
				MissionManager.pInstance.AcceptMission(
						cmdInput.missionID,
						AcceptMissionCallback
				);
			} else {
				BTConsole.WriteLine("Mission '" + cmdInput.missionID + "' found but not ready - loading MissionData...");
				MissionManager.pInstance.LoadMissionData(
						-1,
						OnLoadAcceptMissionData,
						cmdInput.missionID
				);
			}
		}

		private class AcceptInput : BTConsoleCommand.BTCommandInput {
			public int missionID;

			private void SetMissionID(object missionID, bool isPresent) {
				this.missionID = isPresent ? (int) missionID : -1;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
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

		private static void OnLoadAcceptMissionData(List<Mission> missions) {
			if (missions == null || missions.Count <= 0) {
				BTConsole.WriteLine("unable to load missionData - no missions found.");
				return;
			}

			Mission mission = missions[0];
			BTConsole.WriteLine("MissionData loaded for id '" + mission.MissionID + "' - accepting Mission...");
			MissionManager.pInstance.AcceptMission(
					mission.MissionID,
					AcceptMissionCallback
			);
		}

		private static void AcceptMissionCallback(bool success, Mission mission) {
			if (mission == null) {
				BTConsole.WriteLine("error - received AcceptMission callback for `null` mission");
				return;
			}

			if (!success) {
				BTConsole.WriteLine("error - AcceptMissionCallback was unsuccessful for mission '" + mission.MissionID + "'");
				return;
			}

			Mission rootMission = MissionManager.pInstance.GetRootMission(mission);
			MarkPrerequisiteMissionsComplete(rootMission);
			if (rootMission.pData.Hidden) {
				BTConsole.WriteLine("warning - rootMissionData was hidden, unable to set active task");
				return;
			}

			List<Task> list = new List<Task>();
			MissionManager.pInstance.GetNextTask(mission, ref list);
			if (list.Count <= 0) {
				BTConsole.WriteLine("warning - MissionManager found no next task, unable to set active task");
				return;
			}

			MissionManagerDO.SetCurrentActiveTask(list[0].TaskID);
		}

		private static void MarkPrerequisiteMissionsComplete(Mission mission) {
			List<int> prerequisites = mission.MissionRule.GetPrerequisites<int>(PrerequisiteRequiredType.Mission);
			foreach (Mission prerequisiteMission in prerequisites
					.Select(prerequisite => MissionManager.pInstance.GetMission(prerequisite))
					.Where(prerequisiteMission => prerequisiteMission != null)) {
				prerequisiteMission.Completed = 1;
				MarkPrerequisiteMissionsComplete(prerequisiteMission);
			}
		}
	}
}