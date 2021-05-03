using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SoD_BaseMod.basemod.console {
	public class BTConsoleCommand {
		private readonly List<string> commandNamespace;
		private readonly BTCommandInput commandInput;
		private readonly string helpText;
		private readonly Action<BTCommandInput> executeCallback;

		public BTCommandInput GetCommandInput() {
			return commandInput;
		}

		public string GetInfoText() {
			return helpText;
		}

		public BTConsoleCommand(List<string> commandNamespace, BTCommandInput commandInput, string helpText, Action<BTCommandInput> executeCallback) {
			this.commandNamespace = commandNamespace;
			this.commandInput = commandInput;
			this.helpText = helpText;
			this.executeCallback = executeCallback;
		}

		private static bool InputPartiallyMatches(string input, string target) {
			char[] inputChars = input.ToCharArray();
			int inputCharsLength = inputChars.GetLength(0);
			char[] targetChars = target.ToCharArray();
			int targetCharsLength = targetChars.GetLength(0);
			int targetIndex = 0;
			for (int inputIndex = 0; inputIndex < inputCharsLength; inputIndex++) {
				char inputChar = char.ToUpperInvariant(inputChars[inputIndex]);
				while (inputChar != char.ToUpperInvariant(targetChars[targetIndex])) {
					targetIndex++;
					if (targetIndex >= targetCharsLength) {
						// unable to find matching character in target
						return false;
					}
				}

				targetIndex++;
				if (targetIndex >= targetCharsLength && inputIndex + 1 < inputCharsLength) {
					// reached end of target string, but still have input chars to check
					return false;
				}
			}

			return true;
		}

		public int ShowAsSuggestion(List<string> input) {
			int inputCount = input.Count;
			int namespaceCount = commandNamespace.Count;
			int totalArgumentCount = commandInput.TotalArgumentCount();
			if (namespaceCount + totalArgumentCount < inputCount) {
				// input is too long to ever match this
				return 0;
			}

			int consumedNamespaceKeywords = 0;
			for (int inputIndex = 0, namespaceIndex = 0; inputIndex < inputCount; inputIndex++) {
				int remainingInputCount = inputCount - inputIndex;
				string inputString = input[inputIndex];

				while (namespaceIndex < namespaceCount) {
					if (InputPartiallyMatches(inputString, commandNamespace[namespaceIndex])) {
						remainingInputCount--;
						namespaceIndex++;
						consumedNamespaceKeywords++;
						break;
					}

					namespaceIndex++;
				}

				if (namespaceIndex >= namespaceCount) {
					return remainingInputCount > totalArgumentCount ? 0 : consumedNamespaceKeywords;
				}
			}

			// reached end of input
			return consumedNamespaceKeywords;
		}

		public string Autocomplete(List<string> input) {
			int inputCount = input.Count;
			int namespaceCount = commandNamespace.Count;
			var resultBuilder = new StringBuilder();

			int inputConsumptionCount = 0;
			for (int namespaceIndex = 0; namespaceIndex < namespaceCount; namespaceIndex++) {
				string namespaceString = commandNamespace[namespaceIndex];

				if (namespaceIndex != 0) {
					resultBuilder.Append(" ");
				}

				if (inputConsumptionCount < inputCount) {
					string nextInputString = input[inputConsumptionCount];
					if (InputPartiallyMatches(nextInputString, namespaceString)) {
						inputConsumptionCount++;
					}
				}

				resultBuilder.Append(namespaceString);
			}

			for (int inputIndex = inputConsumptionCount; inputIndex < inputCount; inputIndex++) {
				resultBuilder.Append(" ").Append(input[inputIndex]);
			}

			return resultBuilder.ToString();
		}

		public bool IsFullNamespaceMatching(List<string> input) {
			int inputCount = input.Count;
			int namespaceCount = commandNamespace.Count;
			int totalArgumentCount = commandInput.TotalArgumentCount();
			if (inputCount < namespaceCount || inputCount > namespaceCount + totalArgumentCount) {
				return false;
			}

			for (int index = 0; index < namespaceCount; index++) {
				string inputString = input[index];
				string namespaceString = commandNamespace[index];
				if (!InputPartiallyMatches(inputString, namespaceString)) {
					return false;
				}
			}

			return true;
		}

		public bool IsCommandMatching(List<string> input) {
			int inputCount = input.Count;
			int namespaceCount = commandNamespace.Count;
			int totalArgumentCount = commandInput.TotalArgumentCount();
			int requiredArgumentCount = commandInput.RequiredArgumentCount();
			if (namespaceCount + requiredArgumentCount > inputCount) {
				// input too short to match this command
				return false;
			}

			if (namespaceCount + totalArgumentCount < inputCount) {
				// input too long to match this command
				return false;
			}

			for (int namespaceIndex = 0; namespaceIndex < namespaceCount; namespaceIndex++) {
				string inputString = input[namespaceIndex];
				string namespaceString = commandNamespace[namespaceIndex];
				if (!InputPartiallyMatches(inputString, namespaceString)) {
					return false;
				}
			}

			return true;
		}

		public void Execute(List<string> input) {
			commandInput.ParseArguments(input, commandNamespace.Count);
			executeCallback.Invoke(commandInput);
		}

		public string GetNamespaceString() {
			return string.Join(" ", commandNamespace);
		}

		public string ShortHelp() {
			var builder = new StringBuilder("");
			builder.Append(GetNamespaceString());
			builder.Append(" ").Append(commandInput.GetArgumentTemplate());
			builder.Append(" - ").Append(helpText);
			return builder.ToString();
		}

		public string Help() {
			var builder = new StringBuilder("");
			builder.Append(GetNamespaceString());
			builder.Append(" ").Append(commandInput.GetArgumentTemplate());
			builder.Append("\n").Append(helpText);
			foreach (string argHelpText in commandInput.GetArgumentHelp()) {
				builder.Append("\n\t").Append(argHelpText);
			}

			return builder.ToString();
		}

		public abstract class BTCommandInput {
			private List<BTConsoleArgument> requiredArguments;
			private List<BTConsoleArgument> optionalArguments;

			protected BTCommandInput() {
				Prepare();
			}

			protected abstract IEnumerable<BTConsoleArgument> BuildConsoleArguments();

			private void Prepare() {
				requiredArguments = new List<BTConsoleArgument>();
				optionalArguments = new List<BTConsoleArgument>();
				foreach (BTConsoleArgument argument in BuildConsoleArguments()) {
					if (argument.IsOptional()) {
						optionalArguments.Add(argument);
					} else {
						requiredArguments.Add(argument);
					}
				}
			}

			private IEnumerable<BTConsoleArgument> GetConsoleArguments() {
				if (requiredArguments == null || optionalArguments == null) {
					Prepare();
				}

				return requiredArguments.Concat(optionalArguments);
			}

			public void ParseArguments(List<string> stringArguments, int consumedArguments) {
				int argumentCount = stringArguments.Count;
				foreach (BTConsoleArgument argument in GetConsoleArguments()) {
					argument.Reset();
					if (consumedArguments < argumentCount) {
						argument.Consume(stringArguments[consumedArguments]);
						consumedArguments++;
					}
				}
			}

			public int TotalArgumentCount() {
				return requiredArguments.Count + optionalArguments.Count;
			}

			public int RequiredArgumentCount() {
				return requiredArguments.Count;
			}

			public string GetArgumentTemplate() {
				List<string> argumentTexts = GetConsoleArguments()
						.Select(arg => arg.GetFormattedDisplayName())
						.ToList();
				return string.Join(" ", argumentTexts);
			}

			public IEnumerable<string> GetArgumentHelp() {
				return GetConsoleArguments()
						.Select(arg => arg.GetFormattedHelpText())
						.ToList();
			}
		}

		public class BTConsoleArgument {
			private readonly string displayName;
			private readonly bool optional;
			private readonly string helpText;
			private readonly Action<object, bool> valueConsumer;
			private readonly Type valueType;

			public BTConsoleArgument(string displayName, bool optional, string helpText, Action<object, bool> valueConsumer, Type valueType) {
				this.displayName = displayName;
				this.optional = optional;
				this.helpText = helpText;
				this.valueConsumer = valueConsumer;
				this.valueType = valueType;
			}

			public bool IsOptional() {
				return optional;
			}

			public void Reset() {
				valueConsumer.Invoke(valueType.IsValueType ? Activator.CreateInstance(valueType) : null, false);
			}

			public void Consume(string value) {
				object argument = valueType.IsEnum
						? Enum.Parse(valueType, value, true)
						: Convert.ChangeType(value, valueType, CultureInfo.InvariantCulture);
				valueConsumer.Invoke(argument, true);
			}

			public string GetFormattedDisplayName() {
				if (optional) {
					return "<" + displayName + ">";
				}

				return "[" + displayName + "]";
			}

			public string GetFormattedHelpText() {
				return GetFormattedDisplayName()
						+ ": "
						+ valueType.Name
						+ " - "
						+ helpText;
			}
		}
	}
}