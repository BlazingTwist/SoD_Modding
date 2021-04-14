using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Text;
using System.Globalization;
using System.Xml.Serialization;
using HarmonyLib;
using SoD_BlazingTwist_Core;

namespace SoD_Enable_Logging.AsmFirstpass
{
	class ServiceCallPatcher : RuntimePatcher
	{
		public override void ApplyPatches() {
			Type originalType = new ServiceCall<Object>().GetType();
			Type patcherType = typeof(ServiceCallPatcher);

			/*MethodInfo original = AccessTools.Method(originalType, "PostprocessCall", new Type[] { typeof(string) });
			MethodInfo patch = AccessTools.Method(patcherType, "PostprocessCall", new Type[] { typeof(string), typeof(object), typeof(ServiceRequest), typeof(object) });
			harmony.Patch(original, null, new HarmonyMethod(patch));*/

			MethodInfo original = AccessTools.Method(originalType, "ProcessDecrypt", new Type[] { typeof(string) });
			MethodInfo patch = AccessTools.Method(patcherType, "ProcessDecrypt", new Type[] { typeof(string), typeof(object) });
			harmony.Patch(original, null, new HarmonyMethod(patch));

			logger.LogInfo("ServiceCall: finished patching");
		}

		public static void ProcessDecrypt(string inData, object __result) {
			UnityEngine.Debug.Log("ProcessDecrypt called with inData: " + inData + " | result: " + __result);
		}

		public static void PostprocessCall(string inData, object __instance, ServiceRequest ___mServiceRequest, object ___mProcessCallback) {
			if(!SodEnableLogging.IsServiceCallLoggingEnabled()) {
				return;
			}

			if(___mServiceRequest == null || SodEnableLogging.GetFilteredServiceCalls().Contains(___mServiceRequest._Type)) {
				return;
			}

			using(StreamWriter writer = new StreamWriter(SodEnableLogging.basePath.Replace('/', Path.DirectorySeparatorChar) + "serviceCallLogger.txt", true)) {
				Type dataType = __instance.GetType().DeclaringType.GetGenericArguments()[0];
				object inObject;

				if(___mProcessCallback != null) {
					MethodInfo methodInfo = ___mProcessCallback.GetType().GetMethod("Invoke");
					inObject = methodInfo.Invoke(___mProcessCallback, new object[] { inData });
				} else {
					inObject = UtUtilities.DeserializeFromXml(inData, dataType);
				}

				writer.WriteLine("ServiceCall: " + ___mServiceRequest._Type);
				writer.WriteLine("URL: " + ___mServiceRequest._URL);
				if(___mServiceRequest._Params != null) {
					writer.WriteLine("Params:");
					foreach(KeyValuePair<string, object> paramPair in ___mServiceRequest._Params) {
						StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
						XmlSerializer serializer = new XmlSerializer(paramPair.Value.GetType());

						serializer.Serialize(stringWriter, paramPair.Value);
						string text = stringWriter.ToString().Replace("\r", "");
						string[] lines = text.Split(new char[] { '\n' });

						StringBuilder builder = new StringBuilder();
						for(int i = 1; i < lines.Length; i++) {
							if(lines[i].Contains("xsi:nil")) {
								continue;
							}
							builder.Append(lines[i]);
							if(i < (lines.Length - 1)) {
								builder.Append("\n");
							}
						}
						writer.WriteLine("\t" + paramPair.Key + " = " + builder.ToString());
					}
				} else {
					writer.WriteLine("Params: no params");
				}

				if(___mProcessCallback != null || inObject == null) {
					writer.WriteLine("----- rawData -----\n" + inData);
				}

				if(inObject != null) {
					writer.WriteLine("----- responseXML -----\n" + UtUtilities.SerializeToXml(inObject, false));
				} else {
					writer.WriteLine("responseXML: no response");
				}
				writer.WriteLine("");
			}
		}
	}
}
