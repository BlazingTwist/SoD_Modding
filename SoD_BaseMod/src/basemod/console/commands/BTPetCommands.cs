﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SoD_BaseMod.extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SoD_BaseMod.console {
	public static class BTPetCommands {
		public static void Register() {
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Pet", "State" },
					new BTNoArgsInput(),
					"prints debug info of the active pet",
					OnExecutePetState
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Pet", "Get" },
					new BTPetCreateInput(),
					"creates a pet of specified type",
					OnExecutePetGet
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "Pet", "Release" },
					new BTNoArgsInput(),
					"releases the active pet",
					OnExecutePetRelease
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "pet", "meter", "get" },
					new BTPetMeterGetInput(),
					"prints the meterValue of the active pet",
					OnExecutePetMeterGet
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "pet", "meter", "set" },
					new BTPetMeterSetInput(),
					"sets the meterValue of the active pet",
					OnExecutePetMeterSet
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "pet", "age", "get" },
					new BTNoArgsInput(),
					"prints the age of the active pet",
					OnExecutePetAgeGet
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "pet", "age", "set" },
					new BTPetAgeSetInput(),
					"sets the age of the active pet",
					OnExecutePetAgeSet
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "pet", "color", "set" },
					new BTPetColorInput(),
					"sets the color of the pet",
					OnExecutePetColorSet
			));
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "pet", "color", "setf" },
					new BTPetColorFloatInput(),
					"sets the color of the pet",
					OnExecutePetColorSet
			));
		}

		private static void OnExecutePetState(BTConsoleCommand.BTCommandInput input) {
			if (SanctuaryManager.pCurPetInstance == null) {
				BTConsole.WriteLine("error - No Pet found");
				return;
			}

			BTConsole.WriteLine(SanctuaryManager.pCurPetData.GetDebugString());
		}

		private static void OnExecutePetGet(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTPetCreateInput) input;

			SanctuaryPetTypeInfo petTypeInfo = SanctuaryData.FindSanctuaryPetTypeInfo(cmdInput.type);
			if (petTypeInfo == null) {
				BTConsole.WriteLine("No PetTypeInfo found for type: " + cmdInput.type);
				return;
			}

			string prefab = petTypeInfo._AgeData[RaisedPetData.GetAgeIndex(cmdInput.age)]._PetResList[0]._Prefab;
			var raisedPetData = RaisedPetData.CreateCustomizedPetData(petTypeInfo._TypeID, cmdInput.age, prefab, cmdInput.gender, null, true);
			raisedPetData.pNoSave = false;
			raisedPetData.Name = petTypeInfo._Name;
			Object.Destroy(SanctuaryManager.pCurPetInstance);
			SanctuaryManager.pCurPetInstance = null;
			SanctuaryManager.pCurPetData = raisedPetData;
			SanctuaryManager.CreatePet(raisedPetData, Vector3.zero, Quaternion.identity, SanctuaryManager.pInstance.gameObject, "Full");
			SanctuaryManager.pInstance.pSetFollowAvatar = false;
			SanctuaryManager.pCurrentPetType = cmdInput.type;
			SanctuaryManager.pInstance.pCreateInstance = false;
			RaisedPetData.UpdateActivePet(cmdInput.type, raisedPetData);
			BTConsole.WriteLine("Created temporary pet of type: " + cmdInput.type + " | age: " + cmdInput.age + " | gender: " + cmdInput.gender);
		}

		private class BTPetCreateInput : BTConsoleCommand.BTCommandInput {
			public int type;
			public RaisedPetStage age;
			public Gender gender;

			private void SetType(object type, bool isPresent) {
				this.type = isPresent ? (int) type : 11;
			}

			private void SetAge(object age, bool isPresent) {
				this.age = isPresent ? (RaisedPetStage) age : RaisedPetStage.BABY;
			}

			private void SetGender(object gender, bool isPresent) {
				this.gender = isPresent ? (Gender) gender : Gender.Male;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"type",
								true,
								"petTypeID of the pet to create, defaults to 11",
								SetType,
								typeof(int)
						),
						new BTConsoleCommand.BTConsoleArgument(
								"age",
								true,
								"age to provide to the pet creator, defaults to 'BABY', valid input {BABY, CHILD, TEEN, ADULT, TITAN}",
								SetAge,
								typeof(RaisedPetStage)
						),
						new BTConsoleCommand.BTConsoleArgument(
								"gender",
								true,
								"gender to provide to the pet creator, defaults to 'Male', valid input {Male, Female}",
								SetGender,
								typeof(Gender)
						)
				};
			}
		}

		private static void OnExecutePetRelease(BTConsoleCommand.BTCommandInput input) {
			SanctuaryManager.pCurPetData.ReleasePet(null);
			if (SanctuaryManager.pCurPetInstance != null) {
				Object.Destroy(SanctuaryManager.pCurPetInstance.gameObject);
			}

			SanctuaryManager.pCurPetInstance = null;
			BTConsole.WriteLine("Pet Released.");
		}

		private static void OnExecutePetMeterGet(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTPetMeterGetInput) input;
			SanctuaryPet activePet = SanctuaryManager.pCurPetInstance;
			if (activePet == null) {
				BTConsole.WriteLine("error - No Pet found");
				return;
			}

			SanctuaryPetMeterInstance meterInstance = activePet.GetPetMeter(cmdInput.meterType);
			if (meterInstance == null) {
				BTConsole.WriteLine("error - No Meter of type '" + cmdInput.meterType + "' found in active pet!");
				return;
			}

			float meterValue = meterInstance.mMeterValData.Value;
			float maxMeterValue = SanctuaryData.GetMaxMeter(cmdInput.meterType, activePet.pData);
			BTConsole.WriteLine("meterValue of meterType: '" + cmdInput.meterType + "' is: '" + meterValue + "' | maxValue: '" + maxMeterValue + "'");
		}

		private static void OnExecutePetMeterSet(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTPetMeterSetInput) input;
			SanctuaryPet activePet = SanctuaryManager.pCurPetInstance;
			if (activePet == null) {
				BTConsole.WriteLine("error - No Pet found");
				return;
			}

			SanctuaryPetMeterInstance meterInstance = activePet.GetPetMeter(cmdInput.meterType);
			if (meterInstance == null) {
				BTConsole.WriteLine("error - No Meter of type '" + cmdInput.meterType + "' found in active pet!");
				return;
			}

			float meterValue = meterInstance.mMeterValData.Value;
			if (Math.Abs(cmdInput.meterValue - BTPetMeterSetInput.MAX_VALUE) < float.Epsilon) {
				cmdInput.meterValue = SanctuaryData.GetMaxMeter(cmdInput.meterType, activePet.pData);
			}

			activePet.SetMeter(cmdInput.meterType, cmdInput.meterValue, cmdInput.forceUpdate);
			float meterValueNew = meterInstance.mMeterValData.Value;
			BTConsole.WriteLine("changed meterValue of meterType: '" + cmdInput.meterType + "' from: '" + meterValue + "' to: '" + cmdInput.meterValue +
					"' | actualMeterValue: '" + meterValueNew + "'");
		}

		private class BTPetMeterGetInput : BTConsoleCommand.BTCommandInput {
			public SanctuaryPetMeterType meterType;

			private void SetMeterType(object meterType, bool isPresent) {
				this.meterType = (SanctuaryPetMeterType) meterType;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"meterType",
								false,
								"meterType to change - {Happiness, Energy, Racing_Energy, Racing_Fire, Health}",
								SetMeterType,
								typeof(SanctuaryPetMeterType)
						)
				};
			}
		}

		private class BTPetMeterSetInput : BTConsoleCommand.BTCommandInput {
			public SanctuaryPetMeterType meterType;
			public float meterValue;
			public bool forceUpdate;

			public const float MAX_VALUE = -1;

			private void SetMeterType(object meterType, bool isPresent) {
				this.meterType = (SanctuaryPetMeterType) meterType;
			}

			private void SetMeterValue(object meterValue, bool isPresent) {
				this.meterValue = isPresent ? (float) meterValue : MAX_VALUE;
			}

			private void SetForceUpdate(object forceUpdate, bool isPresent) {
				this.forceUpdate = isPresent && (bool) forceUpdate;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"meterType",
								false,
								"meterType to change - {Happiness, Energy, Racing_Energy, Racing_Fire, Health}",
								SetMeterType,
								typeof(SanctuaryPetMeterType)
						),
						new BTConsoleCommand.BTConsoleArgument(
								"meterValue",
								true,
								"value to set the meter to, defaults to the maxValue of the meter (" + MAX_VALUE + ")",
								SetMeterValue,
								typeof(float)
						),
						new BTConsoleCommand.BTConsoleArgument(
								"forceUpdate",
								true,
								"shouldn't be necessary, but there may be some cases where the meterValue is locked.",
								SetForceUpdate,
								typeof(bool)
						)
				};
			}
		}

		private static void OnExecutePetAgeGet(BTConsoleCommand.BTCommandInput input) {
			SanctuaryPet activePet = SanctuaryManager.pCurPetInstance;
			if (activePet == null) {
				BTConsole.WriteLine("error - No Pet found");
				return;
			}

			RaisedPetStage petStage = RaisedPetData.GetGrowthStage(activePet.pAge);
			BTConsole.WriteLine("Current pet-age is: " + petStage);
		}

		private static void OnExecutePetAgeSet(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTPetAgeSetInput) input;
			int ageIndex = RaisedPetData.GetAgeIndex(cmdInput.age);
			SanctuaryPet activePet = SanctuaryManager.pCurPetInstance;
			if (activePet == null) {
				BTConsole.WriteLine("error - No Pet found");
				return;
			}

			RaisedPetStage previousStage = RaisedPetData.GetGrowthStage(activePet.pAge);
			bool success = activePet.SetAge(ageIndex, cmdInput.save, cmdInput.resetSkills);
			if (!success) {
				BTConsole.WriteLine("error - unable to set pet age (unknown cause)");
				return;
			}

			RaisedPetStage currentStage = RaisedPetData.GetGrowthStage(activePet.pAge);
			BTConsole.WriteLine("changed pet age from: '" + previousStage + "' to: '" + currentStage + "'");
		}

		private class BTPetAgeSetInput : BTConsoleCommand.BTCommandInput {
			public RaisedPetStage age;
			public bool save;
			public bool resetSkills;

			private void SetAge(object age, bool isPresent) {
				this.age = (RaisedPetStage) age;
			}

			private void SetSave(object save, bool isPresent) {
				this.save = !isPresent || (bool) save;
			}

			private void SetResetSkills(object resetSkills, bool isPresent) {
				this.resetSkills = !isPresent || (bool) resetSkills;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"age",
								false,
								"age to apply to the active pet, valid input {BABY, CHILD, TEEN, ADULT, TITAN}",
								SetAge,
								typeof(RaisedPetStage)
						),
						new BTConsoleCommand.BTConsoleArgument(
								"save",
								true,
								"default is 'true', I'd recommend leaving it unchanged",
								SetSave,
								typeof(bool)
						),
						new BTConsoleCommand.BTConsoleArgument(
								"resetSkills",
								true,
								"default is 'true', I'd recommend leaving it unchanged",
								SetResetSkills,
								typeof(bool)
						)
				};
			}
		}

		private static void OnExecutePetColorSet(BTConsoleCommand.BTCommandInput input) {
			var cmdInput = (BTPetColorInputBase) input;
			SanctuaryPet activePet = SanctuaryManager.pCurPetInstance;
			Color primary = cmdInput.primary ?? activePet.pData.GetColor(0);
			Color secondary = cmdInput.secondary ?? activePet.pData.GetColor(1);
			Color tertiary = cmdInput.tertiary ?? activePet.pData.GetColor(2);
			activePet.SetColors(primary, secondary, tertiary, cmdInput.saveData);
			BTConsole.WriteLine($"New Pet Colors are: {primary.ToString()} | {secondary.ToString()} | {tertiary.ToString()}");
		}

		private abstract class BTPetColorInputBase : BTConsoleCommand.BTCommandInput {
			public Color? primary;
			public Color? secondary;
			public Color? tertiary;
			public bool saveData;
		}
		
		private class BTPetColorInput : BTPetColorInputBase {
			private static Color? GetColorFromInt(int color) {
				if (color < 0) {
					return null;
				}
				return ColorExtensions.GetColorFromInt(color);
			}

			private void SetPrimary(object color, bool isPresent) {
				primary = isPresent ? GetColorFromInt((int) color) : null;
			}

			private void SetSecondary(object color, bool isPresent) {
				secondary = isPresent ? GetColorFromInt((int) color) : null;
			}

			private void SetTertiary(object color, bool isPresent) {
				tertiary = isPresent ? GetColorFromInt((int) color) : null;
			}

			private void SetSaveData(object value, bool isPresent) {
				saveData = isPresent && (bool) value;
			}

			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"primary",
								false,
								"primary color to set or -1 to keep current color, example: 0xFF0000 is red",
								SetPrimary,
								typeof(int)
						),
						new BTConsoleCommand.BTConsoleArgument(
								"secondary",
								false,
								"secondary color to set or -1 to keep current color, example: 0x00FF00 is green",
								SetSecondary,
								typeof(int)
						),
						new BTConsoleCommand.BTConsoleArgument(
								"tertiary",
								false,
								"tertiary color to set or -1 to keep current color, example: 0x0000FF is blue",
								SetTertiary,
								typeof(int)
						),
						new BTConsoleCommand.BTConsoleArgument(
								"saveData",
								true,
								"store colors on the server, default is false",
								SetSaveData,
								typeof(bool)
						)
				};
			}
		}

		private class BTPetColorFloatInput : BTPetColorInputBase {
			private static Color? GetColorFromString(string color) {
				float[] colorData = color.Split(',')
						.Where(str => !string.IsNullOrWhiteSpace(str))
						.Select(str => float.Parse(str, CultureInfo.InvariantCulture))
						.ToArray();
				if (colorData.Length != 3) {
					return null;
				}
				return new Color(colorData[0], colorData[1], colorData[2]);
			}
			
			private void SetPrimary(object color, bool isPresent) {
				primary = isPresent ? GetColorFromString((string) color) : null;
			}

			private void SetSecondary(object color, bool isPresent) {
				secondary = isPresent ? GetColorFromString((string) color) : null;
			}

			private void SetTertiary(object color, bool isPresent) {
				tertiary = isPresent ? GetColorFromString((string) color) : null;
			}

			private void SetSaveData(object value, bool isPresent) {
				saveData = isPresent && (bool) value;
			}
			
			protected override IEnumerable<BTConsoleCommand.BTConsoleArgument> BuildConsoleArguments() {
				return new List<BTConsoleCommand.BTConsoleArgument> {
						new BTConsoleCommand.BTConsoleArgument(
								"primary",
								false,
								"comma separated RGB primary color OR '-1' to keep current color, example: '1.0,0.5,0' is 0xFF7F00",
								SetPrimary,
								typeof(string)
						),
						new BTConsoleCommand.BTConsoleArgument(
								"secondary",
								false,
								"comma separated RGB secondary color OR '-1' to keep current color",
								SetSecondary,
								typeof(string)
						),
						new BTConsoleCommand.BTConsoleArgument(
								"tertiary",
								false,
								"comma separated RGB tertiary color OR '-1' to keep current color",
								SetTertiary,
								typeof(string)
						),
						new BTConsoleCommand.BTConsoleArgument(
								"saveData",
								true,
								"store colors on the server, default is false",
								SetSaveData,
								typeof(bool)
						)
				};
			}
		}
	}
}