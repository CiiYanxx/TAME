using UnityEngine;
using System.Collections.Generic;

public class RescueController : MonoBehaviour
{
    public static RescueController Instance { get; private set; }

    [Header("Animal Prefabs")]
    [Tooltip("List of all possible animal prefabs you can spawn.")]
    public List<GameObject> animalPrefabs;

    // State variables
    private GameObject currentSpawnedAnimal = null; 
    private NPC activeQuestGiver = null; // Reference to the NPC that started the mission

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

    /// <summary>
    /// Starts a mission by spawning the target animal at the given location and tracking the quest giver.
    /// </summary>
    /// <param name="questGiver">The NPC instance that started the mission (NEW).</param>
    /// <param name="animalName">The name of the animal prefab to find and spawn.</param>
    /// <param name="spawnPoint">The world location to spawn the animal.</param>
    public void StartMission(NPC questGiver, string animalName, Vector3 spawnPoint) // <--- Modified signature
    {
        CleanupMission(); // Ensure any previous animal is removed before starting a new one
        activeQuestGiver = questGiver; // <--- Store the NPC reference

        // Find the correct prefab by name
        GameObject animalPrefab = animalPrefabs.Find(p => p.name == animalName);

        if (animalPrefab != null)
        {
            // Instantiate the animal and store the reference
            currentSpawnedAnimal = Instantiate(animalPrefab, spawnPoint, Quaternion.identity);
            currentSpawnedAnimal.name = animalName + "_ActiveMission";
            Debug.Log($"Mission started: Spawning {animalName} at {spawnPoint}.");
        }
        else
        {
            Debug.LogError($"Animal prefab not found for name: {animalName}");
        }
    }

    /// <summary>
    /// Called by the AnimalInteractable component after the minigame is complete.
    /// </summary>
    /// <param name="success">True if the rescue minigame was successful.</param>
    public void ReportMissionOutcome(bool success)
    {
        // --- 1. Pass the result back to the specific NPC ---
        if (activeQuestGiver != null)
        {
            activeQuestGiver.ReportQuestOutcome(success);
        }

        if (success)
        {
            Debug.Log("Mission successful! Removing animal from scene.");
            // 2. Immediately destroy the animal prefab upon success.
            CleanupMission(); 
        }
        else
        {
            Debug.Log("Mission failed. The animal remains in the world for a retry.");
        }
    }

    /// <summary>
    /// Removes the currently active spawned animal from the scene.
    /// This is called internally upon success or externally upon mission abandonment (by NPC).
    /// </summary>
    public void CleanupMission()
    {
        if (currentSpawnedAnimal != null)
        {
            Debug.Log($"Cleaning up mission object: {currentSpawnedAnimal.name}");
            Destroy(currentSpawnedAnimal);
            currentSpawnedAnimal = null; // Clear the reference
        }
        activeQuestGiver = null; // Clear the NPC reference as the mission is over
    }
}