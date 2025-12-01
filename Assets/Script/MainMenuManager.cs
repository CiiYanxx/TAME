using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro; 

// --- 1. Character Selector Component (Attach to each character model) ---
public class CharacterSelector : MonoBehaviour
{
    // Unique ID for this character (e.g., "Cat", "Dog", "Panda")
    public string CharacterId;

    // Provides visual feedback on selection in the scene
    public void Highlight(bool isSelected)
    {
        // Example: Change material color. In a real game, you might enable an outline effect.
        Renderer rend = GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            // Simple color change for demonstration
            rend.material.color = isSelected ? Color.yellow : Color.white;
        }
    }
}

// --- 2. Main Menu Logic Manager (Attach to an Empty GameObject) ---
public class MainMenuManager : MonoBehaviour
{
    #region Data & Setup Fields

    [Header("Game Data Persistence")]
    // Keys used for saving data locally via PlayerPrefs
    private const string SelectedCharacterKey = "SelectedCharacterId";
    private const string PlayerNameKey = "PlayerName";

    [Header("Audio Mixer")]
    // Drag your "MainMixer" AudioMixer asset here
    [SerializeField] private AudioMixer mainAudioMixer;
    private const string MasterVolumeParameter = "MasterVolume"; // Must match the exposed parameter name

    [Header("UI Panels")]
    // Drag your UI panels here
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject charSelectPanel;
    [SerializeField] private GameObject customizationPanel; 
    
    [Header("Character Selection UI")]
    [SerializeField] private TMP_InputField nameInputField;

    [Header("Character Camera")]
    // Drag the Transform of the CharZoomTarget GameObject here
    [SerializeField] private Transform characterZoomPosition; 
    private Camera mainCamera;
    private Vector3 initialCameraPosition;
    private Quaternion initialCameraRotation;

    // State Variables
    private string currentSelectedCharId = ""; // The ID of the currently selected character
    private CharacterSelector highlightedCharacter = null;

    #endregion

    #region Initialization & Data Loading

    private void Awake()
    {
        // 1. Setup Camera References
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            initialCameraPosition = mainCamera.transform.position;
            initialCameraRotation = mainCamera.transform.rotation;
        }

        // 2. Initialize UI states and load persistent data
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        charSelectPanel.SetActive(false);
        if (customizationPanel != null) { customizationPanel.SetActive(false); } 
        
        LoadVolumeSetting();
    }
    
    private void LoadVolumeSetting()
    {
        // Load the saved volume setting
        float initialVolume = PlayerPrefs.GetFloat(MasterVolumeParameter, 0f);
        if (mainAudioMixer != null)
        {
            mainAudioMixer.SetFloat(MasterVolumeParameter, initialVolume);
        }
    }

    #endregion

    #region Core Game Functions (Scene Management)

    public void PlayGame()
    {
        // Guard check: ensure character selection is done first
    
        
        // Always starts a new game with the current selection
        SceneManager.LoadScene("02_GameScene");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Application...");
        Application.Quit();

        // Stops the game in the Editor for easy testing
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    #endregion

    #region UI Panel & Camera Control

    public void OpenSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void OpenCharacterSelection()
    {
        mainMenuPanel.SetActive(false);
        charSelectPanel.SetActive(true);

        // --- CAMERA CONTROL: Zoom In ---
        if (mainCamera != null && characterZoomPosition != null)
        {
            mainCamera.transform.position = characterZoomPosition.position;
            mainCamera.transform.rotation = characterZoomPosition.rotation;
        }
        
        // Set the name input field to the last saved name
        nameInputField.text = PlayerPrefs.GetString(PlayerNameKey, "New Player");
    }

    public void BackToMainMenu()
    {
        charSelectPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        
        // Clear visual highlight on the character
        if (highlightedCharacter != null)
        {
            highlightedCharacter.Highlight(false);
            highlightedCharacter = null;
        }

        // --- CAMERA CONTROL: Zoom Out ---
        if (mainCamera != null)
        {
            mainCamera.transform.position = initialCameraPosition;
            mainCamera.transform.rotation = initialCameraRotation;
        }
    }

    #endregion

    #region Character Selection & Input Handling

    private void Update()
    {
        // Handle input for selection only when the character selection panel is active
        if (charSelectPanel.activeInHierarchy && !customizationPanel.activeInHierarchy && (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)))
        {
            HandleCharacterClick();
        }
    }

    private void HandleCharacterClick()
    {
        Vector3 clickPosition = Input.mousePosition;
        if (Input.touchCount > 0)
        {
            clickPosition = Input.GetTouch(0).position;
        }

        Ray ray = mainCamera.ScreenPointToRay(clickPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            CharacterSelector selector = hit.collider.GetComponent<CharacterSelector>();
            if (selector != null)
            {
                // Unhighlight previous character if different
                if (highlightedCharacter != null && highlightedCharacter != selector)
                {
                    highlightedCharacter.Highlight(false);
                }

                // Update selection state and visual feedback
                currentSelectedCharId = selector.CharacterId;
                highlightedCharacter = selector;
                selector.Highlight(true);
            }
        }
    }

    public void ConfirmCharacterSelection()
    {
        if (string.IsNullOrEmpty(currentSelectedCharId))
        {
            Debug.LogError("Please click and select a character before confirming.");
            return;
        }

        // 1. Save selected character ID
        PlayerPrefs.SetString(SelectedCharacterKey, currentSelectedCharId);
        
        // 2. Save player name
        string playerName = nameInputField.text.Trim();
        if (string.IsNullOrEmpty(playerName))
        {
             playerName = "Tamer"; // Default name if field is empty
        }
        PlayerPrefs.SetString(PlayerNameKey, playerName);
        
        BackToMainMenu();
    }
    
    public void OpenCustomization()
    {
        if (string.IsNullOrEmpty(currentSelectedCharId))
        {
             Debug.LogError("Select a character first before customizing!");
             return;
        }

        Debug.Log($"Opening detailed customization for {currentSelectedCharId}.");
        
        // --- Customization Logic ---
        charSelectPanel.SetActive(false);
        
        if (customizationPanel != null)
        {
            customizationPanel.SetActive(true);
        }
    }

    public void CloseCustomization()
    {
        if (customizationPanel != null)
        {
            customizationPanel.SetActive(false);
        }
        charSelectPanel.SetActive(true);
    }

    #endregion

    #region Settings Logic (Audio)

    public void SetMasterVolume(float value)
    {
        float dB;
        if (value <= 0.0001f) // Converts 0-1 slider value to log scale (-80dB to 0dB)
        {
            dB = -80f; // Mute
        }
        else
        {
            dB = Mathf.Log10(value) * 20;
        }

        mainAudioMixer.SetFloat(MasterVolumeParameter, dB);
        PlayerPrefs.SetFloat(MasterVolumeParameter, dB);
        PlayerPrefs.Save();
    }

    #endregion
}