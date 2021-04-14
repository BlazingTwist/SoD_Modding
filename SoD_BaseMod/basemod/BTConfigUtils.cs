using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Reflection;

namespace SoD_BaseMod.basemod
{
	public class BTConfigUtils
	{
		public static void LoadConfig<configType>(TextReader reader, configType instance) {
			ConfigNode rootNode = new ConfigNode {
				listValues = BuildNodes(Tokenize(reader))
			};
			ParseObject(rootNode, typeof(configType), (Object)instance);
		}

		public static configType LoadConfig<configType>(TextReader reader) {
			return BuildConfig<configType>(BuildNodes(Tokenize(reader)));
		}

		private class ConfigNode
		{
			public string key = null;
			public string value = null;
			public List<ConfigNode> listValues = null;

			public override string ToString() {
				StringBuilder builder = new StringBuilder();
				builder.Append("Key: ");
				builder.Append(key ?? "null");

				builder.Append(" | Value: ");
				builder.Append(value ?? "null");

				builder.Append(" | listValues: ");
				if(listValues == null) {
					builder.Append("null");
				} else {
					builder.Append("{Node: (");
					builder.Append(string.Join("), Node: (", listValues.Select(node => node.ToString())));
					builder.Append(")}");
				}

				return builder.ToString();
			}
		}

		private static configType BuildConfig<configType>(List<ConfigNode> nodes) {
			configType result = (configType)Activator.CreateInstance(typeof(configType));
			ConfigNode rootNode = new ConfigNode {
				listValues = nodes
			};
			ParseObject(rootNode, typeof(configType), (Object)result);
			return result;
		}

		private static object ParseValue(string value, Type type) {
			if(value == null) {
				throw new ArgumentNullException("valled parseValue with no value");
			}
			if(type.IsEnum) {
				return Enum.Parse(type, value, true);
			}
			return Convert.ChangeType(value, type, System.Globalization.CultureInfo.InvariantCulture);
		}

		private static void ParseList(ConfigNode node, Type type, IList list) {
			if(node.listValues == null) {
				throw new ArgumentNullException("Called parseList with no list");
			}

			foreach(ConfigNode entry in node.listValues) {
				if(entry.value != null) {
					list.Add(ParseValue(entry.value, type));
				} else if(entry.listValues != null) {
					if(type.IsGenericType) {
						if(type.GetGenericTypeDefinition() == typeof(List<>)) {
							IList value = (IList)Activator.CreateInstance(type);
							ParseList(entry, type.GetGenericArguments()[0], value);
							list.Add(value);
							continue;
						} else if(type.GetGenericTypeDefinition() == typeof(Dictionary<,>)) {
							IDictionary value = (IDictionary)Activator.CreateInstance(type);
							ParseDictionary(entry, type.GetGenericArguments()[0], type.GetGenericArguments()[1], value);
							list.Add(value);
							continue;
						}
					}

					{
						//if not a list or dictionary, assume it's an object
						Object value = (Object)Activator.CreateInstance(type);
						ParseObject(entry, type, value);
						list.Add(value);
						continue;
					}
				} else {
					throw new InvalidOperationException("got node without any values! key = " + (node.key ?? "null"));
				}
			}
		}

		private static void ParseDictionary(ConfigNode node, Type keyType, Type type, IDictionary dict) {
			if(node.listValues == null) {
				throw new ArgumentNullException("Called parseDictionary with no dictionary");
			}

			foreach(ConfigNode entry in node.listValues) {
				if(entry.key == null) {
					throw new ArgumentNullException("dictionary node was missing key!");
				}
				object parsedKey = ParseValue(entry.key, keyType);
				if(entry.value != null) {
					dict[parsedKey] = ParseValue(entry.value, type);
				} else if(entry.listValues != null) {
					if(type.IsGenericType) {
						if(type.GetGenericTypeDefinition() == typeof(List<>)) {
							IList value = (IList)Activator.CreateInstance(type);
							ParseList(entry, type.GetGenericArguments()[0], value);
							dict[parsedKey] = value;
							continue;
						} else if(type.GetGenericTypeDefinition() == typeof(Dictionary<,>)) {
							IDictionary value = (IDictionary)Activator.CreateInstance(type);
							ParseDictionary(entry, type.GetGenericArguments()[0], type.GetGenericArguments()[1], value);
							dict[parsedKey] = value;
							continue;
						}
					}

					{
						//if not a list or dictionary, assume it's an object
						Object value = (Object)Activator.CreateInstance(type);
						ParseObject(entry, type, value);
						dict[parsedKey] = value;
						continue;
					}
				} else {
					throw new InvalidOperationException("got node without any values! key = " + (node.key ?? "null"));
				}
			}
		}

