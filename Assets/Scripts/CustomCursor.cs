using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    [Header("Cursor Settings")]
    [Tooltip("Drag your custom cursor Texture2D here.")]
    public Texture2D cursorTexture;

    [Tooltip("The exact pixel coordinate where the click happens (0,0 is top-left).")]
    public Vector2 hotSpot = Vector2.zero;

    // CursorMode.Auto uses hardware cursors when possible for better performance
    private CursorMode cursorMode = CursorMode.Auto;

    void Start()
    {
        // Change the cursor as soon as this scene starts
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

    void OnDestroy()
    {
        // Reset back to the default system cursor when this scene unloads or the object is destroyed
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }
}
