using UnityEngine;
using UnityEngine.UI;

public class PointerController : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The RectTransform of the safe zone (sweet spot).")]
    public RectTransform safeZoneRect;
    [Tooltip("The RectTransform defining the full bar's travel area.")]
    public RectTransform travelAreaRect;
    [Tooltip("The parent object holding the entire minigame UI.")]
    public GameObject minigameUIContainer;
    
    [Header("Player Control UI")]
    [Tooltip("The GameObject for the player's movement joystick. Must be linked.")]
    public GameObject playerJoystick; 
    [Tooltip("The GameObject for the general interaction button. Must be linked.")]
    public GameObject playerInteractButton; 
    
    [Header("Movement Settings")]
    [Tooltip("Speed of the pointer movement (units per second).")]
    public float moveSpeed = 300f; 
    
    [Header("Game Attempts")]
    [Tooltip("Number of successful hits required to win the minigame.")]
    public int attemptsRequired = 5;
    [Tooltip("Maximum distance (in pixels) the safe zone can move from the center.")]
    public float maxSafeZoneOffset = 250f; 

    // Internal State
    private AnimalInteractable animalInteractable;
    private RectTransform pointerTransform;
    private bool isActive = false;
    private int successfulAttempts = 0;
    private float barWidth;

    void Awake()
    {
        pointerTransform = GetComponent<RectTransform>();
        
        // Ensure UI starts hidden
        if (minigameUIContainer != null)
        {
            minigameUIContainer.SetActive(false);
        }

        if (travelAreaRect != null)
        {
            // Get the width of the travel bar to constrain movement/randomization
            barWidth = travelAreaRect.sizeDelta.x;
        }
        else
        {
            // Critical check for null reference
            Debug.LogError("TravelAreaRect is null in PointerController. Awake cannot determine bar width.");
        }
    }

    void Update()
    {
        if (!isActive) return;

        // Move the Pointer using PingPong
        float travelRange = barWidth;
        // The pointer moves from -travelRange/2 to +travelRange/2
        float centerOffsetMovement = Mathf.PingPong(Time.time * moveSpeed, travelRange) - (travelRange / 2f);
        
        pointerTransform.anchoredPosition = new Vector2(centerOffsetMovement, pointerTransform.anchoredPosition.y); 
    }

    /// <summary>
    /// Starts the minigame and sets the initial state.
    /// </summary>
    public void StartMinigame(AnimalInteractable caller)
    {
        // Null checks for mandatory UI elements (Fix for previous NRE)
        if (safeZoneRect == null || minigameUIContainer == null || travelAreaRect == null)
        {
            Debug.LogError("PointerController UI references are missing. Cannot start minigame.");
            return;
        }
        
        animalInteractable = caller;
        successfulAttempts = 0; // Reset attempts
        isActive = true;
        minigameUIContainer.SetActive(true);
        
        // --- NEW: Hide Player Controls ---
        if (playerJoystick != null) playerJoystick.SetActive(false);
        if (playerInteractButton != null) playerInteractButton.SetActive(false);
        // ---------------------------------
        
        // Reset pointer and place the safe zone for the first attempt
        pointerTransform.anchoredPosition = new Vector2(0f, pointerTransform.anchoredPosition.y); 
        RandomizeSafeZonePosition();
        
        Debug.Log("Minigame: Started with " + attemptsRequired + " required hits.");
    }
    
    /// <summary>
    /// Called by the MOBILE UI BUTTON's onClick event to check for success.
    /// </summary>
    public void AttemptRescue()
    {
        if (!isActive) return;

        float cursorX = pointerTransform.anchoredPosition.x;
        
        // Calculate the safe zone bounds relative to the bar's center (0, 0)
        float sweetSpotHalfWidth = safeZoneRect.sizeDelta.x / 2f;
        float sweetSpotCenter = safeZoneRect.anchoredPosition.x;

        float sweetSpotMinX = sweetSpotCenter - sweetSpotHalfWidth;
        float sweetSpotMaxX = sweetSpotCenter + sweetSpotHalfWidth;

        bool success = (cursorX >= sweetSpotMinX) && (cursorX <= sweetSpotMaxX);

        if (success)
        {
            successfulAttempts++;
            Debug.Log($"Hit! Successes: {successfulAttempts}/{attemptsRequired}");

            if (successfulAttempts >= attemptsRequired)
            {
                EndMinigame(true); // Minigame completed successfully
            }
            else
            {
                // Successful attempt, reset the pointer and move the target
                ResetPointerAndAdvance();
            }
        }
        else
        {
            Debug.Log("Miss! Minigame Failed.");
            EndMinigame(false); // One miss means immediate failure
        }
    }
    
    /// <summary>
    /// Repositions the pointer to the center and moves the safe zone for the next round.
    /// </summary>
    private void ResetPointerAndAdvance()
    {
        // 1. Reset the pointer's position
        pointerTransform.anchoredPosition = new Vector2(0f, pointerTransform.anchoredPosition.y); 

        // 2. Randomize the safe zone's position for the next hit
        RandomizeSafeZonePosition();
    }

    /// <summary>
    /// Randomly moves the safe zone RectTransform along the X-axis.
    /// </summary>
    private void RandomizeSafeZonePosition()
    {
        // Calculate the maximum offset, respecting the bar's edges.
        float maxOffset = (barWidth / 2f) - (safeZoneRect.sizeDelta.x / 2f);
        
        // Use the smaller of the defined max offset or the calculated edge limit.
        float finalMaxOffset = Mathf.Min(maxSafeZoneOffset, maxOffset); 

        // Generate a random position between -finalMaxOffset and finalMaxOffset
        float newX = Random.Range(-finalMaxOffset, finalMaxOffset);
        
        // Apply the new position (Y remains the same)
        safeZoneRect.anchoredPosition = new Vector2(newX, safeZoneRect.anchoredPosition.y);
        
        Debug.Log($"Safe Zone moved to X: {newX:F2}");
    }

    /// <summary>
    /// Ends the minigame and reports the result to the AnimalInteractable.
    /// </summary>
    private void EndMinigame(bool missionSuccess)
    {
        if (!isActive) return;
        isActive = false;

        minigameUIContainer.SetActive(false);
        
        // --- NEW: Show Player Controls ---
        if (playerJoystick != null) playerJoystick.SetActive(true);
        if (playerInteractButton != null) playerInteractButton.SetActive(true);
        // ---------------------------------

        Debug.Log(missionSuccess ? "Rescue Success!" : "Rescue Failed.");

        // Report the final outcome back to the AnimalInteractable
        if (animalInteractable != null)
        {
            animalInteractable.ReportMissionOutcome(missionSuccess);
        }
    }
}