		private static void ParseObject(ConfigNode node, Type type, Object resultObject) {
			if(node.listValues == null) {
				throw new ArgumentNullException("Called ParseObject with no object");
			}

			FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

			foreach(ConfigNode entry in node.listValues) {
				IEnumerable<FieldInfo> matchingFieldInfos = fields.Where(field => field.Name == entry.key);
				if(matchingFieldInfos.Count() == 0) {
					throw new InvalidOperationException("config file has key: " + entry.key + " | which is unknown to the config-class. Remove this key.");
				}
				FieldInfo targetField = matchingFieldInfos.First();

				if(entry.value != null) {
					targetField.SetValue(resultObject, ParseValue(entry.value, targetField.FieldType));
				} else if(entry.listValues != null) {
					if(targetField.FieldType.IsGenericType) {
						if(targetField.FieldType.GetGenericTypeDefinition() == typeof(List<>)) {
							IList value = (IList)Activator.CreateInstance(targetField.FieldType);
							ParseList(entry, targetField.FieldType.GetGenericArguments()[0], value);
							targetField.SetValue(resultObject, value);
							continue;
						} else if(targetField.FieldType.GetGenericTypeDefinition() == typeof(Dictionary<,>)) {
							IDictionary value = (IDictionary)Activator.CreateInstance(targetField.FieldType);
							ParseDictionary(entry, targetField.FieldType.GetGenericArguments()[0], targetField.FieldType.GetGenericArguments()[1], value);
							targetField.SetValue(resultObject, value);
							continue;
						}
					}

					{
						//if not a list or dictionary, assume it's an object
						Object value = (Object)Activator.CreateInstance(targetField.FieldType);
						ParseObject(entry, targetField.FieldType, value);
						targetField.SetValue(resultObject, value);
						continue;
					}
				} else {
					throw new InvalidOperationException("got node without any values! key = " + (node.key ?? "null"));
				}
			}
		}

		private static void BuildNode(List<List<string>> tokenList, int startIndex, int targetDepth, ref ConfigNode keyNode) {
			keyNode.listValues = new List<ConfigNode>();

			for(int i = startIndex; i < tokenList.Count; i++) {
				List<string> tokens = tokenList[i];

				if(tokens[0].Length < targetDepth) {
					return;
				}

				if(tokens[0].Length > targetDepth) {
					//what to do with that?
					continue;
				}

				ConfigNode subNode = new ConfigNode();
				if(tokens.Count == 2) {
					//raw value `- val`
					//or listMapping `- :`
					if(tokens[1] == ":") {
						BuildNode(tokenList, i + 1, targetDepth + 1, ref subNode);
					} else {
						subNode.value = tokens[1];
					}
				} else if(tokens.Count == 3) {
					//subMapping `- key :`
					subNode.key = tokens[1];
					BuildNode(tokenList, i + 1, targetDepth + 1, ref subNode);
				} else if(tokens.Count == 4) {
					//key val pair `- key = val`
					subNode.key = tokens[1];
					subNode.value = tokens[3];
				} else {
					throw new InvalidOperationException("got line with invalid amounts of tokens: " + tokens.Count + " | tokens = " + string.Join(", ", tokens));
				}
				keyNode.listValues.Add(subNode);
			}
		}

