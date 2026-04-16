using UnityEngine;
using UnityEngine.UI; // Required for UI Slider and Button
using UnityEngine.Video; // Required for VideoPlayer

public class VideoPlayerControllerSeek : MonoBehaviour
{
    [Header("Components")]
    [Tooltip("Drag your Video Player here")]
    public VideoPlayer videoPlayer;

    [Tooltip("Drag your UI Slider (Progress Bar) here")]
    public Slider progressBar;

    [Header("Play Button Icon Settings (Optional)")]
    [Tooltip("Drag your Play icon sprite here")]
    public Sprite playIcon;

    [Tooltip("Drag your Pause icon sprite here")]
    public Sprite pauseIcon;

    [Tooltip("Drag the image component on the play button object here")]
    public Image buttonIconImage;

    // A flag to keep track of whether the user is actively seeking
    private bool _isUserSeeking = false;

    // Remember if the video was playing before the user started seeking
    private bool _wasPlayingBeforeSeek = false;

    void Start()
    {
        // Set up the progress bar for percentage (0 to 1 is cleaner for seeking)
        if (progressBar != null)
        {
            progressBar.minValue = 0f;
            progressBar.maxValue = 1f;
            progressBar.value = 0f;
        }

        // Optional: Ensure video starts playing and update button icon
        if (videoPlayer != null && videoPlayer.playOnAwake)
        {
            UpdateButtonIcon(true);
        }
    }

    void Update()
    {
        // Check if the video is ready and has a length greater than 0
        // CRITICAL: Only update the slider if the video is playing AND the user IS NOT actively seeking
        if (
            videoPlayer != null
            && videoPlayer.length > 0
            && videoPlayer.isPlaying
            && !_isUserSeeking
        )
        {
            // Calculate the percentage of completion (0 to 1)
            float progressPercentage = (float)(videoPlayer.time / videoPlayer.length);

            // Update the slider's position to match the video time
            progressBar.value = progressPercentage;
        }
    }

    // --- BUTTON METHODS ---

    public void TogglePlayPause()
    {
        if (videoPlayer != null)
        {
            if (videoPlayer.isPlaying)
            {
                videoPlayer.Pause();
                UpdateButtonIcon(false);
            }
            else
            {
                videoPlayer.Play();
                UpdateButtonIcon(true);
            }
        }
    }

    private void UpdateButtonIcon(bool isPlaying)
    {
        if (buttonIconImage != null && playIcon != null && pauseIcon != null)
        {
            buttonIconImage.sprite = isPlaying ? pauseIcon : playIcon;
        }
    }

    // --- SEEKING INTERACTION METHODS (Called from the Slider itself) ---

    // 1. Called constantly while the user is actively dragging the slider
    // Link this to the Slider's `onValueChanged` event in the Unity Editor
    public void OnProgressBarValueChanged(float value)
    {
        // Check if the user is seeking and the video is valid
        if (_isUserSeeking && videoPlayer != null && videoPlayer.length > 0)
        {
            // Calculate the target time based on the slider value (percentage * length)
            // Note: videoPlayer.time is a double, slider value is a float
            double targetTime = (double)(value * videoPlayer.length);

            // Seek the video to that time
            videoPlayer.time = targetTime;
        }
    }

    // 2. Called from a separate script on the Slider when user clicks down
    public void OnProgressBarPointerDown()
    {
        if (videoPlayer != null)
        {
            // Set the flag to true
            _isUserSeeking = true;

            // Remember if the video was playing
            _wasPlayingBeforeSeek = videoPlayer.isPlaying;

            // Optional: Pause the video while seeking for smoother scrubbing
            if (_wasPlayingBeforeSeek)
            {
                videoPlayer.Pause();
            }
        }
    }

    // 3. Called from a separate script on the Slider when user lets go
    public void OnProgressBarPointerUp()
    {
        if (videoPlayer != null)
        {
            // Set the flag to false
            _isUserSeeking = false;

            // Optional: Resume playing if it was playing before the seek
            if (_wasPlayingBeforeSeek)
            {
                videoPlayer.Play();
            }
        }
    }
}
