// ============================================================
// SceneLoader.cs
// Handles smooth fade-to-black transitions between scenes.
// Attach to the same GameObject as QuizManager / ScenarioManager.
// Requires a full-screen Image child called "FadePanel" in Canvas.
// ============================================================
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [Header("Fade Panel")]
    // A full-screen black Image in the Canvas — set alpha to 0 by default
    public Image fadePanel;

    [Header("Fade Settings")]
    public float fadeDuration = 0.5f;

    private void Awake()
    {
        // Make sure fade panel is invisible at scene start
        if (fadePanel != null)
        {
            Color c = fadePanel.color;
            c.a = 0f;
            fadePanel.color = c;
            fadePanel.gameObject.SetActive(true);
            fadePanel.raycastTarget = false; // Don't block input when invisible
        }
    }

    // ── Public Methods ────────────────────────────────────────

    /// Load a scene with a fade-out → load → fade-in transition
    public void LoadScene(string sceneName)
    {
        StartCoroutine(FadeAndLoad(sceneName));
    }

    /// Fade in from black on scene start (call from Start() of scene manager)
    public void FadeIn()
    {
        StartCoroutine(FadeFromBlack());
    }

    // ── Coroutines ────────────────────────────────────────────
    private IEnumerator FadeAndLoad(string sceneName)
    {
        // Block input during fade
        if (fadePanel != null)
            fadePanel.raycastTarget = true;

        // Fade to black
        yield return StartCoroutine(Fade(0f, 1f));

        // Load next scene
        SceneManager.LoadScene(sceneName);

        // The new scene's Start() should call FadeIn() to fade back in
    }

    private IEnumerator FadeFromBlack()
    {
        // Start fully black
        if (fadePanel != null)
        {
            Color c = fadePanel.color;
            c.a = 1f;
            fadePanel.color = c;
            fadePanel.raycastTarget = true;
        }
        yield return new WaitForEndOfFrame();
        yield return StartCoroutine(Fade(1f, 0f));
        if (fadePanel != null)
            fadePanel.raycastTarget = false;
    }

    /// Generic alpha fade for the fade panel
    private IEnumerator Fade(float fromAlpha, float toAlpha)
    {
        if (fadePanel == null)
            yield break;

        float elapsed = 0f;
        Color c = fadePanel.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(fromAlpha, toAlpha, elapsed / fadeDuration);
            fadePanel.color = c;
            yield return null; // Wait one frame
        }

        c.a = toAlpha;
        fadePanel.color = c;
    }
}
