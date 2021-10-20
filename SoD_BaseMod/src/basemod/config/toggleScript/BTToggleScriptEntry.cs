using System;
using System.Globalization;
using UnityEngine;
using System.Runtime.Serialization;

namespace SoD_BaseMod.config.toggleScript {
	/**
	 * The class representation for toggleScript elements
	 */
	[DataContract(Namespace = "", Name = xmlContractName)]
	public class BTToggleScriptEntry {
		private const string xmlContractName = "TargetObject";
		private const float epsilon = 0.01f;
		
		[DataMember(IsRequired = true, EmitDefaultValue = false)] private string name;
		[DataMember(IsRequired = false, EmitDefaultValue = false)] private float? x;
		[DataMember(IsRequired = false, EmitDefaultValue = false)] private float? y;
		[DataMember(IsRequired = false, EmitDefaultValue = false)] private float? z;

		public BTToggleScriptEntry() { }

		public BTToggleScriptEntry(GameObject gameObject) : this(gameObject.name, gameObject.transform.position) { }

		public BTToggleScriptEntry(string name, Vector3 position) {
			this.name = name;
			x = position.x;
			y = position.y;
			z = position.z;
		}

		public string Serialize() {
			string SerializeField(string name, string data) => $"<{name}>{data}</{name}>";
			string result = $"<{xmlContractName}>{SerializeField(nameof(name), name)}";
			if (x != null) {
				string xValue = x.Value.ToString(CultureInfo.InvariantCulture);
				result += SerializeField(nameof(x), xValue);
			}
			if (y != null) {
				string yValue = y.Value.ToString(CultureInfo.InvariantCulture);
				result += SerializeField(nameof(y), yValue);
			}
			if (z != null) {
				string zValue = z.Value.ToString(CultureInfo.InvariantCulture);
				result += SerializeField(nameof(z), zValue);
			}
			result += $"</{xmlContractName}>";
			return result;
		}

		public bool ObjectMatches(GameObject gameObject) {
			if (!gameObject.name.Equals(name, StringComparison.InvariantCulture)) {
				return false;
			}
			Vector3 objectPosition = gameObject.transform.position;
			if (x != null) {
				if (Math.Abs(x.Value - objectPosition.x) > epsilon) {
					return false;
				}
			}
			if (y != null) {
				if (Math.Abs(y.Value - objectPosition.y) > epsilon) {
					return false;
				}
			}
			if (z != null) {
				if (Math.Abs(z.Value - objectPosition.z) > epsilon) {
					return false;
				}
			}
			return true;
		}
	}
}