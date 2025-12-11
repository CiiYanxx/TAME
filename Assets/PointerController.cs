using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class PointerController : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform safeZoneRect;
    public RectTransform travelAreaRect;
    public GameObject minigameUIContainer;
    
    [Header("Outcome Panel References")]
    public GameObject outcomePanel; 
    public TextMeshProUGUI outcomeText;
    
    [Header("Player Control UI")]
    public GameObject playerJoystick; 
    public GameObject playerInteractButton; 
    
    [Header("Movement Settings")]
    public float moveSpeed = 300f; 
    
    [Header("Game Attempts")]
    [Tooltip("Number of successful hits required to win the minigame.")]
    public int attemptsRequired = 5;
    [Tooltip("Max number of misses before the minigame is failed.")]
    public int maxFailedAttempts = 3; // 🚨 NEW REQUIREMENT: 3 attempts for failed minigame
    [Tooltip("Maximum distance (in pixels) the safe zone can move from the center.")]
    public float maxSafeZoneOffset = 250f; 

    // Internal State
    private AnimalInteractable animalInteractable;
    private RectTransform pointerTransform;
    private bool isActive = false;
    private int successfulAttempts = 0;
    private int failedAttemptsCount = 0; // 🚨 NEW: Tracks current fails
    private float barWidth;

    void Awake()
    {
        pointerTransform = GetComponent<RectTransform>();
        
        if (minigameUIContainer != null)
        {
            minigameUIContainer.SetActive(false);
        }
        if (outcomePanel != null)
        {
             outcomePanel.SetActive(false); 
        }

        if (travelAreaRect != null)
        {
            barWidth = travelAreaRect.sizeDelta.x;
        }
        else
        {
            Debug.LogError("TravelAreaRect is null in PointerController. Awake cannot determine bar width.");
        }
    }

    void Update()
    {
        if (!isActive) return;

        float travelRange = barWidth;
        float centerOffsetMovement = Mathf.PingPong(Time.time * moveSpeed, travelRange) - (travelRange / 2f);
        
        pointerTransform.anchoredPosition = new Vector2(centerOffsetMovement, pointerTransform.anchoredPosition.y); 
    }

    public void StartMinigame(AnimalInteractable caller)
    {
        if (safeZoneRect == null || minigameUIContainer == null || travelAreaRect == null || outcomePanel == null)
        {
            Debug.LogError("PointerController UI references are missing. Cannot start minigame.");
            return;
        }
        
        animalInteractable = caller;
        successfulAttempts = 0; 
        failedAttemptsCount = 0; // 🚨 NEW: Reset failed attempts
        isActive = true;
        minigameUIContainer.SetActive(true);
        
        if (playerJoystick != null) playerJoystick.SetActive(false);
        if (playerInteractButton != null) playerInteractButton.SetActive(false);
        
        pointerTransform.anchoredPosition = new Vector2(0f, pointerTransform.anchoredPosition.y); 
        RandomizeSafeZonePosition();
    }
    
    public void AttemptRescue()
    {
        if (!isActive) return;

        float cursorX = pointerTransform.anchoredPosition.x;
        
        float sweetSpotHalfWidth = safeZoneRect.sizeDelta.x / 2f;
        float sweetSpotCenter = safeZoneRect.anchoredPosition.x;

        float sweetSpotMinX = sweetSpotCenter - sweetSpotHalfWidth;
        float sweetSpotMaxX = sweetSpotCenter + sweetSpotHalfWidth;

        bool success = (cursorX >= sweetSpotMinX) && (cursorX <= sweetSpotMaxX);

        if (success)
        {
            successfulAttempts++;

            if (successfulAttempts >= attemptsRequired)
            {
                EndMinigame(true); // Minigame completed successfully
            }
            else
            {
                ResetPointerAndAdvance();
            }
        }
        else // Player missed the safe zone
        {
            failedAttemptsCount++; // 🚨 NEW: Increment fail count
            
            if (failedAttemptsCount >= maxFailedAttempts)
            {
                EndMinigame(false); // Minigame failed: hit max attempts
            }
            else
            {
                // Player failed one attempt but has more chances left
                ResetPointerAndAdvance(); 
            }
        }
    }
    
    private void ResetPointerAndAdvance()
    {
        pointerTransform.anchoredPosition = new Vector2(0f, pointerTransform.anchoredPosition.y); 
        RandomizeSafeZonePosition();
    }

    private void RandomizeSafeZonePosition()
    {
        float maxOffset = (barWidth / 2f) - (safeZoneRect.sizeDelta.x / 2f);
        float finalMaxOffset = Mathf.Min(maxSafeZoneOffset, maxOffset); 
        float newX = Random.Range(-finalMaxOffset, finalMaxOffset);
        safeZoneRect.anchoredPosition = new Vector2(newX, safeZoneRect.anchoredPosition.y);
    }

    private void EndMinigame(bool missionSuccess)
    {
        if (!isActive) return;
        isActive = false;

        minigameUIContainer.SetActive(false);
        
        if (playerJoystick != null) playerJoystick.SetActive(true);
        if (playerInteractButton != null) playerInteractButton.SetActive(true);

        // Report the final outcome back to the AnimalInteractable, which handles points and cleanup.
        if (animalInteractable != null)
        {
            animalInteractable.ReportMissionOutcome(missionSuccess);
        }
        
        // --- Display the outcome panel ---
        ShowOutcomePanel(missionSuccess);
    }
    
    /// <summary>
    /// Displays a message panel after the minigame is finished.
    /// </summary>
    private void ShowOutcomePanel(bool success)
    {
        if (outcomePanel != null && outcomeText != null)
        {
            if (success)
            {
                outcomeText.text = "You Successfully Tamed the Animal! Go back to Dr. Kevin";
            }
            else
            {
                // 🚨 Updated message to reflect final failure after multiple attempts
                outcomeText.text = "It appears the animal is fleeing, try again later!"; 
            }
            outcomePanel.SetActive(true);
            
            // Auto-hide the panel after a delay (e.g., 2 seconds)
            Invoke(nameof(HideOutcomePanel), 5f); 
        }
    }

    private void HideOutcomePanel()
    {
        if (outcomePanel != null)
        {
            outcomePanel.SetActive(false);
        }
    }
}