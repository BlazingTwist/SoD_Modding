using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SoD_BaseMod.config.toggleScript;
using SoD_BaseMod.extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace SoD_BaseMod.console {
	public static class BTLevelCommands {
		public static void Register() {
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Level", "load" },
					new BTLevelLoadInput(),
					"load a specified level",
					OnExecuteLevelLoad
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Level", "get" },
					new BTNoArgsInput(),
					"prints the current level name (name of the active scene)",
					OnExecuteLevelGet
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Level", "set", "lighting" },
					new BTLevelSetLightingInput(),
					"set the lighting in the current scene",
					OnExecuteLevelSetLighting
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "level", "runToggleScript" },
					new BTLevelRunToggleScriptInput(),
					"execute a toggleScript (enables/disables objects in the scene)",
					OnExecuteRunToggleScript
			));
		}

		private static void OnExecuteLevelLoad(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTLevelLoadInput) input;
			BTConsole.WriteLine("Loading level: " + cmdInput.level);
			RsResourceManager.LoadLevel(cmdInput.level);
		}

		private class BTLevelLoadInput : BTConsoleCommand.BTCommandInput {
			public string level;

			private void SetLevel(object level, bool isPresent) {
				this.level = (string) level;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"level",
								false,
								"name of the level to load",
								SetLevel,
								typeof(string)
						)
				};
			}
		}

		private static void OnExecuteLevelGet(BTConsoleCommand.BTCommandInput input) {
			BTConsole.WriteLine("Current Level: " + RsResourceManager.pCurrentLevel);
		}

		private static void OnExecuteLevelSetLighting(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTLevelSetLightingInput) input;
			StringBuilder outputBuilder = new StringBuilder();

			outputBuilder.AppendLine($"Current Scene: {SceneManager.GetActiveScene().name}");

			// Ambient Light
			{
				outputBuilder.AppendLine("Ambient Light disabled, previously was:");
				outputBuilder.AppendDictionary("\t", new Dictionary<object, object> {
						{ nameof(RenderSettings.ambientIntensity), RenderSettings.ambientIntensity },
						{ nameof(RenderSettings.ambientLight), RenderSettings.ambientLight },
						{ nameof(RenderSettings.ambientMode), RenderSettings.ambientMode },
						{ nameof(RenderSettings.ambientEquatorColor), RenderSettings.ambientEquatorColor },
						{ nameof(RenderSettings.ambientGroundColor), RenderSettings.ambientGroundColor },
						{ nameof(RenderSettings.ambientSkyColor), RenderSettings.ambientSkyColor },
				});

				RenderSettings.ambientLight = Color.black;
				RenderSettings.ambientEquatorColor = Color.black;
				RenderSettings.ambientGroundColor = Color.black;
				RenderSettings.ambientSkyColor = Color.black;
				RenderSettings.ambientIntensity = 0f;
			}

			// Scene Light sources
			{
				Light[] sceneLights = Object.FindObjectsOfType<Light>();
				outputBuilder.AppendLine($"Disabling {sceneLights.Length} lights in scene...");
				int counter = 0;
				foreach (Light sceneLight in sceneLights) {
					counter++;
					outputBuilder.AppendLine($"light #{counter}");
					outputBuilder.AppendDictionary("\t", new Dictionary<object, object> {
							{ nameof(sceneLight.transform.position), sceneLight.transform.position },
							{ nameof(sceneLight.color), sceneLight.color },
							{ nameof(sceneLight.intensity), sceneLight.intensity },
							{ nameof(sceneLight.range), sceneLight.range },
							{ nameof(sceneLight.type), sceneLight.type },
							{ nameof(sceneLight.enabled), sceneLight.enabled },
					});
					sceneLight.enabled = false;
				}
			}

			// Custom Lighting
			{
				outputBuilder.AppendLine("Creating new directional lightsource...");
				GameObject gameObject = new GameObject("CustomLightSource");
				Light lightComponent = gameObject.AddComponent<Light>();
				outputBuilder.AppendDictionary("\t", new Dictionary<object, object> {
						{ nameof(cmdInput.lightColor), cmdInput.lightColor },
						{ nameof(cmdInput.lightIntensity), cmdInput.lightIntensity },
						{ nameof(cmdInput.lightDirection), cmdInput.lightDirection },
				});
				lightComponent.color = cmdInput.lightColor;
				lightComponent.intensity = cmdInput.lightIntensity;
				lightComponent.type = LightType.Directional;
				lightComponent.enabled = true;
				lightComponent.transform.LookAt(lightComponent.transform.position + cmdInput.lightDirection);
			}

			outputBuilder.AppendLine("==========");
			BTConsole.WriteLine(outputBuilder.ToString());
		}

		private class BTLevelSetLightingInput : BTConsoleCommand.BTCommandInput {
			public Color lightColor;
			public float lightIntensity;
			public Vector3 lightDirection;

			private static Vector3? GetDirectionFromString(string direction) {
				if (string.IsNullOrWhiteSpace(direction)) {
					return null;
				}
				float[] vectorData = direction.Split(',')
						.Where(str => !string.IsNullOrWhiteSpace(str))
						.Select(float.Parse)
						.ToArray();
				if (vectorData.Length != 3) {
					return null;
				}
				return new Vector3(vectorData[0], vectorData[1], vectorData[2]);
			}

			private void SetLightColor(object color, bool isPresent) {
				lightColor = isPresent ? ColorExtensions.GetColorFromInt((int) color) : Color.white;
			}

			private void SetLightIntensity(object intensity, bool isPresent) {
				lightIntensity = isPresent ? (float) intensity : 1.0f;
			}

			private void SetLightDirection(object direction, bool isPresent) {
				Vector3 defaultValue = Vector3.down;
				lightDirection = isPresent
						? GetDirectionFromString((string) direction) ?? defaultValue
						: defaultValue;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"lightColor",
								true,
								"color to use for the sunlight, default is white, example: 0xFFFFFF is white",
								SetLightColor,
								typeof(int)
						),
						new BTConsoleCommand.BTConsoleArgument(
								"lightIntensity",
								true,
								"intensity of the sunlight, default is 1.0",
								SetLightIntensity,
								typeof(float)
						),
						new BTConsoleCommand.BTConsoleArgument(
								"lightDirection",
								true,
								"comma separated xyz direction of the sunlight, default is '0,-1,0' (straight down), example: '1.5,-1,2.7'",
								SetLightDirection,
								typeof(string)
						)
				};
			}
		}

		private static void OnExecuteRunToggleScript(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTLevelRunToggleScriptInput) input;
			List<BTToggleScriptEntry> toggleScript;
			try {
				toggleScript = BTConfigHolder.LoadToggleScript(cmdInput.scriptName);
			} catch (Exception e) {
				BTConsole.WriteLine("RunToggleScript failed due to an error while loading the toggleScript.\n"
						+ $"Error: {e.Message}");
				return;
			}
			StringBuilder outputBuilder = new StringBuilder();
			outputBuilder.AppendLine($"Executing toggleScript: {cmdInput.scriptName} in mode: {cmdInput.mode}");
			Dictionary<BTToggleScriptEntry, int> matches = toggleScript.ToDictionary(script => script, script => 0);
			int total = 0;
			foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>()) {
				BTToggleScriptEntry matchingEntry = toggleScript.FirstOrDefault(entry => entry.ObjectMatches(gameObject, cmdInput.verbose, cmdInput.accuracy));
				if (matchingEntry != null) {
					switch (cmdInput.mode) {
						case BTLevelRunToggleScriptInput.ToggleScriptMode.ENABLE:
							gameObject.SetActive(true);
							break;
						case BTLevelRunToggleScriptInput.ToggleScriptMode.DISABLE:
							gameObject.SetActive(false);
							break;
						case BTLevelRunToggleScriptInput.ToggleScriptMode.TOGGLE:
							gameObject.SetActive(!gameObject.activeSelf);
							break;
						default:
							throw new InvalidDataException($"Unknown toggleScript-mode: '{cmdInput.mode}'");
					}
					matches[matchingEntry]++;
					total++;
				}
			}

			foreach (BTToggleScriptEntry script in matches.Where(kvp => kvp.Value == 0).Select(kvp => kvp.Key)) {
				outputBuilder.AppendLine($"\tWarning: script-target '{script.Serialize()}' found 0 matches!");
			}
			outputBuilder.Append($"{cmdInput.mode}D {total} objects.");
			BTConsole.WriteLine(outputBuilder.ToString());
		}

		private class BTLevelRunToggleScriptInput : BTConsoleCommand.BTCommandInput {
			public enum ToggleScriptMode {
				ENABLE,
				DISABLE,
				TOGGLE
			}

			public string scriptName;
			public ToggleScriptMode mode;
			public bool verbose;
			public float? accuracy;

			private void SetScriptName(object scriptName, bool isPresent) {
				this.scriptName = (string) scriptName;
			}

			private void SetMode(object mode, bool isPresent) {
				this.mode = isPresent ? (ToggleScriptMode) mode : ToggleScriptMode.TOGGLE;
			}

			private void SetVerbose(object verbose, bool isPresent) {
				this.verbose = isPresent && (bool) verbose;
			}

			private void SetAccuracy(object accuracy, bool isPresent) {
				this.accuracy = isPresent ? new float?((float) accuracy) : null;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"scriptName",
								false,
								"name of the script to execute, toggleScripts are stored in '...\\DOMain_Data\\BlazingTwist\\toggleScripts\\'",
								SetScriptName,
								typeof(string)
						),
						new BTConsoleCommand.BTConsoleArgument(
								"mode",
								true,
								"mode of script execution, 'ENABLE's, 'DISABLE's or 'TOGGLE's all script objects - defaults to TOGGLE",
								SetMode,
								typeof(ToggleScriptMode)
						),
						new BTConsoleCommand.BTConsoleArgument(
								"verbose",
								true,
								"if true, provides debug information to the console - defaults to 'false'",
								SetVerbose,
								typeof(bool)
						),
						new BTConsoleCommand.BTConsoleArgument(
								"accuracy",
								true,
								"provide a custom accuracy for float comparisons, this is the maximum difference between two equal floats - e.g. '2' means that '1.0' and '3.0' are considered equal",
								SetAccuracy,
								typeof(float)
						)
				};
			}
		}
	}
}