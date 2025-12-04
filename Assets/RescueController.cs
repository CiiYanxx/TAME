using UnityEngine;
using System.Collections.Generic;

public class RescueController : MonoBehaviour
{
    public static RescueController Instance { get; private set; }

    [Header("Dependencies")]
    [Tooltip("Drag your NPC GameObject here so the controller can access its quest state.")]
    public NPC questGiverNPC; 

    [Header("Animal Prefabs")]
    [Tooltip("A list of all possible stray animal prefabs (names must match QuestInfo.targetAnimalName).")]
    public List<GameObject> animalPrefabs;
    
    [Header("Current Mission Target")]
    private GameObject currentTargetAnimal;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Called by NPC.cs when the player accepts the quest
    public void StartMission(string animalName, Vector3 spawnPosition)
    {
        CleanupMission();

        GameObject animalPrefab = animalPrefabs.Find(p => p != null && p.name.Equals(animalName, System.StringComparison.OrdinalIgnoreCase));

        if (animalPrefab != null)
        {
            // Instantiate the animal
            currentTargetAnimal = Instantiate(animalPrefab, spawnPosition, Quaternion.identity);
            
            // Ensure it has the interactable component
            AnimalInteractable interactable = currentTargetAnimal.GetComponent<AnimalInteractable>();
            if (interactable == null)
            {
                currentTargetAnimal.AddComponent<AnimalInteractable>();
            }

            Debug.Log($"Mission Started: Find {animalName} at {spawnPosition}.");
        }
        else
        {
            Debug.LogError($"Animal prefab '{animalName}' not found. Check your QuestInfo name.");
        }
    }
    
    // Called by AnimalInteractable.cs when the minigame is finished
    public void ReportMissionOutcome(bool success)
    {
        if (questGiverNPC != null && questGiverNPC.currentActiveQuest != null && questGiverNPC.currentActiveQuest.accepted)
        {
            questGiverNPC.currentActiveQuest.isMissionSuccess = success;
            
            // Clean up the animal immediately after the outcome is set
            CleanupMission();
            
            // Important: The player must now return to the NPC to continue the conversation
            Debug.Log($"Outcome reported: Success={success}. Return to {questGiverNPC.name}.");
        }
        else
        {
            Debug.LogWarning("Attempted to report mission outcome, but no active quest was found.");
        }
    }

    private void CleanupMission()
    {
        if (currentTargetAnimal != null)
        {
            Destroy(currentTargetAnimal);
            currentTargetAnimal = null;
        }
    }
}