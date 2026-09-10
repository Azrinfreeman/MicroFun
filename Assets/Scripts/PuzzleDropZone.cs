using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Accepts only the configured Pengenalan puzzle piece.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class PuzzleDropZone : MonoBehaviour, IDropHandler
{
    [SerializeField] private string expectedPieceId = "piece-09";
    [SerializeField] private RectTransform snapTarget;
    [SerializeField] private PengenalanPuzzleController controller;

    public RectTransform SnapTarget
    {
        get { return snapTarget != null ? snapTarget : transform as RectTransform; }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData == null || eventData.pointerDrag == null || controller == null || controller.IsComplete)
        {
            return;
        }

        DraggablePuzzlePiece piece = eventData.pointerDrag.GetComponent<DraggablePuzzlePiece>();
        if (piece != null && piece.PieceId == expectedPieceId)
        {
            controller.CompletePuzzle(piece, this);
        }
    }
}
