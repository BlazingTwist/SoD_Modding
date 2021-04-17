using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SoD_BaseMod.basemod;
using SoD_BaseMod.basemod.config;
using SoD_BlazingTwist_Core;

namespace SoD_BaseMod.asm {
	[UsedImplicitly]
	public class AvAvatarControllerPatcher : RuntimePatcher {
		private static AvAvatarControllerPatcher instance;

		public override void ApplyPatches() {
			instance = this;

			Type originalType = typeof(AvAvatarController);
			Type patcherType = typeof(AvAvatarControllerPatcher);

			MethodInfo setFlyingDataOriginal = AccessTools.PropertySetter(originalType, "pFlyingData");
			MethodInfo keyboardUpdateOriginal = AccessTools.Method(originalType, "KeyboardUpdate");
			MethodInfo getHorizontalFromInputOriginal = AccessTools.Method(originalType, "GetHorizontalFromInput");
			MethodInfo doUpdateOriginal = AccessTools.Method(originalType, "DoUpdate");
			MethodInfo updateFlyingOriginal = AccessTools.Method(originalType, "UpdateFlying", new[] { typeof(float), typeof(float) });
			MethodInfo updateFlyingControlOriginal = AccessTools.Method(originalType, "UpdateFlyingControl", new[] { typeof(float), typeof(float) });

			HarmonyMethod setPFlyingDataPrefix =
					new HarmonyMethod(patcherType, nameof(SetPFlyingDataPrefix), new[] { typeof(AvAvatarFlyingData).MakeByRefType() });
			HarmonyMethod keyboardUpdatePrefix =
					new HarmonyMethod(patcherType, nameof(KeyboardUpdatePrefix));
			HarmonyMethod keyboardUpdateTranspiler =
					new HarmonyMethod(patcherType, nameof(KeyboardUpdateTranspiler), new[] { typeof(IEnumerable<CodeInstruction>) });
			HarmonyMethod getHorizontalFromInputPostfix =
					new HarmonyMethod(patcherType, nameof(GetHorizontalFromInputPostfix), new[] { typeof(float).MakeByRefType() });
			HarmonyMethod doUpdateTranspiler =
					new HarmonyMethod(patcherType, nameof(DoUpdateTranspiler), new[] { typeof(IEnumerable<CodeInstruction>) });
			HarmonyMethod updateFlyingTranspiler =
					new HarmonyMethod(patcherType, nameof(UpdateFlyingTranspiler), new[] { typeof(IEnumerable<CodeInstruction>) });
			HarmonyMethod updateFlyingControlTranspiler =
					new HarmonyMethod(patcherType, nameof(UpdateFlyingControlTranspiler), new[] { typeof(IEnumerable<CodeInstruction>) });

			harmony.Patch(setFlyingDataOriginal, setPFlyingDataPrefix);
			harmony.Patch(keyboardUpdateOriginal, keyboardUpdatePrefix, null, keyboardUpdateTranspiler);
			harmony.Patch(getHorizontalFromInputOriginal, null, getHorizontalFromInputPostfix);
			harmony.Patch(doUpdateOriginal, null, null, doUpdateTranspiler);
			harmony.Patch(updateFlyingOriginal, null, null, updateFlyingTranspiler);
			harmony.Patch(updateFlyingControlOriginal, null, null, updateFlyingControlTranspiler);
		}

		private static void SetPFlyingDataPrefix(ref AvAvatarFlyingData value) {
			BTConfigHolder configHolder = BTDebugCamInputManager.GetConfigHolder();
			BTHackConfig hackConfig = configHolder.hackConfig;
			AvAvatarFlyingData flyingData = configHolder.flightStats;
			if (hackConfig != null && hackConfig.controls_useFlightStatsOverride && flyingData != null) {
				value = flyingData;
			}
		}

		private static bool KeyboardUpdatePrefix() {
			return !BTDebugCam.useDebugCam;
		}

