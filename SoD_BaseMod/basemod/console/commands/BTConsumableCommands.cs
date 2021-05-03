using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SoD_BaseMod.basemod.console.commands {
	public static class BTConsumableCommands {
		public static void Register() {
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Consumable", "list" },
					new BTNoArgsInput(),
					"prints all available consumables",
					OnExecuteConsumableList
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Consumable", "add" },
					new BTConsumableAddInput(),
					"adds a consumable to the game",
					OnExecuteConsumableAdd
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Consumable", "chart" },
					new BTConsumableChartInput(),
					"shows/hides the probability charts of consumables",
					OnExecuteConsumableChart
			));
		}

		private static void OnExecuteConsumableList(BTConsoleCommand.BTCommandInput input) {
			if (!ConsumableData.pIsReady) {
				BTConsole.WriteLine("Can't print - ConsumableData is not ready");
				return;
			}

			GameObject gameObject = GameObject.Find("PfUiConsumable");
			if (gameObject == null) {
				BTConsole.WriteLine("Can't print - PfUiConsumable not found");
				return;
			}

			var component = gameObject.GetComponent<UiConsumable>();
			ConsumableType consumableType = ConsumableData.GetConsumableTypeByGame(component.pGameName, "Game");
			if (consumableType == null) {
				BTConsole.WriteLine("Can't print - consumableType not found");
				return;
			}

			BTConsole.WriteLine("Consumables:");
			foreach (Consumable consumable in consumableType.Consumables) {
				BTConsole.WriteLine("\t" + consumable.name);
			}
		}

		private static void OnExecuteConsumableAdd(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTConsumableAddInput) input;
			if (!ConsumableData.pIsReady) {
				BTConsole.WriteLine("Can't add - ConsumableData is not ready");
				return;
			}

			GameObject gameObject = GameObject.Find("PfUiConsumable");
			if (gameObject == null) {
				BTConsole.WriteLine("Can't add - PfUiConsumable not found");
				return;
			}

			var component = gameObject.GetComponent<UiConsumable>();
			ConsumableType consumableType = ConsumableData.GetConsumableTypeByGame(component.pGameName, "Game");
			if (consumableType == null) {
				BTConsole.WriteLine("Can't add - consumableType not found");
				return;
			}

			foreach (Consumable consumable in consumableType.Consumables) {
				if (string.Equals(consumable.name, cmdInput.consumable, StringComparison.OrdinalIgnoreCase)) {
					BTConsole.WriteLine("Added consumable: " + consumable.name);
					component.RegisterConsumable(consumable);
					return;
				}
			}

			BTConsole.WriteLine("Can't add - consumable of name '" + cmdInput.consumable + "' not found");
		}

		private class BTConsumableAddInput : BTConsoleCommand.BTCommandInput {
			public string consumable;

			private void SetConsumable(object consumable, bool isPresent) {
				this.consumable = (string) consumable;
			}

			protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"consumable",
								false,
								"name of the consumable to add",
								SetConsumable,
								typeof(string)
						)
				};
			}
		}

		private static void OnExecuteConsumableChart(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTConsumableChartInput) input;
			var levelManager = Object.FindObjectOfType<LevelManager>();
			if (levelManager == null) {
				BTConsole.WriteLine("Can't show/hide chart - LevelManager not found");
				return;
			}

			string showString = cmdInput.show ? "shown" : "hidden";
			if (cmdInput.show == levelManager._ShowProbabilityChart) {
				BTConsole.WriteLine("Probability charts are already " + showString);
			} else {
				levelManager._ShowProbabilityChart = cmdInput.show;
				BTConsole.WriteLine("Probability charts are now " + showString);
			}
		}

		private class BTConsumableChartInput : BTConsoleCommand.BTCommandInput {
			public bool show;

			private void SetShow(object show, bool isPresent) {
				this.show = (bool) show;
			}

			protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"show",
								false,
								"show/hide probability charts",
								SetShow,
								typeof(bool)
						)
				};
			}
		}
	}
}