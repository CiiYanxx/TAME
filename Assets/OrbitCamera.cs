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
    public float rotateSpeed = 0.25f;
    public float pitchMin = -20f;
    public float pitchMax = 45f;

    private float yaw = 0f;
    private float pitch = 10f;

    private int cameraFinger = -1;
    private Vector2 lastPos;

    void LateUpdate()
    {
        HandleCameraTouch();
        UpdateCamera();
    }

    void HandleCameraTouch()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch t = Input.GetTouch(i);

            // Ignore joystick area ONLY
            if (IsTouchOnJoystick(t.position))
                continue;

            // Assign finger to camera
            if (cameraFinger == -1 && t.phase == TouchPhase.Began)
            {
                cameraFinger = t.fingerId;
                lastPos = t.position;
            }

            if (t.fingerId == cameraFinger)
            {
                if (t.phase == TouchPhase.Moved)
                {
                    Vector2 delta = t.position - lastPos;
                    lastPos = t.position;

                    yaw += delta.x * rotateSpeed;
                    pitch -= delta.y * rotateSpeed;
                    pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);
                }

                if (t.phase == TouchPhase.Ended ||
                    t.phase == TouchPhase.Canceled)
                {
                    cameraFinger = -1;
                }
            }
        }
    }

    void UpdateCamera()
    {
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0);
        Vector3 pos = target.position + Vector3.up * heightOffset - rot * Vector3.forward * distance;

        camTransform.position = pos;
        camTransform.LookAt(target.position + Vector3.up * heightOffset);
    }

    // Real joystick detector
    bool IsTouchOnJoystick(Vector2 screenPos)
    {
    if (EventSystem.current == null) return false;
    PointerEventData ped = new PointerEventData(EventSystem.current) { position = screenPos };
    var results = new List<RaycastResult>();
    EventSystem.current.RaycastAll(ped, results);
    foreach (var r in results)
    {
        if (r.gameObject.GetComponentInParent<bl_Joystick>() != null) return true;
    }
    return false;
    }
}