		private static IEnumerable<CodeInstruction> KeyboardUpdateTranspiler(IEnumerable<CodeInstruction> instructions) {
			MethodInfo movementFactorProvider = AccessTools.Method(typeof(AvAvatarControllerPatcher), nameof(GetFastMovementFactor));

			List<CodeInstruction> instructionList = new List<CodeInstruction>(instructions);
			List<CodeInstruction> result = new List<CodeInstruction>();
			int found = 0;
			int instructionCount = instructionList.Count;

			for (int i = 0; i < instructionCount; i++) {
				CodeInstruction inst = instructionList[i];

				if (inst.opcode == OpCodes.Ldarg_0 && (i + 1) < instructionCount) {
					CodeInstruction inst1 = instructionList[i + 1];

					if (inst1.opcode == OpCodes.Ldfld) {
						FieldInfo field1 = inst1.operand as FieldInfo;

						if (field1 != null && field1.DeclaringType == typeof(AvAvatarController) && field1.Name.Equals("mIsBouncing")) {
							CodeInstruction baseInstruction = new CodeInstruction(OpCodes.Call, movementFactorProvider);
							baseInstruction.labels.AddRange(inst.labels);
							inst.labels.Clear();
							result.Add(baseInstruction);
							result.Add(new CodeInstruction(OpCodes.Ldloc_0)); // assuming no-one messed with this
							result.Add(new CodeInstruction(OpCodes.Mul));
							result.Add(new CodeInstruction(OpCodes.Stloc_0));

							found++;
							i += 1;
							result.Add(inst);
							result.Add(inst1);
							continue;
						}
					}
				}

				result.Add(inst);
			}

			if (found != 1) {
				instance?.logger.LogWarning("KeyboardUpdate found " + found + " entrypoints, but expected: 1");
			}

			return result;
		}

		private static void GetHorizontalFromInputPostfix(ref float __result) {
			BTHackConfig hackConfig = BTDebugCamInputManager.GetConfigHolder().hackConfig;
			if (hackConfig != null && hackConfig.controls_useSpeedHacks) {
				__result *= 2f;
			}
		}

		private static IEnumerable<CodeInstruction> DoUpdateTranspiler(IEnumerable<CodeInstruction> instructions) {
			MethodInfo movementFactorProvider = AccessTools.Method(typeof(AvAvatarControllerPatcher), nameof(GetFastMovementFactor));

			List<CodeInstruction> instructionList = new List<CodeInstruction>(instructions);
			List<CodeInstruction> result = new List<CodeInstruction>();
			int found = 0;
			int instructionCount = instructionList.Count;

			for (int i = 0; i < instructionCount; i++) {
				CodeInstruction inst = instructionList[i];

				if (inst.opcode == OpCodes.Call && i + 1 < instructionCount) {
					CodeInstruction inst1 = instructionList[i + 1];

					if (inst1.opcode == OpCodes.Stfld) {
						MethodInfo method0 = inst.operand as MethodInfo;
						FieldInfo field1 = inst1.operand as FieldInfo;

						if (method0 != null && field1 != null
								&& method0.DeclaringType == typeof(AvAvatarController) && method0.Name.Equals("GetJumpVelocity")
								&& field1.DeclaringType == typeof(AvAvatarController) && field1.Name.Equals("mVi")) {
							result.Add(inst);
							result.Add(new CodeInstruction(OpCodes.Call, movementFactorProvider));
							result.Add(new CodeInstruction(OpCodes.Mul));
							result.Add(inst1);
							found++;
							i += 1;
							continue;
						}
					}
				}

				result.Add(inst);
			}

			if (found != 1) {
				instance?.logger.LogWarning("DoUpdate found " + found + " entrypoints, but expected: 1");
			}

			return result;
		}

