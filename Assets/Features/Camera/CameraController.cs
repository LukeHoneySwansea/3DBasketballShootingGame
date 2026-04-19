using UnityEngine;
using UnityEngine.InputSystem;

// Controls the camera rotation based on mouse input
public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float sensitivity = 0.1f;

    private float xRotation = 0f;
    private bool isActive = false;

    void Awake()
    {
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();

            if (playerCamera == null)
            {
                Debug.LogError("CameraController: No Camera found in children.");
            }
        }
    }

    // Call this from your menu event to enable camera control
    public void EnableCamera()
    {
        isActive = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void DisableCamera()
    {
        isActive = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        if (!isActive) return;

        Vector2 mouseInput = Mouse.current.delta.ReadValue() * sensitivity;

        float mouseX = mouseInput.x;
        float mouseY = mouseInput.y;

        // Vertical rotation
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Horizontal rotation
        transform.Rotate(Vector3.up * mouseX);
    }
}