using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Profiling;

namespace SoD_BaseMod.basemod
{
	public class BTMemoryProfiler
	{
		public static void ForceMemoryCleanUp() {
			foreach(SpawnPool spawnPool in UnityEngine.Object.FindObjectsOfType(typeof(SpawnPool)) as SpawnPool[]) {
				for(int i = spawnPool._prefabPools.Count - 1; i >= 0; i--) {
					PrefabPool prefabPool = spawnPool._prefabPools[i];
					int j = 0;
					int count = prefabPool.despawned.Count;
					while(j < count) {
						if(prefabPool.despawned[j] != null) {
							UnityEngine.Object.Destroy(prefabPool.despawned[j].gameObject);
						}
						j++;
					}
					prefabPool._despawned.Clear();
					if(prefabPool.spawned.Count <= 0) {
						spawnPool.prefabs._prefabs.Remove(prefabPool.prefab.name);
						spawnPool._prefabPools.RemoveAt(i);
					}
				}
			}
			RsResourceManager.UnloadUnusedAssets(true);
		}

		public void RenderGUI() {
			if(this.boxReflectFontStyle == null) {
				this.boxReflectFontStyle = new GUIStyle(GUI.skin.box);
				this.boxReflectFontStyle.alignment = TextAnchor.MiddleLeft;
			}
			GUILayoutOption guilayoutOption = GUILayout.ExpandWidth(false);
			if(this.mCollapsed) {
				if(GUILayout.Button("MemProfiler", new GUILayoutOption[]
				{
				guilayoutOption
				})) {
					this.mCollapsed = false;
				}
				return;
			}
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			if(GUILayout.Button("Close", new GUILayoutOption[]
			{
			guilayoutOption
			})) {
				this.CloseProfiler();
			}
			if(GUILayout.Button("Collapse", new GUILayoutOption[]
			{
			guilayoutOption
			})) {
				this.mCollapsed = true;
			}
			GUILayout.Space(20f);
			if(GUILayout.Button("SnapShot", Array.Empty<GUILayoutOption>())) {
				this.TakeSnapShot();
			}
			if(GUILayout.Button("Refresh", Array.Empty<GUILayoutOption>())) {
				this.TakeLightSnapShot();
			}
			GUILayout.EndHorizontal();
			this.RenderGUI_Stats();
		}

		private void CloseProfiler() {
			if(this._OwnerGO != null) {
				UnityEngine.Object.Destroy(this._OwnerGO);
			}
		}

		private void RenderGUI_StatsViewTab(string Name, BTMemoryProfiler.StatsViewTab Mode) {
			Color backgroundColor = GUI.backgroundColor;
			if(this.mStatsViewIndex != Mode) {
				GUI.backgroundColor = Color.gray;
			}
			if(GUILayout.Button(Name, Array.Empty<GUILayoutOption>())) {
				this.mStatsViewIndex = Mode;
			}
			GUI.backgroundColor = backgroundColor;
		}

		private void RenderGUI_Stats() {
			this.GUI_BeginContents();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			this.RenderGUI_StatsViewTab("Current Stats", BTMemoryProfiler.StatsViewTab.CURRENT_STATS);
			this.RenderGUI_StatsViewTab("Current Objects", BTMemoryProfiler.StatsViewTab.CURRENT_OBJECTS);
			this.RenderGUI_StatsViewTab("Dif Stats", BTMemoryProfiler.StatsViewTab.DIF_STATS);
			this.RenderGUI_StatsViewTab("Dif Objects", BTMemoryProfiler.StatsViewTab.DIF_OBJECTS);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("Filter:", new GUILayoutOption[]
			{
			GUILayout.Width(40f)
			});
			this.mFilter_Text = GUILayout.TextField(this.mFilter_Text, new GUILayoutOption[]
			{
			GUILayout.ExpandWidth(true)
			});
			if(GUILayout.Button("X", new GUILayoutOption[]
			{
			GUILayout.Width(20f)
			})) {
				this.mFilter_Text = "";
			}
			this.mFilter_ShowDependencies = GUILayout.Toggle(this.mFilter_ShowDependencies, "Dependencies", new GUILayoutOption[]
			{
			GUILayout.ExpandWidth(false)
			});
			BTMemoryProfiler.reflectOnProperties = GUILayout.Toggle(BTMemoryProfiler.reflectOnProperties, "reflectOnProperties", new GUILayoutOption[]{
			GUILayout.ExpandWidth(false)
		});
			GUILayout.EndHorizontal();
			switch(this.mStatsViewIndex) {
				case BTMemoryProfiler.StatsViewTab.CURRENT_STATS:
					this.RenderGUI_Stats(this.mStats_Current1);
					break;
				case BTMemoryProfiler.StatsViewTab.CURRENT_OBJECTS:
					this.RenderGUI_List(this.mList_Snapshot);
					break;
				case BTMemoryProfiler.StatsViewTab.DIF_STATS:
					this.RenderGUI_Stats(this.mStats_Dif1);
					break;
				case BTMemoryProfiler.StatsViewTab.DIF_OBJECTS:
					this.RenderGUI_List(this.mList_Differences);
					break;
			}
			this.GUI_EndContents();
		}

