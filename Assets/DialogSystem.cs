using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{
    public static DialogSystem Instance { get; private set; }

    [Header("UI Elements")]
    public GameObject dialogPanel;
    public TextMeshProUGUI dialogText;
    public Button option1BTN;
    public Button option2BTN;
    
    // 🚨 REVISION: OutcomePanel references REMOVED

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

    private void Start()
    {
        if (dialogPanel != null) dialogPanel.SetActive(false);
        if (option2BTN != null) option2BTN.gameObject.SetActive(false);
    }

    public void OpenDialogUI()
    {
        if (dialogPanel != null) dialogPanel.SetActive(true);
        if (option2BTN != null) option2BTN.gameObject.SetActive(false);
    }

    public void CloseDialogUI()
    {
        if (dialogPanel != null) dialogPanel.SetActive(false);
        if (option2BTN != null) option2BTN.gameObject.SetActive(false);
    }
}