using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropSlot : MonoBehaviour, IDropHandler
{
    [Header("Slot Data")]
    public string slotID;
    public Transform cardHolder;
    public TextMeshProUGUI slotLabel;

    [Header("Current Card")]
    public DraggableCard currentCard;

    public void OnDrop(PointerEventData eventData)
    {
        DraggableCard draggedCard = eventData.pointerDrag.GetComponent<DraggableCard>();

        if (draggedCard == null)
        {
            return;
        }

        if (currentCard != null)
        {
            currentCard.ReturnToOriginalPosition();
            currentCard.currentSlot = null;
        }

        currentCard = draggedCard;
        draggedCard.PlaceInSlot(this);
    }

    public void ClearSlot()
    {
        currentCard = null;
    }

    public bool HasCorrectCard(string correctCardID)
    {
        if (currentCard == null)
        {
            return false;
        }

        return currentCard.cardID == correctCardID;
    }
}
