using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Xml.Serialization;
using HarmonyLib;
using JetBrains.Annotations;
using SoD_BaseMod.basemod;
using SoD_BaseMod.basemod.config;
using BlazingTwist_Core;
using UnityEngine;

namespace SoD_Enable_Logging.AsmFirstpass {
	[UsedImplicitly]
	public class ServiceCallPatcher : RuntimePatcher {
		private static ServiceCallPatcher instance;

		public override void ApplyPatches() {
			instance = this;

			string postprocessCallName = nameof(ServiceCall<object>.PostprocessCall);
			Type[] postprocessCallArgs = { typeof(string) };
			MethodInfo postprocessReferenceType = AccessTools.Method(typeof(ServiceCall<object>), postprocessCallName, postprocessCallArgs);
			MethodInfo postProcessSbyte = AccessTools.Method(typeof(ServiceCall<sbyte>), postprocessCallName, postprocessCallArgs);
			MethodInfo postProcessByte = AccessTools.Method(typeof(ServiceCall<byte>), postprocessCallName, postprocessCallArgs);
			MethodInfo postProcessShort = AccessTools.Method(typeof(ServiceCall<short>), postprocessCallName, postprocessCallArgs);
			MethodInfo postProcessUshort = AccessTools.Method(typeof(ServiceCall<ushort>), postprocessCallName, postprocessCallArgs);
			MethodInfo postProcessInt = AccessTools.Method(typeof(ServiceCall<int>), postprocessCallName, postprocessCallArgs);
			MethodInfo postProcessUint = AccessTools.Method(typeof(ServiceCall<uint>), postprocessCallName, postprocessCallArgs);
			MethodInfo postProcessLong = AccessTools.Method(typeof(ServiceCall<long>), postprocessCallName, postprocessCallArgs);
			MethodInfo postProcessULong = AccessTools.Method(typeof(ServiceCall<ulong>), postprocessCallName, postprocessCallArgs);
			MethodInfo postProcessChar = AccessTools.Method(typeof(ServiceCall<char>), postprocessCallName, postprocessCallArgs);
			MethodInfo postProcessFloat = AccessTools.Method(typeof(ServiceCall<float>), postprocessCallName, postprocessCallArgs);
			MethodInfo postProcessDouble = AccessTools.Method(typeof(ServiceCall<double>), postprocessCallName, postprocessCallArgs);
			MethodInfo postProcessBool = AccessTools.Method(typeof(ServiceCall<bool>), postprocessCallName, postprocessCallArgs);
			MethodInfo postProcessDecimal = AccessTools.Method(typeof(ServiceCall<decimal>), postprocessCallName, postprocessCallArgs);

			var postProcessPostfix = new HarmonyMethod(SymbolExtensions.GetMethodInfo(() => PostprocessCallPostfix(null, null, null, null)));
			var postProcessTranspiler = new HarmonyMethod(GetType(), nameof(PostprocessCallTranspiler), new[] { typeof(IEnumerable<CodeInstruction>) });

			harmony.Patch(postprocessReferenceType, null, postProcessPostfix, postProcessTranspiler);
			harmony.Patch(postProcessSbyte, null, postProcessPostfix, postProcessTranspiler);
			harmony.Patch(postProcessByte, null, postProcessPostfix, postProcessTranspiler);
			harmony.Patch(postProcessShort, null, postProcessPostfix, postProcessTranspiler);
			harmony.Patch(postProcessUshort, null, postProcessPostfix, postProcessTranspiler);
			harmony.Patch(postProcessInt, null, postProcessPostfix, postProcessTranspiler);
			harmony.Patch(postProcessUint, null, postProcessPostfix, postProcessTranspiler);
			harmony.Patch(postProcessLong, null, postProcessPostfix, postProcessTranspiler);
			harmony.Patch(postProcessULong, null, postProcessPostfix, postProcessTranspiler);
			harmony.Patch(postProcessChar, null, postProcessPostfix, postProcessTranspiler);
			harmony.Patch(postProcessFloat, null, postProcessPostfix, postProcessTranspiler);
			harmony.Patch(postProcessDouble, null, postProcessPostfix, postProcessTranspiler);
			harmony.Patch(postProcessBool, null, postProcessPostfix, postProcessTranspiler);
			harmony.Patch(postProcessDecimal, null, postProcessPostfix, postProcessTranspiler);
		}

