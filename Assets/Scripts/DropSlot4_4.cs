using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DropSlot4_4 : MonoBehaviour, IDropHandler
{
    [Header("Correct Answer")]
    public string correctItemName = "Water";

    [Header("UI")]
    public TMP_Text feedbackText;

    [Header("Quiz Manager")]
    public QuizManager4 quizManager;

    private bool alreadyAnswered = false;

    public void OnDrop(PointerEventData eventData)
    {
        if (alreadyAnswered)
        {
            return;
        }

        GameObject droppedObject = eventData.pointerDrag;

        if (droppedObject == null)
        {
            return;
        }

        DraggableItem draggableItem = droppedObject.GetComponent<DraggableItem>();

        if (draggableItem == null)
        {
            return;
        }

        RectTransform droppedRect = droppedObject.GetComponent<RectTransform>();
        RectTransform slotRect = GetComponent<RectTransform>();

        droppedObject.transform.SetParent(transform);
        droppedRect.anchoredPosition = Vector2.zero;

        if (draggableItem.itemName == correctItemName)
        {
            alreadyAnswered = true;
            feedbackText.text = "Correct! Water helps microorganisms grow.";
            quizManager.CompleteDragDrop(true);
        }
        else
        {
            feedbackText.text = "Not quite. Try another answer.";
            draggableItem.ReturnToStart();
        }
    }
}