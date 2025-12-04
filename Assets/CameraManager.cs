using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [Header("Camera References")]
    [Tooltip("Your main player-following camera.")]
    public Camera mainGameCamera; 
    
    [Tooltip("The dedicated camera used for dialogue and interactions.")]
    public Camera dialogueCamera; 

    [Header("Dialogue Camera Settings")]
    [Tooltip("The distance the dialogue camera should be from the target.")]
    public float dialogueCameraDistance = 3f;
    [Tooltip("The height of the dialogue camera relative to the target.")]
    public float dialogueCameraHeight = 1.5f;
    [Tooltip("An angle offset applied to the camera's rotation (e.g., 30 for a 30-degree side view).")]
    public float dialogueCameraRotationOffset = 0f; // <-- NEW FIELD FOR MANUAL ROTATION

    [Header("Player Visibility")]
    [Tooltip("The player's body/mesh renderer to hide during dialogue.")]
    public GameObject playerBodyToHide; 

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
        // Ensure the dialogue camera is disabled initially
        if (dialogueCamera != null)
        {
            dialogueCamera.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Switches to the dialogue camera, positions it relative to the NPC/Player, applies rotation offset, and hides the player.
    /// </summary>
    /// <param name="targetTransform">The position of the NPC or Animal to focus on.</param>
    public void StartDialogueView(Transform targetTransform)
    {
        if (mainGameCamera == null || dialogueCamera == null)
        {
            Debug.LogError("Camera references not set in CameraManager!");
            return;
        }

        // We assume PlayerState is initialized and active
        if (PlayerState.Instance == null || PlayerState.Instance.playerBody == null)
        {
            Debug.LogError("PlayerState or playerBody is missing. Cannot calculate camera direction.");
            return;
        }

        Vector3 targetPos = targetTransform.position;
        
        // 1. Calculate base camera position
        // Direction from the target (NPC) to the player
        Vector3 directionToPlayer = (PlayerState.Instance.playerBody.position - targetPos).normalized;
        
        // Position the camera back from the target, opposite the player, and at a set height
        Vector3 cameraPosition = targetPos - directionToPlayer * dialogueCameraDistance + Vector3.up * dialogueCameraHeight;
        
        // 2. Apply position
        dialogueCamera.transform.position = cameraPosition;
        
        // 3. Apply base LookAt (look at NPC's chest/face height, e.g., 1.5f)
        // We use the 'dialogueCameraHeight' as a reasonable look-at height reference for the NPC.
        dialogueCamera.transform.LookAt(targetPos + Vector3.up * dialogueCameraHeight); 

        // 4. Apply Manual Rotation Offset (around the Y-axis)
        // Rotate the camera around the target's position by the defined offset angle
        dialogueCamera.transform.RotateAround(
            targetPos + Vector3.up * dialogueCameraHeight, // Point to orbit around
            Vector3.up,                                    // Axis of rotation
            dialogueCameraRotationOffset                   // Angle in degrees
        );

        // 5. Switch Cameras
        mainGameCamera.gameObject.SetActive(false);
        dialogueCamera.gameObject.SetActive(true);

        // 6. Hide the Player Character
        SetPlayerVisibility(false);
    }

    /// <summary>
    /// Switches back to the main game camera and shows the player.
    /// </summary>
    public void EndDialogueView()
    {
        if (mainGameCamera == null || dialogueCamera == null) return;

        dialogueCamera.gameObject.SetActive(false);
        mainGameCamera.gameObject.SetActive(true);
        
        // Show the Player Character
        SetPlayerVisibility(true);
    }
    
    private void SetPlayerVisibility(bool visible)
    {
        if (playerBodyToHide != null)
        {
            // We use SetActive(visible) on the main player body GameObject
            playerBodyToHide.SetActive(visible);
        }
    }
}