		private static void PostprocessCallPostfix(string inData, object __instance, ServiceRequest ___mServiceRequest, object ___mProcessCallback) {
			try {
				BTConfig config = BTDebugCamInputManager.GetConfigHolder().config;
				if (config == null || !config.logServiceCalls || config.serviceCallFilter.Contains(___mServiceRequest._Type)) {
					return;
				}

				string targetFile = SodEnableLogging.basePath.Replace('/', Path.DirectorySeparatorChar) + "serviceCallLogger.txt";
				using (var writer = new StreamWriter(targetFile, true)) {
					Type dataType = __instance.GetType().GetGenericArguments()[0];
					object inObject;

					if (___mProcessCallback != null) {
						MethodInfo methodInfo = ___mProcessCallback.GetType().GetMethod("Invoke");
						inObject = methodInfo?.Invoke(___mProcessCallback, new object[] { inData });
					} else {
						inObject = UtUtilities.DeserializeFromXml(inData, dataType);
					}

					writer.WriteLine("ServiceCall: " + ___mServiceRequest._Type);
					writer.WriteLine("URL: " + ___mServiceRequest._URL);
					if (___mServiceRequest._Params != null) {
						writer.WriteLine("Params:");
						foreach (KeyValuePair<string, object> paramPair in ___mServiceRequest._Params) {
							Type valueType = paramPair.Value.GetType();
							if (valueType == typeof(string)) {
								writer.WriteLine("\t" + paramPair.Key + " = " + paramPair.Value);
							} else {
								var stringWriter = new StringWriter(CultureInfo.InvariantCulture);
								var serializer = new XmlSerializer(valueType);

								serializer.Serialize(stringWriter, paramPair.Value);
								string text = stringWriter.ToString().Replace("\r", "");
								string[] lines = text.Split('\n');

								var builder = new StringBuilder();
								for (int i = 1; i < lines.Length; i++) {
									if (lines[i].Contains("xsi:nil")) {
										continue;
									}

									builder.Append(lines[i]);
									if (i < (lines.Length - 1)) {
										builder.Append("\n");
									}
								}

								writer.WriteLine("\t" + paramPair.Key + " = " + builder);
							}
						}
					} else {
						writer.WriteLine("Params: no params");
					}

					if (___mProcessCallback != null || inObject == null) {
						writer.WriteLine("----- rawData -----\n" + inData);
					}

					if (inObject != null) {
						writer.WriteLine("----- responseXML -----\n" + UtUtilities.SerializeToXml(inObject));
					} else {
						writer.WriteLine("responseXML: no response");
					}

					writer.WriteLine("");
				}
			} catch (Exception e) {
				Debug.LogError("serviceCallPostfix caught exception: " + e);
			}
		}

		private static IEnumerable<CodeInstruction> PostprocessCallTranspiler(IEnumerable<CodeInstruction> instructions) {
			MethodInfo getTypeFromHandleMethod = AccessTools.Method(typeof(Type), nameof(Type.GetTypeFromHandle), new[] { typeof(RuntimeTypeHandle) });
			MethodInfo getTypeMethod = AccessTools.Method(typeof(System.Object), nameof(GetType));
			MethodInfo genericTypeArgumentsGetter = AccessTools.PropertyGetter(typeof(Type), nameof(Type.GenericTypeArguments));

			var instructionList = new List<CodeInstruction>(instructions);
			var result = new List<CodeInstruction>();
			int instructionCount = instructionList.Count;
			int found = 0;

			for (int i = 0; i < instructionCount; i++) {
				CodeInstruction instruction = instructionList[i];

				if (instruction.opcode == OpCodes.Ldtoken && i + 1 < instructionCount) {
					CodeInstruction instruction1 = instructionList[i + 1];
					if (instruction1.Calls(getTypeFromHandleMethod)) {
						result.Add(new CodeInstruction(OpCodes.Ldarg_0));
						result.Add(new CodeInstruction(OpCodes.Call, getTypeMethod));
						result.Add(new CodeInstruction(OpCodes.Callvirt, genericTypeArgumentsGetter));
						result.Add(new CodeInstruction(OpCodes.Ldc_I4_0));
						result.Add(new CodeInstruction(OpCodes.Ldelem_Ref));

						found++;
						i += 1;
						continue;
					}
				}

				result.Add(instruction);
			}

			if (found != 1) {
				instance?.logger.LogWarning("PostprocessCall found " + found + " entry-points, but expected 1!");
			}

			return result;
		}
	}
}