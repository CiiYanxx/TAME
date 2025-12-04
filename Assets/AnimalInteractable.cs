using UnityEngine;

public class AnimalInteractable : MonoBehaviour
{
    [Tooltip("The range within which the player can interact to start the rescue minigame.")]
    public float interactionRange = 3f;

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
        // 1. You would usually hide the game world UI and show your minigame UI here.
        
        Debug.Log($"Starting rescue minigame for {gameObject.name}...");
        
        // --- SIMULATION FOR TESTING ---
        // Simulates a 50/50 chance of success after a brief delay.
        Invoke(nameof(SimulateMinigameEnd), 2f); 
    }
    
    private void SimulateMinigameEnd()
    {
        bool success = Random.Range(0, 2) == 1; 
        
        // Report the outcome back to the controller
        RescueController.Instance.ReportMissionOutcome(success);
    }
}