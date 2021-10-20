using System;
using System.Globalization;
using UnityEngine;
using System.Runtime.Serialization;
using SoD_BaseMod.console;

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

		public bool ObjectMatches(GameObject gameObject, bool verbose, float? accuracy) {
			if (!gameObject.name.Equals(name, StringComparison.InvariantCulture)) {
				return false;
			}
			float maxDelta = accuracy ?? epsilon;
			Vector3 objectPosition = gameObject.transform.position;
			if (x != null) {
				if (Math.Abs(x.Value - objectPosition.x) > maxDelta) {
					if (verbose) {
						BTConsole.WriteLine($"object passed name check: {gameObject.name} | {objectPosition} - but failed x check: expected={x.Value} != actual={objectPosition.x} | maxDelta={maxDelta}");
					}
					return false;
				}
			}
			if (y != null) {
				if (Math.Abs(y.Value - objectPosition.y) > maxDelta) {
					if (verbose) {
						BTConsole.WriteLine($"object passed name check: {gameObject.name} | {objectPosition} - but failed y check: expected={y.Value} != actual={objectPosition.y} | maxDelta={maxDelta}");
					}
					return false;
				}
			}
			if (z != null) {
				if (Math.Abs(z.Value - objectPosition.z) > maxDelta) {
					if (verbose) {
						BTConsole.WriteLine($"object passed name check: {gameObject.name} | {objectPosition} - but failed z check: expected={z.Value} != actual={objectPosition.z} | maxDelta={maxDelta}");
					}
					return false;
				}
			}
			if (verbose) {
				BTConsole.WriteLine($"object passed all checks: {gameObject.name} | {objectPosition}");
			}
			return true;
		}
	}
}