using UnityEngine;

public class AnimalInteractable : MonoBehaviour
{
    [Tooltip("The range within which the player can interact to start the rescue minigame.")]
    public float interactionRange = 3f;
    
    [Tooltip("The actual PointerController UI logic. MUST be linked in the Inspector.")]
    public PointerController pointerController; 

    // This is called by the PlayerInteraction script when the player presses the button near the animal
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
        // 1. --- CAMERA ACTION: Focus Camera on the Animal for the Minigame ---
        if (CameraManager.Instance != null)
        {
            CameraManager.Instance.StartDialogueView(transform);
        }
        
        // 2. Start the actual minigame logic in the PointerController
        if (pointerController != null)
        {
            // This starts the UI minigame and handles control visibility
            pointerController.StartMinigame(this); 
            Debug.Log($"Starting rescue minigame for {gameObject.name}...");
        }
        else
        {
            // Safety measure: If the PointerController reference is missing, report failure.
            Debug.LogError("PointerController reference is missing on the AnimalInteractable! Reporting automatic failure.");
            ReportMissionOutcome(false); 
        }
    }
    
    /// <summary>
    /// CALLED BY: The PointerController when the minigame finishes (win or lose).
    /// This reports the outcome to the central RescueController and handles cleanup.
    /// </summary>
    /// <param name="success">True if the player successfully stopped the pointer in the sweet spot.</param>
    public void ReportMissionOutcome(bool success)
    {
        // 1. --- CAMERA ACTION: Revert Camera view after the minigame is finished ---
        if (CameraManager.Instance != null)
        {
            CameraManager.Instance.EndDialogueView();
        }
        
        // 2. Report the final outcome back to the central RescueController.
        // The RescueController updates the NPC quest state.
        if (RescueController.Instance != null)
        {
            RescueController.Instance.ReportMissionOutcome(success);
        }
        
        // 3. --- NEW: Cleanup/Destroy the Animal Interactable Object ---
        // This ensures the animal disappears after the minigame concludes (win or lose).
        Debug.Log($"Rescue outcome reported. Destroying {gameObject.name}. Success: {success}");
        Destroy(gameObject);
    }
    
    // --- NEW REQUESTED METHOD ---
    private void SimulateMinigameEnd()
    {
        if (RescueController.Instance == null)
        {
            Debug.LogError("RescueController is missing. Cannot report outcome.");
            return;
        }

        // Logic to determine success (50/50 chance)
        bool success = Random.Range(0, 2) == 1; 
        
        // Revert Camera view after the minigame is finished
        if (CameraManager.Instance != null)
        {
            CameraManager.Instance.EndDialogueView();
        }

        // Report the outcome back to the controller (which handles the cleanup if successful)
        RescueController.Instance.ReportMissionOutcome(success);

        // Since the AnimalInteractable is done, destroy it here if this simulation is used
        Debug.Log($"Simulated minigame end. Destroying {gameObject.name}. Success: {success}");
        Destroy(gameObject);
    }
}