﻿using System.Collections.Generic;
using HarmonyLib;
using KA.Framework;

namespace SoD_BaseMod.console {
	public static class BTServerCommands {
		public static void Register() {
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "ServerTime", "get" },
					new BTNoArgsInput(),
					"prints the current ServerTime",
					OnExecuteServerTimeGet
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "ServerTime", "reset" },
					new BTNoArgsInput(),
					"resets the ServerTime and re-enables timeHack prevention",
					OnExecuteServerTimeReset
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "ServerTime", "add" },
					new BTServerTimeAddInput(),
					"adds a time offset to the serverTime and disables timeHack prevention",
					OnExecuteServerTimeAdd
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string>{"Server", "get", "info"},
					new BTNoArgsInput(), 
					"prints all the relevant info for communicating with the servers",
					OnExecuteServerGetInfo));
		}

		private static void OnExecuteServerTimeGet(BTConsoleCommand.BTCommandInput input) {
			BTConsole.WriteLine("Current Server Time: " + ServerTime.pCurrentTime);
		}

		private static void OnExecuteServerTimeReset(BTConsoleCommand.BTCommandInput input) {
			if (TimeHackPrevent.pInstance != null) {
				TimeHackPrevent.pInstance.gameObject.SetActive(true);
			}

			ServerTime.ResetOffsetTime();
			BTConsole.WriteLine("Reset Server Time - Current Server Time is " + ServerTime.pCurrentTime);
		}

		private static void OnExecuteServerTimeAdd(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTServerTimeAddInput) input;
			if (TimeHackPrevent.pInstance != null) {
				TimeHackPrevent.pInstance.gameObject.SetActive(false);
			}

			ServerTime.AddOffsetTime(cmdInput.duration);
			BTConsole.WriteLine("Offset Server Time by: " + cmdInput.duration);
			BTConsole.WriteLine("Server Time now is " + ServerTime.pCurrentTime);
		}

		private class BTServerTimeAddInput : BTConsoleCommand.BTCommandInput {
			public string duration;

			private void SetDuration(object duration, bool isPresent) {
				this.duration = (string) duration;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"duration",
								false,
								"formatted time to offset ServerTime with",
								SetDuration,
								typeof(string)
						)
				};
			}
		}

		private static void OnExecuteServerGetInfo(BTConsoleCommand.BTCommandInput input) {
			BTConsole.WriteLine("apiToken       : '" + WsWebService.pUserToken + "'");
			BTConsole.WriteLine("apiKey         : '" + WsWebService.ApiKey + "'");
			BTConsole.WriteLine("apiSecret      : '" + WsWebService.pSecret + "'");
			BTConsole.WriteLine("apiAsciiSecret : '" + ProductSettings.pInstance.pXMLSecret + "'");
			BTConsole.WriteLine("ticks          : '" + AccessTools.Field(typeof(WsWebService), "_Ticks").GetValue(null) + "'");
		}
	}
}