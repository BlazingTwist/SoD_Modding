using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using HarmonyLib;
using JetBrains.Annotations;
using SoD_BaseMod.basemod;
using SoD_BaseMod.basemod.config;
using SoD_BlazingTwist_Core;
using UnityEngine;

namespace SoD_Enable_Logging.AsmFirstpass {
	[UsedImplicitly]
	class ServiceCallPatcher : RuntimePatcher {
		public override void ApplyPatches() {
			return;
			
			string postprocessCall = nameof(ServiceCall<object>.PostprocessCall);
			Type[] argumentTypes = { typeof(string) };
			MethodInfo postProcessUserInfo = AccessTools.Method(typeof(ServiceCall<UserInfo>), postprocessCall, argumentTypes);
			MethodInfo postProcessSubscriptionInfo = AccessTools.Method(typeof(ServiceCall<SubscriptionInfo>), postprocessCall, argumentTypes);
			MethodInfo postProcessInt = AccessTools.Method(typeof(ServiceCall<int>), postprocessCall, argumentTypes);

			HarmonyMethod postProcessPrefix = new HarmonyMethod(typeof(ServiceCallPatcher), nameof(PostprocessCallPrefix),
					new[] { typeof(object) });
			HarmonyMethod postProcessPostfix = new HarmonyMethod(typeof(ServiceCallPatcher), nameof(PostprocessCallPostfix),
					new[] { typeof(string), typeof(object), typeof(ServiceRequest), typeof(object) });

			harmony.Patch(postProcessUserInfo, postProcessPrefix, postProcessPostfix);
			harmony.Patch(postProcessSubscriptionInfo, postProcessPrefix, postProcessPostfix);
			harmony.Patch(postProcessInt, postProcessPrefix, postProcessPostfix);
		}

		public static void PostprocessCallPrefix(object __instance) {
			if (__instance == null) {
				Debug.LogError("PostprocessCall prefix | instance is null!");
				return;
			}

			Type type = __instance.GetType();
			Debug.LogError("PostProcessCall prefix | type is: " + type);
		}

		public static void PostprocessCallPostfix(string inData, object __instance, ServiceRequest ___mServiceRequest, object ___mProcessCallback) {
			try {
				BTConfig config = BTDebugCamInputManager.GetConfigHolder().config;
				if (config == null || !config.logServiceCalls || config.serviceCallFilter.Contains(___mServiceRequest._Type)) {
					return;
				}

				string targetFile = SodEnableLogging.basePath.Replace('/', Path.DirectorySeparatorChar) + "serviceCallLogger.txt";
				Debug.LogError("PostprocessCall postfix | targetFile: " + targetFile);
				using (StreamWriter writer = new StreamWriter(targetFile, true)) {
					Type dataType = __instance.GetType().GetGenericArguments()[0];
					object inObject;

					if (___mProcessCallback != null) {
						MethodInfo methodInfo = ___mProcessCallback.GetType().GetMethod("Invoke");
						inObject = methodInfo.Invoke(___mProcessCallback, new object[] { inData });
					} else {
						inObject = UtUtilities.DeserializeFromXml(inData, dataType);
					}

					writer.WriteLine("ServiceCall: " + ___mServiceRequest._Type);
					writer.WriteLine("URL: " + ___mServiceRequest._URL);
					if (___mServiceRequest._Params != null) {
						writer.WriteLine("Params:");
						foreach (KeyValuePair<string, object> paramPair in ___mServiceRequest._Params) {
							StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
							XmlSerializer serializer = new XmlSerializer(paramPair.Value.GetType());

							serializer.Serialize(stringWriter, paramPair.Value);
							string text = stringWriter.ToString().Replace("\r", "");
							string[] lines = text.Split('\n');

							StringBuilder builder = new StringBuilder();
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
	}
}