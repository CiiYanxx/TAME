using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float mouseSensitivity = 150f;

    CharacterController controller;
    float rotationY = 0f;   // character rotation

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;   // lock mouse in center
    }

    void Update()
    {
        // -------- Mouse Rotation --------
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        rotationY += mouseX;

        transform.rotation = Quaternion.Euler(0, rotationY, 0);

        // -------- Movement (WASD) --------
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Move relative to character's forward direction
        Vector3 move = transform.right * horizontal + transform.forward * vertical;

        controller.Move(move * moveSpeed * Time.deltaTime);
    }
}