		private void RenderGUI_Stats(List<KeyValuePair<string, int>> mStats) {
			this.mScrollViewPos_Stats = GUILayout.BeginScrollView(this.mScrollViewPos_Stats, new GUILayoutOption[]
			{
			GUILayout.ExpandHeight(true)
			});
			TextAnchor alignment = GUI.skin.label.alignment;
			GUI.skin.label.alignment = TextAnchor.MiddleRight;
			int i = 0;
			int count = mStats.Count;
			while(i < count) {
				KeyValuePair<string, int> keyValuePair = mStats[i];
				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				GUILayout.Label((keyValuePair.Value < 1048576) ? ((keyValuePair.Value / 1024).ToString() + " Kb") : ((keyValuePair.Value / 1048576).ToString("0.00") + " Mb"), new GUILayoutOption[]
				{
				GUILayout.Width(80f)
				});
				if(GUILayout.Button(keyValuePair.Key, Array.Empty<GUILayoutOption>())) {
					this.mFilter_Text = keyValuePair.Key;
					BTMemoryProfiler.StatsViewTab statsViewTab = this.mStatsViewIndex;
					if(statsViewTab != BTMemoryProfiler.StatsViewTab.CURRENT_STATS) {
						if(statsViewTab == BTMemoryProfiler.StatsViewTab.DIF_STATS) {
							this.mStatsViewIndex = BTMemoryProfiler.StatsViewTab.DIF_OBJECTS;
						}
					} else {
						this.mStatsViewIndex = BTMemoryProfiler.StatsViewTab.CURRENT_OBJECTS;
					}
				}
				GUILayout.EndHorizontal();
				i++;
			}
			GUI.skin.label.alignment = alignment;
			GUILayout.EndScrollView();
		}

		public void GUI_BeginContents() {
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Space(4f);
			GUILayout.BeginHorizontal(new GUILayoutOption[]
			{
			GUILayout.ExpandHeight(true)
			});
			GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
			GUILayout.Space(2f);
		}

		public void GUI_EndContents() {
			GUILayout.Space(3f);
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			GUILayout.Space(3f);
			GUILayout.EndHorizontal();
			GUILayout.Space(3f);
		}

		public void TakeSnapShot() {
			this.TakeLightSnapShot();
			this.mList_LastSnapshot.Clear();
			this.mList_LastSnapshot.Capacity = this.mList_Snapshot.Count;
			int i = 0;
			int count = this.mList_Snapshot.Count;
			while(i < count) {
				this.mList_LastSnapshot.Add(this.mList_Snapshot[i]);
				i++;
			}
		}

		private int IndexOfObjInArray(ref UnityEngine.Object[] Objs, UnityEngine.Object Obj) {
			int i = 0;
			int num = Objs.Length;
			while(i < num) {
				if(Objs[i] == Obj) {
					return i;
				}
				i++;
			}
			return -1;
		}

