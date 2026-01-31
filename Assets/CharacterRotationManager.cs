using UnityEngine;

public class CharacterRotationManager : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("Drag the child of your Prefab that contains the meshes here.")]
    [SerializeField] private Transform characterModel; 

    [Header("Settings")]
    [SerializeField] private float rotationSpeed = 200f;
    [SerializeField] private float snapSpeed = 8f;

    private Quaternion _initialRotation;
    private int _rotateDirection = 0; // 0 = stop, 1 = left, -1 = right

    void Start()
    {
        if (characterModel != null)
        {
            // Store the local rotation so it snaps back to '0' relative to the parent
            _initialRotation = characterModel.localRotation;
        }
    }

    void LateUpdate()
    {
        if (characterModel == null) return;

        if (_rotateDirection != 0)
        {
            // While a button is held, this runs
            characterModel.Rotate(Vector3.up, _rotateDirection * rotationSpeed * Time.deltaTime, Space.Self);
        }
        else
        {
            // When no button is held (_rotateDirection is 0), this snaps it back
            characterModel.localRotation = Quaternion.Lerp(
                characterModel.localRotation, 
                _initialRotation, 
                Time.deltaTime * snapSpeed
            );
        }
    }

    // Called by Left Button (Pointer Down)
    public void StartRotateLeft()
    {
        _rotateDirection = 1;
    }

    // Called by Right Button (Pointer Down)
    public void StartRotateRight()
    {
        _rotateDirection = -1;
    }

    // Called by BOTH Buttons (Pointer Up)
    public void StopRotation()
    {
        _rotateDirection = 0;
    }
}