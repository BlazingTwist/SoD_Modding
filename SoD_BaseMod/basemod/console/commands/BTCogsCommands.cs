using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SoD_BaseMod.basemod.console.commands {
	public static class BTCogsCommands {
		public static void Register() {
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Cogs", "load" },
					new BTNoArgsInput(),
					"loads into the 'cogs' mini-game",
					OnExecuteCogsLoad
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Cogs", "unlock" },
					new BTNoArgsInput(),
					"unlocks all levels in the 'cogs' mini-game",
					OnExecuteCogsUnlock
			));
		}

		private static void OnExecuteCogsLoad(BTConsoleCommand.BTCommandInput input) {
			BTConsole.WriteLine("Loading Cogs mini-game...");
			RsResourceManager.LoadAssetFromBundle(GameConfig.GetKeyData("CogsAsset"), OnCogsBundleReady, typeof(GameObject));
			KAUICursorManager.SetDefaultCursor("Loading");
		}

		private static void OnCogsBundleReady(string inURL, RsResourceLoadEvent inEvent, float inProgress, object inObject, object inUserData) {
			switch (inEvent) {
				case RsResourceLoadEvent.COMPLETE: {
					if (inObject == null) {
						BTConsole.WriteLine("Cogs Asset not found in the bundle!");
					} else {
						BTConsole.WriteLine("Asset found, instantiating...");
						Object.Instantiate((GameObject) inObject);
					}

					KAUICursorManager.SetDefaultCursor("Arrow");
					break;
				}
				case RsResourceLoadEvent.ERROR:
					BTConsole.WriteLine("Failed to download the cogs asset bundle!");
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

		private static void OnExecuteCogsUnlock(BTConsoleCommand.BTCommandInput input) {
			BTConsole.WriteLine("Unlocking Cogs mini-game Levels");
			CogsLevelManager.UnlockAllLevels();
		}
	}
}