		public void TakeLightSnapShot() {
			Dictionary<string, List<BTMemoryProfiler.ObjMemDef>> dictionary = new Dictionary<string, List<BTMemoryProfiler.ObjMemDef>>();
			int i = 0;
			int count = this.mList_LastSnapshot.Count;
			while(i < count) {
				this.AddLastSnapShotElement(ref dictionary, this.mList_LastSnapshot[i]);
				i++;
			}
			this.mList_Differences.Clear();
			this.mList_Snapshot.Clear();
			int num = 0;
			this.mStats_Current.Clear();
			this.mStats_Dif.Clear();
			UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(UnityEngine.Object));
			int j = 0;
			int num2 = array.Length;
			while(j < num2) {
				UnityEngine.Object @object = array[j];
				int num3 = (int)Profiler.GetRuntimeMemorySizeLong(@object);
				num += num3;
				string text = @object.GetType().ToString();
				if(text.StartsWith("UnityEngine.")) {
					text = text.Substring(12);
				}
				int num4 = 0;
				this.mStats_Current.TryGetValue(text, out num4);
				this.mStats_Current[text] = num4 + num3;
				BTMemoryProfiler.ObjMemDef objMemDef = new BTMemoryProfiler.ObjMemDef(num3, text, @object.name, this.HasADependantInTheList(@object, ref array), @object);
				this.mList_Snapshot.Add(objMemDef);
				if(!this.RemoveLastSnapShotElement(ref dictionary, objMemDef)) {
					this.mList_Differences.Add(objMemDef);
					num4 = 0;
					this.mStats_Dif.TryGetValue(text, out num4);
					this.mStats_Dif[text] = num4 + num3;
				}
				j++;
			}
			this.mStats_Dif1.Clear();
			this.mStats_Current1.Clear();
			foreach(KeyValuePair<string, int> item in this.mStats_Dif) {
				this.mStats_Dif1.Add(item);
			}
			this.mStats_Dif1.Sort((KeyValuePair<string, int> v1, KeyValuePair<string, int> v2) => v2.Value - v1.Value);
			foreach(KeyValuePair<string, int> item2 in this.mStats_Current) {
				this.mStats_Current1.Add(item2);
			}
			this.mStats_Current1.Sort((KeyValuePair<string, int> v1, KeyValuePair<string, int> v2) => v2.Value - v1.Value);
			this.mStats_Dif.Clear();
			this.mStats_Current.Clear();
			this.mList_Snapshot.Sort((BTMemoryProfiler.ObjMemDef p1, BTMemoryProfiler.ObjMemDef p2) => p2.CompareTo(ref p1));
			this.mList_Differences.Sort((BTMemoryProfiler.ObjMemDef p1, BTMemoryProfiler.ObjMemDef p2) => p2.CompareTo(ref p1));
		}

		private bool HasADependantInTheList(UnityEngine.Object Obj, ref UnityEngine.Object[] Objs) {
			GameObject gameObject = Obj as GameObject;
			return gameObject == null || gameObject.transform.parent != null;
		}

		public BTMemoryProfiler() {
			this.mStats_Dif1 = new List<KeyValuePair<string, int>>();
			this.mStats_Dif = new Dictionary<string, int>();
			this.mList_Differences = new List<BTMemoryProfiler.ObjMemDef>();
			this.mList_LastSnapshot = new List<BTMemoryProfiler.ObjMemDef>();
			this.mScrollViewPos_Stats = new Vector2(0f, 0f);
			this.mFilter_Text = "";
			this.mFilter_ShowDependencies = true;
		}

		private static BTMemoryProfiler.ObjectContentInfo buildContentInfo(object instance) {
			if(instance == null) {
				return null;
			}
			Debug.LogWarning("building contentInfo for object: " + instance.ToString());
			List<object> objectHistory = new List<object>();
			return new BTMemoryProfiler.ObjectContentInfo(instance.GetType().Name, "", instance.ToString()) {
				contentInfo = BTMemoryProfiler.buildContentInfoRecursive(instance, 0, objectHistory)
			};
		}

