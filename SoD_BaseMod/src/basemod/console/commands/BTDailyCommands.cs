using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SoD_BaseMod.console {
	public static class BTDailyCommands {
		public static void Register() {
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "DailyBonus" },
					new BTDailyBonusInput(),
					"sets the dailyBonus acquired after the next restart",
					OnExecuteDailyBonus
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "DailyQuest", "show" },
					new BTNoArgsInput(),
					"tries opening the DailyQuest UI",
					OnExecuteDailyQuestShow
			));
		}

		private static void OnExecuteDailyBonus(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTDailyBonusInput) input;
			if (!ServerTime.pIsReady) {
				BTConsole.WriteLine("error - ServerTime did not start yet");
				return;
			}

			BTConsole.WriteLine("Requesting pair-data 1216...");
			PairData.Load(1216, OnPairDataReady, Mathf.Max(1, cmdInput.day), false, ParentData.pInstance.pUserInfo.UserID);
		}

		private static void OnPairDataReady(bool success, PairData inData, object inUserData) {
			int targetDay = (int) inUserData;
			inData.SetValue("LP", UtUtilities
					.GetPSTTimeFromUTC(ServerTime.pCurrentTime)
					.AddDays(targetDay >= 5 ? -2.0 : -1.0)
					.Date
					.ToString(UtUtilities.GetCultureInfo("en-US"))
			);

			inData.SetValue("LPC", targetDay.ToString());
			PairData.Save(1216, ParentData.pInstance.pUserInfo.UserID);
			BTConsole.WriteLine("Pair-data has been modified, reward '" + targetDay + "' will be awarded after the next restart");
		}

		private class BTDailyBonusInput : BTConsoleCommand.BTCommandInput {
			public int day;

			private void SetDay(object day, bool isPresent) {
				this.day = isPresent ? (int) day : 5;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"day",
								true,
								"target reward day, defaults to 5",
								SetDay,
								typeof(int)
						)
				};
			}
		}

		private static void OnExecuteDailyQuestShow(BTConsoleCommand.BTCommandInput input) {
			KAUICursorManager.SetDefaultCursor("Loading");
			AvAvatar.pState = AvAvatarState.PAUSED;
			AvAvatar.SetUIActive(false);
			BTConsole.WriteLine("loading UI bundle...");
			RsResourceManager.LoadAssetFromBundle("RS_DATA/PfUiDailyQuestDO.unity3d/PfUiDailyQuestDO", OnDailyQuestLoaded, typeof(GameObject));
		}

		private static void OnDailyQuestLoaded(string inURL, RsResourceLoadEvent inEvent, float inProgress, object inObject, object inUserData) {
			switch (inEvent) {
				case RsResourceLoadEvent.COMPLETE: {
					BTConsole.WriteLine("Bundle loaded, UI should open now.");
					AvAvatar.pState = AvAvatarState.PAUSED;
					AvAvatar.SetUIActive(false);
					UiDailyQuests.pMissionGroup = MissionManager.pInstance._DailyMissions[0]._GroupID;
					GameObject gameObject = Object.Instantiate((GameObject) inObject);
					gameObject.name = "PfUiDailyQuestDO";
					KAUICursorManager.SetDefaultCursor("Arrow");
					gameObject.GetComponent<UiDailyQuests>().pOnUiClosed = OnUiClosed;
					break;
				}
				case RsResourceLoadEvent.ERROR:
					BTConsole.WriteLine("error - unknown error from the webserver, can't open ui");
					AvAvatar.pState = AvAvatarState.IDLE;
					AvAvatar.SetUIActive(true);
					KAUICursorManager.SetDefaultCursor("Arrow");
					break;
				case RsResourceLoadEvent.NONE:
					break;
				case RsResourceLoadEvent.PROGRESS:
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(inEvent), inEvent, null);
			}
		}

		private static void OnUiClosed() {
			AvAvatar.pState = AvAvatarState.IDLE;
			AvAvatar.SetUIActive(true);
		}
	}
}