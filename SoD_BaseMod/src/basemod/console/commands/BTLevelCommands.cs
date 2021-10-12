using System.Collections.Generic;
using System.Linq;
using System.Text;
using SoD_BaseMod.extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

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
	}
}