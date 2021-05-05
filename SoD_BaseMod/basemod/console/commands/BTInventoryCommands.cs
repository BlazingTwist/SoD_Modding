using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
					OnAllItemsLoaded();
				} else if (batchProgress >= batchSize) {
					RequestNextBatch();
				}
			}

			private bool AllItemsLoaded() {
				return loadedItemData.Count >= endID - startID + 1;
			}

			private class ItemDataRow {
				public int? itemState_ID;
				public string itemState_Name;
				public ItemStateCriteriaType? itemState_rule_criteria;
				public StateTransition? itemState_rule_completionAction;
				public int? itemState_order;

				public ItemRarity? itemRarity;

				public int? possibleStats_stats_statsID;
				public int? possibleStats_stats_rangeMap_tierID;
				public int? possibleStats_stats_rangeMap_startRange;
				public int? possibleStats_stats_rangeMap_endRange;

				public int? statsMap_itemID;
				public ItemTier? statsMap_itemTier;
				public int? statsMap_itemStats_ID;
				public string statsMap_itemStats_Name;
				public string statsMap_itemStats_Value;

				public int? itemSale_rewardItemID;
				public int? itemSale_quantity;

				public DeductibleType? blueprint_deductibles_type;
				public int? blueprint_deductibles_itemID;
				public int? blueprint_deductibles_quantity;
				public int? blueprint_ingredients_specID;
				public int? blueprint_ingredients_itemID;
				public int? blueprint_ingredients_categoryID;
				public ItemRarity? blueprint_ingredients_itemRarity;
				public ItemTier? blueprint_ingredients_tier;
				public int? blueprint_ingredients_quantity;
				public int? blueprint_outputs_specID;
				public int? blueprint_outputs_itemID;
				public int? blueprint_outputs_categoryID;
				public ItemRarity? blueprint_outputs_itemRarity;
				public ItemTier? blueprint_outputs_tier;
				public int? blueprint_outputs_quantity;

				public string assetName;

				public string attributes_key;
				public string attributes_value;

				public int? categories_catID;
				public string categories_name;

				public int? cost;
				public int? cashCost;
				public int? creativePoints;
				public string description;
				public string iconName;
				public int? inventoryMax;
				public int? itemID;
				public string itemName;
				public string itemNamePlural;
				public bool? locked;
				public string geometry2;

				public string rollover_dialogName;
				public string rollover_bundle;

				public int? rankID;

				public string relationship_type;
				public int? relationship_itemID;
				public int? relationship_weight;
				public int? relationship_quantity;

				public int? saleFactor;

				public string textures_name;
				public string textures_typeName;

				public int? uses;

				public DateTime? availabilities_startDate;
				public DateTime? availabilities_endDate;

				public int? rewardTypeID;
				public int? points;

				public static IEnumerable<string> GetRowHeader() {
					return new List<string> {
							"itemState.ID",
							"itemState.Name",
							"itemState.rule.criteria",
							"itemState.rule.completionAction",
							"itemState.order",
							"itemRarity",
							"possibleStats.stats.statsID",
							"possibleStats.stats.rangeMap.tierID",
							"possibleStats.stats.rangeMap.startRange",
							"possibleStats.stats.rangeMap.endRange",
							"statsMap.itemID",
							"statsMap.itemTier",
							"statsMap.itemStats.ID",
							"statsMap.itemStats.Name",
							"statsMap.itemStats.Value",
							"itemSale.rewardItemID",
							"itemSale.quantity",
							"blueprint.deductibles.type",
							"blueprint.deductibles.itemID",
							"blueprint.deductibles.quantity",
							"blueprint.ingredients.specID",
							"blueprint.ingredients.itemID",
							"blueprint.ingredients.categoryID",
							"blueprint.ingredients.itemRarity",
							"blueprint.ingredients.tier",
							"blueprint.ingredients.quantity",
							"blueprint.outputs.specID",
							"blueprint.outputs.itemID",
							"blueprint.outputs.categoryID",
							"blueprint.outputs.itemRarity",
							"blueprint.outputs.tier",
							"blueprint.outputs.quantity",
							"assetName",
							"attributes.key",
							"attributes.value",
							"categories.catID",
							"categories.name",
							"cost",
							"cashCost",
							"creativePoints",
							"description",
							"iconName",
							"inventoryMax",
							"itemID",
							"itemName",
							"itemNamePlural",
							"locked",
							"geometry2",
							"rollover.dialogName",
							"rollover.bundle",
							"rankID",
							"relationship.type",
							"relationship.itemID",
							"relationship.weight",
							"relationship.quantity",
							"saleFactor",
							"textures.name",
							"textures.typeName",
							"uses",
							"availabilities.startDate",
							"availabilities.endDate",
							"rewardTypeID",
							"points"
					};
				}

				public string GetRowString() {
					var resultBuilder = new StringBuilder();
					resultBuilder.Append(itemState_ID).Append("<COL_SEPARATOR>");
					resultBuilder.Append(itemState_Name).Append("<COL_SEPARATOR>");
					resultBuilder.Append(itemState_rule_criteria).Append("<COL_SEPARATOR>");
					resultBuilder.Append(itemState_rule_completionAction).Append("<COL_SEPARATOR>");
					resultBuilder.Append(itemState_order).Append("<COL_SEPARATOR>");
					resultBuilder.Append(itemRarity).Append("<COL_SEPARATOR>");
					resultBuilder.Append(possibleStats_stats_statsID).Append("<COL_SEPARATOR>");
					resultBuilder.Append(possibleStats_stats_rangeMap_tierID).Append("<COL_SEPARATOR>");
					resultBuilder.Append(possibleStats_stats_rangeMap_startRange).Append("<COL_SEPARATOR>");
					resultBuilder.Append(possibleStats_stats_rangeMap_endRange).Append("<COL_SEPARATOR>");
					resultBuilder.Append(statsMap_itemID).Append("<COL_SEPARATOR>");
					resultBuilder.Append(statsMap_itemTier).Append("<COL_SEPARATOR>");
					resultBuilder.Append(statsMap_itemStats_ID).Append("<COL_SEPARATOR>");
					resultBuilder.Append(statsMap_itemStats_Name).Append("<COL_SEPARATOR>");
					resultBuilder.Append(statsMap_itemStats_Value).Append("<COL_SEPARATOR>");
					resultBuilder.Append(itemSale_rewardItemID).Append("<COL_SEPARATOR>");
					resultBuilder.Append(itemSale_quantity).Append("<COL_SEPARATOR>");
					resultBuilder.Append(blueprint_deductibles_type).Append("<COL_SEPARATOR>");
					resultBuilder.Append(blueprint_deductibles_itemID).Append("<COL_SEPARATOR>");
					resultBuilder.Append(blueprint_deductibles_quantity).Append("<COL_SEPARATOR>");
					resultBuilder.Append(blueprint_ingredients_specID).Append("<COL_SEPARATOR>");
					resultBuilder.Append(blueprint_ingredients_itemID).Append("<COL_SEPARATOR>");
					resultBuilder.Append(blueprint_ingredients_categoryID).Append("<COL_SEPARATOR>");
					resultBuilder.Append(blueprint_ingredients_itemRarity).Append("<COL_SEPARATOR>");
					resultBuilder.Append(blueprint_ingredients_tier).Append("<COL_SEPARATOR>");
					resultBuilder.Append(blueprint_ingredients_quantity).Append("<COL_SEPARATOR>");
					resultBuilder.Append(blueprint_outputs_specID).Append("<COL_SEPARATOR>");
					resultBuilder.Append(blueprint_outputs_itemID).Append("<COL_SEPARATOR>");
					resultBuilder.Append(blueprint_outputs_categoryID).Append("<COL_SEPARATOR>");
					resultBuilder.Append(blueprint_outputs_itemRarity).Append("<COL_SEPARATOR>");
					resultBuilder.Append(blueprint_outputs_tier).Append("<COL_SEPARATOR>");
					resultBuilder.Append(blueprint_outputs_quantity).Append("<COL_SEPARATOR>");
					resultBuilder.Append(assetName).Append("<COL_SEPARATOR>");
					resultBuilder.Append(attributes_key).Append("<COL_SEPARATOR>");
					resultBuilder.Append(attributes_value).Append("<COL_SEPARATOR>");
					resultBuilder.Append(categories_catID).Append("<COL_SEPARATOR>");
					resultBuilder.Append(categories_name).Append("<COL_SEPARATOR>");
					resultBuilder.Append(cost).Append("<COL_SEPARATOR>");
					resultBuilder.Append(cashCost).Append("<COL_SEPARATOR>");
					resultBuilder.Append(creativePoints).Append("<COL_SEPARATOR>");
					resultBuilder.Append(description).Append("<COL_SEPARATOR>");
					resultBuilder.Append(iconName).Append("<COL_SEPARATOR>");
					resultBuilder.Append(inventoryMax).Append("<COL_SEPARATOR>");
					resultBuilder.Append(itemID).Append("<COL_SEPARATOR>");
					resultBuilder.Append(itemName).Append("<COL_SEPARATOR>");
					resultBuilder.Append(itemNamePlural).Append("<COL_SEPARATOR>");
					resultBuilder.Append(locked).Append("<COL_SEPARATOR>");
					resultBuilder.Append(geometry2).Append("<COL_SEPARATOR>");
					resultBuilder.Append(rollover_dialogName).Append("<COL_SEPARATOR>");
					resultBuilder.Append(rollover_bundle).Append("<COL_SEPARATOR>");
					resultBuilder.Append(rankID).Append("<COL_SEPARATOR>");
					resultBuilder.Append(relationship_type).Append("<COL_SEPARATOR>");
					resultBuilder.Append(relationship_itemID).Append("<COL_SEPARATOR>");
					resultBuilder.Append(relationship_weight).Append("<COL_SEPARATOR>");
					resultBuilder.Append(relationship_quantity).Append("<COL_SEPARATOR>");
					resultBuilder.Append(saleFactor).Append("<COL_SEPARATOR>");
					resultBuilder.Append(textures_name).Append("<COL_SEPARATOR>");
					resultBuilder.Append(textures_typeName).Append("<COL_SEPARATOR>");
					resultBuilder.Append(uses).Append("<COL_SEPARATOR>");
					resultBuilder.Append(availabilities_startDate).Append("<COL_SEPARATOR>");
					resultBuilder.Append(availabilities_endDate).Append("<COL_SEPARATOR>");
					resultBuilder.Append(rewardTypeID).Append("<COL_SEPARATOR>");
					resultBuilder.Append(points);
					return resultBuilder
							.ToString()
							.Replace("\t", "\\t")
							.Replace("\n", "\\n")
							.Replace("\r", "\\r")
							.Replace("<COL_SEPARATOR>", "\t");
				}

				public void Merge(ItemDataRow other) {
					itemState_ID = itemState_ID ?? other.itemState_ID;
					itemState_Name = itemState_Name ?? other.itemState_Name;
					itemState_rule_criteria = itemState_rule_criteria ?? other.itemState_rule_criteria;
					itemState_rule_completionAction = itemState_rule_completionAction ?? other.itemState_rule_completionAction;
					itemState_order = itemState_order ?? other.itemState_order;
					itemRarity = itemRarity ?? other.itemRarity;
					possibleStats_stats_statsID = possibleStats_stats_statsID ?? other.possibleStats_stats_statsID;
					possibleStats_stats_rangeMap_tierID = possibleStats_stats_rangeMap_tierID ?? other.possibleStats_stats_rangeMap_tierID;
					possibleStats_stats_rangeMap_startRange = possibleStats_stats_rangeMap_startRange ?? other.possibleStats_stats_rangeMap_startRange;
					possibleStats_stats_rangeMap_endRange = possibleStats_stats_rangeMap_endRange ?? other.possibleStats_stats_rangeMap_endRange;
					statsMap_itemID = statsMap_itemID ?? other.statsMap_itemID;
					statsMap_itemTier = statsMap_itemTier ?? other.statsMap_itemTier;
					statsMap_itemStats_ID = statsMap_itemStats_ID ?? other.statsMap_itemStats_ID;
					statsMap_itemStats_Name = statsMap_itemStats_Name ?? other.statsMap_itemStats_Name;
					statsMap_itemStats_Value = statsMap_itemStats_Value ?? other.statsMap_itemStats_Value;
					itemSale_rewardItemID = itemSale_rewardItemID ?? other.itemSale_rewardItemID;
					itemSale_quantity = itemSale_quantity ?? other.itemSale_quantity;
					blueprint_deductibles_type = blueprint_deductibles_type ?? other.blueprint_deductibles_type;
					blueprint_deductibles_itemID = blueprint_deductibles_itemID ?? other.blueprint_deductibles_itemID;
					blueprint_deductibles_quantity = blueprint_deductibles_quantity ?? other.blueprint_deductibles_quantity;
					blueprint_ingredients_specID = blueprint_ingredients_specID ?? other.blueprint_ingredients_specID;
					blueprint_ingredients_itemID = blueprint_ingredients_itemID ?? other.blueprint_ingredients_itemID;
					blueprint_ingredients_categoryID = blueprint_ingredients_categoryID ?? other.blueprint_ingredients_categoryID;
					blueprint_ingredients_itemRarity = blueprint_ingredients_itemRarity ?? other.blueprint_ingredients_itemRarity;
					blueprint_ingredients_tier = blueprint_ingredients_tier ?? other.blueprint_ingredients_tier;
					blueprint_ingredients_quantity = blueprint_ingredients_quantity ?? other.blueprint_ingredients_quantity;
					blueprint_outputs_specID = blueprint_outputs_specID ?? other.blueprint_outputs_specID;
					blueprint_outputs_itemID = blueprint_outputs_itemID ?? other.blueprint_outputs_itemID;
					blueprint_outputs_categoryID = blueprint_outputs_categoryID ?? other.blueprint_outputs_categoryID;
					blueprint_outputs_itemRarity = blueprint_outputs_itemRarity ?? other.blueprint_outputs_itemRarity;
					blueprint_outputs_tier = blueprint_outputs_tier ?? other.blueprint_outputs_tier;
					blueprint_outputs_quantity = blueprint_outputs_quantity ?? other.blueprint_outputs_quantity;
					assetName = assetName ?? other.assetName;
					attributes_key = attributes_key ?? other.attributes_key;
					attributes_value = attributes_value ?? other.attributes_value;
					categories_catID = categories_catID ?? other.categories_catID;
					categories_name = categories_name ?? other.categories_name;
					cost = cost ?? other.cost;
					cashCost = cashCost ?? other.cashCost;
					creativePoints = creativePoints ?? other.creativePoints;
					description = description ?? other.description;
					iconName = iconName ?? other.iconName;
					inventoryMax = inventoryMax ?? other.inventoryMax;
					itemID = itemID ?? other.itemID;
					itemName = itemName ?? other.itemName;
					itemNamePlural = itemNamePlural ?? other.itemNamePlural;
					locked = locked ?? other.locked;
					geometry2 = geometry2 ?? other.geometry2;
					rollover_dialogName = rollover_dialogName ?? other.rollover_dialogName;
					rollover_bundle = rollover_bundle ?? other.rollover_bundle;
					rankID = rankID ?? other.rankID;
					relationship_type = relationship_type ?? other.relationship_type;
					relationship_itemID = relationship_itemID ?? other.relationship_itemID;
					relationship_weight = relationship_weight ?? other.relationship_weight;
					relationship_quantity = relationship_quantity ?? other.relationship_quantity;
					saleFactor = saleFactor ?? other.saleFactor;
					textures_name = textures_name ?? other.textures_name;
					textures_typeName = textures_typeName ?? other.textures_typeName;
					uses = uses ?? other.uses;
					availabilities_startDate = availabilities_startDate ?? other.availabilities_startDate;
					availabilities_endDate = availabilities_endDate ?? other.availabilities_endDate;
					rewardTypeID = rewardTypeID ?? other.rewardTypeID;
					points = points ?? other.points;
				}
			}

			private void OnAllItemsLoaded() {
				BTConsole.WriteLine("All requested items loaded!");

				var resultBuilder = new StringBuilder();
				resultBuilder.Append(string.Join("\t", ItemDataRow.GetRowHeader())).Append("\n");
				for (int i = startID; i <= endID; i++) {
					if (!loadedItemData.ContainsKey(i)) {
						BTConsole.WriteLine("ERROR - itemData for ID: " + i + " was never loaded!");
						continue;
					}

					ItemData itemData = loadedItemData[i];
					if (itemData == null) {
						continue;
					}

					var mainItemRow = new ItemDataRow {
							itemRarity = itemData.ItemRarity,
							assetName = itemData.AssetName,
							cost = itemData.Cost,
							cashCost = itemData.CashCost,
							creativePoints = itemData.CreativePoints,
							description = itemData.Description,
							iconName = itemData.IconName,
							inventoryMax = itemData.InventoryMax,
							itemID = itemData.ItemID,
							itemName = itemData.ItemName,
							itemNamePlural = itemData.ItemNamePlural,
							locked = itemData.Locked,
							geometry2 = itemData.Geometry2,
							rankID = itemData.RankId,
							saleFactor = itemData.SaleFactor,
							uses = itemData.Uses,
							rewardTypeID = itemData.RewardTypeID,
							points = itemData.Points,
							rollover_dialogName = itemData.Rollover?.DialogName,
							rollover_bundle = itemData.Rollover?.Bundle
					};


					List<List<ItemDataRow>> segmentedItemDataRows = new List<List<ItemDataRow>>();
					List<ItemDataRow> activeRows;
					if (itemData.ItemStates != null) {
						activeRows = new List<ItemDataRow>();
						segmentedItemDataRows.Add(activeRows);
						foreach (ItemState itemState in itemData.ItemStates) {
							var firstStateRow = new ItemDataRow();
							activeRows.Add(firstStateRow);
							firstStateRow.itemState_ID = itemState?.ItemStateID;
							firstStateRow.itemState_Name = itemState?.Name;
							firstStateRow.itemState_order = itemState?.Order;
							firstStateRow.itemState_rule_completionAction = itemState?.Rule?.CompletionAction?.Transition;

							List<ItemDataRow> ruleCriteria = itemState?.Rule?.Criterias
									?.Select(criterion => new ItemDataRow {
											itemState_rule_criteria = criterion?.Type
									}).ToList();

							if (ruleCriteria != null && ruleCriteria.Count > 0) {
								firstStateRow.Merge(ruleCriteria[0]);
								ruleCriteria.RemoveAt(0);
								activeRows.AddRange(ruleCriteria);
							}
						}						
					}

					if (itemData.PossibleStatsMap?.Stats != null) {
						activeRows = new List<ItemDataRow>();
						segmentedItemDataRows.Add(activeRows);
						foreach (Stat stat in itemData.PossibleStatsMap.Stats) {
							var firstStatRow = new ItemDataRow();
							activeRows.Add(firstStatRow);
							firstStatRow.possibleStats_stats_statsID = stat?.ItemStatsID;

							List<ItemDataRow> rangeMaps = stat?.ItemStatsRangeMaps
									?.Select(rangeMap => new ItemDataRow {
											possibleStats_stats_rangeMap_tierID = rangeMap?.ItemTierID,
											possibleStats_stats_rangeMap_startRange = rangeMap?.StartRange,
											possibleStats_stats_rangeMap_endRange = rangeMap?.EndRange
									}).ToList();

							if (rangeMaps != null && rangeMaps.Count > 0) {
								firstStatRow.Merge(rangeMaps[0]);
								rangeMaps.RemoveAt(0);
								activeRows.AddRange(rangeMaps);
							}
						}						
					}

					if (itemData.ItemStatsMap != null) {
						mainItemRow.statsMap_itemID = itemData.ItemStatsMap.ItemID;
						mainItemRow.statsMap_itemTier = itemData.ItemStatsMap.ItemTier;
						activeRows = new List<ItemDataRow>();
						segmentedItemDataRows.Add(activeRows);
						activeRows.AddRange(itemData.ItemStatsMap.ItemStats
								?.Select(stat => new ItemDataRow {
										statsMap_itemStats_ID = stat?.ItemStatID,
										statsMap_itemStats_Name = stat?.Name,
										statsMap_itemStats_Value = stat?.Value
								}));
					}

					if (itemData.ItemSaleConfigs != null) {
						activeRows = new List<ItemDataRow>();
						segmentedItemDataRows.Add(activeRows);
						activeRows.AddRange(itemData.ItemSaleConfigs
								.Select(saleConfig => new ItemDataRow {
										itemSale_rewardItemID = saleConfig?.RewardItemID,
										itemSale_quantity = saleConfig?.Quantity
								}));						
					}

					if (itemData.BluePrint?.Deductibles != null) {
						activeRows = new List<ItemDataRow>();
						segmentedItemDataRows.Add(activeRows);
						activeRows.AddRange(itemData.BluePrint.Deductibles
								.Select(deductible => new ItemDataRow {
										blueprint_deductibles_type = deductible?.DeductibleType,
										blueprint_deductibles_itemID = deductible?.ItemID,
										blueprint_deductibles_quantity = deductible?.Quantity
								}));						
					}

					if (itemData.BluePrint?.Ingredients != null) {
						activeRows = new List<ItemDataRow>();
						segmentedItemDataRows.Add(activeRows);
						activeRows.AddRange(itemData.BluePrint.Ingredients
								.Select(ingredient => new ItemDataRow {
										blueprint_ingredients_specID = ingredient?.BluePrintSpecID,
										blueprint_ingredients_itemID = ingredient?.ItemID,
										blueprint_ingredients_categoryID = ingredient?.CategoryID,
										blueprint_ingredients_itemRarity = ingredient?.ItemRarity,
										blueprint_ingredients_tier = ingredient?.Tier,
										blueprint_ingredients_quantity = ingredient?.Quantity
								}));						
					}

					if (itemData.BluePrint?.Outputs != null) {
						activeRows = new List<ItemDataRow>();
						segmentedItemDataRows.Add(activeRows);
						activeRows.AddRange(itemData.BluePrint.Outputs
								.Select(output => new ItemDataRow {
										blueprint_outputs_specID = output?.BluePrintSpecID,
										blueprint_outputs_itemID = output?.ItemID,
										blueprint_outputs_categoryID = output?.CategoryID,
										blueprint_outputs_itemRarity = output?.ItemRarity,
										blueprint_outputs_tier = output?.Tier,
										blueprint_outputs_quantity = output?.Quantity
								}));						
					}

					if (itemData.Attribute != null) {
						activeRows = new List<ItemDataRow>();
						segmentedItemDataRows.Add(activeRows);
						activeRows.AddRange(itemData.Attribute
								.Select(attribute => new ItemDataRow {
										attributes_key = attribute?.Key,
										attributes_value = attribute?.Value
								}));						
					}

					if (itemData.Category != null) {
						activeRows = new List<ItemDataRow>();
						segmentedItemDataRows.Add(activeRows);
						activeRows.AddRange(itemData.Category
								.Select(category => new ItemDataRow {
										categories_catID = category?.CategoryId,
										categories_name = category?.CategoryName
								}));						
					}

					if (itemData.Relationship != null) {
						activeRows = new List<ItemDataRow>();
						segmentedItemDataRows.Add(activeRows);
						activeRows.AddRange(itemData.Relationship
								.Select(relationship => new ItemDataRow {
										relationship_type = relationship?.Type,
										relationship_itemID = relationship?.ItemId,
										relationship_weight = relationship?.Weight,
										relationship_quantity = relationship?.Quantity
								}));						
					}

					if (itemData.Texture != null) {
						activeRows = new List<ItemDataRow>();
						segmentedItemDataRows.Add(activeRows);
						activeRows.AddRange(itemData.Texture
								.Select(texture => new ItemDataRow {
										textures_name = texture?.TextureName,
										textures_typeName = texture?.TextureTypeName
								}));						
					}

					if (itemData.Availability != null) {
						activeRows = new List<ItemDataRow>();
						segmentedItemDataRows.Add(activeRows);
						activeRows.AddRange(itemData.Availability
								.Select(availability => new ItemDataRow {
										availabilities_startDate = availability?.StartDate,
										availabilities_endDate = availability?.EndDate
								}));						
					}

					int mergedRowCount = 1;
					List<ItemDataRow> mergedDataRows = new List<ItemDataRow> { mainItemRow };
					foreach (List<ItemDataRow> dataRowSegment in segmentedItemDataRows) {
						int count = dataRowSegment.Count;
						if (count == 0) {
							continue;
						}

						for (; mergedRowCount < count; mergedRowCount++) {
							mergedDataRows.Add(new ItemDataRow());
						}

						for (int rowIndex = 0; rowIndex < count; rowIndex++) {
							mergedDataRows[rowIndex].Merge(dataRowSegment[rowIndex]);
						}
					}

					foreach (ItemDataRow dataRow in mergedDataRows) {
						resultBuilder.Append(dataRow.GetRowString()).Append("\n");
					}
				}

				string resultString = resultBuilder.ToString();
				BTConsole.WriteLine("Finished building itemData, check log file");
				Debug.LogError(resultString);
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