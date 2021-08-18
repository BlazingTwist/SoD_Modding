using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SoD_BaseMod.console {
	public static class BTAssetBundleCommands {
		private static readonly List<GameObject> loadedAssets = new List<GameObject>();

		public static void Register() {
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "AssetBundle", "list" },
					new BTLoadAssetInput(),
					"lists all the assets in a local AssetBundle",
					OnExecuteAssetBundleList
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "AssetBundle", "load", "asset" },
					new BTAssetBundleLoadInput(),
					"loads an asset from a local AssetBundle",
					OnExecuteLoadAsset
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "AssetBundle", "load", "scene" },
					new BTAssetBundleLoadSceneInput(),
					"loads a scene from a local AssetBundle",
					OnExecuteLoadScene
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "AssetBundle", "unload" },
					new BTNoArgsInput(),
					"unloads all loaded assetBundle assets",
					OnExecuteUnload
			));
		}

		private static void OnExecuteAssetBundleList(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTLoadAssetInput) input;
			AssetBundle assetBundle = AssetBundle.LoadFromFile(cmdInput.assetPath);
			try {
				if (assetBundle == null) {
					BTConsole.WriteLine("Unable to load assetBundle at path: " + cmdInput.assetPath);
					return;
				}

				BTConsole.WriteLine("AssetNames: {'" + string.Join("', '", assetBundle.GetAllAssetNames()) + "'}");
				BTConsole.WriteLine("ScenePaths: {'" + string.Join("', '", assetBundle.GetAllScenePaths()) + "'}");
			} catch (Exception e) {
				BTConsole.WriteLine("AssetBundle list failed - " + e);
			} finally {
				if (assetBundle != null) {
					assetBundle.Unload(true);
				}
			}
		}

		private class BTLoadAssetInput : BTConsoleCommand.BTCommandInput {
			public string assetPath;

			private void SetAssetPath(object assetPath, bool isPresent) {
				this.assetPath = (string) assetPath;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"assetPath",
								false,
								"full path to the assetBundle to load",
								SetAssetPath,
								typeof(string)
						)
				};
			}
		}

		private static void OnExecuteLoadAsset(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTAssetBundleLoadInput) input;
			AssetBundle assetBundle = AssetBundle.LoadFromFile(cmdInput.assetPath);
			try {
				if (assetBundle == null) {
					BTConsole.WriteLine("Unable to load assetBundle at path: " + cmdInput.assetPath);
					return;
				}

				var gameObject = assetBundle.LoadAsset<GameObject>(cmdInput.assetName);
				if (gameObject == null) {
					BTConsole.WriteLine("Unable to find GameObject (" + cmdInput.assetName + ") in loaded assetBundle!");
					return;
				}

				gameObject = Object.Instantiate(gameObject);
				loadedAssets.Add(gameObject);
				BTConsole.WriteLine("instantiated gameObject.");
			} catch (Exception e) {
				BTConsole.WriteLine("AssetBundle load asset failed - " + e);
			} finally {
				if (assetBundle != null) {
					assetBundle.Unload(false);
				}
			}
		}

		private static void OnExecuteLoadScene(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTAssetBundleLoadSceneInput) input;
			AssetBundle assetBundle = AssetBundle.LoadFromFile(cmdInput.assetPath);
			try {
				if (assetBundle == null) {
					BTConsole.WriteLine("Unable to load assetBundle at path: " + cmdInput.assetPath);
					return;
				}

				if (!assetBundle.isStreamedSceneAssetBundle) {
					BTConsole.WriteLine("Unable to load scene, bundle has incompatible format!");
					return;
				}

				string[] scenePaths = assetBundle.GetAllScenePaths();
				if (scenePaths.Length == 0) {
					BTConsole.WriteLine("Unable to load scene, no scenes found in the bundle!");
					return;
				}

				if (scenePaths.Length <= cmdInput.sceneIndex || cmdInput.sceneIndex < 0) {
					BTConsole.WriteLine("Index: " + cmdInput.sceneIndex + " | is out of bounds!");
					return;
				}

				BTConsole.WriteLine("loading scene: " + scenePaths[cmdInput.sceneIndex]);
				RsResourceManager.LoadLevel(Path.GetFileNameWithoutExtension(scenePaths[cmdInput.sceneIndex]));
			} catch (Exception e) {
				BTConsole.WriteLine("AssetBundle load asset failed - " + e);
			} finally {
				if (assetBundle != null) {
					assetBundle.Unload(false);
				}
			}
		}

		private static void OnExecuteUnload(BTConsoleCommand.BTCommandInput input) {
			try {
				BTConsole.WriteLine("Destroying " + loadedAssets.Count + " assets.");
				foreach (GameObject asset in loadedAssets) {
					Object.Destroy(asset);
				}

				BTConsole.WriteLine("done.");
			} catch (Exception e) {
				BTConsole.WriteLine("AssetBundle unload failed - " + e);
			} finally {
				loadedAssets.Clear();
			}
		}

		private class BTAssetBundleLoadInput : BTConsoleCommand.BTCommandInput {
			public string assetPath;
			public string assetName;

			private void SetAssetPath(object assetPath, bool isPresent) {
				this.assetPath = (string) assetPath;
			}

			private void SetAssetName(object assetName, bool isPresent) {
				this.assetName = (string) assetName;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"assetPath",
								false,
								"full path to the assetBundle to load",
								SetAssetPath,
								typeof(string)
						),
						new BTConsoleCommand.BTConsoleArgument(
								"assetName",
								false,
								"name of the asset to load, must be a GameObject",
								SetAssetName,
								typeof(string)
						)
				};
			}
		}

		private class BTAssetBundleLoadSceneInput : BTConsoleCommand.BTCommandInput {
			public string assetPath;
			public int sceneIndex;

			private void SetAssetPath(object assetPath, bool isPresent) {
				this.assetPath = (string) assetPath;
			}

			private void SetSceneIndex(object sceneIndex, bool isPresent) {
				this.sceneIndex = isPresent ? (int) sceneIndex : 0;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"assetPath",
								false,
								"full path to the assetBundle to load",
								SetAssetPath,
								typeof(string)
						),
						new BTConsoleCommand.BTConsoleArgument(
								"sceneIndex",
								true,
								"index of the scene to load, otherwise 0",
								SetSceneIndex,
								typeof(int)
						)
				};
			}
		}
	}
}