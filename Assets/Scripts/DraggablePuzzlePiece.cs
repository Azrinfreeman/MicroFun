using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Draggable UI puzzle piece with deterministic reset and snap behaviour.
/// </summary>
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class DraggablePuzzlePiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private string pieceId = "piece-09";
    [SerializeField] private RectTransform dragLayer;

    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Canvas _rootCanvas;
    private Transform _startParent;
    private Vector2 _startAnchoredPosition;
    private Vector2 _startAnchorMin;
    private Vector2 _startAnchorMax;
    private Vector2 _startPivot;
    private Vector2 _startSizeDelta;
    private int _startSiblingIndex;
    private bool _placed;
    private bool _dragging;

    public string PieceId { get { return pieceId; } }

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _rootCanvas = GetComponentInParent<Canvas>();

        _startParent = transform.parent;
        _startSiblingIndex = transform.GetSiblingIndex();
        _startAnchoredPosition = _rectTransform.anchoredPosition;
        _startAnchorMin = _rectTransform.anchorMin;
        _startAnchorMax = _rectTransform.anchorMax;
        _startPivot = _rectTransform.pivot;
        _startSizeDelta = _rectTransform.sizeDelta;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_placed)
        {
            return;
        }

        _dragging = true;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.alpha = 0.9f;

        if (dragLayer != null)
        {
            transform.SetParent(dragLayer, true);
        }

        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_dragging || _placed)
        {
            return;
        }

        float scaleFactor = _rootCanvas != null ? Mathf.Max(0.0001f, _rootCanvas.scaleFactor) : 1f;
        _rectTransform.anchoredPosition += eventData.delta / scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_dragging)
        {
            return;
        }

        _dragging = false;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.alpha = 1f;

        if (!_placed)
        {
            ReturnToStart();
        }
    }

    public void PlaceInSlot(RectTransform snapTarget)
    {
        if (snapTarget == null)
        {
            return;
        }

        _placed = true;
        _dragging = false;
        transform.SetParent(snapTarget, false);
        _rectTransform.anchorMin = Vector2.zero;
        _rectTransform.anchorMax = Vector2.one;
        _rectTransform.pivot = new Vector2(0.5f, 0.5f);
        _rectTransform.offsetMin = Vector2.zero;
        _rectTransform.offsetMax = Vector2.zero;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.alpha = 1f;
    }

    public void ResetPiece()
    {
        if (_rectTransform == null)
        {
            return;
        }

        _placed = false;
        _dragging = false;
        transform.SetParent(_startParent, false);
        transform.SetSiblingIndex(Mathf.Min(_startSiblingIndex, transform.parent.childCount - 1));
        _rectTransform.anchorMin = _startAnchorMin;
        _rectTransform.anchorMax = _startAnchorMax;
        _rectTransform.pivot = _startPivot;
        _rectTransform.sizeDelta = _startSizeDelta;
        _rectTransform.anchoredPosition = _startAnchoredPosition;
        _rectTransform.localScale = Vector3.one;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.alpha = 1f;
    }

    private void ReturnToStart()
    {
        transform.SetParent(_startParent, false);
        transform.SetSiblingIndex(Mathf.Min(_startSiblingIndex, transform.parent.childCount - 1));
        _rectTransform.anchorMin = _startAnchorMin;
        _rectTransform.anchorMax = _startAnchorMax;
        _rectTransform.pivot = _startPivot;
        _rectTransform.sizeDelta = _startSizeDelta;
        _rectTransform.anchoredPosition = _startAnchoredPosition;
        _rectTransform.localScale = Vector3.one;
    }
}
