using System.Collections.Generic;
using KA.Framework;
using UnityEngine;

namespace SoD_BaseMod.basemod.console.commands {
	public static class BTBuildInfoCommand {
		public static void Register() {
			BTConsole.AddCommand(new BTConsoleCommand(
					new List<string> { "BuildInfo" },
					new BTNoArgsInput(),
					"prints build information to console",
					OnExecute
			));
		}

		private static void OnExecute(BTConsoleCommand.BTCommandInput input) {
			BTConsole.WriteLine("Unity Project ID - " + Application.cloudProjectId);
			string changeListNumber = ProductSettings.pInstance.GetChangelistNumber();
			string formattedCLNumber = string.IsNullOrEmpty(changeListNumber) ? "not found" : changeListNumber;
			BTConsole.WriteLine("Application.version - " + Application.version);
			BTConsole.WriteLine("Application.unityVersion - " + Application.unityVersion);
			BTConsole.WriteLine("ChangeList Number - " + formattedCLNumber);
			BTConsole.WriteLine("Version - " + ProductConfig.pProductVersion);
			BTConsole.WriteLine("Environment - " + ProductConfig.GetEnvironmentForBundles());
			BTConsole.WriteLine("Current Platform - " + UtPlatform.GetPlatformName());
		}

		// TODO below is testCode for the partLoader

