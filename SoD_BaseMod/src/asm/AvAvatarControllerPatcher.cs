using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx.Logging;
using BTHarmonyUtils.TranspilerUtils;
using HarmonyLib;
using SoD_BaseMod.config;
using SoD_BaseMod.utils;
using UnityEngine;

namespace SoD_BaseMod {
	[HarmonyPatch(declaringType: typeof(AvAvatarController))]
	public static class AvAvatarControllerPatcher {
		private static readonly ManualLogSource logger = LoggerUtils.GetLogger();

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

			MethodInfo patchMethod_GetFastMovementFactor = SymbolExtensions.GetMethodInfo(() => HackLogic.GetFastMovementFactor());

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
			patch.ApplySafe(instructionList, logger);
			return instructionList;
		}

		[HarmonyPostfix, HarmonyPatch(methodName: "GetHorizontalFromInput", argumentTypes: new Type[] { })]
		private static void GetHorizontalFromInputPostfix(ref float __result) {
			HackLogic.ModifyHorizontalInput(ref __result);
		}

		[HarmonyTranspiler, HarmonyPatch(methodName: nameof(AvAvatarController.DoUpdate), argumentTypes: new Type[] { })]
		private static IEnumerable<CodeInstruction> DoUpdateTranspiler(IEnumerable<CodeInstruction> instructions) {
			List<CodeInstruction> instructionList = new List<CodeInstruction>(instructions);

			FieldInfo field_mVi = AccessTools.Field(typeof(AvAvatarController), "mVi");
			MethodInfo method_GetJumpVelocity = AccessTools.Method(typeof(AvAvatarController), nameof(AvAvatarController.GetJumpVelocity));

			MethodInfo patchMethod_GetFastMovementFactor = SymbolExtensions.GetMethodInfo(() => HackLogic.GetFastMovementFactor());

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
			patch.ApplySafe(instructionList, logger);
			return instructionList;
		}

		[HarmonyTranspiler, HarmonyPatch(methodName: "UpdateFlying", argumentTypes: new[] { typeof(float), typeof(float) })]
		private static IEnumerable<CodeInstruction> UpdateFlyingTranspiler(IEnumerable<CodeInstruction> instructions) {
			List<CodeInstruction> instructionList = new List<CodeInstruction>(instructions);

			FieldInfo field_mTurnFactor = AccessTools.Field(typeof(AvAvatarController), "mTurnFactor");

			MethodInfo patchMethod_GetTurnFactor = SymbolExtensions.GetMethodInfo(() => HackLogic.GetTurnFactor(0));

			CodeReplacementPatch patch = new CodeReplacementPatch(
					expectedMatches: 1,
					insertInstructionSequence: new List<CodeInstruction> {
							new CodeInstruction(OpCodes.Call, patchMethod_GetTurnFactor)
					},
					postfixInstructionSequence: new List<CodeInstruction> {
							new CodeInstruction(OpCodes.Stfld, field_mTurnFactor)
					}
			);
			patch.ApplySafe(instructionList, logger);
			return instructionList;
		}

		[HarmonyTranspiler, HarmonyPatch(methodName: "UpdateFlyingControl", argumentTypes: new[] { typeof(float), typeof(float) })]
		private static IEnumerable<CodeInstruction> UpdateFlyingControlTranspiler(IEnumerable<CodeInstruction> instructions) {
			List<CodeInstruction> instructionList = new List<CodeInstruction>(instructions);

			FieldInfo field_mFlightSpeed = AccessTools.Field(typeof(AvAvatarController), "mFlightSpeed");
			MethodInfo method_Max = AccessTools.Method(typeof(Mathf), nameof(Mathf.Max), new[] { typeof(float), typeof(float) });

			float ref_val = 0;
			MethodInfo patchMethod_ApplySpeedModifiers = SymbolExtensions.GetMethodInfo(() => HackLogic.ApplySpeedModifiers(ref ref_val, ref ref_val));
			MethodInfo patchMethod_ApplyWingsuitSpeedModifier = SymbolExtensions.GetMethodInfo(() => HackLogic.ApplyWingsuitSpeedModifier(0));

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
			speedModifierPatch.ApplySafe(instructionList, logger);
			wingsuitSpeedModifierPatch.ApplySafe(instructionList, logger);
			return instructionList;
		}
	}
}