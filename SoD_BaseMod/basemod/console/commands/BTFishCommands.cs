using System.Collections.Generic;

namespace SoD_BaseMod.basemod.console.commands {
	public static class BTFishCommands {
		public static void Register() {
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Fish", "weight" },
					new BTFishWeightInput(),
					"sets the fish weight for the next fish",
					OnExecuteWeight
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Fish", "rank" },
					new BTFishRankInput(),
					"sets the fish rank for the next fish",
					OnExecuteFishRank
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Fish", "rodPower" },
					new BTFishRodPowerInput(),
					"sets the rodPower for the next fishing attempt",
					OnExecuteFishRodPower
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Fish", "poleID" },
					new BTFishPoleIDInput(),
					"sets the fishing pole id for the next fishing attempt",
					OnExecuteFishPoleID
			));
		}

		private static void OnExecuteWeight(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTFishWeightInput) input;
			FishingZone._CheatFishWeight = cmdInput.weight;
			BTConsole.WriteLine("Next fish will have weight: " + cmdInput.weight);
		}

		private class BTFishWeightInput : BTConsoleCommand.BTCommandInput {
			public float weight;

			private void SetWeight(object weight, bool isPresent) {
				this.weight = (float) weight;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"weight",
								false,
								"weight of the fish, must be > 0",
								SetWeight,
								typeof(float)
						)
				};
			}
		}

		private static void OnExecuteFishRank(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTFishRankInput) input;
			FishingZone._CheatFishRank = cmdInput.rank;
			BTConsole.WriteLine("Next fish will have rank: " + cmdInput.rank);
		}

		private class BTFishRankInput : BTConsoleCommand.BTCommandInput {
			public int rank;

			private void SetRank(object rank, bool isPresent) {
				this.rank = (int) rank;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"rank",
								false,
								"rank of the fish",
								SetRank,
								typeof(int)
						)
				};
			}
		}

		private static void OnExecuteFishRodPower(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTFishRodPowerInput) input;
			FishingZone._CheatRodPower = cmdInput.rodPower;
			BTConsole.WriteLine("Next fishing attempt will have rodPower: " + cmdInput.rodPower);
		}

		private class BTFishRodPowerInput : BTConsoleCommand.BTCommandInput {
			public float rodPower;

			private void SetRodPower(object rodPower, bool isPresent) {
				this.rodPower = (float) rodPower;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"rodPower",
								false,
								"rodPower for the next fishing attempt",
								SetRodPower,
								typeof(float)
						)
				};
			}
		}
		
		private static void OnExecuteFishPoleID(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTFishPoleIDInput) input;
			FishingZone._CheatPoleID = cmdInput.poleID;
			BTConsole.WriteLine("Next fishing attempt will have poleID: " + cmdInput.poleID);
		}

		private class BTFishPoleIDInput : BTConsoleCommand.BTCommandInput {
			public int poleID;

			private void SetPoleID(object poleID, bool isPresent) {
				this.poleID = (int) poleID;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"poleID",
								false,
								"fishing pole id for the next fishing attempt",
								SetPoleID,
								typeof(int)
						)
				};
			}
		}
	}
}