		private static List<ConfigNode> BuildNodes(List<List<string>> tokenList) {
			List<ConfigNode> result = new List<ConfigNode>();

			for(int i = 0; i < tokenList.Count; i++) {
				List<string> tokens = tokenList[i];
				if(tokens[0].Length != 1) {
					//not baseLevel-token
					continue;
				}

				ConfigNode node = new ConfigNode();
				if(tokens.Count == 2) {
					//raw value `- val`
					//or listMapping `- :`
					if(tokens[1] == ":") {
						BuildNode(tokenList, i + 1, 2, ref node);
					} else {
						node.value = tokens[1];
					}
				} else if(tokens.Count == 3) {
					//subMapping `- key :`
					node.key = tokens[1];
					BuildNode(tokenList, i + 1, 2, ref node);
				} else if(tokens.Count == 4) {
					//key val pair `- key = val`
					node.key = tokens[1];
					node.value = tokens[3];
				} else {
					throw new InvalidOperationException("got line with invalid amounts of tokens: " + tokens.Count + " | tokens = " + string.Join(", ", tokens));
				}
				result.Add(node);
			}

			return result;
		}

		private static List<List<string>> Tokenize(TextReader reader) {
			List<List<string>> result = new List<List<string>>();

			List<string> lines = new List<string>();
			{
				string line;
				while((line = reader.ReadLine()) != null) {
					if(!string.IsNullOrWhiteSpace(line)) {
						lines.Add(line.Trim());
					}
				}
			}

			lines = Minimize(lines);

			foreach(string line in lines) {
				List<string> tokens = new List<string>();
				StringBuilder currentToken = new StringBuilder();

				char[] chars = line.ToCharArray();
				int charCount = chars.Length;
				for(int i = 0; i < charCount;) {
					//token 0 is always indenters '-'
					if(tokens.Count == 0) {
						if(chars[i] == '-') {
							currentToken.Append('-');
							i++;
							continue;
						} else {
							tokens.Add(currentToken.ToString());
							currentToken = new StringBuilder();
							continue;
						}
					}

					//is always key or value
					//if key then transition to next token '='
					//or transition to next line on ':'
					if(i == (charCount - 1) && chars[i] == ':') {
						//could be listMapping `- :`
						if(currentToken.Length != 0) {
							tokens.Add(currentToken.ToString());
						}
						tokens.Add(":");
						break;
					}
					if(chars[i] == '=') {
						tokens.Add(currentToken.ToString());
						currentToken = new StringBuilder();
						tokens.Add("=");
						i++;

						// if this line is `---key=` (i.e. value is a blank string) we have to add the token too
						if(i == (charCount)) {
							tokens.Add("");
						}

						continue;
					}
					currentToken.Append(chars[i]);
					i++;

					if(i == charCount) {
						tokens.Add(currentToken.ToString());
					}
				}

				result.Add(tokens);
			}

			return result;
		}

		private static List<string> Minimize(List<string> lines) {
			List<string> result = new List<string>();

			bool currentlyInString = false;
			bool currentlyInRangedComment = false;

			foreach(string line in lines) {
				StringBuilder lineBuilder = new StringBuilder();

				char[] chars = line.ToCharArray();
				for(int i = 0; i < chars.Length; i++) {
					if(!currentlyInString && i < (chars.Length - 1)) {
						if(chars[i] == '/' && chars[i + 1] == '*') {
							currentlyInRangedComment = true;
							i++;
							continue;
						}
					}
					if(currentlyInRangedComment) {
						if(i < (chars.Length - 1)
						  && chars[i] == '*' && chars[i + 1] == '/') {
							currentlyInRangedComment = false;
							i++;
						}
						continue;
					}

					// this will happen on the second (and subsequent) " when reading '""'
					if(chars[i] == '"') {
						currentlyInString = !currentlyInString;
						continue;
					}

					if(i < (chars.Length - 1)) {
						if(chars[i] == '\\' && chars[i + 1] == '"') {
							lineBuilder.Append('"');
							i++;
							continue;
						} else if(chars[i] != '\\' && chars[i + 1] == '"') {
							if(currentlyInString || !char.IsWhiteSpace(chars[i])) {
								lineBuilder.Append(chars[i]);
							}
							currentlyInString = !currentlyInString;
							i++;
							continue;
						}
					}
					if(!currentlyInString && char.IsWhiteSpace(chars[i])) {
						continue;
					}
					if(!currentlyInString && i < (chars.Length - 1)) {
						if(chars[i] == '/' && chars[i + 1] == '/') {
							break;
						}
					}
					lineBuilder.Append(chars[i]);
				}

				string newLine = lineBuilder.ToString();
				if(!string.IsNullOrWhiteSpace(newLine)) {
					result.Add(newLine);
				}
			}
			return result;
		}
	}
}
