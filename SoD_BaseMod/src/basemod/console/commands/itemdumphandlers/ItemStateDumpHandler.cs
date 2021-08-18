using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SoD_BaseMod.console {
	public class ItemStateDumpHandler {
		private class ItemDataRow {
			public int? itemID;
			public string stateName;
			public int? itemStateID;
			public int? consumableItemID;
			public int? consumableItemAmount;
			public int? stateDuration;
			public int? speedUpItemID;
			public int? speedUpEndStateID;
			public int? expireDuration;
			public int? expireEndStateID;
			public bool? canDestroyOnHarvest;
			public StateTransition? completionAction;
			public int? order;

			public static IEnumerable<string> GetRowHeader() {
				return new List<string> {
						"itemID",
						"stateName",
						"itemStateID",
						"consumableItemID",
						"consumableItemAmount",
						"stateDuration",
						"speedUpItemID",
						"speedUpEndStateID",
						"expireDuration",
						"expireEndStateID",
						"canDestroyOnHarvest",
						"completionAction",
						"order"
				};
			}

			public string GetRowString() {
				var resultBuilder = new StringBuilder();
				resultBuilder.Append(itemID).Append("<COL_SEPARATOR>");
				resultBuilder.Append(stateName).Append("<COL_SEPARATOR>");
				resultBuilder.Append(itemStateID).Append("<COL_SEPARATOR>");
				resultBuilder.Append(consumableItemID).Append("<COL_SEPARATOR>");
				resultBuilder.Append(consumableItemAmount).Append("<COL_SEPARATOR>");
				resultBuilder.Append(stateDuration).Append("<COL_SEPARATOR>");
				resultBuilder.Append(speedUpItemID).Append("<COL_SEPARATOR>");
				resultBuilder.Append(speedUpEndStateID).Append("<COL_SEPARATOR>");
				resultBuilder.Append(expireDuration).Append("<COL_SEPARATOR>");
				resultBuilder.Append(expireEndStateID).Append("<COL_SEPARATOR>");
				resultBuilder.Append(canDestroyOnHarvest).Append("<COL_SEPARATOR>");
				resultBuilder.Append(completionAction?.ToString()).Append("<COL_SEPARATOR>");
				resultBuilder.Append(order);
				return resultBuilder
						.ToString()
						.Replace("\t", "\\t")
						.Replace("\n", "\\n")
						.Replace("\r", "\\r")
						.Replace("<COL_SEPARATOR>", "\t");
			}

			public void Merge(ItemDataRow other) {
				itemID = itemID ?? other.itemID;
				stateName = stateName ?? other.stateName;
				itemStateID = itemStateID ?? other.itemStateID;
				consumableItemID = consumableItemID ?? other.consumableItemID;
				consumableItemAmount = consumableItemAmount ?? other.consumableItemAmount;
				stateDuration = stateDuration ?? other.stateDuration;
				speedUpItemID = speedUpItemID ?? other.speedUpItemID;
				speedUpEndStateID = speedUpEndStateID ?? other.speedUpEndStateID;
				expireDuration = expireDuration ?? other.expireDuration;
				expireEndStateID = expireEndStateID ?? other.expireEndStateID;
				canDestroyOnHarvest = canDestroyOnHarvest ?? other.canDestroyOnHarvest;
				completionAction = completionAction ?? other.completionAction;
				order = order ?? other.order;
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
						itemID = itemData.ItemID
				};

				List<List<ItemDataRow>> segmentedItemDataRows = new List<List<ItemDataRow>>();
				if (itemData.ItemStates != null) {
					List<ItemDataRow> activeRows = new List<ItemDataRow>();
					segmentedItemDataRows.Add(activeRows);
					foreach (ItemState itemState in itemData.ItemStates) {
						var firstStateRow = new ItemDataRow();
						activeRows.Add(firstStateRow);
						firstStateRow.itemStateID = itemState?.ItemStateID;
						firstStateRow.stateName = itemState?.Name;
						firstStateRow.canDestroyOnHarvest = itemState?.Rule?.CompletionAction?.Transition == StateTransition.Deletion;
						firstStateRow.completionAction = itemState?.Rule?.CompletionAction?.Transition;
						firstStateRow.order = itemState?.Order;

						List<ItemDataRow> consumables = new List<ItemDataRow>();
						if (itemState?.Rule?.Criterias != null) {
							foreach (ItemStateCriteria criterion in itemState.Rule.Criterias.Where(criterion => criterion != null)) {
								// ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
								switch (criterion.Type) {
									case ItemStateCriteriaType.Length: {
										var realType = (ItemStateCriteriaLength) criterion;
										firstStateRow.stateDuration = realType.Period;
										break;
									}
									case ItemStateCriteriaType.ConsumableItem: {
										var realType = (ItemStateCriteriaConsumable) criterion;
										consumables.Add(new ItemDataRow {
												consumableItemID = realType.ItemID,
												consumableItemAmount = realType.Amount
										});
										break;
									}
									case ItemStateCriteriaType.SpeedUpItem: {
										var realType = (ItemStateCriteriaSpeedUpItem) criterion;
										firstStateRow.speedUpItemID = realType.ItemID;
										firstStateRow.speedUpEndStateID = realType.EndStateID;
										break;
									}
									case ItemStateCriteriaType.StateExpiry: {
										var realType = (ItemStateCriteriaExpiry) criterion;
										firstStateRow.expireDuration = realType.Period;
										firstStateRow.expireEndStateID = realType.EndStateID;
										break;
									}
								}
							}
						}

						if (consumables.Count > 0) {
							firstStateRow.Merge(consumables[0]);
							consumables.RemoveAt(0);
							activeRows.AddRange(consumables);
						}
					}
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