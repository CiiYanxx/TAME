using UnityEngine;
using System.Collections.Generic;

public class RescueController : MonoBehaviour
{
    public static RescueController Instance { get; private set; }

    [Header("Animal Prefabs")]
    public List<GameObject> animalPrefabs;

    private GameObject currentSpawnedAnimal = null; 
    private NPC activeQuestGiver = null; 

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

    public void StartMission(NPC questGiver, string animalName, Vector3 spawnPoint)
    {
        // Cleanup the previously abandoned mission if necessary
        CleanupMission(); 
        
        activeQuestGiver = questGiver; 

        GameObject animalPrefab = animalPrefabs.Find(p => p.name == animalName);

        if (animalPrefab != null)
        {
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
    public void ReportMissionOutcome(bool success)
    {
        // --- 1. Pass the result back to the specific NPC ---
        // 🚨 CRITICAL REVISION: ONLY REPORT SUCCESS/FAILURE STATE ON SUCCESS.
        // IF FAILURE, WE DO NOT UPDATE THE NPC, ALLOWING THE PLAYER TO RETRY THE MINIGAME.
        if (success)
        {
             if (activeQuestGiver != null)
             {
                 activeQuestGiver.ReportQuestOutcome(success);
             }
        }
        // If it's a failure, the NPC's state is NOT updated. The quest remains 'accepted' and 'unsuccessful'.

        // 2. Clear the reference since AnimalInteractable destroyed itself on success only.
        if (success)
        {
            currentSpawnedAnimal = null; 
        }
        // If failed, currentSpawnedAnimal reference is kept so it can be cleaned up later if abandoned.
    }

    /// <summary>
    /// Removes the currently active spawned animal from the scene.
    /// This is called externally upon mission abandonment (by NPC) or when starting a new mission.
    /// </summary>
    public void CleanupMission()
    {
        if (currentSpawnedAnimal != null)
        {
            Debug.Log($"Cleaning up mission object: {currentSpawnedAnimal.name}");
            Destroy(currentSpawnedAnimal);
        }
        currentSpawnedAnimal = null; 
    }
}