using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SoD_BaseMod.console {
	public static class FullItemDumpHandler {
		private class ItemDataRow {
			public int? itemState_ID;
			public ItemStateCriteriaType? itemState_rule_criteria;
			public ItemStateCriteria itemState_rule_criteriaObject;
			public StateTransition? itemState_rule_completionAction;
			public int? itemState_order;

			public ItemRarity? itemRarity;

			public int? possibleStats_stats_statsID;
			public int? possibleStats_stats_rangeMap_startRange;
			public int? possibleStats_stats_rangeMap_endRange;
			
			public ItemTier? statsMap_itemTier;
			public int? statsMap_itemStats_ID;
			public string statsMap_itemStats_Name;
			public string statsMap_itemStats_Value;

			public int? itemSale_rewardItemID;
			public int? itemSale_quantity;
			
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

			public int? cost;
			public int? cashCost;
			public int? creativePoints;
			public string description;
			public string iconName;
			public int? inventoryMax;
			public int? itemID;
			public string itemName;
			public string itemNamePlural;
			public string geometry2;

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
						"itemState.rule.criteria",
						"itemState.rule.criteriaData",
						"itemState.rule.completionAction",
						"itemState.order",
						"itemRarity",
						"possibleStats.stats.statsID",
						"possibleStats.stats.rangeMap.startRange",
						"possibleStats.stats.rangeMap.endRange",
						"statsMap.itemTier",
						"statsMap.itemStats.ID",
						"statsMap.itemStats.Name",
						"statsMap.itemStats.Value",
						"itemSale.rewardItemID",
						"itemSale.quantity",
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
						"cost",
						"cashCost",
						"creativePoints",
						"description",
						"iconName",
						"inventoryMax",
						"itemID",
						"itemName",
						"itemNamePlural",
						"geometry2",
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
				resultBuilder.Append(itemState_rule_criteria).Append("<COL_SEPARATOR>");

				switch (itemState_rule_criteria) {
					case ItemStateCriteriaType.Length: {
						var len = (ItemStateCriteriaLength) itemState_rule_criteriaObject;
						resultBuilder.Append("Period: " + len.Period);
						break;
					}
					case ItemStateCriteriaType.ConsumableItem: {
						var item = (ItemStateCriteriaConsumable) itemState_rule_criteriaObject;
						resultBuilder.Append("Amount: " + item.Amount + " | itemID: " + item.ItemID);
						break;
					}
					case ItemStateCriteriaType.ReplenishableItem: {
						resultBuilder.Append("--unused--");
						break;
					}
					case ItemStateCriteriaType.SpeedUpItem: {
						var item = (ItemStateCriteriaSpeedUpItem) itemState_rule_criteriaObject;
						resultBuilder.Append("itemID: " + item.ItemID);
						break;
					}
					case ItemStateCriteriaType.StateExpiry: {
						var expiry = (ItemStateCriteriaExpiry) itemState_rule_criteriaObject;
						resultBuilder.Append("period: " + expiry.Period + " | type: " + expiry.Type);
						break;
					}
					case null:
						break;
					default:
						Debug.LogError("received unexpected criteria type: " + itemState_rule_criteria);
						throw new ArgumentOutOfRangeException();
				}

				resultBuilder.Append("<COL_SEPARATOR>");

				resultBuilder.Append(itemState_rule_completionAction).Append("<COL_SEPARATOR>");
				resultBuilder.Append(itemState_order).Append("<COL_SEPARATOR>");
				resultBuilder.Append(itemRarity).Append("<COL_SEPARATOR>");
				resultBuilder.Append(possibleStats_stats_statsID).Append("<COL_SEPARATOR>");
				resultBuilder.Append(possibleStats_stats_rangeMap_startRange).Append("<COL_SEPARATOR>");
				resultBuilder.Append(possibleStats_stats_rangeMap_endRange).Append("<COL_SEPARATOR>");
				resultBuilder.Append(statsMap_itemTier).Append("<COL_SEPARATOR>");
				resultBuilder.Append(statsMap_itemStats_ID).Append("<COL_SEPARATOR>");
				resultBuilder.Append(statsMap_itemStats_Name).Append("<COL_SEPARATOR>");
				resultBuilder.Append(statsMap_itemStats_Value).Append("<COL_SEPARATOR>");
				resultBuilder.Append(itemSale_rewardItemID).Append("<COL_SEPARATOR>");
				resultBuilder.Append(itemSale_quantity).Append("<COL_SEPARATOR>");
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
				resultBuilder.Append(cost).Append("<COL_SEPARATOR>");
				resultBuilder.Append(cashCost).Append("<COL_SEPARATOR>");
				resultBuilder.Append(creativePoints).Append("<COL_SEPARATOR>");
				resultBuilder.Append(description).Append("<COL_SEPARATOR>");
				resultBuilder.Append(iconName).Append("<COL_SEPARATOR>");
				resultBuilder.Append(inventoryMax).Append("<COL_SEPARATOR>");
				resultBuilder.Append(itemID).Append("<COL_SEPARATOR>");
				resultBuilder.Append(itemName).Append("<COL_SEPARATOR>");
				resultBuilder.Append(itemNamePlural).Append("<COL_SEPARATOR>");
				resultBuilder.Append(geometry2).Append("<COL_SEPARATOR>");
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
				itemState_rule_criteria = itemState_rule_criteria ?? other.itemState_rule_criteria;
				itemState_rule_criteriaObject = itemState_rule_criteriaObject ?? other.itemState_rule_criteriaObject;
				itemState_rule_completionAction = itemState_rule_completionAction ?? other.itemState_rule_completionAction;
				itemState_order = itemState_order ?? other.itemState_order;
				itemRarity = itemRarity ?? other.itemRarity;
				possibleStats_stats_statsID = possibleStats_stats_statsID ?? other.possibleStats_stats_statsID;
				possibleStats_stats_rangeMap_startRange = possibleStats_stats_rangeMap_startRange ?? other.possibleStats_stats_rangeMap_startRange;
				possibleStats_stats_rangeMap_endRange = possibleStats_stats_rangeMap_endRange ?? other.possibleStats_stats_rangeMap_endRange;
				statsMap_itemTier = statsMap_itemTier ?? other.statsMap_itemTier;
				statsMap_itemStats_ID = statsMap_itemStats_ID ?? other.statsMap_itemStats_ID;
				statsMap_itemStats_Name = statsMap_itemStats_Name ?? other.statsMap_itemStats_Name;
				statsMap_itemStats_Value = statsMap_itemStats_Value ?? other.statsMap_itemStats_Value;
				itemSale_rewardItemID = itemSale_rewardItemID ?? other.itemSale_rewardItemID;
				itemSale_quantity = itemSale_quantity ?? other.itemSale_quantity;
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
				cost = cost ?? other.cost;
				cashCost = cashCost ?? other.cashCost;
				creativePoints = creativePoints ?? other.creativePoints;
				description = description ?? other.description;
				iconName = iconName ?? other.iconName;
				inventoryMax = inventoryMax ?? other.inventoryMax;
				itemID = itemID ?? other.itemID;
				itemName = itemName ?? other.itemName;
				itemNamePlural = itemNamePlural ?? other.itemNamePlural;
				geometry2 = geometry2 ?? other.geometry2;
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

		public static void OnAllItemsLoaded(BTInventoryCommands.BTInventoryDumpItemDataProgress progress) {
			BTConsole.WriteLine("All requested items loaded!");

			var resultBuilder = new StringBuilder();
			resultBuilder.Append(string.Join("\t", ItemDataRow.GetRowHeader())).Append("\n");
			for (int i = progress.startID; i <= progress.endID; i++) {
				if (!progress.loadedItemData.ContainsKey(i)) {
					BTConsole.WriteLine("ERROR - itemData for ID: " + i + " was never loaded!");
					continue;
				}

				ItemData itemData = progress.loadedItemData[i];
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
						geometry2 = itemData.Geometry2,
						rankID = itemData.RankId,
						saleFactor = itemData.SaleFactor,
						uses = itemData.Uses,
						rewardTypeID = itemData.RewardTypeID,
						points = itemData.Points,
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
						firstStateRow.itemState_order = itemState?.Order;
						firstStateRow.itemState_rule_completionAction = itemState?.Rule?.CompletionAction?.Transition;

						List<ItemDataRow> ruleCriteria = itemState?.Rule?.Criterias
								?.Select(criterion => {
									if (criterion?.Type == ItemStateCriteriaType.ReplenishableItem) {
										BTConsole.WriteLine("Found 'ReplenishableItem' at id: " + i);
									}
									return new ItemDataRow {
											itemState_rule_criteria = criterion?.Type,
											itemState_rule_criteriaObject = criterion
									};
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
									categories_catID = category?.CategoryId
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
}