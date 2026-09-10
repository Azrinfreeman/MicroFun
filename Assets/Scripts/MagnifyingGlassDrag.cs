using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Attach to the MagnifyingGlass Image. Drag over a card to highlight its answer.
/// </summary>
public class MagnifyingGlassDrag : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Drop Targets (assign Card1, Card2, Card3 in Inspector)")]
    public RectTransform[] dropTargets;

    [Header("Answer Rows (assign Answer1, Answer2, Answer3 in Inspector)")]
    public Image[] answerHighlights;

    [Header("Highlight Color")]
    public Color highlightColor = new Color(0.4f, 0.85f, 0.3f, 1f);
    public float returnSpeed = 8f;

    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 homePosition;
    private bool returning;
    private int homeSiblingIndex;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        homePosition = rectTransform.anchoredPosition;
        homeSiblingIndex = rectTransform.GetSiblingIndex();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        returning = false;
        StopAllCoroutines();
        rectTransform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas == null)
        {
            return;
        }

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint))
        {
            rectTransform.anchoredPosition = localPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        int hit = FindHit(eventData);

        for (int index = 0; index < answerHighlights.Length; index++)
        {
            if (answerHighlights[index] == null)
            {
                continue;
            }

            answerHighlights[index].color = index == hit
                ? highlightColor
                : new Color(1f, 0.99f, 0.93f, 0.95f);
        }

        StartCoroutine(ReturnHome());
    }

    private int FindHit(PointerEventData eventData)
    {
        for (int index = 0; index < dropTargets.Length; index++)
        {
            RectTransform target = dropTargets[index];
            if (target != null && RectTransformUtility.RectangleContainsScreenPoint(
                    target, eventData.position, eventData.pressEventCamera))
            {
                return index;
            }
        }

        return -1;
    }

    private IEnumerator ReturnHome()
    {
        returning = true;

        while (returning && Vector2.Distance(rectTransform.anchoredPosition, homePosition) > 1f)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(
                rectTransform.anchoredPosition,
                homePosition,
                Time.deltaTime * returnSpeed);
            yield return null;
        }

        rectTransform.anchoredPosition = homePosition;
        rectTransform.SetSiblingIndex(homeSiblingIndex);
        returning = false;
    }
}