		private static IEnumerable<CodeInstruction> UpdateFlyingTranspiler(IEnumerable<CodeInstruction> instructions) {
			MethodInfo turnFactorFunction = AccessTools.Method(typeof(AvAvatarControllerPatcher), nameof(GetTurnFactor), new[] { typeof(float) });

			List<CodeInstruction> instructionList = new List<CodeInstruction>(instructions);
			List<CodeInstruction> result = new List<CodeInstruction>();
			int found = 0;
			int instructionCount = instructionList.Count;

			for (int i = 0; i < instructionCount; i++) {
				CodeInstruction inst = instructionList[i];

				if (inst.opcode == OpCodes.Stfld) {
					if (inst.operand is FieldInfo field && field.DeclaringType == typeof(AvAvatarController) && field.Name.Equals("mTurnFactor")) {
						result.Add(new CodeInstruction(OpCodes.Call, turnFactorFunction));
						result.Add(inst);
						found++;
						continue;
					}
				}

				result.Add(inst);
			}

			if (found != 1) {
				instance?.logger.LogWarning("UpdateFlying found " + found + " entrypoints, but expected: 1");
			}

			return result;
		}

		private static IEnumerable<CodeInstruction> UpdateFlyingControlTranspiler(IEnumerable<CodeInstruction> instructions) {
			MethodInfo speedApplier = AccessTools.Method(typeof(AvAvatarControllerPatcher), nameof(ApplySpeedModifiers),
					new[] { typeof(float).MakeByRefType(), typeof(float).MakeByRefType() });
			MethodInfo wingsuitSpeedApplier = AccessTools.Method(typeof(AvAvatarControllerPatcher), nameof(ApplyWingsuitSpeedModifier),
					new[] { typeof(float) });

			List<CodeInstruction> instructionList = new List<CodeInstruction>(instructions);
			List<CodeInstruction> result = new List<CodeInstruction>();
			int found = 0;
			int foundWingsuitSpeedAssignment = 0;
			int instructionCount = instructionList.Count;

			for (int i = 0; i < instructionCount; i++) {
				CodeInstruction inst = instructionList[i];

				if (inst.opcode == OpCodes.Ldarg_0 && (i + 3) < instructionCount) {
					CodeInstruction inst1 = instructionList[i + 1];
					CodeInstruction inst2 = instructionList[i + 2];
					CodeInstruction inst3 = instructionList[i + 3];

					if (inst1.opcode == OpCodes.Ldfld && inst2.opcode == OpCodes.Ldloc_S &&
							(inst3.opcode == OpCodes.Bge_Un_S || inst3.opcode == OpCodes.Bge_Un)) {
						FieldInfo field1 = inst1.operand as FieldInfo;

						if (field1 != null && field1.DeclaringType == typeof(AvAvatarController) && field1.Name.Equals("mFlightSpeed")) {
							CodeInstruction baseInstruction = new CodeInstruction(OpCodes.Ldloca_S, 8);
							baseInstruction.labels.AddRange(inst.labels);
							inst.labels.Clear();
							result.Add(baseInstruction);
							result.Add(new CodeInstruction(OpCodes.Ldloca_S, 10));
							result.Add(new CodeInstruction(OpCodes.Call, speedApplier));

							result.Add(inst);
							result.Add(inst1);
							result.Add(inst2);
							result.Add(inst3);
							found++;
							i += 3;
							continue;
						}
					}
				}

				if (inst.opcode == OpCodes.Stloc || inst.opcode == OpCodes.Stloc_S) {
					if (inst.operand is LocalBuilder local0 && local0.LocalType == typeof(float) && local0.LocalIndex == 4) {
						foundWingsuitSpeedAssignment++;

						if (foundWingsuitSpeedAssignment == 2) {
							CodeInstruction baseInstruction = new CodeInstruction(OpCodes.Call, wingsuitSpeedApplier);
							baseInstruction.labels.AddRange(inst.labels);
							inst.labels.Clear();
							result.Add(baseInstruction);
							result.Add(inst);
							continue;
						}
					}
				}

				result.Add(inst);
			}

			if (found != 1) {
				instance?.logger.LogWarning("UpadteFlyingControl found " + found + " entrypoints, but expected: 1");
			}

			if (foundWingsuitSpeedAssignment != 2) {
				instance?.logger.LogWarning("UpdateFlyingControl found " + foundWingsuitSpeedAssignment + " wingsuit entrypoints, but expected: 2");
			}

			return result;
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