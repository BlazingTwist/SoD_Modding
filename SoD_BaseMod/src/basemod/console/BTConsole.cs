using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SoD_BaseMod.console {
	public class BTConsole : MonoBehaviour {
		private static readonly List<BTConsoleCommand> commandList = new List<BTConsoleCommand>();
		private static bool showConsole;

		private const int maxHistoryCount = 20;
		private static readonly List<string> commandHistory = new List<string>();

		private static string inputText = "help";
		private static string consoleText = "";
		private static Vector2 intellisenseScrollViewPos = Vector2.zero;
		private static Vector2 consoleScrollViewPos = Vector2.zero;
		private static int currentHistoryIndex = -1;
		private static bool consoleInputIsFocused;

		private static List<BTConsoleCommand> sortedSuggestions;
		private static List<string> lastParsedInput;
		private static bool inputIsHelpRequest;
		private static string suggestionInput;
		private static bool inputChanged;

		private static BTConfigHolder ConfigHolder => BTDebugCamInputManager.GetConfigHolder();

		private static float ConsoleHeight => ConfigHolder.config?.consoleHeight ?? 0.3f;

		private static int SuggestionCount => ConfigHolder.config?.suggestionCount ?? 5;

		private static string ConfigDefaultCommand => ConfigHolder.config != null ? ConfigHolder.config.consoleDefaultCommand : "help";

		private static bool ConfigOpenByDefault => ConfigHolder.config == null || ConfigHolder.config.consoleOpenByDefault;

		private void Start() {
			BTCommandUtils.RegisterAll();
			inputText = ConfigDefaultCommand;
			showConsole = ConfigOpenByDefault;
		}

		private void LateUpdate() {
			if (BTDebugCamInputManager.IsKeyJustDown("ToggleConsole")) {
				showConsole = !showConsole;
			}

			IEnumerable<KeyValuePair<string, List<string>>> pressedCommandBinds =
					ConfigHolder.config.commandBinds.Where(kvp => BTDebugCamInputManager.AreKeysJustDown(kvp.Value));
			foreach (KeyValuePair<string, List<string>> kvp in pressedCommandBinds) {
				OnCommandSubmitted(kvp.Key, false);
			}
		}

		private void OnGUI() {
			if (!showConsole) {
				return;
			}

			// Check input to enable caching
			inputChanged = !string.Equals(suggestionInput, inputText);
			suggestionInput = inputText;
			BuildSuggestions();

			GUILayout.BeginArea(new Rect(0f, Screen.height * (1f - ConsoleHeight), Screen.width, Screen.height * ConsoleHeight));
			RenderCommandLine();
			RenderIntelliSense();
			RenderConsoleOutput();
			GUILayout.EndArea();
		}

		private static void BuildSuggestions() {
			if (!inputChanged) {
				return;
			}

			inputIsHelpRequest = false;
			List<string> inputCommands = SplitInputMultiCommand(inputText);
			if (inputCommands.Count == 0) {
				lastParsedInput = new List<string>();
				sortedSuggestions = new List<BTConsoleCommand>();
			} else {
				string inputCommand = inputCommands.Last();
				if (inputCommand.EndsWith(" ?")) {
					inputCommand = inputCommand.Substring(0, inputCommand.Length - 2);
					inputIsHelpRequest = true;
				}

				lastParsedInput = SplitInputCommand(inputCommand);
				sortedSuggestions = commandList
						.Select(cmd => new { matchLength = cmd.ShowAsSuggestion(lastParsedInput), cmd })
						.Where(x => x.matchLength > 0)
						.OrderByDescending(x => x.matchLength)
						.Select(x => x.cmd)
						.ToList();
			}
		}

		private static void RenderCommandLine() {
			if (consoleInputIsFocused && Event.current.type == EventType.KeyDown && Event.current.isKey) {
				// ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
				switch (Event.current.keyCode) {
					case KeyCode.Return:
					case KeyCode.KeypadEnter:
						OnCommandSubmitted(inputText, true);
						break;
					case KeyCode.UpArrow:
						SelectPreviousCommand();
						break;
					case KeyCode.DownArrow:
						SelectNextCommand();
						break;
					case KeyCode.Escape:
						GUI.FocusControl(null);
						break;
				}
			}

			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUI.SetNextControlName("consoleInputField");
			inputText = GUILayout.TextField(inputText, GUILayout.ExpandWidth(true));
			consoleInputIsFocused = GUI.GetNameOfFocusedControl() == "consoleInputField";
			if (GUILayout.Button("X", GUILayout.Width(20f))) {
				inputText = "";
			}

			if (GUILayout.Button("clear output", GUILayout.ExpandWidth(false))) {
				consoleText = "";
			}

			if (GUILayout.Button("execute", GUILayout.ExpandWidth(false))) {
				OnCommandSubmitted(inputText, true);
			}

			GUILayout.EndHorizontal();
		}

		private static void RenderIntelliSense() {
			float lineHeight = GUI.skin.button.lineHeight + GUI.skin.button.margin.top + GUI.skin.button.padding.top + GUI.skin.button.padding.bottom;
			intellisenseScrollViewPos = GUILayout.BeginScrollView(intellisenseScrollViewPos, GUILayout.MaxHeight(lineHeight * SuggestionCount),
					GUILayout.ExpandHeight(true));

			TextAnchor boxAlignmentBackup = GUI.skin.box.alignment;
			TextAnchor buttonAlignmentBackup = GUI.skin.button.alignment;
			GUI.skin.box.alignment = TextAnchor.MiddleLeft;
			GUI.skin.button.alignment = TextAnchor.MiddleLeft;

			if (inputIsHelpRequest) {
				List<BTConsoleCommand> showHelpCommands = sortedSuggestions
						.Where(cmd => cmd.IsFullNamespaceMatching(lastParsedInput))
						.ToList();
				if (showHelpCommands.Count == 0) {
					GUILayout.Box("no command found!", Array.Empty<GUILayoutOption>());
				} else {
					foreach (BTConsoleCommand helpCommand in showHelpCommands) {
						GUILayout.Box(helpCommand.Help(), Array.Empty<GUILayoutOption>());
					}
				}
			} else {
				int viewportStartLine = (int) (intellisenseScrollViewPos.y / lineHeight);
				int viewportEndLine = viewportStartLine + (int) (Screen.height * ConsoleHeight / lineHeight);
				int extraSpacingLines = 0;
				int renderedLines = 0;
				foreach (BTConsoleCommand command in sortedSuggestions) {
					if (renderedLines > 0 && (renderedLines < viewportStartLine || renderedLines > viewportEndLine)) {
						extraSpacingLines++;
					} else {
						if (extraSpacingLines > 0) {
							GUILayout.Space(extraSpacingLines * lineHeight);
							extraSpacingLines = 0;
						}

						GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
						bool autocompletePressed =
								GUILayout.Button(command.GetNamespaceString(), GUILayout.Width(300))
								|| GUILayout.Button(command.GetCommandInput().GetArgumentTemplate(), GUILayout.Width(400))
								|| GUILayout.Button(command.GetInfoText(), Array.Empty<GUILayoutOption>());
						GUILayout.EndHorizontal();

						if (autocompletePressed) {
							inputText = command.Autocomplete(lastParsedInput);
						}
					}

					renderedLines++;
				}

				if (extraSpacingLines > 0) {
					GUILayout.Space(extraSpacingLines * lineHeight);
				}
			}

			GUI.skin.box.alignment = boxAlignmentBackup;
			GUI.skin.button.alignment = buttonAlignmentBackup;
			GUILayout.EndScrollView();
		}

		private static void RenderConsoleOutput() {
			consoleScrollViewPos = GUILayout.BeginScrollView(consoleScrollViewPos, GUILayout.ExpandHeight(true));
			GUILayout.TextArea(consoleText, GUILayout.ExpandHeight(true));
			GUILayout.EndScrollView();
		}

		private static void AddCommandToHistory(string command) {
			if (commandHistory.Count > 0 && commandHistory[commandHistory.Count - 1].Equals(command)) {
				return;
			}

			commandHistory.Add(command);
			if (commandHistory.Count > maxHistoryCount) {
				commandHistory.RemoveAt(0);
			}
		}

		private static void SelectPreviousCommand() {
			currentHistoryIndex++;
			if (currentHistoryIndex >= commandHistory.Count) {
				currentHistoryIndex--;
			} else {
				inputText = commandHistory[commandHistory.Count - (currentHistoryIndex + 1)];
			}
		}

		private static void SelectNextCommand() {
			currentHistoryIndex--;
			if (currentHistoryIndex < -1) {
				currentHistoryIndex = -1;
			} else {
				inputText = currentHistoryIndex == -1
						? ""
						: commandHistory[commandHistory.Count - (currentHistoryIndex + 1)];
			}
		}

		private static List<string> SplitInputMultiCommand(string command) {
			return command.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
		}

		private static List<string> SplitInputCommand(string command) {
			List<string> result = new List<string>();
			var currentArgument = new StringBuilder();
			bool isString = false;
			bool isEscaped = false;
			foreach (char c in command) {
				bool wasEscaped = isEscaped;
				isEscaped = c == '\\';
				if (isEscaped) {
					if (wasEscaped) {
						// backslash must escape backslash too
						isEscaped = false;
					} else {
						// if not escaped, skip this backslash
						continue;
					}
				}

				// isEscaped is ALWAYS false below here.

				if (c == '"' && !wasEscaped) {
					isString = !isString;
					continue;
				}

				if (char.IsWhiteSpace(c) && !isString) {
					if (currentArgument.Length != 0) {
						result.Add(currentArgument.ToString());
						currentArgument.Clear();
					}

					continue;
				}

				currentArgument.Append(c);
			}

			if (currentArgument.Length != 0) {
				result.Add(currentArgument.ToString());
			}

			return result;
		}

		private static BTConsoleCommand GetBestMatchingCommand(string inputString, List<string> input, bool isHelpRequest, bool isMultilineCommand) {
			BTConsoleCommand exactMatchCommand = commandList.FirstOrDefault(cmd => cmd.IsCommandMatchingExact(input));
			if (exactMatchCommand != null) {
				return exactMatchCommand;
			}

			Dictionary<int, List<BTConsoleCommand>> suggestionDict = new Dictionary<int, List<BTConsoleCommand>>();
			int maxSuggestionStrength = 0;
			foreach (BTConsoleCommand cmd in commandList) {
				int suggestionStrength = cmd.ShowAsSuggestion(input);
				if (suggestionStrength == 0) {
					continue;
				}

				if (suggestionStrength > maxSuggestionStrength) {
					maxSuggestionStrength = suggestionStrength;
				}

				if (!suggestionDict.ContainsKey(suggestionStrength)) {
					suggestionDict.Add(suggestionStrength, new List<BTConsoleCommand>());
				}

				suggestionDict[suggestionStrength].Add(cmd);
			}
			
			if (maxSuggestionStrength == 0) {
				WriteLine("unable to find command: '" + inputString + "'");
				return null;
			}
			
			List<BTConsoleCommand> matchingCommands;
			if (isHelpRequest) {
				matchingCommands = commandList
						.Where(cmd => cmd.IsFullNamespaceMatching(input))
						.ToList();
			} else {
				matchingCommands = commandList
						.Where(cmd => cmd.IsCommandMatching(input))
						.ToList();
			}
			
			if (matchingCommands.Count == 0) {
				if (isHelpRequest) {
					WriteLine("unable to print help for command: '" + inputString + "' - no such command found");
					return null;
				}

				if (isMultilineCommand) {
					WriteLine("unable to find command: '" + inputString + "' - autocomplete is not supported for multiline-commands");
					return null;
				}

				// no full matches -> autocomplete best suggestion
				List<BTConsoleCommand> bestSuggestions = suggestionDict[maxSuggestionStrength];
				if (bestSuggestions.Count > 1) {
					WriteLine("Found multiple equally strong suggestions - can't autocomplete");
					return null;
				}

				inputText = bestSuggestions[0].Autocomplete(input);
				return null;
			}
			
			if (matchingCommands.Count == 1) {
				return matchingCommands[0];
			}
			{ //count > 1
				List<BTConsoleCommand> bestSuggestions = suggestionDict[maxSuggestionStrength];
				List<BTConsoleCommand> bestMatchingCommands = matchingCommands
						.Where(cmd => bestSuggestions.Contains(cmd))
						.ToList();
				int bestMatchingCommandsCount = bestMatchingCommands.Count;
				switch (bestMatchingCommandsCount) {
					case 0:
						WriteLine("Can't identify command: '" + inputString + "'! BestSuggestions and MatchingCommands are disjointed (somehow)");
						return null;
					case 1:
						return bestMatchingCommands.First();
					default:
						WriteLine("Can't identify command: '" + inputString + "'! Found multiple full matches of equal suggestionStrength!");
						return null;
				}
			}
		}
		
		private static void OnCommandSubmitted(string multiCommand, bool addToHistory) {
			if (addToHistory) {
				AddCommandToHistory(multiCommand);
				currentHistoryIndex = -1;
			}

			List<string> inputCommandList = SplitInputMultiCommand(multiCommand);
			int inputCommandCount = inputCommandList.Count;
			foreach (string _command in inputCommandList) {
				string command = _command;
				bool isHelpRequest = false;
				if (command.EndsWith(" ?")) {
					command = command.Substring(0, command.Length - 2);
					isHelpRequest = true;
				}

				List<string> input = SplitInputCommand(command);
				BTConsoleCommand bestMatchingCommand = GetBestMatchingCommand(command, input, isHelpRequest, inputCommandCount > 1);

				inputText = "";
				if (isHelpRequest) {
					WriteLine(bestMatchingCommand.Help());
				} else {
					try {
						bestMatchingCommand.Execute(input);
					} catch (Exception e) {
						WriteLine("Execution of command encountered exception: " + e);
					}
				}
			}
		}

		public static void AddCommand(BTConsoleCommand command) {
			if (commandList.Contains(command)) {
				return;
			}

			commandList.Add(command);
		}

		public static void HelpAll() {
			var builder = new StringBuilder("");
			foreach (BTConsoleCommand command in commandList) {
				builder.Append(command.ShortHelp()).Append("\n");
			}

			consoleText += builder.ToString();
		}

		public static void BuildMDHelpTable() {
			var builder = new StringBuilder("");
			List<List<string>> sortedCommands = commandList
					.Select(command => new List<string> {
							command.GetNamespaceString(),
							command.GetCommandInput().GetArgumentTemplate(),
							command.GetInfoText()
					}).ToList();

			sortedCommands.Sort((x, y) => string.CompareOrdinal(x[0], y[0]));
			builder.Append("| Command | Input | Description |\n|:---|:---|:---|\n");
			foreach (List<string> outputDataOrder in sortedCommands) {
				builder.Append(string.Join("|", outputDataOrder)).Append("\n");
			}

			consoleText += builder.ToString();
		}

		public static void WriteLine(string line) {
			if (ConfigHolder.config != null && ConfigHolder.config.consoleReverseOutput) {
				consoleText = line + "\n" + consoleText;
			} else {
				consoleText += line + "\n";
			}
		}

		public static void ClearConsole() {
			consoleText = "";
		}
	}
}