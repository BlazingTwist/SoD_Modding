using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx.Logging;
using BTHarmonyUtils.TranspilerUtils;
using HarmonyLib;
using SoD_BaseMod.config;
using UnityEngine;

namespace SoD_BaseMod {
	[HarmonyPatch(declaringType: typeof(AvAvatarController))]
	public static class AvAvatarControllerPatcher {
		private static readonly string loggerName = $"BT_{MethodBase.GetCurrentMethod().DeclaringType?.Name}";
		private static ManualLogSource Logger => _logger ?? (_logger = BepInEx.Logging.Logger.CreateLogSource(loggerName));
		private static ManualLogSource _logger;

		[HarmonyPrefix, HarmonyPatch(methodName: "set_" + nameof(AvAvatarController.pFlyingData), argumentTypes: new[] { typeof(AvAvatarFlyingData) })]
		private static void SetPFlyingDataPrefix(ref AvAvatarFlyingData value) {
			BTConfigHolder configHolder = BTDebugCamInputManager.GetConfigHolder();
			BTHackConfig hackConfig = configHolder.hackConfig;
			AvAvatarFlyingData flyingData = configHolder.flightStats;
			if (hackConfig != null && hackConfig.controls_useFlightStatsOverride && flyingData != null) {
				value = flyingData;
			}
		}

		[HarmonyPrefix, HarmonyPatch(methodName: "KeyboardUpdate", argumentTypes: new Type[] { })]
		private static bool KeyboardUpdatePrefix() {
			return !BTDebugCam.useDebugCam;
		}

		[HarmonyTranspiler, HarmonyPatch(methodName: "KeyboardUpdate", argumentTypes: new Type[] { })]
		private static IEnumerable<CodeInstruction> KeyboardUpdateTranspiler(IEnumerable<CodeInstruction> instructions) {
			List<CodeInstruction> instructionList = new List<CodeInstruction>(instructions);

			FieldInfo field_mIsBouncing = AccessTools.Field(typeof(AvAvatarController), "mIsBouncing");

			MethodInfo patchMethod_GetFastMovementFactor = AccessTools.Method(typeof(AvAvatarControllerPatcher), nameof(GetFastMovementFactor));

			CodeReplacementPatch patch = new CodeReplacementPatch(
					expectedMatches: 1,
					insertInstructionSequence: new List<CodeInstruction> {
							new CodeInstruction(OpCodes.Call, patchMethod_GetFastMovementFactor),
							new CodeInstruction(OpCodes.Ldloc_0),
							new CodeInstruction(OpCodes.Mul),
							new CodeInstruction(OpCodes.Stloc_0)
					},
					postfixInstructionSequence: new List<CodeInstruction> {
							new CodeInstruction(OpCodes.Ldarg_0),
							new CodeInstruction(OpCodes.Ldfld, field_mIsBouncing)
					}
			);
			patch.ApplySafe(instructionList, Logger);
			return instructionList;
		}

		[HarmonyPostfix, HarmonyPatch(methodName: "GetHorizontalFromInput", argumentTypes: new Type[] { })]
		private static void GetHorizontalFromInputPostfix(ref float __result) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if (hackConfig != null && hackConfig.controls_useSpeedHacks) {
				__result *= 2f;
			}
		}

		[HarmonyTranspiler, HarmonyPatch(methodName: nameof(AvAvatarController.DoUpdate), argumentTypes: new Type[] { })]
		private static IEnumerable<CodeInstruction> DoUpdateTranspiler(IEnumerable<CodeInstruction> instructions) {
			List<CodeInstruction> instructionList = new List<CodeInstruction>(instructions);

			FieldInfo field_mVi = AccessTools.Field(typeof(AvAvatarController), "mVi");
			MethodInfo method_GetJumpVelocity = AccessTools.Method(typeof(AvAvatarController), nameof(AvAvatarController.GetJumpVelocity));

			MethodInfo patchMethod_GetFastMovementFactor = AccessTools.Method(typeof(AvAvatarControllerPatcher), nameof(GetFastMovementFactor));

			CodeReplacementPatch patch = new CodeReplacementPatch(
					expectedMatches: 1,
					prefixInstructionSequence: new List<CodeInstruction> {
							new CodeInstruction(OpCodes.Call, method_GetJumpVelocity)
					},
					insertInstructionSequence: new List<CodeInstruction> {
							new CodeInstruction(OpCodes.Call, patchMethod_GetFastMovementFactor),
							new CodeInstruction(OpCodes.Mul)
					},
					postfixInstructionSequence: new List<CodeInstruction> {
							new CodeInstruction(OpCodes.Stfld, field_mVi)
					}
			);
			patch.ApplySafe(instructionList, Logger);
			return instructionList;
		}

