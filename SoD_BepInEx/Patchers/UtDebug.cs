using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoD_BepInEx
{
	public class UtDebug
	{
		public static IEnumerable<string> TargetDLLs => GetDLLs();

		public static IEnumerable<string> GetDLLs() {
			yield return "Assembly-CSharp-firstpass.dll";
		}

		public static void Patch(AssemblyDefinition assembly) {
			assembly.
		}
	}
}
