using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public bl_Joystick moveJoystick;
    public Transform cameraPivot;

    [Header("Gravity")]
    public float gravity = -20f;     // stronger for stable feel
    private float verticalVelocity = 0f;

    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        Vector2 input = moveJoystick != null ? moveJoystick.Direction : Vector2.zero;

        // deadzone
        if (Mathf.Abs(input.x) < 0.12f) input.x = 0;
        if (Mathf.Abs(input.y) < 0.12f) input.y = 0;

        Vector3 move = Vector3.zero;

        if (input.magnitude > 0.01f)
        {
            // camera-relative movement
            Vector3 camForward = cameraPivot.forward;
            Vector3 camRight = cameraPivot.right;
            camForward.y = 0;
            camRight.y = 0;
            Vector3 dir = (camForward * input.y + camRight * input.x).normalized;

            // instant rotation to movement (no momentum)
            transform.rotation = Quaternion.LookRotation(dir);

            move = dir * moveSpeed;
        }

        // gravity (CharacterController does NOT apply gravity for you)
        if (controller.isGrounded)
        {
            verticalVelocity = -1f; // small downward to keep grounded
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        Vector3 velocity = move + Vector3.up * verticalVelocity;
        controller.Move(velocity * Time.deltaTime);
    }
}
