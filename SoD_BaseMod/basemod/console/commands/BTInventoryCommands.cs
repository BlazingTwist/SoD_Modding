using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

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

			protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
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

			protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
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
			bool isReverse = (cmdInput.idStart > cmdInput.idEnd);
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

			protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
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

			protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
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

			protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
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

			bool isReverse = (cmdInput.idStart > cmdInput.idEnd);
			int start = isReverse ? cmdInput.idEnd : cmdInput.idStart;
			int end = isReverse ? cmdInput.idStart : cmdInput.idEnd;

			ItemDataEventHandler callback = InventoryDumpItemDataCallback;
			var progress = new BTInventoryDumpItemDataProgress(callback, start, end, cmdInput.batchSize);
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

		private class BTInventoryDumpItemDataProgress {
			private readonly Dictionary<int, ItemData> loadedItemData;
			private readonly ItemDataEventHandler callback;
			private readonly int startID;
			private readonly int endID;
			private readonly int batchSize;

			private int batchProgress;

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
					OnAllItemsLoaded();
				} else if (batchProgress >= batchSize) {
					RequestNextBatch();
				}
			}

			private bool AllItemsLoaded() {
				return loadedItemData.Count >= (endID - startID + 1);
			}

			private void OnAllItemsLoaded() {
				BTConsole.WriteLine("All requested items loaded!");
				var resultBuilder = new StringBuilder();
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
				for (int i = startID; i <= endID; i++) {
					if (!loadedItemData.ContainsKey(i)) {
						BTConsole.WriteLine("ERROR - itemData for ID: " + i + " was never loaded!");
						continue;
					}

					ItemData itemData = loadedItemData[i];
					if (itemData == null) {
						continue;
					}

					resultBuilder.Append("\n");
					resultBuilder.Append(itemData.ItemID);
					resultBuilder.Append("\t").Append(ReplaceNewline(itemData.ItemName));
					resultBuilder.Append("\t").Append(ReplaceNewline(itemData.AssetName));
					resultBuilder.Append("\t").Append(ReplaceNewline(itemData.IconName));
					ItemDataRollover rollover = itemData.Rollover;
					if (rollover == null) {
						resultBuilder.Append("\t\t");
					} else {
						resultBuilder.Append("\t").Append(ReplaceNewline(rollover.DialogName));
						resultBuilder.Append("\t").Append(ReplaceNewline(rollover.Bundle));
					}

					resultBuilder.Append("\t").Append(ReplaceNewline(itemData.Description));
					resultBuilder.Append("\t").Append(ReplaceNewline(itemData.Geometry2));
					if (itemData.Texture == null) {
						resultBuilder.Append("\t").Append("0");
						resultBuilder.Append("\t\t\t\t");
					} else {
						resultBuilder.Append("\t").Append(itemData.Texture.Length);
						string textureNames = "";
						string textureTypeNames = "";
						string offsetX = "";
						string offsetY = "";
						for (int index = 0; index < itemData.Texture.Length; index++) {
							ItemDataTexture texture = itemData.Texture[index];
							if (index == 0) {
								textureNames += texture.TextureName;
								textureTypeNames += texture.TextureTypeName;
								offsetX += (texture.OffsetX == null) ? "null" : texture.OffsetX.Value.ToString(CultureInfo.InvariantCulture);
								offsetY += (texture.OffsetY == null) ? "null" : texture.OffsetY.Value.ToString(CultureInfo.InvariantCulture);
							} else {
								textureNames += (", " + texture.TextureName);
								textureTypeNames += (", " + texture.TextureTypeName);
								offsetX += (", " + ((texture.OffsetX == null) ? "null" : texture.OffsetX.Value.ToString(CultureInfo.InvariantCulture)));
								offsetY += (", " + ((texture.OffsetY == null) ? "null" : texture.OffsetY.Value.ToString(CultureInfo.InvariantCulture)));
							}
						}

						resultBuilder.Append("\t").Append(ReplaceNewline(textureNames));
						resultBuilder.Append("\t").Append(ReplaceNewline(textureTypeNames));
						resultBuilder.Append("\t").Append(ReplaceNewline(offsetX));
						resultBuilder.Append("\t").Append(ReplaceNewline(offsetY));
					}

					if (itemData.Category == null) {
						resultBuilder.Append("\t").Append("0");
						resultBuilder.Append("\t\t\t");
					} else {
						resultBuilder.Append("\t").Append(itemData.Category.Length);
						string categoryIDs = "";
						string categoryNames = "";
						string iconNames = "";
						for (int index = 0; index < itemData.Category.Length; index++) {
							ItemDataCategory category = itemData.Category[index];
							if (index == 0) {
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

					if (itemData.Relationship == null) {
						resultBuilder.Append("\t").Append("0");
						resultBuilder.Append("\t\t");
					} else {
						resultBuilder.Append("\t").Append(itemData.Relationship.Length);
						string relationshipTypes = "";
						string relationshipItemIDs = "";
						string relationshipWeights = "";
						string relationshipQuantities = "";
						for (int index = 0; index < itemData.Relationship.Length; index++) {
							ItemDataRelationship relationship = itemData.Relationship[index];
							if (index == 0) {
								relationshipTypes += relationship.Type;
								relationshipItemIDs += relationship.ItemId;
								relationshipWeights += relationship.Weight;
								relationshipQuantities += relationship.Quantity;
							} else {
								relationshipTypes += (", " + relationship.Type);
								relationshipItemIDs += (", " + relationship.ItemId);
								relationshipWeights += (", " + relationship.Weight);
								relationshipQuantities += (", " + relationship.Quantity);
							}
						}

						resultBuilder.Append("\t").Append(ReplaceNewline(relationshipTypes));
						resultBuilder.Append("\t").Append(ReplaceNewline(relationshipItemIDs));
						resultBuilder.Append("\t").Append(ReplaceNewline(relationshipWeights));
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
			return input == null ? "" : input.Replace("\n", "\\n");
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

			protected override List<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
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
						userItemData.ItemID = statsMap.ItemStatsMap.ItemID;
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