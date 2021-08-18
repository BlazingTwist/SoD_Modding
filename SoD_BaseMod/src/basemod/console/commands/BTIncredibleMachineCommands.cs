using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SoD_BaseMod.console {
	public static class BTIncredibleMachineCommands {
		public static void Register() {
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "IncredibleMachine", "load" },
					new BTNoArgsInput(),
					"loads into the 'IncredibleMachine' mini-game",
					OnExecuteIncredibleMachineLoad
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "IncredibleMachine", "unlock" },
					new BTNoArgsInput(),
					"unlocks all levels in the 'IncredibleMachine' mini-game",
					OnExecuteIncredibleMachineUnlock
			));
		}

		private static void OnExecuteIncredibleMachineLoad(BTConsoleCommand.BTCommandInput input) {
			BTConsole.WriteLine("Loading IncredibleMachine mini-game...");
			RsResourceManager.LoadAssetFromBundle(GameConfig.GetKeyData("IncredibleMachineAsset"), OnIncredibleMachineBundleReady, typeof(GameObject));
			KAUICursorManager.SetDefaultCursor("Loading");
		}

		private static void OnIncredibleMachineBundleReady(string inURL, RsResourceLoadEvent inEvent, float inProgress, object inObject,
				object inUserData) {
			switch (inEvent) {
				case RsResourceLoadEvent.COMPLETE: {
					if (inObject == null) {
						BTConsole.WriteLine("IncredibleMachine Asset not found in the bundle!");
					} else {
						BTConsole.WriteLine("Asset found, instantiating...");
						GameObject gameObject = Object.Instantiate((GameObject) inObject);
						gameObject.name = gameObject.name.Replace("(Clone)", "");
					}

					KAUICursorManager.SetDefaultCursor("Arrow");
					break;
				}
				case RsResourceLoadEvent.ERROR:
					BTConsole.WriteLine("Failed to download the IncredibleMachine asset bundle!");
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

		private static void OnExecuteIncredibleMachineUnlock(BTConsoleCommand.BTCommandInput input) {
			BTConsole.WriteLine("Unlocking IncredibleMachine mini-game Levels");
			CTLevelManager.UnlockAllLevels();
		}
	}
}