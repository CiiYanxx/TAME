using System;
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
    [Tooltip("Drag QuestInfo ScriptableObjects here.")]
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
        
        // --- CAMERA ACTION: Start Dialogue View ---
        CameraManager.Instance.StartDialogueView(transform);
        
        if (firstTimeInteraction)
        {
            firstTimeInteraction = false;
            currentDialog = 0;
            StartQuestInitialDialog();
        }
        else 
        {
            // 1. If quest is already completed (No more interaction needed)
            if (currentActiveQuest.isCompleted)
            {
                npcDialogText.text = currentActiveQuest.info.finalWords; 
                SetCloseOption();
            }
            // 2. If the mission was accepted AND the result (success/fail) is known
            // This is the condition that checks the state set by ReportQuestOutcome(success)
            else if (currentActiveQuest.accepted && (currentActiveQuest.isMissionSuccess || !currentActiveQuest.isMissionSuccess))
            {
                if (currentActiveQuest.isMissionSuccess)
                {
                    npcDialogText.text = currentActiveQuest.info.comebackSuccess;
                    SetRewardOption();
                }
                else // Mission Failure Reported
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
                currentDialog = 0;
                StartQuestInitialDialog();
            }
        }
    }

    // --- NEW METHOD FOR EXTERNAL STATE UPDATE ---
    /// <summary>
    /// Called by the RescueController to update the quest success/failure state.
    /// </summary>
    public void ReportQuestOutcome(bool success)
    {
        if (currentActiveQuest != null && currentActiveQuest.accepted)
        {
            currentActiveQuest.isMissionSuccess = success;
            // The NPC now has the result and will display the correct dialogue 
            // the next time the player talks to it.
            Debug.Log($"NPC received mission outcome: Success={success}. Quest state updated.");
        }
    }
    
    // --- Option Setter Methods ---

    private void SetRewardOption()
    {
        optionButton1Text.text = "Receive Reward";
        optionButton1.onClick.RemoveAllListeners();
        optionButton1.onClick.AddListener(() => {
            ReceiveRewardAndCompleteQuest(true); 
        });
        optionButton2.gameObject.SetActive(false);
    }
    
    private void SetFailureOptions()
    {
        optionButton1Text.text = "Accept Deduction (Try Again)";
        optionButton1.onClick.RemoveAllListeners();
        optionButton1.onClick.AddListener(() => {
            ReceiveRewardAndCompleteQuest(false); 
        });
        
        optionButton2.gameObject.SetActive(true);
        optionButton2Text.text = "Abandon Mission";
        optionButton2.onClick.RemoveAllListeners();
        optionButton2.onClick.AddListener(() => {
            DeclinedQuest(); 
        });
    }
    
    private void SetCloseOption()
    {
        optionButton1Text.text = "Close";
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
        
        // --- CAMERA ACTION: End Dialogue View ---
        CameraManager.Instance.EndDialogueView();
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
        // Safety Check for Empty Dialog List
        if (currentActiveQuest.info.initialDialog.Count == 0)
        {
            Debug.LogWarning($"Quest '{currentActiveQuest.info.name}' has no initial dialog. Skipping to options.");
            currentActiveQuest.initialDialogCompleted = true;
            SetAcceptAndDeclineOptions(); 
            return; 
        }

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
        currentActiveQuest.isMissionSuccess = false; // Reset success flag when accepting

        // Announce the mission and the location hint
        npcDialogText.text = $"{currentActiveQuest.info.acceptAnswer}\n\nHint: Find the {currentActiveQuest.info.targetAnimalName} near the {currentActiveQuest.info.rescueLocationHint}.";
        
        // --- START MISSION: Spawn the Animal ---
        Vector3 missionSpawnPoint = FindMissionLocation(currentActiveQuest.info.rescueLocationHint); 
        
        // PASS 'THIS' (the NPC instance) to the controller
        RescueController.Instance.StartMission(this, currentActiveQuest.info.targetAnimalName, missionSpawnPoint);

        CloseDialogAfterAcceptance();
    }
    
    private void CloseDialogAfterAcceptance()
    {
        optionButton1Text.text = "Close";
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
            
            npcDialogText.text = $"Wonderful! You gained {currentActiveQuest.info.progressPointsReward} progress points and {currentActiveQuest.info.coinReward} coins. You are a true animal hero!";
            
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
        else // Failure (Punishment/Re-attempt)
        {
            ProgressSystem.Instance.DeductProgress(currentActiveQuest.info.progressPointsDeduction);
            
            npcDialogText.text = $"That's unfortunate. We've deducted {currentActiveQuest.info.progressPointsDeduction} points. The animal is still out there. You may try again.";
            
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
        RescueController.Instance.CleanupMission(); // Direct call is better than SendMessage

        npcDialogText.text = currentActiveQuest.info.declineAnswer;
        CloseDialogAfterAcceptance();
    }

    // Placeholder to get spawn coordinates from the text hint
    private Vector3 FindMissionLocation(string hint)
    {
        // Added 'f' suffix for correct float conversion (5f, 324.39f, etc.)
        if (hint.Contains("Residential")) return new Vector3(67.2f, 5f, 324.39f);
        if (hint.Contains("park")) return new Vector3(-20f, 0f, 10f);
        return new Vector3(0f, 0f, 0f); 
    }

    public void LookAtPlayer()
    {
        // This function remains available but is not called during StartConversation().
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