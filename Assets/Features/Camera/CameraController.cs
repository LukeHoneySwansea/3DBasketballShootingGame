using UnityEngine;
using UnityEngine.InputSystem;

// Controls camera rotation based on mouse input (FPS-style look)

public class CameraController : MonoBehaviour
{
    #region SERIALIZED FIELDS

    [SerializeField] private Camera playerCamera;

    [Header("Settings")]
    [SerializeField] private float sensitivity = 0.1f;
    [SerializeField] private float minVerticalAngle = -80f;
    [SerializeField] private float maxVerticalAngle = 80f;

    #endregion

    #region PRIVATE FIELDS

    private float _xRotation = 0f;
    private bool _isActive = false;

    #endregion

    #region UNITY METHODS

    private void Awake()
    {
        // Auto-assign camera if not set
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();

            if (playerCamera == null)
            {
                Debug.LogError("CameraController: No Camera found in children.", this);
            }
        }
    }

    private void Update()
    {
        if (!_isActive) return;
        if (Mouse.current == null) return;

        HandleMouseLook();
    }

    #endregion

    #region CAMERA CONTROL

    /// <summary>
    /// Handles mouse input and applies camera rotation
    /// </summary>
    private void HandleMouseLook()
    {
        Vector2 mouseInput = Mouse.current.delta.ReadValue() * sensitivity;

        float mouseX = mouseInput.x;
        float mouseY = mouseInput.y;

        // Vertical rotation (camera only)
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, minVerticalAngle, maxVerticalAngle);

        playerCamera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

        // Horizontal rotation (player body)
        transform.Rotate(Vector3.up * mouseX);
    }

    #endregion

    #region PUBLIC CONTROL

    /// <summary>
    /// Enables camera control and locks cursor
    /// </summary>
    public void EnableCamera()
    {
        _isActive = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// Disables camera control and unlocks cursor
    /// </summary>
    public void DisableCamera()
    {
        _isActive = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    #endregion
}