using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace SoD_BaseMod.console {
	public static class BTStoreCommands {
		public static void Register() {
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Store", "DumpData" },
					new BTStoreDumpDataInput(),
					"tries to load the specified StoreIDs and dumps them to the logFile (as errors)",
					OnExecuteStoreDumpData
			));
		}

		private static void OnExecuteStoreDumpData(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTStoreDumpDataInput) input;
			bool isReverse = cmdInput.idStart > cmdInput.idEnd;
			int startID = isReverse ? cmdInput.idEnd : cmdInput.idStart;
			int endID = isReverse ? cmdInput.idStart : cmdInput.idEnd;

			int[] storeIDs = new int[endID - startID + 1];
			for (int id = startID, index = 0; id <= endID; id++, index++) {
				storeIDs[index] = id;
			}

			new BTStoreLoader(OnCallbackStoreDumpData, storeIDs).RequestNextStore();
		}

		private delegate void OnStoreDataLoaded(List<ItemsInStoreData> storeData);

		private static void OnCallbackStoreDumpData(List<ItemsInStoreData> inStoreData) {
			if (inStoreData == null) {
				BTConsole.WriteLine("StoreDumpData received no StoreData.");
				return;
			}

			BTConsole.WriteLine("Dumping StoreData to logFile.");
			var builder = new StringBuilder();
			builder.Append("StoreDumpData result:\r\n")
					.Append("StoreID\tStoreName\tStoreDescription\tItemID\n");
			foreach (ItemsInStoreData storeData in inStoreData) {
				string storeID = storeData.ID != null ? storeData.ID.ToString() : "NULL";
				string storeName = storeData.StoreName != null ? Regex.Replace(storeData.StoreName, "(\r|\n|\t)", "\\$1") : "NULL";
				string storeDescription = storeData.Description != null ? Regex.Replace(storeData.Description, "(\r|\n|\t)", "\\$1") : "NULL";
				
				string prefix = storeID + "\t" + storeName + "\t" + storeDescription + "\t";

				if (storeData.Items == null) {
					builder.Append(prefix).Append("NULL\n");
				} else {
					foreach (ItemData itemData in storeData.Items) {
						builder.Append(prefix).Append(itemData.ItemID).Append("\n");
					}
				}
			}

			Debug.LogError(builder.ToString());
		}

		private class BTStoreDumpDataInput : BTConsoleCommand.BTCommandInput {
			public int idStart;
			public int idEnd;

			private void SetIDStart(object idStart, bool isPresent) {
				this.idStart = (int) idStart;
			}

			private void SetIDEnd(object idEnd, bool isPresent) {
				this.idEnd = (int) idEnd;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"idStart",
								false,
								"first ID to check",
								SetIDStart,
								typeof(int)
						),
						new BTConsoleCommand.BTConsoleArgument(
								"idEnd",
								false,
								"last ID to check",
								SetIDEnd,
								typeof(int)
						)
				};
			}
		}

		private class BTStoreLoader {
			private readonly OnStoreDataLoaded onDoneCallback;
			private readonly int[] storeIDs;
			private readonly Dictionary<int, ItemsInStoreData> loadedStores;

			private int storeProgress = 0;

			public BTStoreLoader(OnStoreDataLoaded callback, int[] storeIDs) {
				loadedStores = new Dictionary<int, ItemsInStoreData>();
				onDoneCallback = callback;
				this.storeIDs = storeIDs;
			}

			public void RequestNextStore() {
				if (storeProgress > storeIDs.Length) {
					return;
				}

				if (storeProgress == storeIDs.Length) {
					onDoneCallback(loadedStores.Values.ToList());
					return;
				}

				WsWebService.GetStore(new[] { storeIDs[storeProgress] }, OnItemsInStoreCallback, this);
			}

			private static void OnItemsInStoreCallback(WsServiceType inType, WsServiceEvent inEvent, float inProgress, object inObject, object inUserData) {
				if (inEvent != WsServiceEvent.COMPLETE && inEvent != WsServiceEvent.ERROR) {
					return;
				}

				if (!(inUserData is BTStoreLoader)) {
					BTConsole.WriteLine("Callback received no userData?!");
					return;
				}

				var loader = (BTStoreLoader) inUserData;
				int storeID = loader.storeIDs[loader.storeProgress];
				var response = inObject as GetStoreResponse;

				if (response?.Stores == null || response.Stores.Length == 0 || response.Stores[0].ID == null) {
					BTConsole.WriteLine("received no StoreData for id: " + storeID);
					loader.storeProgress++;
					loader.RequestNextStore();
				} else {
					BTConsole.WriteLine("received storeData for id: " + storeID);
					loader.storeProgress++;
					loader.loadedStores[storeID] = response.Stores[0];
					loader.RequestNextStore();
				}
			}
		}
	}
}