		private static List<BTMemoryProfiler.ObjectContentInfo> buildContentInfoRecursive(object value, int depth, List<object> objectHistory) {
			if(value == null) {
				return null;
			}
			if(depth > 15) {
				return null;
			}
			if(objectHistory.Contains(value)) {
				return null;
			}
			objectHistory.Add(value);
			Type type = value.GetType();
			if(type.IsPrimitive) {
				return null;
			}
			if(typeof(string).IsInstanceOfType(value)) {
				return null;
			}
			if(typeof(DateTime).IsInstanceOfType(value)) {
				return null;
			}
			List<BTMemoryProfiler.ObjectContentInfo> result = new List<BTMemoryProfiler.ObjectContentInfo>();
			try {
				if(type.IsEnum) {
					foreach(string enumName in type.GetEnumNames()) {
						result.Add(new BTMemoryProfiler.ObjectContentInfo("Enum", type.Name, enumName));
					}
				} else if(typeof(Array).IsAssignableFrom(type)) {
					Array array = (Array)value;
					int length = array.GetLength(0);
					for(int index = 0; index < length; index++) {
						object arrayValue = array.GetValue(index);
						BTMemoryProfiler.ObjectContentInfo arrayContentInfo = new BTMemoryProfiler.ObjectContentInfo(arrayValue.GetType().Name, "arr_" + index, arrayValue);
						result.Add(arrayContentInfo);
						arrayContentInfo.contentInfo = BTMemoryProfiler.buildContentInfoRecursive(arrayValue, depth + 1, objectHistory);
					}
				} else if(type.GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IDictionary<,>)).Any()) {
					IDictionary<object, object> dictionary = (IDictionary<object, object>)value;
					foreach(KeyValuePair<object, object> kvp in dictionary) {
						BTMemoryProfiler.ObjectContentInfo enumerableContentInfo = new BTMemoryProfiler.ObjectContentInfo(kvp.Key.GetType().Name, "dict[" + kvp.Key.ToString() + "]", kvp.Value);
						result.Add(enumerableContentInfo);
						enumerableContentInfo.contentInfo = BTMemoryProfiler.buildContentInfoRecursive(kvp.Value, depth + 1, objectHistory);
					}
				} else if(type.GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>)).Any()) {
					IEnumerable<object> enumerable = (IEnumerable<object>)value;
					int quasiIndex = 0;
					foreach(object item in enumerable) {
						BTMemoryProfiler.ObjectContentInfo enumerableContentInfo = new BTMemoryProfiler.ObjectContentInfo(item.GetType().Name, "enumerable~" + quasiIndex, item);
						result.Add(enumerableContentInfo);
						enumerableContentInfo.contentInfo = BTMemoryProfiler.buildContentInfoRecursive(item, depth + 1, objectHistory);
						quasiIndex++;
					}
				} else {
					foreach(FieldInfo fieldInfo in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)) {
						object fieldValue = fieldInfo.GetValue(value);
						BTMemoryProfiler.ObjectContentInfo fieldContentInfo = new BTMemoryProfiler.ObjectContentInfo(fieldInfo.FieldType.Name, fieldInfo.Name, fieldValue);
						result.Add(fieldContentInfo);
						fieldContentInfo.contentInfo = BTMemoryProfiler.buildContentInfoRecursive(fieldValue, depth + 1, objectHistory);
					}
					if(BTMemoryProfiler.reflectOnProperties) {
						foreach(PropertyInfo propertyInfo in type.GetProperties()) {
							if(propertyInfo.CanRead) {
								ParameterInfo[] parameters = propertyInfo.GetIndexParameters();
								if(parameters.GetLength(0) != 0) {
									ObjectContentInfo indexedPropContentInfo = new ObjectContentInfo(propertyInfo.PropertyType.Name, propertyInfo.Name, "[can't reflect on indexed property! listing parameters instead]");
									result.Add(indexedPropContentInfo);
									List<ObjectContentInfo> parameterInfo = new List<ObjectContentInfo>();
									foreach(ParameterInfo parameter in parameters) {
										string defaultParamValue = parameter.HasDefaultValue ? parameter.DefaultValue.ToString() : "none";
										parameterInfo.Add(new ObjectContentInfo(parameter.ParameterType.Name, parameter.Name, "default: " + defaultParamValue));
									}
									indexedPropContentInfo.contentInfo = parameterInfo;
								} else {
									object propertyValue = propertyInfo.GetValue(value);
									if(propertyValue != null && !propertyValue.GetType().IsAssignableFrom(type)) {
										BTMemoryProfiler.ObjectContentInfo propContentInfo = new BTMemoryProfiler.ObjectContentInfo(propertyInfo.PropertyType.Name, propertyInfo.Name, propertyValue);
										result.Add(propContentInfo);
										propContentInfo.contentInfo = BTMemoryProfiler.buildContentInfoRecursive(propertyValue, depth + 1, objectHistory);
									}
								}
							}
						}
					}
				}
			} catch(Exception e) {
				Debug.LogWarning("reflection usage failed, error: " + e.ToString());
				return new List<ObjectContentInfo>() { new ObjectContentInfo("error", "encountered error", e) };
			}
			return result;
		}

		private void RenderGUI_List(List<BTMemoryProfiler.ObjMemDef> mListDef) {
			this.mScrollViewPos_Stats = GUILayout.BeginScrollView(this.mScrollViewPos_Stats, new GUILayoutOption[]
			{
			GUILayout.ExpandHeight(true)
			});
			float lineHeight = GUI.skin.button.lineHeight + (float)GUI.skin.button.margin.top + (float)GUI.skin.button.padding.top + (float)GUI.skin.button.padding.bottom;
			int viewportStartLine = (int)(this.mScrollViewPos_Stats.y / lineHeight);
			int viewportEndLine = viewportStartLine + (int)((float)Screen.height / lineHeight);
			int extraSpacingLines = 0;
			int renderedLines = 0;
			TextAnchor alignment = GUI.skin.label.alignment;
			GUI.skin.label.alignment = TextAnchor.MiddleRight;
			int count = mListDef.Count;
			bool doReflect = false;
			string filterText = "";
			string[] reflectPath = null;
			if(!string.IsNullOrEmpty(this.mFilter_Text)) {
				doReflect = this.mFilter_Text.EndsWith("@reflect");
				if(doReflect) {
					reflectPath = this.mFilter_Text.Replace("@reflect", "").ToLowerInvariant().Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
				} else {
					filterText = this.mFilter_Text.ToLowerInvariant();
				}
			}
			for(int i = 0; i < count; i++) {
				BTMemoryProfiler.ObjMemDef objMemDef = mListDef[i];
				if(doReflect) {
					if(objMemDef._Name.ToLowerInvariant().Equals(reflectPath[0]) || objMemDef._ObjType.ToLowerInvariant().Equals(reflectPath[0])) {
						this.RenderContentInfoRecursive(objMemDef.GetContentInfo(), 0, 20f, ref renderedLines, viewportStartLine, viewportEndLine, ref extraSpacingLines, lineHeight, reflectPath);
					}
				} else if((this.mFilter_ShowDependencies || !objMemDef._IsADependency) && (filterText.Equals("") || objMemDef._Name.ToLowerInvariant().Contains(filterText) || objMemDef._ObjType.ToLowerInvariant().Contains(filterText))) {
					if(renderedLines > 0 && (renderedLines < viewportStartLine || renderedLines > viewportEndLine)) {
						extraSpacingLines++;
					} else {
						if(extraSpacingLines > 0) {
							GUILayout.Space((float)extraSpacingLines * lineHeight);
							extraSpacingLines = 0;
						}
						GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
						GUILayout.Label((objMemDef._Size < 1048576) ? ((objMemDef._Size / 1024).ToString() + " Kb") : ((objMemDef._Size / 1048576).ToString("0.00") + " Mb"), new GUILayoutOption[]
						{
						GUILayout.Width(80f)
						});
						if(GUILayout.Button("reflect on type", new GUILayoutOption[] { GUILayout.ExpandWidth(false) })) {
							this.mFilter_Text = objMemDef._ObjType + "@reflect";
						}
						if(GUILayout.Button("reflect on name", new GUILayoutOption[] { GUILayout.ExpandWidth(false) })) {
							this.mFilter_Text = objMemDef._Name + "@reflect";
						}
						if(GUILayout.Button(objMemDef._ObjType, new GUILayoutOption[] { GUILayout.Width(200f) })) {
							this.mFilter_Text = objMemDef._ObjType;
						}
						if(GUILayout.Button(objMemDef._Name, Array.Empty<GUILayoutOption>())) {
							this.mFilter_Text = objMemDef._Name;
						}
						GUILayout.EndHorizontal();
					}
					renderedLines++;
				}
			}
			GUI.skin.label.alignment = alignment;
			if(extraSpacingLines > 0) {
				GUILayout.Space((float)extraSpacingLines * lineHeight);
			}
			GUILayout.EndScrollView();
		}

		private void AddLastSnapShotElement(ref Dictionary<string, List<BTMemoryProfiler.ObjMemDef>> LastSnapshot, BTMemoryProfiler.ObjMemDef ObjDef) {
			List<BTMemoryProfiler.ObjMemDef> list = null;
			if(!LastSnapshot.TryGetValue(ObjDef._Name, out list)) {
				list = new List<BTMemoryProfiler.ObjMemDef>();
				LastSnapshot[ObjDef._Name] = list;
			}
			list.Add(ObjDef);
		}

		private bool RemoveLastSnapShotElement(ref Dictionary<string, List<BTMemoryProfiler.ObjMemDef>> LastSnapshot, BTMemoryProfiler.ObjMemDef ObjDef) {
			List<BTMemoryProfiler.ObjMemDef> list = null;
			if(!LastSnapshot.TryGetValue(ObjDef._Name, out list)) {
				return false;
			}
			int num = list.FindIndex((BTMemoryProfiler.ObjMemDef p) => ObjDef._ObjType == p._ObjType);
			if(num < 0) {
				return false;
			}
			list.RemoveAt(num);
			return true;
		}

		private void RenderContentInfoRecursive(BTMemoryProfiler.ObjectContentInfo content, int depth, float spaceWidth, ref int renderedLines, int viewPortStartLine, int viewPortEndLine, ref int skippedLines, float lineHeight, string[] reflectPath) {
			if(content == null) {
				return;
			}
			if(renderedLines == 0 || (renderedLines <= viewPortEndLine && renderedLines >= viewPortStartLine)) {
				if(skippedLines > 0) {
					GUILayout.Space(lineHeight * (float)skippedLines);
					skippedLines = 0;
				}
				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				GUILayout.Space(spaceWidth * (float)depth);
				if(content.contentInfo != null) {
					if(GUILayout.Button(content.isFolded ? ">" : "v", new GUILayoutOption[]
					{
					GUILayout.Width(20f)
					})) {
						content.isFolded = !content.isFolded;
					}
				} else {
					GUILayout.Box("o", new GUILayoutOption[]
					{
					GUILayout.Width(20f)
					});
				}
				GUILayout.Box(content.typeText, this.boxReflectFontStyle, new GUILayoutOption[]
				{
				GUILayout.Width(200f)
				});
				GUILayout.Box(content.nameText, this.boxReflectFontStyle, new GUILayoutOption[]
				{
				GUILayout.Width(200f)
				});
				GUILayout.Box(content.valueText, this.boxReflectFontStyle, Array.Empty<GUILayoutOption>());
				GUILayout.EndHorizontal();
			} else {
				skippedLines++;
			}
			renderedLines++;
			if(content.contentInfo != null && !content.isFolded) {
				string pathTypeString = null;
				string pathNameString = null;
				if(reflectPath.Length > (depth + 1)) {
					string pathString = reflectPath[depth + 1];
					if(pathString.EndsWith("@type")) {
						pathTypeString = pathString.Substring(0, pathString.Length - "@type".Length);
					} else {
						pathNameString = pathString;
					}

					if(string.Equals(pathTypeString, "*")) {
						pathTypeString = null;
					}
					if(string.Equals(pathNameString, "*")) {
						pathNameString = null;
					}
				}
				foreach(BTMemoryProfiler.ObjectContentInfo innerContent in content.contentInfo) {
					if(pathTypeString != null && !InputPartiallyMatches(pathTypeString, innerContent.typeString)) {
						continue;
					}
					if(pathNameString != null && !InputPartiallyMatches(pathNameString, innerContent.nameString)) {
						continue;
					}
					this.RenderContentInfoRecursive(innerContent, depth + 1, spaceWidth, ref renderedLines, viewPortStartLine, viewPortEndLine, ref skippedLines, lineHeight, reflectPath);
				}
			}
		}

		/// <summary>
		/// checks whether the inputString contains characters in the same sequence as the targetString
		/// </summary>
		/// <param name="input"></param>
		/// <param name="target"></param>
		/// <returns></returns>
		public static bool InputPartiallyMatches(string input, string target) {
			char[] inputChars = input.ToCharArray();
			int inputCharsLength = inputChars.GetLength(0);
			char[] targetChars = target.ToCharArray();
			int targetCharsLength = targetChars.GetLength(0);
			int targetIndex = 0;
			for(int inputIndex = 0; inputIndex < inputCharsLength; inputIndex++) {
				char inputChar = char.ToUpperInvariant(inputChars[inputIndex]);
				while(inputChar != char.ToUpperInvariant(targetChars[targetIndex])) {
					targetIndex++;
					if(targetIndex >= targetCharsLength) {
						// unable to find matching character in target
						return false;
					}
				}
				targetIndex++;
				if(targetIndex >= targetCharsLength && (inputIndex + 1) < inputCharsLength) {
					// reached end of target string, but still have input chars to check
					return false;
				}
			}
			return true;
		}

		private List<KeyValuePair<string, int>> mStats_Current1 = new List<KeyValuePair<string, int>>();

		private Dictionary<string, int> mStats_Current = new Dictionary<string, int>();

		private List<KeyValuePair<string, int>> mStats_Dif1;

		private Dictionary<string, int> mStats_Dif;

		public GameObject _OwnerGO;

		private Vector2 mScrollViewPos_Stats;

		private BTMemoryProfiler.StatsViewTab mStatsViewIndex;

		private string mFilter_Text;

		private static bool reflectOnProperties = false;

		private bool mFilter_ShowDependencies;

		private bool mCollapsed;

		private GUIStyle boxReflectFontStyle;

		private List<BTMemoryProfiler.ObjMemDef> mList_Snapshot = new List<BTMemoryProfiler.ObjMemDef>();

		private List<BTMemoryProfiler.ObjMemDef> mList_Differences;

		private List<BTMemoryProfiler.ObjMemDef> mList_LastSnapshot;

		private class ObjMemDef
		{
			public ObjMemDef(int Size, string ObjType, string Name, bool IsADependency, object instance) {
				this._Size = Size;
				this._ObjType = ObjType;
				this._Name = Name;
				this._IsADependency = IsADependency;
				this.instance = instance;
				this.contentLoaded = false;
				this.contentInfo = null;
			}

			public BTMemoryProfiler.ObjectContentInfo GetContentInfo() {
				if(!this.contentLoaded) {
					this.contentInfo = BTMemoryProfiler.buildContentInfo(this.instance);
					this.contentLoaded = true;
				}
				return this.contentInfo;
			}

			public int CompareTo(ref BTMemoryProfiler.ObjMemDef other) {
				if(this._Size != other._Size) {
					return this._Size - other._Size;
				}
				if(this._IsADependency != other._IsADependency) {
					return (this._IsADependency ? 1 : 0) - (other._IsADependency ? 1 : 0);
				}
				int num = string.Compare(this._Name, other._Name);
				if(num != 0) {
					return num;
				}
				return string.Compare(this._ObjType, other._ObjType);
			}

			public int _Size;

			public bool _IsADependency;

			public string _ObjType;

			public string _Name;

			private BTMemoryProfiler.ObjectContentInfo contentInfo;

			private object instance;

			private bool contentLoaded;
		}

		private enum StatsViewTab
		{
			CURRENT_STATS,
			CURRENT_OBJECTS,
			DIF_STATS,
			DIF_OBJECTS
		}

		private class ObjectContentInfo
		{
			public ObjectContentInfo(string typeName, string displayName, object value) {
				this.typeString = typeName;
				this.nameString = displayName;
				this.typeText = "type: " + typeName;
				this.nameText = "name: " + displayName;
				this.valueText = ((value == null) ? "null" : value.ToString());
				this.isFolded = false;
			}

			public List<BTMemoryProfiler.ObjectContentInfo> contentInfo;

			public string typeString;
			public string typeText;

			public string nameString;
			public string nameText;

			public string valueText;

			public bool isFolded;
		}
	}
}
