using UnityEngine;
using DialogueEditor;

public class ConversationStarter : MonoBehaviour
{
    [SerializeField] private NPCConversation myConversation;

    private bool playerInside = false;

    private void Update()
    {
        // If player is inside trigger and touches screen
        if (playerInside && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                TryStartConversationByTouch(touch.position);
            }
        }
    }

    /// <summary>
    /// Attempts to start conversation by tapping anywhere or the NPC.
    /// </summary>
    private void TryStartConversationByTouch(Vector2 screenPos)
    {
        // Raycast from touch point into the 3D world
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            // If you want talking only when tapping the NPC:
            if (hit.collider.CompareTag("NPC"))
            {
                ConversationManager.Instance.StartConversation(myConversation);
                return;
            }
        }

        // Otherwise allow tapping anywhere while standing near
        ConversationManager.Instance.StartConversation(myConversation);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }
}
