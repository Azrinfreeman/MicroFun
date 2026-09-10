using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Plays or pauses the narration assigned to a learning-page audio button.
/// </summary>
[RequireComponent(typeof(AudioSource), typeof(Button))]
public class AudioNarrationButton : MonoBehaviour
{
    [SerializeField] private AudioClip narrationClip;
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private string idleLabel = "Dengar audio";
    [SerializeField] private string playingLabel = "Jeda audio";

    private AudioSource audioSource;
    private Button button;
    private bool isPaused;
    private bool wasPlaying;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        button = GetComponent<Button>();
        label ??= GetComponentInChildren<TextMeshProUGUI>(true);

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.clip = narrationClip;
        button.onClick.AddListener(ToggleNarration);
        UpdateLabel(false);
    }

    private void Update()
    {
        if (wasPlaying && !audioSource.isPlaying && !isPaused)
        {
            UpdateLabel(false);
        }

        wasPlaying = audioSource.isPlaying;
    }

    private void OnDisable()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }

        isPaused = false;
        UpdateLabel(false);
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(ToggleNarration);
        }
    }

    private void ToggleNarration()
    {
        if (narrationClip == null)
        {
            Debug.LogWarning($"{name} has no narration clip assigned.", this);
            return;
        }

        if (audioSource.isPlaying)
        {
            audioSource.Pause();
            isPaused = true;
            UpdateLabel(false);
            return;
        }

        if (isPaused)
        {
            audioSource.UnPause();
        }
        else
        {
            audioSource.Play();
        }

        isPaused = false;
        UpdateLabel(true);
    }

    private void UpdateLabel(bool isPlaying)
    {
        if (label != null)
        {
            label.text = isPlaying ? playingLabel : idleLabel;
        }
    }
}