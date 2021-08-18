using UnityEngine;

namespace SoD_BaseMod
{
	public class BTMemoryProfilerContainer : MonoBehaviour
	{
		private readonly BTMemoryProfiler memProfiler = new BTMemoryProfiler();

		private void Start() {
			memProfiler._OwnerGO = gameObject;
		}

		private void OnGUI() {
			GUILayout.BeginArea(new Rect(0f, 0f, Screen.width, Screen.height));
			memProfiler.RenderGUI();
			GUILayout.EndArea();
		}
	}
}
