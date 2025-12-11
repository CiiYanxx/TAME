using UnityEngine;

public class PauseController : MonoBehaviour
{
    public static PauseController Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource; // New AudioSource for Sound Effects

    [Header("Default SFX Clips")]
    public AudioClip buttonClickSFX; // Clip for general button presses
    public AudioClip catMeowSFX;     // Clip for cat interaction
    public AudioClip dogBarkSFX;     // Clip for dog interaction

    private void Awake()
    {
        // --- Enforce Singleton Pattern ---
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps the Manager alive across scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // --- Initialize AudioSources if not set in Inspector ---

        // Music Source Check
        if (musicSource == null)
        {
            // You can also consider using GetOrAddComponent pattern
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true; // Music typically loops
        }

        // SFX Source Check (NEW)
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false; // SFX typically do not loop
        }
    }

    // --- Music Controls (Kept as before) ---

    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = volume;
        }
    }

    public void ToggleMusicMute(bool isMuted)
    {
        if (musicSource != null)
        {
            musicSource.mute = isMuted;
        }
    }

    // --- SFX Controls (NEW) ---

    /// <summary>
    /// Plays a one-shot AudioClip through the sfxSource.
    /// </summary>
    /// <param name="clip">The AudioClip to play.</param>
    /// <param name="volumeScale">Optional volume multiplier for this specific clip.</param>
    public void PlaySFX(AudioClip clip, float volumeScale = 1.0f)
    {
        if (sfxSource != null && clip != null)
        {
            // PlayOneShot allows multiple clips to be played simultaneously
            // (e.g., button click and a sword swing can happen at the same time)
            sfxSource.PlayOneShot(clip, volumeScale);
        }
    }

    // --- Convenience Methods for Default SFX (NEW) ---

    public void PlayButtonClick()
    {
        PlaySFX(buttonClickSFX);
    }

    public void PlayCatMeow()
    {
        PlaySFX(catMeowSFX);
    }

    public void PlayDogBark()
    {
        PlaySFX(dogBarkSFX);
    }
}