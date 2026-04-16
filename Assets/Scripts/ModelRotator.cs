using UnityEngine;
using UnityEngine.InputSystem;

public class ModelRotator : MonoBehaviour
{
    [Header("Input Actions")]
    public InputActionProperty clickAction;
    public InputActionProperty dragAction;

    [Tooltip("The action for zooming (e.g., Mouse Scroll).")]
    public InputActionProperty scrollAction;

    [Header("Rotation Settings")]
    public float rotationSpeed = 0.5f;
    public bool invertX = false;
    public bool invertY = false;

    [Header("Zoom Settings")]
    [Tooltip("How fast the camera zooms. Scroll wheel values are large, so keep this small.")]
    public float zoomSpeed = 0.05f;

    [Tooltip("The closest you can zoom in.")]
    public float minZoom = 20f;

    [Tooltip("The furthest you can zoom out.")]
    public float maxZoom = 90f;

    private bool isDragging = false;
    private Camera mainCamera;

    private void Start()
    {
        // Automatically find the main camera in your scene
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        clickAction.action.Enable();
        dragAction.action.Enable();
        scrollAction.action.Enable();

        clickAction.action.started += _ => isDragging = true;
        clickAction.action.canceled += _ => isDragging = false;
    }

    private void OnDisable()
    {
        clickAction.action.Disable();
        dragAction.action.Disable();
        scrollAction.action.Disable();

        clickAction.action.started -= _ => isDragging = true;
        clickAction.action.canceled -= _ => isDragging = false;
    }

    void Update()
    {
        // --- ROTATION LOGIC ---
        if (isDragging)
        {
            Vector2 dragDelta = dragAction.action.ReadValue<Vector2>();

            float rotationX = dragDelta.x * rotationSpeed;
            float rotationY = dragDelta.y * rotationSpeed;

            if (invertX)
                rotationX = -rotationX;
            if (invertY)
                rotationY = -rotationY;

            transform.Rotate(Vector3.up, -rotationX, Space.World);
            transform.Rotate(Vector3.right, rotationY, Space.World);
        }

        // --- ZOOM LOGIC ---
        if (mainCamera != null)
        {
            // The scroll wheel outputs a Vector2. The Y axis represents scrolling up/down.
            float scrollValue = scrollAction.action.ReadValue<Vector2>().y;

            if (scrollValue != 0)
            {
                // Subtracting the value zooms in when scrolling up, adding zooms out
                mainCamera.fieldOfView -= scrollValue * zoomSpeed;

                // Clamp the FOV so the camera doesn't flip upside down or zoom into infinity
                mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView, minZoom, maxZoom);
            }
        }
    }
}
