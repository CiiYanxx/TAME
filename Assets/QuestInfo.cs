using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/QuestInfo", order = 1)]
public class QuestInfo : ScriptableObject
{
    [TextArea(5, 10)]
    public List<string> initialDialog = new List<string>();

    [Header("Quest Target")]
    public string targetAnimalName = "Stray Animals"; 
    [TextArea(1, 5)]
    public string rescueLocationHint = "I last saw it near the old market fountain."; 

    [Header("Options")]
    public string acceptOption = "Accept Rescue Mission";
    public string acceptAnswer = "Fantastic! Find the Stray animal and bring it back safely.";
    public string declineOption = "Decline";
    public string declineAnswer = "That's a shame. I hope you'll reconsider.";
    public string comebackAfterDecline = "The animal is still out there. Are you ready now?";
    public string comebackInProgress = "You haven't rescued it yet. Be careful out there!"; 
    public string comebackSuccess = "You brought it back! Thank you so much."; 
    public string finalWords = "Thank you for all your help. No more missions for now.";

    [Header("Reward & Penalty Values")]
    [Tooltip("Reward for success (given when talking to NPC) - Should be 10.")]
    public int progressPointsReward = 10;
    public int coinReward = 5; 
    
    // 🚨 REVISION: Penalty for minigame loss is now -2 points.
    [Tooltip("Penalty for minigame loss is applied immediately in AnimalInteractable (-2 points).")]
    public int minigameLossPoints = 2;
    
    [Tooltip("Penalty for abandoning a quest (given when talking to NPC) - Should be 20.")]
    public int abandonmentPenalty = 20; 
}