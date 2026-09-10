using UnityEngine;

public class LandingPageController : MonoBehaviour
{
    public GameObject landingPage;
    public GameObject subMenu;
    public AudioSource audioSource;
    public AudioClip voiceoverClip;

    public void OnMulaTerokaClicked()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        if (landingPage != null)
        {
            landingPage.SetActive(false);
        }
        
        if (subMenu != null)
        {
            subMenu.SetActive(true);
        }
    }

    public void OnDengarAudioClicked()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        if (voiceoverClip != null)
        {
            audioSource.clip = voiceoverClip;
        }

        if (audioSource.clip != null)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            else
            {
                audioSource.Play();
            }
        }
    }
}
