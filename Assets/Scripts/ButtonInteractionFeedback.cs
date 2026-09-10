using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Reusable tactile feedback for the large learning-page controls.
/// </summary>
public class ButtonInteractionFeedback : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField, Range(0.9f, 1f)] private float pressedScale = 0.95f;
    [SerializeField, Range(1f, 1.1f)] private float hoverScale = 1.03f;

    private Vector3 initialScale;

    private void Awake()
    {
        initialScale = transform.localScale;
    }

    private void OnDisable()
    {
        transform.localScale = initialScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = initialScale * hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = initialScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.localScale = initialScale * pressedScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.localScale = initialScale * hoverScale;
    }
}