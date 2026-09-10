using UnityEngine;

/// <summary>Fits this page only; leaves the shared activity Canvas unchanged.</summary>
[ExecuteAlways]
public sealed class KehadiranAirLayout : MonoBehaviour
{
    void OnEnable() { Fit(); }
    void LateUpdate() { Fit(); }
    void Fit()
    {
        var parent = transform.parent as RectTransform;
        if (parent == null) return;
        float scale = Mathf.Min(parent.rect.width / 1920f, parent.rect.height / 1080f);
        if (scale > 0f) transform.localScale = Vector3.one * scale;
    }
}
