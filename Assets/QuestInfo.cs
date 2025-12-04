using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/QuestInfo", order = 1)]
public class QuestInfo : ScriptableObject
{
    [TextArea(5, 10)]
    public List<string> initialDialog = new List<string>();

    [Header("Quest Target")]
    public string targetAnimalName = "Stray Dog"; 
    [TextArea(1, 5)]
    public string rescueLocationHint = "I last saw it near the old market fountain."; 

    [Header("Options")]
    [TextArea(1, 5)]
    public string acceptOption = "[Accept Rescue Mission]";
    [TextArea(1, 10)] // Increased size for the main dialogue text
    public string acceptAnswer = "Fantastic! Find the animal and bring it back safely.";
    [TextArea(1, 5)]
    public string declineOption = "[Decline]";
    [TextArea(1, 5)]
    public string declineAnswer = "That's a shame. I hope you'll reconsider.";
    [TextArea(1, 5)]
    public string comebackAfterDecline = "The animal is still out there. Are you ready now?";
    [TextArea(1, 5)]
    public string comebackInProgress = "You haven't rescued it yet. Be careful out there!"; 
    [TextArea(1, 5)]
    public string comebackSuccess = "You brought it back! Thank you so much."; 
    [TextArea(1, 5)]
    public string finalWords = "Thank you for all your help. No more missions for now.";

    [Header("Reward & Punishment")]
    public int progressPointsReward = 100;
    public int coinReward = 0; 
    public int progressPointsDeduction = 50; 
}