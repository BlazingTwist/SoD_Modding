using System.Collections.Generic;
using KnowledgeAdventure.Multiplayer.Utility;
using UnityEngine;

namespace SoD_BaseMod.basemod.console.commands {
	public static class BTDebugCommands {
		public static void Register() {
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Debug", "unload" },
					new BTNoArgsInput(),
					"unloads all unused assets",
					OnExecuteDebugUnload
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Debug", "fix" },
					new BTNoArgsInput(),
					"resets avatar-state and UI - tries to clear soft-locks",
					OnExecuteDebugFix
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Debug", "info" },
					new BTDebugInfoInput(),
					"shows/hides debug info ui",
					OnExecuteDebugInfo
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Debug", "delete", "player", "prefs" },
					new BTNoArgsInput(),
					"deletes ALL player prefs",
					OnExecuteDebugDeletePlayerPrefs
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Debug", "memDump" },
					new BTDebugMemDumpInput(),
					"Dumps the memory to a file",
					OnExecuteDebugMemDump
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Debug", "memWarn" },
					new BTNoArgsInput(),
					"triggers a low memory warning - forces memory cleanup, lowers rendered player-count...",
					OnExecuteDebugMemWarn
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Debug", "SceneObjects" },
					new BTNoArgsInput(),
					"toggles (delete/create) sceneObjects",
					OnExecuteDebugSceneObjects
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Debug", "particles" },
					new BTDebugParticlesInput(),
					"enables/disables all particleSystems",
					OnExecuteDebugParticles
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "DebugMask" },
					new BTDebugMaskInput(),
					"adds or clears a UtDebug._Mask",
					OnExecuteDebugMask
			));
		}

		private static void OnExecuteDebugUnload(BTConsoleCommand.BTCommandInput input) {
			BTConsole.WriteLine("unloading unused assets...");
			Resources.UnloadUnusedAssets();
		}

		private static void OnExecuteDebugFix(BTConsoleCommand.BTCommandInput input) {
			BTConsole.WriteLine("resetting avatar-state and ui");
			AvAvatar.pState = AvAvatarState.IDLE;
			AvAvatar.SetUIActive(true);
			if (KAUI._GlobalExclusiveUI != null) {
				KAUI.RemoveExclusive(KAUI._GlobalExclusiveUI);
				Object.Destroy(KAUI._GlobalExclusiveUI.gameObject);
			}
		}

		private static void OnExecuteDebugInfo(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTDebugInfoInput) input;
			GameObject debugUI = GameObject.Find("PfUiDebugInfo");
			if (debugUI == null) {
				BTConsole.WriteLine("error - Can't find debug UI");
				return;
			}

			debugUI.SendMessage("SetVisibility", cmdInput.show);
			BTConsole.WriteLine("Debug UI is now " + (cmdInput.show ? "shown" : "hidden"));
		}

		private class BTDebugInfoInput : BTConsoleCommand.BTCommandInput {
			public bool show;

			private void SetShow(object show, bool isPresent) {
				this.show = (bool) show;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"show",
								false,
								"show/hide debug info ui",
								SetShow,
								typeof(bool)
						)
				};
			}
		}

		private static void OnExecuteDebugDeletePlayerPrefs(BTConsoleCommand.BTCommandInput input) {
			PlayerPrefs.DeleteAll();
			BTConsole.WriteLine("Deleted all Player Prefs");
		}

		private static void OnExecuteDebugMemDump(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTDebugMemDumpInput) input;
			string outputFileName = MemDump.WriteToFile(cmdInput.fileName);
			BTConsole.WriteLine("Dumping Memory to file " + outputFileName);
		}

		private class BTDebugMemDumpInput : BTConsoleCommand.BTCommandInput {
			public string fileName;

			private void SetFileName(object fileName, bool isPresent) {
				this.fileName = isPresent ? "_" + (string) fileName : "";
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"fileName",
								true,
								"name of the file to dump to",
								SetFileName,
								typeof(string)
						)
				};
			}
		}

		private static void OnExecuteDebugMemWarn(BTConsoleCommand.BTCommandInput input) {
			MemoryManager.pInstance.gameObject.SendMessage("OnReceivedMemoryWarning", "dbgMsg");
			BTConsole.WriteLine("Triggered memory warning");
		}

		private static void OnExecuteDebugSceneObjects(BTConsoleCommand.BTCommandInput input) {
			GameObject sceneObjectsObject = GameObject.Find("SceneObjects");
			if (sceneObjectsObject != null) {
				Object.Destroy(sceneObjectsObject);
				BTConsole.WriteLine("Destroyed SceneObjects");
			} else {
				new GameObject("SceneObjects").AddComponent<SceneObjects>();
				BTConsole.WriteLine("Created SceneObjects");
			}
		}

		private static void OnExecuteDebugParticles(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTDebugParticlesInput) input;
			if (!(Resources.FindObjectsOfTypeAll(typeof(ParticleSystem)) is ParticleSystem[] allParticleSystems)) {
				BTConsole.WriteLine("Did not find any particle systems!");
				return;
			}

			foreach (ParticleSystem particleSystem in allParticleSystems) {
				if (cmdInput.enable) {
					particleSystem.Play();
				} else {
					particleSystem.Stop();
				}
			}
		}

		private class BTDebugParticlesInput : BTConsoleCommand.BTCommandInput {
			public bool enable;

			private void SetEnable(object enable, bool isPresent) {
				this.enable = (bool) enable;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"enable",
								false,
								"enable/disable all particle systems",
								SetEnable,
								typeof(bool)
						)
				};
			}
		}

		private static void OnExecuteDebugMask(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTDebugMaskInput) input;
			if (cmdInput.add) {
				UtDebug._Mask |= cmdInput.mask;
				BTConsole.WriteLine("DebugMask " + cmdInput.mask + " added.");
			} else {
				UtDebug._Mask &= ~cmdInput.mask;
				BTConsole.WriteLine("DebugMask " + cmdInput.mask + " cleared.");
			}

			Log._Mask = UtDebug._Mask;
			BTConsole.WriteLine("DebugMask is " + UtDebug._Mask);
		}

		private class BTDebugMaskInput : BTConsoleCommand.BTCommandInput {
			public uint mask;
			public bool add;

			private void SetMask(object mask, bool isPresent) {
				this.mask = (uint) mask;
			}

			private void SetAdd(object add, bool isPresent) {
				this.add = (bool) add;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"mask",
								false,
								"number of the render-mask to add/clear",
								SetMask,
								typeof(uint)
						),
						new BTConsoleCommand.BTConsoleArgument(
								"add/clear",
								false,
								"true = add | false = clear",
								SetAdd,
								typeof(bool)
						)
				};
			}
		}
	}
}