		[HarmonyTranspiler, HarmonyPatch(methodName: "UpdateFlying", argumentTypes: new[] { typeof(float), typeof(float) })]
		private static IEnumerable<CodeInstruction> UpdateFlyingTranspiler(IEnumerable<CodeInstruction> instructions) {
			List<CodeInstruction> instructionList = new List<CodeInstruction>(instructions);

			FieldInfo field_mTurnFactor = AccessTools.Field(typeof(AvAvatarController), "mTurnFactor");

			MethodInfo patchMethod_GetTurnFactor = AccessTools.Method(typeof(AvAvatarControllerPatcher), nameof(GetTurnFactor), new[] { typeof(float) });

			CodeReplacementPatch patch = new CodeReplacementPatch(
					expectedMatches: 1,
					insertInstructionSequence: new List<CodeInstruction> {
							new CodeInstruction(OpCodes.Call, patchMethod_GetTurnFactor)
					},
					postfixInstructionSequence: new List<CodeInstruction> {
							new CodeInstruction(OpCodes.Stfld, field_mTurnFactor)
					}
			);
			patch.ApplySafe(instructionList, Logger);
			return instructionList;
		}

		[HarmonyTranspiler, HarmonyPatch(methodName: "UpdateFlyingControl", argumentTypes: new[] { typeof(float), typeof(float) })]
		private static IEnumerable<CodeInstruction> UpdateFlyingControlTranspiler(IEnumerable<CodeInstruction> instructions) {
			List<CodeInstruction> instructionList = new List<CodeInstruction>(instructions);

			FieldInfo field_mFlightSpeed = AccessTools.Field(typeof(AvAvatarController), "mFlightSpeed");
			MethodInfo method_Max = AccessTools.Method(typeof(Mathf), nameof(Mathf.Max), new[] { typeof(float), typeof(float) });

			MethodInfo patchMethod_ApplySpeedModifiers = AccessTools.Method(typeof(AvAvatarControllerPatcher), nameof(ApplySpeedModifiers),
					new[] { typeof(float).MakeByRefType(), typeof(float).MakeByRefType() });
			MethodInfo patchMethod_ApplyWingsuitSpeedModifier = AccessTools.Method(typeof(AvAvatarControllerPatcher), nameof(ApplyWingsuitSpeedModifier),
					new[] { typeof(float) });

			// replace
			//  if (this.mFlightSpeed < num5)
			// with
			//  ApplySpeedModifiers(ref num5, ref num7)
			//  if (this.mFlightSpeed < num5)
			CodeReplacementPatch speedModifierPatch = new CodeReplacementPatch(
					expectedMatches: 1,
					insertInstructionSequence: new List<CodeInstruction> {
							new CodeInstruction(OpCodes.Ldloca_S, 8),
							new CodeInstruction(OpCodes.Ldloca_S, 10),
							new CodeInstruction(OpCodes.Call, patchMethod_ApplySpeedModifiers)
					},
					postfixInstructionSequence: new List<CodeInstruction> {
							new CodeInstruction(OpCodes.Ldarg_0),
							new CodeInstruction(OpCodes.Ldfld, field_mFlightSpeed),
							new CodeInstruction(OpCodes.Ldloc_S, 8),
							new CodeInstruction(OpCodes.Bge_Un_S)
					}
			);
			
			// replace
			//  num8 = Mathf.Max(1f, num8)
			// with
			//  num8 = ApplyWingsuitSpeedModifier(Mathf.Max(1f, num8))
			CodeReplacementPatch wingsuitSpeedModifierPatch = new CodeReplacementPatch(
					expectedMatches: 1,
					prefixInstructionSequence: new List<CodeInstruction> {
							new CodeInstruction(OpCodes.Ldc_R4, 1),
							new CodeInstruction(OpCodes.Ldloc_S, 4),
							new CodeInstruction(OpCodes.Call, method_Max)
					},
					insertInstructionSequence: new List<CodeInstruction> {
							new CodeInstruction(OpCodes.Call, patchMethod_ApplyWingsuitSpeedModifier)
					},
					postfixInstructionSequence: new List<CodeInstruction> {
							new CodeInstruction(OpCodes.Stloc_S, 4)
					}
			);
			speedModifierPatch.ApplySafe(instructionList, Logger);
			wingsuitSpeedModifierPatch.ApplySafe(instructionList, Logger);
			return instructionList;
		}

		public static float GetFastMovementFactor() {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if (hackConfig != null && hackConfig.controls_fastMovementFactor > 0 && BTDebugCamInputManager.IsKeyDown("DebugCamFastMovement")) {
				return hackConfig.controls_fastMovementFactor;
			}

			return 1f;
		}

		public static float GetTurnFactor(float fallback) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if (hackConfig != null && hackConfig.controls_useSpeedHacks) {
				return 1f;
			}

			return fallback;
		}

		public static void ApplySpeedModifiers(ref float maxSpeed, ref float acceleration) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if (hackConfig != null && hackConfig.controls_useSpeedHacks) {
				maxSpeed *= 10f;
				acceleration *= 3f;
			}
		}

		public static float ApplyWingsuitSpeedModifier(float speed) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if (hackConfig != null && hackConfig.controls_useSpeedHacks) {
				return speed * 5f;
			}

			return speed;
		}
	}
}