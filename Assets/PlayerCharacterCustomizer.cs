using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;

public class PlayerCharacterCustomizer : MonoBehaviour
{
    [Serializable]
    public class BodyPartData {
        public BodyPartType bodyPartType;
        public SkinnedMeshRenderer skinnedMeshRenderer;
        public Mesh[] meshArray;
        [HideInInspector] public int currentIndex = 0;
    }

    public enum BodyPartType { Hair, ClothesTop, ClothesBottom, SkinColor }

    [SerializeField] private List<BodyPartData> bodyPartDataList;
    [SerializeField] private TMP_InputField nameInputField;

    // Changes the mesh and saves the index
    public void ChangeBodyPart(BodyPartType type) {
        BodyPartData data = bodyPartDataList.Find(x => x.bodyPartType == type);
        if (data == null || data.meshArray.Length == 0) return;

        data.currentIndex = (data.currentIndex + 1) % data.meshArray.Length;
        data.skinnedMeshRenderer.sharedMesh = data.meshArray[data.currentIndex];
    }

    // Aligns with the Save Logic at [00:20:58]
    public void SaveCharacter() {
        // Save Name
        string pName = string.IsNullOrEmpty(nameInputField.text) ? "Player" : nameInputField.text;
        PlayerPrefs.SetString("PlayerName", pName);

        // Save Body Part Indices
        foreach (var part in bodyPartDataList) {
            PlayerPrefs.SetInt("Saved_" + part.bodyPartType.ToString(), part.currentIndex);
        }
        
        PlayerPrefs.Save();
    }
}