using UnityEngine;
using TMPro; // Required for TextMeshPro

public class VictoryController : MonoBehaviour
{
    [Header("Victory Elements")]
    public GameObject victoryText;
    public ParticleSystem surpriseParticles;

    [Header("Audio (Optional)")]
    public AudioSource celebrationSFX;

    private void Start()
    {
        // Ensure everything is hidden/stopped at the start of the scene
        if (victoryText != null) victoryText.SetActive(false);
        if (surpriseParticles != null) surpriseParticles.Stop();
    }

    // Call this function to trigger the victory!
    public void TriggerVictory()
    {
        // 1. Show the text
        if (victoryText != null)
        {
            victoryText.SetActive(true);
        }

        // 2. Play the surprise effect
        if (surpriseParticles != null)
        {
            surpriseParticles.Play();
        }

        // 3. Play audio if you have it attached
        if (celebrationSFX != null)
        {
            celebrationSFX.Play();
        }

        Debug.Log("Game Finished! Congratulations!");
    }

    // For testing: Press the Spacebar to trigger the victory instantly
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TriggerVictory();
        }
    }
}