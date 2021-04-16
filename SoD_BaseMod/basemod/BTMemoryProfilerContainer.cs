using System;
using UnityEngine;

namespace SoD_BaseMod.basemod
{
	public class BTMemoryProfilerContainer : MonoBehaviour
	{
		public BTMemoryProfiler memProfiler = new BTMemoryProfiler();

		private void Start() {
			memProfiler._OwnerGO = this.gameObject;
		}

		private void OnGUI() {
			GUILayout.BeginArea(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height));
			memProfiler.RenderGUI();
			GUILayout.EndArea();
		}
	}
}
