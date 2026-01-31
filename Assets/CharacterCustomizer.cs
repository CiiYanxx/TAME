using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;

public class CharacterCustomizer : MonoBehaviour
{
    public enum CustomizationType { ToggleObject, MaterialSwap, MeshSwap }
    public enum BodyPartType { Skin, Hair, Eyes, Shirt, Shorts, Shoes }

    [Serializable]
    public class CustomizationData
    {
        public BodyPartType partType;
        public CustomizationType changeMethod;
        
        [Header("Toggling Objects (For your Hidden Shirts/Shorts)")]
        public GameObject[] objectOptions; // Drag JERSEY_TOP, LONG_SLEEVE, etc. here

        [Header("Swapping Materials (For Skin/Shoes)")]
        public SkinnedMeshRenderer targetRenderer;
        public Material[] materialOptions;

        [Header("Swapping Meshes (For Hair)")]
        public Mesh[] meshOptions;
        
        [HideInInspector] public int currentIndex = 0;
    }

    [SerializeField] private List<CustomizationData> customizationParts = new List<CustomizationData>();
    public TMP_InputField nameInputField;

    private void Awake()
    {
        LoadCharacter();
    }

    public void ChangePart(BodyPartType type)
    {
        CustomizationData data = customizationParts.Find(x => x.partType == type);
        if (data == null) return;

        // Move to next index
        int maxOptions = GetOptionCount(data);
        if (maxOptions == 0) return;
        
        data.currentIndex = (data.currentIndex + 1) % maxOptions;

        ApplyVisuals(data);
    }

    private int GetOptionCount(CustomizationData data)
    {
        if (data.changeMethod == CustomizationType.ToggleObject) return data.objectOptions.Length;
        if (data.changeMethod == CustomizationType.MaterialSwap) return data.materialOptions.Length;
        return data.meshOptions.Length;
    }

    private void ApplyVisuals(CustomizationData data)
    {
        switch (data.changeMethod)
        {
            case CustomizationType.ToggleObject:
                // Disable all, then enable the selected one
                for (int i = 0; i < data.objectOptions.Length; i++)
                {
                    data.objectOptions[i].SetActive(i == data.currentIndex);
                }
                break;

            case CustomizationType.MaterialSwap:
                if (data.targetRenderer != null)
                    data.targetRenderer.material = data.materialOptions[data.currentIndex];
                break;

            case CustomizationType.MeshSwap:
                if (data.targetRenderer != null)
                    data.targetRenderer.sharedMesh = data.meshOptions[data.currentIndex];
                break;
        }
    }

    public void SaveCharacter()
    {
        string saveData = "";
        foreach (var part in customizationParts)
        {
            saveData += part.currentIndex + ",";
        }
        PlayerPrefs.SetString("Character_Save", saveData);
        if (nameInputField != null) PlayerPrefs.SetString("Character_Name", nameInputField.text);
        PlayerPrefs.Save();
    }

    public void LoadCharacter()
    {
        if (!PlayerPrefs.HasKey("Character_Save")) return;

        string[] savedIndices = PlayerPrefs.GetString("Character_Save").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < customizationParts.Count; i++)
        {
            if (i < savedIndices.Length && int.TryParse(savedIndices[i], out int index))
            {
                customizationParts[i].currentIndex = index;
                ApplyVisuals(customizationParts[i]);
            }
        }
    }
}