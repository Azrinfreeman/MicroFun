using UnityEngine;
using UnityEngine.EventSystems; // Required for touch/pointer events

// Implementing these interfaces allows the script to detect mouse/touch interactions
public class UISliderSeekEvents : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Controller Reference")]
    [Tooltip("Drag the GameObject with your 'VideoPlayerControllerSeek' script here")]
    public VideoPlayerControllerSeek videoController;

    // Fired when the user clicks down on the slider
    public void OnPointerDown(PointerEventData eventData)
    {
        if (videoController != null)
        {
            // Tell the main controller the user is starting to seek
            videoController.OnProgressBarPointerDown();
        }
    }

    // Fired when the user lets go of the slider
    public void OnPointerUp(PointerEventData eventData)
    {
        if (videoController != null)
        {
            // Tell the main controller the user has finished seeking
            videoController.OnProgressBarPointerUp();
        }
    }
}
