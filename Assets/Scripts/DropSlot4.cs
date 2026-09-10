using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DropSlot4 : MonoBehaviour, IDropHandler
{
    [Header("Correct Answer")]
    public string correctAnswerID;

    [Header("UI")]
    public TMP_Text feedbackText;

    private RectTransform slotRect;

    void Awake()
    {
        slotRect = GetComponent<RectTransform>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        DraggableAnswer draggedAnswer = eventData.pointerDrag.GetComponent<DraggableAnswer>();

        if (draggedAnswer == null)
            return;

        draggedAnswer.transform.SetParent(transform);

        RectTransform draggedRect = draggedAnswer.GetComponent<RectTransform>();
        draggedRect.anchoredPosition = Vector2.zero;

        if (draggedAnswer.answerID == correctAnswerID)
        {
            feedbackText.text = "Betul! Pemerhatian kamu tepat.";
            feedbackText.color = Color.green;

            draggedAnswer.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
        else
        {
            feedbackText.text = "Cuba lagi. Perhatikan semula hasil simulasi.";
            feedbackText.color = Color.red;

            draggedAnswer.ReturnToOriginalPosition();
        }
    }
}