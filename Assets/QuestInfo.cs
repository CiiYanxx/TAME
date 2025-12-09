using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/QuestInfo", order = 1)]
public class QuestInfo : ScriptableObject
{
    [TextArea(5, 10)]
    public List<string> initialDialog = new List<string>();

    [Header("Quest Target")]
    public string targetAnimalName; // Removed default value
    [TextArea(1, 5)]
    public string rescueLocationHint; // Removed default value

    [Header("Options")]
    [TextArea(1, 5)]
    public string acceptOption; // Removed default value
    [TextArea(1, 10)] 
    public string acceptAnswer; // Removed default value
    [TextArea(1, 5)]
    public string declineOption; // Removed default value
    [TextArea(1, 5)]
    public string declineAnswer; // Removed default value
    [TextArea(1, 5)]
    public string comebackAfterDecline; // Removed default value
    [TextArea(1, 5)]
    public string comebackInProgress; // Removed default value
    [TextArea(1, 5)]
    public string comebackSuccess; // Removed default value
    [TextArea(1, 5)]
    public string finalWords; // Removed default value

    [Header("Reward & Punishment")]
    public int progressPointsReward = 100;
    public int coinReward = 0; 
    public int progressPointsDeduction = 50; 
}