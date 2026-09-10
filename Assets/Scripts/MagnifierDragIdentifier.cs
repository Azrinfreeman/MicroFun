using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class MagnifierDragIdentifier : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Serializable]
    public sealed class IdentificationTarget
    {
        public RectTransform zone;
        public string microorganismName;
        public Color accent = Color.white;
    }

    [Header("References")]
    [SerializeField] private RectTransform draggableRect;
    [SerializeField] private RectTransform dragBounds;
    [SerializeField] private Canvas rootCanvas;
    [SerializeField] private TextMeshProUGUI resultLabel;
    [SerializeField] private Image resultPill;
    [SerializeField] private Image lensGlow;
    [SerializeField] private IdentificationTarget[] targets;

    [Header("Feedback")]
    [SerializeField] private float normalScale = 1f;
    [SerializeField] private float dragScale = 1.08f;

    private int activeTarget = int.MinValue;

    private void Reset()
    {
        draggableRect = transform as RectTransform;
        rootCanvas = GetComponentInParent<Canvas>();
        dragBounds = rootCanvas != null ? rootCanvas.transform as RectTransform : null;
    }

    private void Awake()
    {
        if (draggableRect == null) draggableRect = transform as RectTransform;
        if (rootCanvas == null) rootCanvas = GetComponentInParent<Canvas>();
        if (dragBounds == null && rootCanvas != null) dragBounds = rootCanvas.transform as RectTransform;
        SetIdentification(-1);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetAsLastSibling();
        draggableRect.localScale = Vector3.one * dragScale;
        UpdateIdentification();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggableRect == null || dragBounds == null) return;

        var camera = rootCanvas != null && rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay
            ? rootCanvas.worldCamera
            : null;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(dragBounds, eventData.position, camera, out var point))
            return;

        var bounds = dragBounds.rect;
        var half = draggableRect.rect.size * 0.5f;
        point.x = Mathf.Clamp(point.x, bounds.xMin + half.x, bounds.xMax - half.x);
        point.y = Mathf.Clamp(point.y, bounds.yMin + half.y, bounds.yMax - half.y);
        draggableRect.anchoredPosition = point;
        UpdateIdentification();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        draggableRect.localScale = Vector3.one * normalScale;
        UpdateIdentification();
    }

    private void UpdateIdentification()
    {
        var camera = rootCanvas != null && rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay
            ? rootCanvas.worldCamera
            : null;

        for (var i = 0; i < targets.Length; i++)
        {
            var target = targets[i];
            if (target != null && target.zone != null &&
                RectTransformUtility.RectangleContainsScreenPoint(target.zone, draggableRect.position, camera))
            {
                SetIdentification(i);
                return;
            }
        }

        SetIdentification(-1);
    }

    private void SetIdentification(int index)
    {
        if (activeTarget == index) return;
        activeTarget = index;

        var found = index >= 0 && index < targets.Length && targets[index] != null;
        if (resultLabel != null)
        {
            resultLabel.gameObject.SetActive(found);
            resultLabel.text = found ? targets[index].microorganismName : string.Empty;
        }

        if (resultPill != null)
        {
            resultPill.gameObject.SetActive(found);
            if (found) resultPill.color = targets[index].accent;
        }

        if (lensGlow != null)
        {
            lensGlow.color = found
                ? new Color(targets[index].accent.r, targets[index].accent.g, targets[index].accent.b, .55f)
                : new Color(1f, 1f, 1f, .18f);
        }
    }
}