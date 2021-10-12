using UnityEngine;

namespace SoD_BaseMod.extensions {
	public static class ColorExtensions {
		public static Color GetColorFromInt(int color) {
			return new Color((float)(color >> 16 & 0xFF) / 255, (float)(color >> 8 & 0xFF) / 255, (float)(color & 0xFF) / 255);
		}
	}
}