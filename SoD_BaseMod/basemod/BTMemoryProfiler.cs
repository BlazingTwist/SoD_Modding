using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Profiling;
using Object = UnityEngine.Object;

namespace SoD_BaseMod.basemod {
	public class BTMemoryProfiler {
		public void RenderGUI() {
			if (boxReflectFontStyle == null) {
				boxReflectFontStyle = new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleLeft };
			}

			GUILayoutOption guiLayoutOption = GUILayout.ExpandWidth(false);
			if (mCollapsed) {
				if (GUILayout.Button("MemProfiler", guiLayoutOption)) {
					mCollapsed = false;
				}

				return;
			}

			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			if (GUILayout.Button("Close", guiLayoutOption)) {
				CloseProfiler();
			}

			if (GUILayout.Button("Collapse", guiLayoutOption)) {
				mCollapsed = true;
			}

			GUILayout.Space(20f);
			if (GUILayout.Button("SnapShot", Array.Empty<GUILayoutOption>())) {
				TakeSnapShot();
			}

			if (GUILayout.Button("Refresh", Array.Empty<GUILayoutOption>())) {
				TakeLightSnapShot();
			}

			GUILayout.EndHorizontal();
			RenderGUI_Stats();
		}

		private void CloseProfiler() {
			if (_OwnerGO != null) {
				Object.Destroy(_OwnerGO);
			}
		}

		private void RenderGUI_StatsViewTab(string Name, StatsViewTab Mode) {
			Color backgroundColor = GUI.backgroundColor;
			if (mStatsViewIndex != Mode) {
				GUI.backgroundColor = Color.gray;
			}

			if (GUILayout.Button(Name, Array.Empty<GUILayoutOption>())) {
				mStatsViewIndex = Mode;
			}

			GUI.backgroundColor = backgroundColor;
		}

