using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public bl_Joystick moveJoystick;
    public Transform cameraPivot;

    [Header("Gravity")]
    public float gravity = -20f;
    private float verticalVelocity = 0f;

    public float correctControllerHeight = 0.06f; 
    public float correctControllerRadius = 0.02f;
    

    private CharacterController controller;
    private Animator anim;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        
        anim = GetComponent<Animator>(); // Animator reference

        // 1. Force the CharacterController to the correct dimensions
        controller.height = correctControllerHeight;
        controller.radius = correctControllerRadius;
        
        controller.center = new Vector3(0, correctControllerHeight / 2f, 0);
        
        
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

            // rotate to direction
            transform.rotation = Quaternion.LookRotation(dir);

            move = dir * moveSpeed;
        }

        // gravity
        if (controller.isGrounded)
            verticalVelocity = -1f;
        else
            verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = move + Vector3.up * verticalVelocity;
        controller.Move(velocity * Time.deltaTime);

        // --- ANIMATION ---
        // Speed = how fast character is moving (0 = idle, anything else = run)
        anim.SetFloat("Speed", move.magnitude);
    }
}
