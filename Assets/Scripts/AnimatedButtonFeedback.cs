using UnityEngine;
using UnityEngine.EventSystems;

public class AnimatedButtonFeedback : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float hoverScale = 1.035f;
    [SerializeField] private float pressedScale = 0.93f;
    [SerializeField] private float animationSpeed = 16f;

    private Vector3 baseScale;
    private Vector3 targetScale;
    private bool pointerInside;

    private void Awake()
    {
        baseScale = transform.localScale;
        targetScale = baseScale;
    }

    private void OnEnable()
    {
        if (baseScale == Vector3.zero) baseScale = transform.localScale;
        transform.localScale = baseScale;
        targetScale = baseScale;
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            1f - Mathf.Exp(-animationSpeed * Time.unscaledDeltaTime));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerInside = true;
        targetScale = baseScale * hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerInside = false;
        targetScale = baseScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        targetScale = baseScale * pressedScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        targetScale = pointerInside ? baseScale * hoverScale : baseScale;
    }

    private void OnDisable()
    {
        transform.localScale = baseScale;
    }
}