using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float animalInteractionRange = 3f; 

    // This method MUST be linked to your mobile UI's Interact Button OnClick() event.
    public void OnInteractButtonPressed() 
    {
        Transform playerBody = PlayerState.Instance.playerBody.transform; 

        // A. Check for NPC Interaction (High Priority)
        NPC[] npcs = FindObjectsOfType<NPC>(); 
        foreach (NPC npc in npcs)
        {
            if (npc.playerInRange && !npc.isTalkingWithPlayer)
            {
                npc.StartConversation();
                return; 
            }
        }

        // B. Check for Animal Interaction
        AnimalInteractable[] animals = FindObjectsOfType<AnimalInteractable>();
        foreach(AnimalInteractable animal in animals)
        {
            float distanceToAnimal = Vector3.Distance(animal.transform.position, playerBody.position);
            
            if(distanceToAnimal <= animalInteractionRange)
            {
                animal.PlayerAttemptInteraction(playerBody); 
                return; 
            }
        }
        
        Debug.Log("No NPC or Interactable Animal in range.");
    }
}