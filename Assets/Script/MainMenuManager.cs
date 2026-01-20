using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject optionsPanel;

    [Header("Options UI References")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Toggle muteToggle;

    private void Start()
    {
        // Initial UI state
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (optionsPanel != null) optionsPanel.SetActive(false);

        // Sync with AudioManager (matches PauseController)
        if (AudioManager.Instance != null && AudioManager.Instance.musicSource != null)
        {
            if (volumeSlider != null) volumeSlider.value = AudioManager.Instance.musicSource.volume;
            if (muteToggle != null) muteToggle.isOn = AudioManager.Instance.musicSource.mute;
        }

        // Add Listeners for audio changes
        if (volumeSlider != null) volumeSlider.onValueChanged.AddListener(SetMasterVolume);
        if (muteToggle != null) muteToggle.onValueChanged.AddListener(ToggleMusicMute);
    }

    #region Scene Navigation Buttons

    // 1. PLAY BUTTON (From Main Menu to Customization)
    public void PlayGame()
    {
        SceneManager.LoadScene("Customize Character"); 
    }

    // 2. CUSTOMIZATION COMPLETE BUTTON (From Scene 3 to Gameplay)
    // Attach this to the "Done" or "Start" button in your Customization Scene
    public void CustomizationComplete()
    {
        SceneManager.LoadScene("02_GameScene"); 
    }

    // 3. TUTORIAL BUTTON
    public void OpenTutorial()
    {
        SceneManager.LoadScene("TutorialScene"); 
    }

    // 4. BACK TO MAIN MENU BUTTON
    public void LoadMainMenuScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("01_MainMenu"); 
    }

    // 5. QUIT BUTTON
    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    #endregion

    #region Options Panel Logic

    public void OpenOptions()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    #endregion

    #region Audio Logic (Shared with PauseController)

    public void SetMasterVolume(float volume)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(volume);
            if (volume > 0 && muteToggle != null && muteToggle.isOn)
            {
                muteToggle.isOn = false;
            }
        }
    }

    public void ToggleMusicMute(bool isMuted)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ToggleMusicMute(isMuted);
        }
    }

    #endregion
}