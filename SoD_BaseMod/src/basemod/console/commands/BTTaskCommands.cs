using System.Collections.Generic;

namespace SoD_BaseMod.console {
	public static class BTTaskCommands {
		public static void Register() {
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Task", "Complete" },
					new CompleteInput(),
					"Completes an active Task",
					OnExecuteComplete
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Task", "Console" },
					new ConsoleShowInput(),
					"shows / hides the Task Console",
					OnExecuteConsoleShow
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Task", "Details" },
					new TaskDetailsInput(),
					"prints details of the given task",
					OnExecuteTaskDetails
			));
		}

		private static void OnExecuteComplete(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (CompleteInput) input;
			if (MissionManager.pInstance == null) {
				BTConsole.WriteLine("error - MissionManager not ready");
				return;
			}

			Task task = MissionManager.pInstance.pActiveTasks.Find(t => t.TaskID == cmdInput.taskID);
			if (task == null) {
				BTConsole.WriteLine("error - No active Task found for ID: " + cmdInput.taskID);
				return;
			}

			task.Completed++;
			BTConsole.WriteLine("Task " + cmdInput.taskID + " completed.");
		}

		private class CompleteInput : BTConsoleCommand.BTCommandInput {
			public int taskID;

			private void SetTaskID(object taskID, bool isPresent) {
				this.taskID = (int) taskID;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"TaskID",
								false,
								"ID of the task",
								SetTaskID,
								typeof(int)
						)
				};
			}
		}

		private static void OnExecuteConsoleShow(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (ConsoleShowInput) input;
			var taskConsole = KAConsole.mObject.GetComponent<TaskStatusConsole>();
			if (taskConsole == null) {
				BTConsole.WriteLine("Unable to find Task Console.");
				return;
			}

			if (cmdInput.show) {
				taskConsole.Show();
				BTConsole.WriteLine("Task Console is shown.");
			} else {
				taskConsole.Close();
				BTConsole.WriteLine("Task Console is closed.");
			}
		}

		private class ConsoleShowInput : BTConsoleCommand.BTCommandInput {
			public bool show;

			private void SetShow(object show, bool isPresent) {
				this.show = (bool) show;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"show",
								false,
								"show/hide the Task Console",
								SetShow,
								typeof(bool)
						)
				};
			}
		}

		private static void OnExecuteTaskDetails(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (TaskDetailsInput) input;
			Task task = MissionManager.pInstance.GetTask(cmdInput.taskID);
			if (task == null) {
				BTConsole.WriteLine("Task for ID " + cmdInput.taskID + " is null!");
				return;
			}

			BTConsole.WriteLine(UtUtilities.SerializeToXml(task, true));
		}

		private class TaskDetailsInput : BTConsoleCommand.BTCommandInput {
			public int taskID;

			private void SetTaskID(object taskID, bool isPresent) {
				this.taskID = (int) taskID;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"taskID",
								false,
								"taskID to look up",
								SetTaskID,
								typeof(int)
						)
				};
			}
		}
	}
}