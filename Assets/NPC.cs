using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public bool playerInRange;
    public bool isTalkingWithPlayer;

    TextMeshProUGUI npcDialogText;
    Button optionButton1;
    TextMeshProUGUI optionButton1Text;
    Button optionButton2;
    TextMeshProUGUI optionButton2Text;

    [Header("Quest Management")]
    public List<QuestInfo> questInfoList; 
    [NonSerialized]
    public List<Quest> quests = new List<Quest>(); 
    
    public Quest currentActiveQuest = null;
    public int activeQuestIndex = 0;
    public bool firstTimeInteraction = true;
    public int currentDialog;

    private void Awake()
    {
        foreach (QuestInfo info in questInfoList)
        {
            quests.Add(new Quest(info));
        }
    }

    private void Start()
    {
        // Get references from the DialogSystem singleton
        if (DialogSystem.Instance == null) { Debug.LogError("DialogSystem Missing!"); return; }

        npcDialogText = DialogSystem.Instance.dialogText;
        optionButton1 = DialogSystem.Instance.option1BTN;
        optionButton1Text = optionButton1.GetComponentInChildren<TextMeshProUGUI>();

        optionButton2 = DialogSystem.Instance.option2BTN;
        optionButton2Text = optionButton2.GetComponentInChildren<TextMeshProUGUI>();

        if (quests.Count > 0)
        {
            currentActiveQuest = quests[activeQuestIndex];
        }
    }

    public void StartConversation()
    {
        if (currentActiveQuest == null) 
        {
            DialogSystem.Instance.OpenDialogUI();
            npcDialogText.text = "I have no new missions for you right now. Thank you for your service!";
            SetCloseOption();
            return;
        }
        
        isTalkingWithPlayer = true;
        DialogSystem.Instance.OpenDialogUI(); 
        LookAtPlayer();

        if (firstTimeInteraction)
        {
            firstTimeInteraction = false;
            StartQuestInitialDialog();
            currentDialog = 0;
        }
        else 
        {
            // 1. If quest is already completed (No more interaction needed for this quest)
            if (currentActiveQuest.isCompleted)
            {
                npcDialogText.text = currentActiveQuest.info.finalWords; 
                SetCloseOption();
            }
            // 2. If the mission was accepted AND the result (success/fail) is known
            else if (currentActiveQuest.accepted && currentActiveQuest.isMissionSuccess || currentActiveQuest.accepted && !currentActiveQuest.isMissionSuccess)
            {
                if (currentActiveQuest.isMissionSuccess)
                {
                    npcDialogText.text = currentActiveQuest.info.comebackSuccess;
                    SetRewardOption();
                }
                else
                {
                    npcDialogText.text = "Oh dear, it seems the rescue failed this time. We still need to find that animal. Please try again or abandon the mission.";
                    SetFailureOptions();
                }
            }
            // 3. If accepted, but not yet completed/failed (Player is back too early)
            else if (currentActiveQuest.accepted)
            {
                npcDialogText.text = currentActiveQuest.info.comebackInProgress;
                SetCloseOption();
            }
            // 4. If we return after declining
            else if (currentActiveQuest.declined)
            {
                npcDialogText.text = currentActiveQuest.info.comebackAfterDecline;
                SetAcceptAndDeclineOptions();
            }
            // 5. If the previous quest was completed and we haven't started the next one
            else if (currentActiveQuest.initialDialogCompleted == false)
            {
                StartQuestInitialDialog();
            }
        }
    }
    
    private void SetRewardOption()
    {
        optionButton1Text.text = "[Receive Reward]";
        optionButton1.onClick.RemoveAllListeners();
        optionButton1.onClick.AddListener(() => {
            ReceiveRewardAndCompleteQuest(true); 
        });
        optionButton2.gameObject.SetActive(false);
    }
    
    private void SetFailureOptions()
    {
        optionButton1Text.text = "[Accept Deduction (Try Again)]";
        optionButton1.onClick.RemoveAllListeners();
        optionButton1.onClick.AddListener(() => {
            ReceiveRewardAndCompleteQuest(false); 
        });
        
        optionButton2.gameObject.SetActive(true);
        optionButton2Text.text = "[Abandon Mission]";
        optionButton2.onClick.RemoveAllListeners();
        optionButton2.onClick.AddListener(() => {
            DeclinedQuest(); 
        });
    }
    
    private void SetCloseOption()
    {
        optionButton1Text.text = "[Close]";
        optionButton1.onClick.RemoveAllListeners();
        optionButton1.onClick.AddListener(() => {
            CloseDialogUI(); 
        });
        optionButton2.gameObject.SetActive(false);
    }
    
    private void CloseDialogUI()
    {
        DialogSystem.Instance.CloseDialogUI();
        isTalkingWithPlayer = false;
    }
    
    private void SetAcceptAndDeclineOptions()
    {
        optionButton1Text.text = currentActiveQuest.info.acceptOption;
        optionButton1.onClick.RemoveAllListeners();
        optionButton1.onClick.AddListener(() => {
            AcceptedQuest();
        });

        optionButton2.gameObject.SetActive(true);
        optionButton2Text.text = currentActiveQuest.info.declineOption;
        optionButton2.onClick.RemoveAllListeners();
        optionButton2.onClick.AddListener(() => {
            DeclinedQuest();
        });
    }

    private void StartQuestInitialDialog()
    {
        npcDialogText.text = currentActiveQuest.info.initialDialog[currentDialog];
        optionButton1Text.text = "Next";
        optionButton1.onClick.RemoveAllListeners();
        optionButton1.onClick.AddListener(()=> {
            currentDialog++;
            CheckIfDialogDone();
        });

        optionButton2.gameObject.SetActive(false);
    }

    private void CheckIfDialogDone()
    {
        if (currentDialog >= currentActiveQuest.info.initialDialog.Count) 
        {
            currentActiveQuest.initialDialogCompleted = true;
            SetAcceptAndDeclineOptions(); 
        }
        else 
        {
            npcDialogText.text = currentActiveQuest.info.initialDialog[currentDialog];

            optionButton1Text.text = "Next";
            optionButton1.onClick.RemoveAllListeners();
            optionButton1.onClick.AddListener(() => {
                currentDialog++;
                CheckIfDialogDone();
            });
        }
    }
    
    private void AcceptedQuest()
    {
        currentActiveQuest.accepted = true;
        currentActiveQuest.declined = false;
        currentActiveQuest.isMissionSuccess = false; 

        // Announce the mission and the location hint
        npcDialogText.text = $"{currentActiveQuest.info.acceptAnswer}\n\nHint: Find the **{currentActiveQuest.info.targetAnimalName}** near the **{currentActiveQuest.info.rescueLocationHint}**.";
        
        // --- START MISSION ---
        // NOTE: Define a function to translate your hint text into actual world coordinates (Vector3).
        Vector3 missionSpawnPoint = FindMissionLocation(currentActiveQuest.info.rescueLocationHint); 
        RescueController.Instance.StartMission(currentActiveQuest.info.targetAnimalName, missionSpawnPoint);

        CloseDialogAfterAcceptance();
    }
    
    private void CloseDialogAfterAcceptance()
    {
        optionButton1Text.text = "[Close]";
        optionButton1.onClick.RemoveAllListeners();
        optionButton1.onClick.AddListener(() => {
            CloseDialogUI();
        });
        optionButton2.gameObject.SetActive(false);
    }

    private void ReceiveRewardAndCompleteQuest(bool success)
    {
        if (success)
        {
            currentActiveQuest.isCompleted = true;
            ProgressSystem.Instance.AddProgress(currentActiveQuest.info.progressPointsReward);
            ProgressSystem.Instance.AddCoins(currentActiveQuest.info.coinReward);
            
            npcDialogText.text = $"Wonderful! You gained **{currentActiveQuest.info.progressPointsReward}** progress points and **{currentActiveQuest.info.coinReward}** coins. You are a true animal hero!";
            
            // Advance to the next quest
            activeQuestIndex++;

            if (activeQuestIndex < quests.Count)
            {
                currentActiveQuest = quests[activeQuestIndex];
                currentDialog = 0;
            }
            else
            {
                currentActiveQuest = null; 
            }
        }
        else // Failure (Punishment)
        {
            ProgressSystem.Instance.DeductProgress(currentActiveQuest.info.progressPointsDeduction);
            
            npcDialogText.text = $"That's unfortunate. We've deducted **{currentActiveQuest.info.progressPointsDeduction}** points. The animal is still out there. You may try again.";
            
            // Reset state to allow re-attempt
            currentActiveQuest.isCompleted = false; 
            currentActiveQuest.isMissionSuccess = false; 
            currentActiveQuest.accepted = true; // Still accepted, just failed the attempt
        }

        SetCloseOption();
    }

    private void DeclinedQuest()
    {
        currentActiveQuest.declined = true;
        // Clean up the animal if it was spawned but the mission is abandoned
        RescueController.Instance.SendMessage("CleanupMission");

        npcDialogText.text = currentActiveQuest.info.declineAnswer;
        CloseDialogAfterAcceptance();
    }

    private Vector3 FindMissionLocation(string hint)
    {
        // Placeholder: Replace this with your actual logic to find coordinates based on the hint.
        // For testing, just return a visible spot.
        if (hint.Contains("fountain")) return new Vector3(50, 0, 50);
        if (hint.Contains("park")) return new Vector3(-20, 0, 10);
        return Vector3.zero; // Default spawn point
    }

    public void LookAtPlayer()
    {
        if (PlayerState.Instance == null || PlayerState.Instance.playerBody == null) return;
        
        var player = PlayerState.Instance.playerBody.transform;
        Vector3 direction = player.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isTalkingWithPlayer)
            {
                CloseDialogUI();
            }
            playerInRange = false;
        }
    }
}