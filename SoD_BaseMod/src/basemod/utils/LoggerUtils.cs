using System;
using System.Diagnostics;
using BepInEx.Logging;

namespace SoD_BaseMod.utils {
	public static class LoggerUtils {
		private static string GetLoggerName(Type containingClass) {
			return $"BT_{containingClass.Name}";
		}

		public static ManualLogSource GetLogger() {
			Type containingClass = new StackFrame(1, false).GetMethod().ReflectedType;
			return Logger.CreateLogSource(GetLoggerName(containingClass));
		}
	}
}