		/*private static int GetMaterialIndexSkin(Renderer meshRenderer, out string texName) {
			texName = AvatarData.pShaderSettings.SHADER_PROP_PART;
			if (meshRenderer == null) {
				return -1;
			}

			Material[] meshMaterials = meshRenderer.materials;
			int materialCount = meshMaterials.Length;
			switch (materialCount) {
				case 0:
					return -1;
				case 1:
					texName = AvatarData.pShaderSettings.SHADER_PROP_NON_GLOBAL_SKIN;
					return 0;
			}

			for (int i = 0; i < materialCount; i++) {
				if (meshMaterials[i].name.Contains("skintex")) {
					return i;
				}
			}

			return -1;
		}

		private static int GetMaterialIndexRank(Renderer meshRenderer) {
			if (meshRenderer == null) {
				return -1;
			}

			Material[] meshMaterials = meshRenderer.materials;
			int materialCount = meshMaterials.Length;
			if (materialCount == 0 || materialCount == 1) {
				return -1;
			}

			for (int i = 0; i < materialCount; i++) {
				if (meshMaterials[i].name.Contains("ranktex")) {
					return i;
				}
			}

			return -1;
		}

		private static void ApplyAvatarMeshSwap(GameObject inAvatar, AvatarData.Geometry geometry, string parentBone, string partType, Texture inSkin) {
			Transform partTransform = inAvatar.transform.Find(parentBone == "" ? partType : parentBone + "/" + partType);
			if (partTransform == null) {
				return;
			}

			Renderer partRenderer = AvatarData.FindRenderer(partTransform);
			var partMeshRenderer = partTransform.GetComponentInChildren<SkinnedMeshRenderer>();
			if (partRenderer == null || partMeshRenderer == null) {
				return;
			}

			partMeshRenderer.sharedMesh = geometry.pMesh;
			if (inSkin != null) {
				int partSkinMaterialIndex = GetMaterialIndexSkin(partMeshRenderer, out string shader_PROP_SKIN);
				if (partSkinMaterialIndex >= 0) {
					partMeshRenderer.materials[partSkinMaterialIndex].SetTexture(shader_PROP_SKIN, inSkin);
				}
			}

			int partRankMaterialIndex = GetMaterialIndexRank(partRenderer);
			int meshRankMaterialIndex = GetMaterialIndexRank(partMeshRenderer);
			if (partRankMaterialIndex >= 0 && meshRankMaterialIndex >= 0) {
				string materialName = AvatarData.pShaderSettings.SHADER_PROP_RANK_COLOR;
				Color partMaterialColor = partRenderer.materials[partRankMaterialIndex].GetColor(materialName);
				partMeshRenderer.materials[meshRankMaterialIndex].SetColor(materialName, partMaterialColor);
			}
		}

		private static Color? ApplyAvatarPartRemovePrevious(GameObject inAvatar, string parentBone, string partType) {
			Transform previousPartTransform = inAvatar.transform.Find(parentBone == "" ? partType : parentBone + "/" + partType);
			Color? previousPartColor = null;
			if (previousPartTransform != null) {
				Renderer partRenderer = AvatarData.FindRenderer(previousPartTransform);
				int partMaterialIndex = GetMaterialIndexRank(partRenderer);
				if (partMaterialIndex >= 0) {
					previousPartColor = partRenderer.materials[partMaterialIndex].GetColor(AvatarData.pShaderSettings.SHADER_PROP_RANK_COLOR);
				}

				previousPartTransform.parent = null;
				Object.Destroy(previousPartTransform.gameObject);
			}

			return previousPartColor;
		}

		private static void ApplyAvatarMeshReplace(GameObject inAvatar, AvatarData.Geometry geometry, string parentBone, string partType, Texture inSkin,
				string partBoneAttribute, Transform parentBoneTransform) {
			Color? previousPartColor = ApplyAvatarPartRemovePrevious(inAvatar, parentBone, partType);
			Transform partBoneTransform = string.IsNullOrEmpty(partBoneAttribute) ? null : UtUtilities.FindChildTransform(inAvatar, partBoneAttribute);
			Transform prefabTransform = partBoneTransform != null
					? UnityEngine.Object.Instantiate(partBoneTransform.gameObject).transform
					: geometry.Instantiate();
			if (prefabTransform == null) {
				UtDebug.LogError("NEW PART CREATE FAILED!!!");
				return;
			}

			prefabTransform.parent = new GameObject(partType) {
					transform = {
							parent = parentBoneTransform,
							localPosition = Vector3.zero,
							localEulerAngles = Vector3.zero,
							localScale = Vector3.one
					}
			}.transform;
			prefabTransform.localPosition = Vector3.zero;
			prefabTransform.localEulerAngles = Vector3.zero;
			prefabTransform.localScale = Vector3.one;

			Renderer prefabRenderer = AvatarData.FindRenderer(prefabTransform);
			if (prefabRenderer == null) {
				return;
			}

			if (!string.IsNullOrEmpty(partBoneAttribute)) {
				var skinnedMeshRenderer = prefabRenderer as SkinnedMeshRenderer;
				if (skinnedMeshRenderer != null) {
					skinnedMeshRenderer.sharedMesh = geometry.pMesh;
					skinnedMeshRenderer.sharedMaterials = geometry.pMaterials;
				}
			}

			if (partType == AvatarData.pPartSettings.AVATAR_PART_HAT || partType == AvatarData.pPartSettings.AVATAR_PART_FACEMASK) {
				Transform headgearTransform = inAvatar.transform.Find(parentBone + "/HairScale_J");
				if (headgearTransform != null) {
					prefabTransform.localPosition = headgearTransform.localPosition;
				}
			}

			if (AvatarData.GetGender() == Gender.Male && (partType == AvatarData.pPartSettings.AVATAR_PART_SHOULDERPAD_LEFT ||
					partType == AvatarData.pPartSettings.AVATAR_PART_SHOULDERPAD_RIGHT)) {
				prefabTransform.localPosition = new Vector3(0f, 0.025f, -0.015f);
			}

			if (partType == AvatarData.pPartSettings.AVATAR_PART_BACK) {
				bool activeSelf = inAvatar.activeSelf;
				inAvatar.SetActive(true);
				prefabTransform.SendMessage("ApplyViewInfo", "AvatarBack", SendMessageOptions.DontRequireReceiver);
				prefabTransform.SendMessage("ApplyPropInfo", inAvatar.name, SendMessageOptions.DontRequireReceiver);
				inAvatar.SetActive(activeSelf);
			}

			if (partType == AvatarData.pPartSettings.AVATAR_PART_HAND_PROP_RIGHT) {
				prefabTransform.SendMessage("ApplyPropInfo", inAvatar.name, SendMessageOptions.DontRequireReceiver);
			}

			if (inSkin != null) {
				int materialIndex2 = GetMaterialIndexSkin(prefabRenderer, out string shader_PROP_SKIN);
				if (materialIndex2 >= 0 && prefabRenderer.materials[materialIndex2].HasProperty(shader_PROP_SKIN)) {
					prefabRenderer.materials[materialIndex2].SetTexture(shader_PROP_SKIN, inSkin);
				}
			}

			if (previousPartColor != null) {
				int materialIndex3 = GetMaterialIndexRank(prefabRenderer);
				if (materialIndex3 >= 0) {
					prefabRenderer.materials[materialIndex3].SetColor(AvatarData.pShaderSettings.SHADER_PROP_RANK_COLOR, previousPartColor.Value);
				}
			}

			int materialCount = prefabRenderer.materials.Length;
			for (int i = 0; i < materialCount; i++) {
				UnityEngine.Object.DontDestroyOnLoad(prefabRenderer.materials[i]);
			}
		}

		public static void ApplyAvatarGeometry(GameObject inAvatar, GameObject inPreFab, string parentBone, string partType, bool inMeshSwap, Texture inSkin,
				string partBoneAttribute) {
			if (!inPreFab) {
				return;
			}

			Transform parentBoneTransform = parentBone != "" ? inAvatar.transform.Find(parentBone) : inAvatar.transform;
			if (!parentBoneTransform) {
				UtDebug.Log("GET PARENT BONE FAILED!!");
				return;
			}

			var geometry = new AvatarData.Geometry { _Name = inPreFab.name, _Prefab = inPreFab.transform };
			if (inMeshSwap) {
				ApplyAvatarMeshSwap(inAvatar, geometry, parentBone, partType, inSkin);
			} else {
				ApplyAvatarMeshReplace(inAvatar, geometry, parentBone, partType, inSkin, partBoneAttribute, parentBoneTransform);
			}
		}*/
	}
}