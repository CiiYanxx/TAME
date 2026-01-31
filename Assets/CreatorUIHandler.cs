using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // Required for switching scenes

public class CustomizationUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterCustomizer customizer;
    [SerializeField] private TMP_InputField nameInputField; 

    [Header("Customization Buttons")]
    [SerializeField] private Button skinBtn;
    [SerializeField] private Button hairBtn;
    [SerializeField] private Button shirtBtn;
    [SerializeField] private Button shortsBtn;
    [SerializeField] private Button shoesBtn;

    [Header("Action Buttons")]
    [SerializeField] private Button saveBtn;
    [SerializeField] private Button backBtn; // New Back Button
    
    [Header("Scene Names")]
    [SerializeField] private string nextSceneName = "GameScene";
    [SerializeField] private string mainMenuSceneName = "MainMenu"; // Name of your start screen

    private void Start()
    {
        // ... previous listeners ...
        skinBtn.onClick.AddListener(() => customizer.ChangePart(CharacterCustomizer.BodyPartType.Skin));
        hairBtn.onClick.AddListener(() => customizer.ChangePart(CharacterCustomizer.BodyPartType.Hair));
        shirtBtn.onClick.AddListener(() => customizer.ChangePart(CharacterCustomizer.BodyPartType.Shirt));
        shortsBtn.onClick.AddListener(() => customizer.ChangePart(CharacterCustomizer.BodyPartType.Shorts));
        shoesBtn.onClick.AddListener(() => customizer.ChangePart(CharacterCustomizer.BodyPartType.Shoes));
        
        // Setup Action Buttons
        saveBtn.onClick.AddListener(HandleSaveAndNextScene);
        
        // Setup Back Button
        if (backBtn != null)
            backBtn.onClick.AddListener(ReturnToMainMenu);

        customizer.LoadCharacter();
        
        if(PlayerPrefs.HasKey("CharacterName"))
            nameInputField.text = PlayerPrefs.GetString("CharacterName");
    }

    private void HandleSaveAndNextScene()
    {
        PlayerPrefs.SetString("CharacterName", nameInputField.text);
        customizer.SaveCharacter();
        SceneManager.LoadScene(nextSceneName);
    }

    private void ReturnToMainMenu()
    {
        // We don't save here because the user is "going back"
        SceneManager.LoadScene(mainMenuSceneName);
    }
}