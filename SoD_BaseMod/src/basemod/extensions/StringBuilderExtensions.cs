using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace SoD_BaseMod.extensions {
	[PublicAPI]
	public static class StringBuilderExtensions {
		public static StringBuilder AppendDictionary(this StringBuilder builder, string prefix, Dictionary<object, object> dictionary) {
			Dictionary<string,string> stringDictionary = dictionary.ToDictionary(kvp => kvp.Key.ToString(), kvp => kvp.Value.ToString());
			return builder.AppendDictionary(prefix, stringDictionary);
		}
		
		public static StringBuilder AppendDictionary(this StringBuilder builder, string prefix, Dictionary<string, string> dictionary) {
			int maxKeyLength = dictionary.Keys.Max(key => key.Length);
			foreach (KeyValuePair<string,string> keyValuePair in dictionary) {
				builder.Append(prefix);
				builder.Append(keyValuePair.Key);
				builder.Append(new string(' ', maxKeyLength + 4 - keyValuePair.Key.Length));
				builder.Append(": ");
				builder.Append(keyValuePair.Value);
				builder.Append("\n");
			}
			return builder;
		}
	}
}