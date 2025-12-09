using UnityEngine;

public class AnimalInteractable : MonoBehaviour
{
    [Tooltip("The range within which the player can interact to start the rescue minigame.")]
    public float interactionRange = 3f;
    
    [Tooltip("The actual PointerController UI logic. MUST be linked in the Inspector.")]
    public PointerController pointerController; 

    public void PlayerAttemptInteraction(Transform playerTransform)
    {
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        
        if (distance <= interactionRange)
        {
            StartRescueMinigame(); 
        }
    }
    
    private void StartRescueMinigame()
    {
        if (pointerController != null)
        {
            pointerController.StartMinigame(this); 
            Debug.Log($"Starting rescue minigame for {gameObject.name}...");
        }
        else
        {
            Debug.LogError("PointerController reference is missing on the AnimalInteractable! Reporting automatic failure.");
            ReportMissionOutcome(false); 
        }
    }
    
    /// <summary>
    /// CALLED BY: The PointerController when the minigame finishes (win or lose).
    /// </summary>
    /// <param name="success">True if the player successfully reached attemptsRequired, False if they hit maxFailedAttempts.</param>
    public void ReportMissionOutcome(bool success)
    {
        // --- 1. HANDLE POINT PENALTY/REWARD & CLEANUP ---
        if (success)
        {
            // +10 reward given by NPC later
            // Cleanup: The animal is rescued, remove it from the world
            Debug.Log("Minigame SUCCESS! Animal is removed.");
            Destroy(gameObject); 
        }
        else
        {
            // Failure: -2 penalty applied immediately
            const int FAILURE_PENALTY = 2;
            if (ProgressSystem.Instance != null)
            {
                ProgressSystem.Instance.DeductProgress(FAILURE_PENALTY);
            }
            
            // 🚨 REVERSION: Animal is destroyed when the minigame fails (max attempts hit).
            Debug.LogWarning($"Minigame FAILED! Deducted {FAILURE_PENALTY} points. Animal is removed.");
            Destroy(gameObject);
        }
        
        // --- 2. Report the final outcome back to the central RescueController/NPC. ---
        if (RescueController.Instance != null)
        {
            // The PointerController already showed the outcome panel.
            RescueController.Instance.ReportMissionOutcome(success);
        }
    }
}