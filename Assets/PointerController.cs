using UnityEngine;
using UnityEngine.UI;

public class PointerController : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The RectTransform of the safe zone (sweet spot).")]
    public RectTransform safeZoneRect;
    [Tooltip("The parent object holding the entire minigame UI.")]
    public GameObject minigameUIContainer;
    
    [Header("Movement Settings")]
    [Tooltip("Total travel distance (e.g., the width of the bar).")]
    public float travelDistance = 600f; // Total pixel width the pointer travels
    [Tooltip("Speed of the pointer movement (units per second).")]
    public float moveSpeed = 300f; 
    
    // Internal State
    private AnimalInteractable animalInteractable;
    private RectTransform pointerTransform;
    private bool isActive = false;
    
    // Timer is not used in this version, but can be added back if needed.

    void Awake()
    {
        // Get the RectTransform of this object (the pointer)
        pointerTransform = GetComponent<RectTransform>();
        
        // Ensure UI is hidden at the start
        if (minigameUIContainer != null)
        {
            minigameUIContainer.SetActive(false);
        }
    }

    void Update()
    {
        if (!isActive) return;

        // 1. Move the Pointer using PingPong
        // PingPong cycles a value back and forth between 0 and travelDistance.
        // We offset it by (travelDistance / 2f) to center the movement around X=0 (the bar's center).
        float centerOffsetMovement = Mathf.PingPong(Time.time * moveSpeed, travelDistance) - (travelDistance / 2f);
        
        // Apply movement only to the X position
        pointerTransform.anchoredPosition = new Vector2(centerOffsetMovement, pointerTransform.anchoredPosition.y); 
    }

    /// <summary>
    /// Starts the minigame. Called by the AnimalInteractable script.
    /// </summary>
    /// <param name="caller">The AnimalInteractable instance to report the result to.</param>
    public void StartMinigame(AnimalInteractable caller)
    {
        if (safeZoneRect == null || minigameUIContainer == null)
        {
            Debug.LogError("PointerController UI references are missing. Cannot start minigame.");
            return;
        }
        
        animalInteractable = caller;
        isActive = true;
        minigameUIContainer.SetActive(true);
        
        // Reset pointer to the center position to start
        pointerTransform.anchoredPosition = new Vector2(0f, pointerTransform.anchoredPosition.y); 
        
        Debug.Log("Minigame: Started!");
    }
    
    /// <summary>
    /// Called by the MOBILE UI BUTTON's onClick event.
    /// </summary>
    public void AttemptRescue()
    {
        if (!isActive) return;

        // Get the cursor's current X position relative to the bar's center (0, 0)
        float cursorX = pointerTransform.anchoredPosition.x;
        
        // Calculate the safe zone bounds relative to the bar's center
        // safeZoneRect's anchoredPosition is its center.
        float sweetSpotHalfWidth = safeZoneRect.sizeDelta.x / 2f;
        float sweetSpotCenter = safeZoneRect.anchoredPosition.x;

        float sweetSpotMinX = sweetSpotCenter - sweetSpotHalfWidth;
        float sweetSpotMaxX = sweetSpotCenter + sweetSpotHalfWidth;

        // Success if cursor is within the sweet spot's bounds
        bool success = (cursorX >= sweetSpotMinX) && (cursorX <= sweetSpotMaxX);

        EndMinigame(success);
    }

    /// <summary>
    /// Ends the minigame and reports the result to the AnimalInteractable.
    /// </summary>
    private void EndMinigame(bool missionSuccess)
    {
        if (!isActive) return;
        isActive = false;

        minigameUIContainer.SetActive(false);
        
        Debug.Log(missionSuccess ? "Rescue Success!" : "Rescue Failed.");

        // Report the final outcome back to the AnimalInteractable
        if (animalInteractable != null)
        {
            animalInteractable.ReportMissionOutcome(missionSuccess);
        }
    }
}