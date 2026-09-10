// ============================================================
// UIAnimator.cs
// Reusable animations: fade-in, bounce, shake.
// Attach to any manager or a dedicated AnimationManager GameObject.
// Other scripts call e.g. uiAnimator.AnimateBounce(button.gameObject)
// ============================================================
using System.Collections;
using UnityEngine;

public class UIAnimator : MonoBehaviour
{
    // ── Configurable Animation Settings ───────────────────────
    [Header("Bounce Settings")]
    public float bounceScale = 1.15f;
    public float bounceDuration = 0.12f;

    [Header("Fade In Settings")]
    public float fadeInDuration = 0.3f;

    [Header("Shake Settings")]
    public float shakeMagnitude = 8f;
    public float shakeDuration = 0.3f;

    // ── Public Animation Methods ──────────────────────────────

    /// Scale the object up then back to normal (for button presses)
    public void AnimateBounce(GameObject target)
    {
        if (target == null)
            return;
        StartCoroutine(BounceCoroutine(target));
    }

    /// Fade a CanvasGroup from 0 to 1 alpha (requires CanvasGroup component)
    public void AnimateFadeIn(GameObject target)
    {
        if (target == null)
            return;
        StartCoroutine(FadeInCoroutine(target));
    }

    /// Shake a UI element left-right (for wrong answer feedback)
    public void AnimateShake(GameObject target)
    {
        if (target == null)
            return;
        StartCoroutine(ShakeCoroutine(target));
    }

    // ── Coroutines ────────────────────────────────────────────

    private IEnumerator BounceCoroutine(GameObject target)
    {
        RectTransform rt = target.GetComponent<RectTransform>();
        if (rt == null)
            yield break;

        Vector3 originalScale = rt.localScale;
        Vector3 targetScale = originalScale * bounceScale;
        float half = bounceDuration / 2f;

        // Scale up
        float t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            rt.localScale = Vector3.Lerp(originalScale, targetScale, t / half);
            yield return null;
        }

        // Scale back down
        t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            rt.localScale = Vector3.Lerp(targetScale, originalScale, t / half);
            yield return null;
        }

        rt.localScale = originalScale;
    }

    private IEnumerator FadeInCoroutine(GameObject target)
    {
        // Ensure CanvasGroup exists
        CanvasGroup cg = target.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = target.AddComponent<CanvasGroup>();

        cg.alpha = 0f;
        float elapsed = 0f;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Clamp01(elapsed / fadeInDuration);
            yield return null;
        }

        cg.alpha = 1f;
    }

    private IEnumerator ShakeCoroutine(GameObject target)
    {
        RectTransform rt = target.GetComponent<RectTransform>();
        if (rt == null)
            yield break;

        Vector3 originalPos = rt.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            float x = Random.Range(-shakeMagnitude, shakeMagnitude);
            rt.anchoredPosition = originalPos + new Vector3(x, 0f, 0f);
            yield return null;
        }

        rt.anchoredPosition = originalPos;
    }
}
