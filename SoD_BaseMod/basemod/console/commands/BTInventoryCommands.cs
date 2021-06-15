using System;
using System.Collections.Generic;
using SoD_BaseMod.basemod.console.commands.itemdumphandlers;

namespace SoD_BaseMod.basemod.console.commands {
	public static class BTInventoryCommands {
		public static void Register() {
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Inventory", "clear" },
					new BTNoArgsInput(),
					"clears the avatar's inventory",
					OnExecuteInventoryClear
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Inventory", "save" },
					new BTNoArgsInput(),
					"saves the avatar's inventory",
					OnExecuteInventorySave
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Inventory", "remove" },
					new BTInventoryRemoveInput(),
					"removes an Item from the Inventory",
					OnExecuteInventoryRemove
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Inventory", "add" },
					new BTInventoryAddInput(),
					"adds an Item to the Inventory",
					OnExecuteInventoryAdd
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Inventory", "add", "range" },
					new BTInventoryAddRangeInput(),
					"adds one item of each itemID in the specified range to the inventory",
					OnExecuteInventoryAddRange
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Inventory", "add", "battle", "item" },
					new BTInventoryAddBattleItemInput(),
					"adds 'BattleItems' to the inventory",
					OnExecuteInventoryAddBattleItem
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Inventory", "ItemID" },
					new BTInventoryShowIDInput(),
					"shows / hides ItemIDs in the Inventory",
					OnExecuteInventoryItemID
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Inventory", "dumpItemData" },
					new BTInventoryDumpItemDataInput(),
					"tries to load the specified item IDs and dumps their data to the logFile (as errors)",
					OnExecuteInventoryDumpItemData
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Inventory", "dumpItemStates" },
					new BTInventoryDumpItemDataInput(),
					"tries to load the specified item IDs and dumps their itemStates to the logFile (as errors)",
					OnExecuteInventoryDumpItemStates
			));
		}

		private static void OnExecuteInventoryClear(BTConsoleCommand.BTCommandInput input) {
			CommonInventoryData.pInstance.Clear();
			BTConsole.WriteLine("Inventory cleared.");
		}

		private static void OnExecuteInventorySave(BTConsoleCommand.BTCommandInput input) {
			CommonInventoryData.pInstance.Save(
					InventorySaveCallback,
					null);
		}

		private static void OnExecuteInventoryRemove(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTInventoryRemoveInput) input;
			CommonInventoryData.pInstance.RemoveItem(
					cmdInput.itemID,
					cmdInput.updateServer,
					cmdInput.quantity);
			BTConsole.WriteLine("Removed ItemID: " + cmdInput.itemID + " | Quantity: " + cmdInput.quantity);
		}

		private class BTInventoryRemoveInput : BTConsoleCommand.BTCommandInput {
			public int itemID;
			public int quantity;
			public bool updateServer;

			private void SetItemID(object itemID, bool isPresent) {
				this.itemID = (int) itemID;
			}

			private void SetQuantity(object quantity, bool isPresent) {
				this.quantity = isPresent ? (int) quantity : 1;
			}

			private void SetUpdateServer(object updateServer, bool isPresent) {
				this.updateServer = (bool) updateServer;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"itemID",
								false,
								"ID of the item to remove",
								SetItemID,
								typeof(int)
						),
						new BTConsoleCommand.BTConsoleArgument(
								"quantity",
								true,
								"amount of items to remove, defaults to 1",
								SetQuantity,
								typeof(int)
						),
						new BTConsoleCommand.BTConsoleArgument(
								"updateServer",
								true,
								"whether to sync the item(s) with the server or not",
								SetUpdateServer,
								typeof(bool)
						)
				};
			}
		}

		private static void OnExecuteInventoryAdd(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTInventoryAddInput) input;
			CommonInventoryData.pInstance.AddItem(
					cmdInput.itemID,
					cmdInput.updateServer,
					InventoryItemDataCallback,
					null,
					cmdInput.quantity);
		}

		private class BTInventoryAddInput : BTConsoleCommand.BTCommandInput {
			public int itemID;
			public int quantity;
			public bool updateServer;

			private void SetItemID(object itemID, bool isPresent) {
				this.itemID = (int) itemID;
			}

			private void SetQuantity(object quantity, bool isPresent) {
				this.quantity = isPresent ? (int) quantity : 1;
			}

			private void SetUpdateServer(object updateServer, bool isPresent) {
				this.updateServer = (bool) updateServer;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"itemID",
								false,
								"ID of the item to add",
								SetItemID,
								typeof(int)
						),
						new BTConsoleCommand.BTConsoleArgument(
								"quantity",
								true,
								"amount of items to add, defaults to 1",
								SetQuantity,
								typeof(int)
						),
						new BTConsoleCommand.BTConsoleArgument(
								"updateServer",
								true,
								"whether to sync the item(s) with the server or not",
								SetUpdateServer,
								typeof(bool)
						)
				};
			}
		}

		private static void OnExecuteInventoryAddRange(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTInventoryAddRangeInput) input;
			bool isReverse = cmdInput.idStart > cmdInput.idEnd;
			int start = isReverse ? cmdInput.idEnd : cmdInput.idStart;
			int end = isReverse ? cmdInput.idStart : cmdInput.idEnd;
			for (int itemID = start; itemID <= end; itemID++) {
				CommonInventoryData.pInstance.AddItem(
						itemID,
						cmdInput.updateServer,
						InventoryItemDataCallback,
						null);
			}
		}

		private class BTInventoryAddRangeInput : BTConsoleCommand.BTCommandInput {
			public int idStart;
			public int idEnd;
			public bool updateServer;

			private void SetIDStart(object idStart, bool isPresent) {
				this.idStart = (int) idStart;
			}

			private void SetIDEnd(object idEnd, bool isPresent) {
				this.idEnd = isPresent ? (int) idEnd : 1;
			}

			private void SetUpdateServer(object updateServer, bool isPresent) {
				this.updateServer = (bool) updateServer;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"idStart",
								false,
								"first ID to add",
								SetIDStart,
								typeof(int)
						),
						new BTConsoleCommand.BTConsoleArgument(
								"idEnd",
								false,
								"last ID to add",
								SetIDEnd,
								typeof(int)
						),
						new BTConsoleCommand.BTConsoleArgument(
								"updateServer",
								true,
								"whether to sync the item(s) with the server or not",
								SetUpdateServer,
								typeof(bool)
						)
				};
			}
		}

		private static void OnExecuteInventoryAddBattleItem(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTInventoryAddBattleItemInput) input;
			var request = new AddBattleItemsRequest();
			var list = new List<BattleItemTierMap>();
			var battleItemTierMap = new BattleItemTierMap {
					ItemID = cmdInput.itemID,
					Quantity = cmdInput.quantity
			};
			if (cmdInput.itemTier != 0) {
				battleItemTierMap.Tier = (ItemTier) cmdInput.itemTier;
			}

			list.Add(battleItemTierMap);
			request.BattleItemTierMaps = list;
			WsWebService.BattleReadyItems(
					request,
					AddBattleItemCallback,
					null);
		}

		private class BTInventoryAddBattleItemInput : BTConsoleCommand.BTCommandInput {
			public int itemID;
			public int quantity;
			public int itemTier;

			private void SetItemID(object itemID, bool isPresent) {
				this.itemID = (int) itemID;
			}

			private void SetQuantity(object quantity, bool isPresent) {
				this.quantity = isPresent ? (int) quantity : 1;
			}

			private void SetItemTier(object itemTier, bool isPresent) {
				this.itemTier = (int) itemTier;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"itemID",
								false,
								"ID of the item to add",
								SetItemID,
								typeof(int)
						),
						new BTConsoleCommand.BTConsoleArgument(
								"quantity",
								true,
								"amount of items to add, defaults to 1",
								SetQuantity,
								typeof(int)
						),
						new BTConsoleCommand.BTConsoleArgument(
								"tier",
								true,
								"tier number of the item (1-4)",
								SetItemTier,
								typeof(int)
						)
				};
			}
		}

		private static void OnExecuteInventoryItemID(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTInventoryShowIDInput) input;
			if (cmdInput.show == null) {
				CommonInventoryData.pShowItemID = !CommonInventoryData.pShowItemID;
				string showString = CommonInventoryData.pShowItemID ? "shown" : "hidden";
				BTConsole.WriteLine("ItemIDs are now " + showString);
			} else {
				bool show = (bool) cmdInput.show;
				string showString = show ? "shown" : "hidden";
				if (CommonInventoryData.pShowItemID == show) {
					BTConsole.WriteLine("ItemIDs are already " + showString);
				} else {
					CommonInventoryData.pShowItemID = show;
					BTConsole.WriteLine("ItemIDs are now " + showString);
				}
			}
		}

		private class BTInventoryShowIDInput : BTConsoleCommand.BTCommandInput {
			public object show;

			private void SetShow(object show, bool isPresent) {
				this.show = isPresent ? show : null;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"show",
								true,
								"show/hide the ItemIDs - otherwise toggles",
								SetShow,
								typeof(bool)
						)
				};
			}
		}

		private static void OnExecuteInventoryDumpItemData(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTInventoryDumpItemDataInput) input;

			bool isReverse = cmdInput.idStart > cmdInput.idEnd;
			int start = isReverse ? cmdInput.idEnd : cmdInput.idStart;
			int end = isReverse ? cmdInput.idStart : cmdInput.idEnd;

			ItemDataEventHandler callback = InventoryDumpItemDataCallback;
			var progress = new BTInventoryDumpItemDataProgress(callback, FullItemDumpHandler.OnAllItemsLoaded, start, end, cmdInput.batchSize);
			progress.RequestNextBatch();
		}

		private static void OnExecuteInventoryDumpItemStates(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTInventoryDumpItemDataInput) input;
			
			bool isReverse = cmdInput.idStart > cmdInput.idEnd;
			int start = isReverse ? cmdInput.idEnd : cmdInput.idStart;
			int end = isReverse ? cmdInput.idStart : cmdInput.idEnd;
			
			ItemDataEventHandler callback = InventoryDumpItemDataCallback;
			var progress = new BTInventoryDumpItemDataProgress(callback, ItemStateDumpHandler.OnAllItemsLoaded, start, end, cmdInput.batchSize);
			progress.RequestNextBatch();
		}

		private static void InventoryDumpItemDataCallback(int itemID, ItemData itemData, object inUserData) {
			if (inUserData == null) {
				BTConsole.WriteLine("  ERROR - callback for ItemDump went missing, cannot dump!");
				return;
			}

			var progress = (BTInventoryDumpItemDataProgress) inUserData;
			if (itemData == null || itemData.ItemID != itemID) {
				BTConsole.WriteLine("  WARNING - itemID: " + itemID + " yielded no ItemData");
				progress.AddLoadedItem(itemID, null);
			} else {
				progress.AddLoadedItem(itemID, itemData);
			}
		}

		public delegate void OnAllItemsLoadedCallback(BTInventoryDumpItemDataProgress progress);

		public class BTInventoryDumpItemDataProgress {
			private readonly ItemDataEventHandler callback;
			private readonly OnAllItemsLoadedCallback onDone;
			private readonly int batchSize;

			public readonly Dictionary<int, ItemData> loadedItemData;
			public readonly int startID;
			public readonly int endID;

			private int batchProgress;

			public BTInventoryDumpItemDataProgress(ItemDataEventHandler callback, OnAllItemsLoadedCallback onDone, int startID, int endID, int batchSize) {
				loadedItemData = new Dictionary<int, ItemData>();
				this.callback = callback;
				this.onDone = onDone;
				this.startID = startID;
				this.endID = endID;
				this.batchSize = batchSize;
			}

			public void RequestNextBatch() {
				int loadedItemDataCount = loadedItemData.Count;
				int requestsRemainingCount = endID - startID + 1 - loadedItemDataCount;
				int batchStartID = startID + loadedItemDataCount;
				int batchEndID;
				if (requestsRemainingCount < batchSize) {
					batchEndID = endID;
				} else {
					batchEndID = batchStartID + batchSize - 1;
				}

				BTConsole.WriteLine(" requesting IDs " + batchStartID + " - " + batchEndID);
				batchProgress = 0;
				for (int i = batchStartID; i <= batchEndID; i++) {
					ItemData.Load(i, callback, this);
				}
			}

			public void AddLoadedItem(int itemID, ItemData itemData) {
				loadedItemData[itemID] = itemData;
				batchProgress++;
				if (AllItemsLoaded()) {
					onDone(this);
				} else if (batchProgress >= batchSize) {
					RequestNextBatch();
				}
			}

			private bool AllItemsLoaded() {
				return loadedItemData.Count >= endID - startID + 1;
			}
		}

		private class BTInventoryDumpItemDataInput : BTConsoleCommand.BTCommandInput {
			public int idStart;
			public int idEnd;
			public int batchSize;

			private void SetIDStart(object idStart, bool isPresent) {
				this.idStart = (int) idStart;
			}

			private void SetIDEnd(object idEnd, bool isPresent) {
				this.idEnd = (int) idEnd;
			}

			private void SetBatchSize(object batchSize, bool isPresent) {
				this.batchSize = isPresent ? (int) batchSize : 100;
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
						),
						new BTConsoleCommand.BTConsoleArgument(
								"batchSize",
								true,
								"amount of simultaneous server calls to send, defaults to 100",
								SetBatchSize,
								typeof(int)
						)
				};
			}
		}

		private static void InventorySaveCallback(bool success, object inUserData) {
			if (success) {
				BTConsole.WriteLine("CommonInventory Save successful");
				return;
			}

			BTConsole.WriteLine("CommonInventory Save failed");
		}

		private static void InventoryItemDataCallback(UserItemData dataItem, object inUserData) {
			string logMessage = dataItem != null
					? string.Concat("Item [", dataItem.Item.ItemName, "] added quantity = ", dataItem.Quantity)
					: "Inventory returned no itemData, request failed?";
			BTConsole.WriteLine(logMessage);
		}

		private static void AddBattleItemCallback(WsServiceType inType, WsServiceEvent inEvent, float inProgress, object inObject, object inUserData) {
			KAUICursorManager.SetDefaultCursor("Arrow");
			switch (inEvent) {
				case WsServiceEvent.ERROR:
					BTConsole.WriteLine("error - AddBattleItem request failed!");
					return;
				case WsServiceEvent.COMPLETE when inObject == null:
					BTConsole.WriteLine("error - AddBattleItem returned no data");
					return;
				case WsServiceEvent.COMPLETE: {
					var response = (AddBattleItemsResponse) inObject;
					if (response.InventoryItemStatsMaps.Count <= 0) {
						BTConsole.WriteLine("error - AddBattleItem returned no items");
						return;
					}

					foreach (InventoryItemStatsMap statsMap in response.InventoryItemStatsMaps) {
						var userItemData = new UserItemData {
								UserInventoryID = statsMap.CommonInventoryID,
								Item = statsMap.Item
						};
						userItemData.Item.ItemStatsMap = statsMap.ItemStatsMap;
						userItemData.ItemStats = statsMap.ItemStatsMap.ItemStats;
						userItemData.ItemTier = statsMap.ItemStatsMap.ItemTier;
						userItemData.Quantity = 1;
						CommonInventoryData.pInstance.AddToCategories(userItemData);
						BTConsole.WriteLine("AddBattleItem - added Item, ID: " + userItemData.ItemID);
					}

					break;
				}
				case WsServiceEvent.NONE:
					break;
				case WsServiceEvent.PROGRESS:
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(inEvent), inEvent, null);
			}
		}
	}
}