		private void RenderGUI_Stats() {
			GUI_BeginContents();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			RenderGUI_StatsViewTab("Current Stats", StatsViewTab.CURRENT_STATS);
			RenderGUI_StatsViewTab("Current Objects", StatsViewTab.CURRENT_OBJECTS);
			RenderGUI_StatsViewTab("Dif Stats", StatsViewTab.DIF_STATS);
			RenderGUI_StatsViewTab("Dif Objects", StatsViewTab.DIF_OBJECTS);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("Filter:", GUILayout.Width(40f));
			mFilter_Text = GUILayout.TextField(mFilter_Text, GUILayout.ExpandWidth(true));
			if (GUILayout.Button("X", GUILayout.Width(20f))) {
				mFilter_Text = "";
			}

			mFilter_ShowDependencies = GUILayout.Toggle(mFilter_ShowDependencies, "Dependencies", GUILayout.ExpandWidth(false));
			reflectOnProperties = GUILayout.Toggle(reflectOnProperties, "reflectOnProperties", GUILayout.ExpandWidth(false));
			GUILayout.EndHorizontal();
			switch (mStatsViewIndex) {
				case StatsViewTab.CURRENT_STATS:
					RenderGUI_Stats(mStats_Current1);
					break;
				case StatsViewTab.CURRENT_OBJECTS:
					RenderGUI_List(mList_Snapshot);
					break;
				case StatsViewTab.DIF_STATS:
					RenderGUI_Stats(mStats_Dif1);
					break;
				case StatsViewTab.DIF_OBJECTS:
					RenderGUI_List(mList_Differences);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			GUI_EndContents();
		}

		private void RenderGUI_Stats(IReadOnlyList<KeyValuePair<string, int>> mStats) {
			mScrollViewPos_Stats = GUILayout.BeginScrollView(mScrollViewPos_Stats, GUILayout.ExpandHeight(true));
			TextAnchor alignment = GUI.skin.label.alignment;
			GUI.skin.label.alignment = TextAnchor.MiddleRight;
			int i = 0;
			int count = mStats.Count;
			while (i < count) {
				KeyValuePair<string, int> keyValuePair = mStats[i];
				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				GUILayout.Label(
						keyValuePair.Value < 1048576
								? keyValuePair.Value / 1024 + " Kb"
								: (keyValuePair.Value / 1048576).ToString("0.00") + " Mb"
						, GUILayout.Width(80f));
				if (GUILayout.Button(keyValuePair.Key, Array.Empty<GUILayoutOption>())) {
					mFilter_Text = keyValuePair.Key;
					StatsViewTab statsViewTab = mStatsViewIndex;
					if (statsViewTab != StatsViewTab.CURRENT_STATS) {
						if (statsViewTab == StatsViewTab.DIF_STATS) {
							mStatsViewIndex = StatsViewTab.DIF_OBJECTS;
						}
					} else {
						mStatsViewIndex = StatsViewTab.CURRENT_OBJECTS;
					}
				}

				GUILayout.EndHorizontal();
				i++;
			}

			GUI.skin.label.alignment = alignment;
			GUILayout.EndScrollView();
		}

		private static void GUI_BeginContents() {
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Space(4f);
			GUILayout.BeginHorizontal(GUILayout.ExpandHeight(true));
			GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
			GUILayout.Space(2f);
		}

		private static void GUI_EndContents() {
			GUILayout.Space(3f);
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			GUILayout.Space(3f);
			GUILayout.EndHorizontal();
			GUILayout.Space(3f);
		}

		private void TakeSnapShot() {
			TakeLightSnapShot();
			mList_LastSnapshot.Clear();
			mList_LastSnapshot.Capacity = mList_Snapshot.Count;
			int i = 0;
			int count = mList_Snapshot.Count;
			while (i < count) {
				mList_LastSnapshot.Add(mList_Snapshot[i]);
				i++;
			}
		}

		private void TakeLightSnapShot() {
			Dictionary<string, List<ObjMemDef>> dictionary = new Dictionary<string, List<ObjMemDef>>();
			int i = 0;
			int count = mList_LastSnapshot.Count;
			while (i < count) {
				AddLastSnapShotElement(ref dictionary, mList_LastSnapshot[i]);
				i++;
			}

			mList_Differences.Clear();
			mList_Snapshot.Clear();
			mStats_Current.Clear();
			mStats_Dif.Clear();
			Object[] array = Resources.FindObjectsOfTypeAll(typeof(Object));
			int j = 0;
			while (j < array.Length) {
				Object @object = array[j];
				int num3 = (int) Profiler.GetRuntimeMemorySizeLong(@object);
				string text = @object.GetType().ToString();
				if (text.StartsWith("UnityEngine.")) {
					text = text.Substring(12);
				}

				mStats_Current.TryGetValue(text, out int num4);
				mStats_Current[text] = num4 + num3;
				var objMemDef = new ObjMemDef(num3, text, @object.name, HasADependantInTheList(@object), @object);
				mList_Snapshot.Add(objMemDef);
				if (!RemoveLastSnapShotElement(ref dictionary, objMemDef)) {
					mList_Differences.Add(objMemDef);
					mStats_Dif.TryGetValue(text, out num4);
					mStats_Dif[text] = num4 + num3;
				}

				j++;
			}

			mStats_Dif1.Clear();
			mStats_Current1.Clear();
			foreach (KeyValuePair<string, int> item in mStats_Dif) {
				mStats_Dif1.Add(item);
			}

			mStats_Dif1.Sort((v1, v2) => v2.Value - v1.Value);
			foreach (KeyValuePair<string, int> item2 in mStats_Current) {
				mStats_Current1.Add(item2);
			}

			mStats_Current1.Sort((v1, v2) => v2.Value - v1.Value);
			mStats_Dif.Clear();
			mStats_Current.Clear();
			mList_Snapshot.Sort((p1, p2) => p2.CompareTo(ref p1));
			mList_Differences.Sort((p1, p2) => p2.CompareTo(ref p1));
		}

		private static bool HasADependantInTheList(Object Obj) {
			var gameObject = Obj as GameObject;
			return gameObject == null || gameObject.transform.parent != null;
		}

		public BTMemoryProfiler() {
			mStats_Dif1 = new List<KeyValuePair<string, int>>();
			mStats_Dif = new Dictionary<string, int>();
			mList_Differences = new List<ObjMemDef>();
			mList_LastSnapshot = new List<ObjMemDef>();
			mScrollViewPos_Stats = new Vector2(0f, 0f);
			mFilter_Text = "";
			mFilter_ShowDependencies = true;
		}

		private static ObjectContentInfo buildContentInfo(object instance) {
			if (instance == null) {
				return null;
			}

			Debug.LogWarning("building contentInfo for object: " + instance);
			List<object> objectHistory = new List<object>();
			return new ObjectContentInfo(instance.GetType().Name, "", instance.ToString()) {
					contentInfo = buildContentInfoRecursive(instance, 0, objectHistory)
			};
		}

		private static List<ObjectContentInfo> buildContentInfoRecursive(object value, int depth, ICollection<object> objectHistory) {
			if (value == null) {
				return null;
			}

			if (depth > 15) {
				return null;
			}

			if (objectHistory.Contains(value)) {
				return null;
			}

			objectHistory.Add(value);
			Type type = value.GetType();
			if (type.IsPrimitive) {
				return null;
			}

			if (value is string || value is DateTime) {
				return null;
			}

			List<ObjectContentInfo> result = new List<ObjectContentInfo>();
			try {
				if (type.IsEnum) {
					result.AddRange(type.GetEnumNames().Select(enumName => new ObjectContentInfo("Enum", type.Name, enumName)));
				} else if (typeof(Array).IsAssignableFrom(type)) {
					var array = (Array) value;
					int length = array.GetLength(0);
					for (int index = 0; index < length; index++) {
						object arrayValue = array.GetValue(index);
						var arrayContentInfo = new ObjectContentInfo(arrayValue.GetType().Name, "arr_" + index, arrayValue);
						result.Add(arrayContentInfo);
						arrayContentInfo.contentInfo = buildContentInfoRecursive(arrayValue, depth + 1, objectHistory);
					}
				} else if (type.GetInterfaces().Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IDictionary<,>))) {
					IDictionary<object, object> dictionary = (IDictionary<object, object>) value;
					foreach (KeyValuePair<object, object> kvp in dictionary) {
						var enumerableContentInfo = new ObjectContentInfo(kvp.Key.GetType().Name, "dict[" + kvp.Key + "]", kvp.Value);
						result.Add(enumerableContentInfo);
						enumerableContentInfo.contentInfo = buildContentInfoRecursive(kvp.Value, depth + 1, objectHistory);
					}
				} else if (type.GetInterfaces().Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))) {
					IEnumerable<object> enumerable = (IEnumerable<object>) value;
					int quasiIndex = 0;
					foreach (object item in enumerable) {
						var enumerableContentInfo = new ObjectContentInfo(item.GetType().Name, "enumerable~" + quasiIndex, item);
						result.Add(enumerableContentInfo);
						enumerableContentInfo.contentInfo = buildContentInfoRecursive(item, depth + 1, objectHistory);
						quasiIndex++;
					}
				} else {
					foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)) {
						object fieldValue = fieldInfo.GetValue(value);
						var fieldContentInfo = new ObjectContentInfo(fieldInfo.FieldType.Name, fieldInfo.Name, fieldValue);
						result.Add(fieldContentInfo);
						fieldContentInfo.contentInfo = buildContentInfoRecursive(fieldValue, depth + 1, objectHistory);
					}

					if (reflectOnProperties) {
						foreach (PropertyInfo propertyInfo in type.GetProperties()) {
							if (propertyInfo.CanRead) {
								ParameterInfo[] parameters = propertyInfo.GetIndexParameters();
								if (parameters.GetLength(0) != 0) {
									var indexedPropContentInfo = new ObjectContentInfo(propertyInfo.PropertyType.Name, propertyInfo.Name,
											"[can't reflect on indexed property! listing parameters instead]");
									result.Add(indexedPropContentInfo);
									List<ObjectContentInfo> parameterInfo = (
											from parameter in parameters
											let defaultParamValue = parameter.HasDefaultValue
													? parameter.DefaultValue?.ToString()
													: "none"
											select new ObjectContentInfo(parameter.ParameterType.Name, parameter.Name, "default: " + defaultParamValue)
									).ToList();

									indexedPropContentInfo.contentInfo = parameterInfo;
								} else {
									object propertyValue = propertyInfo.GetValue(value);
									if (propertyValue != null && !propertyValue.GetType().IsAssignableFrom(type)) {
										var propContentInfo =
												new ObjectContentInfo(propertyInfo.PropertyType.Name, propertyInfo.Name, propertyValue);
										result.Add(propContentInfo);
										propContentInfo.contentInfo = buildContentInfoRecursive(propertyValue, depth + 1, objectHistory);
									}
								}
							}
						}
					}
				}
			} catch (Exception e) {
				Debug.LogWarning("reflection usage failed, error: " + e);
				return new List<ObjectContentInfo> { new ObjectContentInfo("error", "encountered error", e) };
			}

			return result;
		}

		private void RenderGUI_List(IReadOnlyList<ObjMemDef> mListDef) {
			mScrollViewPos_Stats = GUILayout.BeginScrollView(mScrollViewPos_Stats, GUILayout.ExpandHeight(true));
			float lineHeight = GUI.skin.button.lineHeight + GUI.skin.button.margin.top + GUI.skin.button.padding.top + GUI.skin.button.padding.bottom;
			int viewportStartLine = (int) (mScrollViewPos_Stats.y / lineHeight);
			int viewportEndLine = viewportStartLine + (int) (Screen.height / lineHeight);
			int extraSpacingLines = 0;
			int renderedLines = 0;
			TextAnchor alignment = GUI.skin.label.alignment;
			GUI.skin.label.alignment = TextAnchor.MiddleRight;
			int count = mListDef.Count;
			bool doReflect = false;
			string filterText = "";
			string[] reflectPath = null;
			if (!string.IsNullOrEmpty(mFilter_Text)) {
				doReflect = mFilter_Text.EndsWith("@reflect");
				if (doReflect) {
					reflectPath = mFilter_Text.Replace("@reflect", "").ToLowerInvariant().Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
				} else {
					filterText = mFilter_Text.ToLowerInvariant();
				}
			}

			for (int i = 0; i < count; i++) {
				ObjMemDef objMemDef = mListDef[i];
				if (doReflect) {
					if (objMemDef._Name.ToLowerInvariant().Equals(reflectPath[0]) || objMemDef._ObjType.ToLowerInvariant().Equals(reflectPath[0])) {
						RenderContentInfoRecursive(objMemDef.GetContentInfo(), 0, 20f, ref renderedLines, viewportStartLine, viewportEndLine,
								ref extraSpacingLines, lineHeight, reflectPath);
					}
				} else if ((mFilter_ShowDependencies || !objMemDef._IsADependency) && (filterText.Equals("") ||
						objMemDef._Name.ToLowerInvariant().Contains(filterText) || objMemDef._ObjType.ToLowerInvariant().Contains(filterText))) {
					if (renderedLines > 0 && (renderedLines < viewportStartLine || renderedLines > viewportEndLine)) {
						extraSpacingLines++;
					} else {
						if (extraSpacingLines > 0) {
							GUILayout.Space(extraSpacingLines * lineHeight);
							extraSpacingLines = 0;
						}

						GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
						GUILayout.Label(
								objMemDef._Size < 1048576 ? objMemDef._Size / 1024 + " Kb" : (objMemDef._Size / 1048576).ToString("0.00") + " Mb",
								GUILayout.Width(80f));
						if (GUILayout.Button("reflect on type", GUILayout.ExpandWidth(false))) {
							mFilter_Text = objMemDef._ObjType + "@reflect";
						}

						if (GUILayout.Button("reflect on name", GUILayout.ExpandWidth(false))) {
							mFilter_Text = objMemDef._Name + "@reflect";
						}

						if (GUILayout.Button(objMemDef._ObjType, GUILayout.Width(200f))) {
							mFilter_Text = objMemDef._ObjType;
						}

						if (GUILayout.Button(objMemDef._Name, Array.Empty<GUILayoutOption>())) {
							mFilter_Text = objMemDef._Name;
						}

						GUILayout.EndHorizontal();
					}

					renderedLines++;
				}
			}

			GUI.skin.label.alignment = alignment;
			if (extraSpacingLines > 0) {
				GUILayout.Space(extraSpacingLines * lineHeight);
			}

			GUILayout.EndScrollView();
		}

		private static void AddLastSnapShotElement(ref Dictionary<string, List<ObjMemDef>> LastSnapshot, ObjMemDef ObjDef) {
			if (!LastSnapshot.TryGetValue(ObjDef._Name, out List<ObjMemDef> list)) {
				list = new List<ObjMemDef>();
				LastSnapshot[ObjDef._Name] = list;
			}

			list.Add(ObjDef);
		}

		private static bool RemoveLastSnapShotElement(ref Dictionary<string, List<ObjMemDef>> LastSnapshot, ObjMemDef ObjDef) {
			if (!LastSnapshot.TryGetValue(ObjDef._Name, out List<ObjMemDef> list)) {
				return false;
			}

			int num = list.FindIndex(p => ObjDef._ObjType == p._ObjType);
			if (num < 0) {
				return false;
			}

			list.RemoveAt(num);
			return true;
		}

		private void RenderContentInfoRecursive(ObjectContentInfo content, int depth, float spaceWidth, ref int renderedLines, int viewPortStartLine,
				int viewPortEndLine, ref int skippedLines, float lineHeight, IReadOnlyList<string> reflectPath) {
			if (content == null) {
				return;
			}

			if (renderedLines == 0 || renderedLines <= viewPortEndLine && renderedLines >= viewPortStartLine) {
				if (skippedLines > 0) {
					GUILayout.Space(lineHeight * skippedLines);
					skippedLines = 0;
				}

				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				GUILayout.Space(spaceWidth * depth);
				if (content.contentInfo != null) {
					if (GUILayout.Button(content.isFolded ? ">" : "v", GUILayout.Width(20f))) {
						content.isFolded = !content.isFolded;
					}
				} else {
					GUILayout.Box("o", GUILayout.Width(20f));
				}

				GUILayout.Box(content.typeText, boxReflectFontStyle, GUILayout.Width(200f));
				GUILayout.Box(content.nameText, boxReflectFontStyle, GUILayout.Width(200f));
				GUILayout.Box(content.valueText, boxReflectFontStyle, Array.Empty<GUILayoutOption>());
				GUILayout.EndHorizontal();
			} else {
				skippedLines++;
			}

			renderedLines++;
			if (content.contentInfo != null && !content.isFolded) {
				string pathTypeString = null;
				string pathNameString = null;
				if (reflectPath.Count > depth + 1) {
					string pathString = reflectPath[depth + 1];
					if (pathString.EndsWith("@type")) {
						pathTypeString = pathString.Substring(0, pathString.Length - "@type".Length);
					} else {
						pathNameString = pathString;
					}

					if (string.Equals(pathTypeString, "*")) {
						pathTypeString = null;
					}

					if (string.Equals(pathNameString, "*")) {
						pathNameString = null;
					}
				}

				foreach (ObjectContentInfo innerContent in content.contentInfo
						.Where(innerContent => pathTypeString == null || InputPartiallyMatches(pathTypeString, innerContent.typeString))
						.Where(innerContent => pathNameString == null || InputPartiallyMatches(pathNameString, innerContent.nameString))) {
					RenderContentInfoRecursive(innerContent, depth + 1, spaceWidth, ref renderedLines, viewPortStartLine, viewPortEndLine, ref skippedLines,
							lineHeight, reflectPath);
				}
			}
		}

		/// <summary>
		/// checks whether the inputString contains characters in the same sequence as the targetString
		/// </summary>
		/// <param name="input"></param>
		/// <param name="target"></param>
		/// <returns></returns>
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

		private readonly List<KeyValuePair<string, int>> mStats_Current1 = new List<KeyValuePair<string, int>>();

		private readonly Dictionary<string, int> mStats_Current = new Dictionary<string, int>();

		private readonly List<KeyValuePair<string, int>> mStats_Dif1;

		private readonly Dictionary<string, int> mStats_Dif;

		public GameObject _OwnerGO;

		private Vector2 mScrollViewPos_Stats;

		private StatsViewTab mStatsViewIndex;

		private string mFilter_Text;

		private static bool reflectOnProperties;

		private bool mFilter_ShowDependencies;

		private bool mCollapsed;

		private GUIStyle boxReflectFontStyle;

		private readonly List<ObjMemDef> mList_Snapshot = new List<ObjMemDef>();

		private readonly List<ObjMemDef> mList_Differences;

		private readonly List<ObjMemDef> mList_LastSnapshot;

		private class ObjMemDef {
			public ObjMemDef(int Size, string ObjType, string Name, bool IsADependency, object instance) {
				_Size = Size;
				_ObjType = ObjType;
				_Name = Name;
				_IsADependency = IsADependency;
				this.instance = instance;
				contentLoaded = false;
				contentInfo = null;
			}

			public ObjectContentInfo GetContentInfo() {
				if (!contentLoaded) {
					contentInfo = buildContentInfo(instance);
					contentLoaded = true;
				}

				return contentInfo;
			}

			public int CompareTo(ref ObjMemDef other) {
				if (_Size != other._Size) {
					return _Size - other._Size;
				}

				if (_IsADependency != other._IsADependency) {
					return (_IsADependency ? 1 : 0) - (other._IsADependency ? 1 : 0);
				}

				int num = string.CompareOrdinal(_Name, other._Name);
				return num != 0 ? num : string.CompareOrdinal(_ObjType, other._ObjType);
			}

			public readonly int _Size;

			public readonly bool _IsADependency;

			public readonly string _ObjType;

			public readonly string _Name;

			private ObjectContentInfo contentInfo;

			private readonly object instance;

			private bool contentLoaded;
		}

		private enum StatsViewTab {
			CURRENT_STATS,
			CURRENT_OBJECTS,
			DIF_STATS,
			DIF_OBJECTS
		}

		private class ObjectContentInfo {
			public ObjectContentInfo(string typeName, string displayName, object value) {
				typeString = typeName;
				nameString = displayName;
				typeText = "type: " + typeName;
				nameText = "name: " + displayName;
				valueText = value == null ? "null" : value.ToString();
				isFolded = false;
			}

			public List<ObjectContentInfo> contentInfo;

			public readonly string typeString;
			public readonly string typeText;

			public readonly string nameString;
			public readonly string nameText;

			public readonly string valueText;

			public bool isFolded;
		}
	}
}