using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class OrbitCamera : MonoBehaviour
{
    public Transform target;
    public Transform camTransform;

    [Header("Settings")]
    public float distance = 4f;
    public float heightOffset = 1.6f;
    public float rotateSpeed = 0.15f; // Lowered for better control
    public float pitchMin = -20f;
    public float pitchMax = 45f;

    [Header("Smoothing")]
    public float smoothTime = 0.12f; // Time to reach the target rotation
    private float yaw = 0f;
    private float pitch = 10f;
    private float currentYaw;
    private float currentPitch;
    private float yawVelocity;
    private float pitchVelocity;

    private int cameraFinger = -1;
    private Vector2 lastPos;

    void Start()
    {
        // Initialize angles to current setup
        currentYaw = yaw = transform.eulerAngles.y;
        currentPitch = pitch = 10f;
    }

    void LateUpdate()
    {
        if (target == null) return;

        HandleCameraTouch();
        UpdateCameraSmoothly();
    }

    void HandleCameraTouch()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch t = Input.GetTouch(i);

            // 1. Identify valid touch area
            if (cameraFinger == -1 && t.phase == TouchPhase.Began)
            {
                if (IsTouchOnJoystick(t.position)) continue;
                
                cameraFinger = t.fingerId;
                lastPos = t.position;
            }

            // 2. Process active camera finger
            if (t.fingerId == cameraFinger)
            {
                if (t.phase == TouchPhase.Moved)
                {
                    Vector2 delta = t.position - lastPos;
                    lastPos = t.position;

                    // Update target angles
                    yaw += delta.x * rotateSpeed;
                    pitch -= delta.y * rotateSpeed;
                    pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);
                }

                if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
                {
                    cameraFinger = -1;
                }
            }
        }
    }

    void UpdateCameraSmoothly()
    {
        // 3. Professional Smoothing (SmoothDamp)
        // This creates the 'gliding' effect instead of raw snapping
        currentYaw = Mathf.SmoothDampAngle(currentYaw, yaw, ref yawVelocity, smoothTime);
        currentPitch = Mathf.SmoothDampAngle(currentPitch, pitch, ref pitchVelocity, smoothTime);

        Quaternion rot = Quaternion.Euler(currentPitch, currentYaw, 0);
        
        // 4. Calculate Position
        Vector3 targetLookAt = target.position + Vector3.up * heightOffset;
        Vector3 pos = targetLookAt - (rot * Vector3.forward * distance);

        // Apply to transform
        camTransform.position = pos;
        camTransform.LookAt(targetLookAt);
    }

    bool IsTouchOnJoystick(Vector2 screenPos)
    {
        if (EventSystem.current == null) return false;
        PointerEventData ped = new PointerEventData(EventSystem.current) { position = screenPos };
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(ped, results);
        foreach (var r in results)
        {
            // Specifically ignore touches on the Joystick or any UI marked as Raycast Target
            if (r.gameObject.GetComponentInParent<bl_Joystick>() != null) return true;
        }
        return false;
    }
}