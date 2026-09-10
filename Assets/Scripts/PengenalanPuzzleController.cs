using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Coordinates the single-piece recognition puzzle used by Aktiviti 8 / Pengenalan.
/// </summary>
public class PengenalanPuzzleController : MonoBehaviour
{
    [Header("Puzzle")]
    [SerializeField] private DraggablePuzzlePiece loosePiece;
    [SerializeField] private PuzzleDropZone targetSlot;
    [SerializeField] private Graphic missingPieceCover;
    [SerializeField] private RectTransform puzzleBoard;

    [Header("Feedback")]
    [SerializeField] private TMP_Text statusLabel;
    [SerializeField] private string readyMessage = "Seret kepingan ungu ke ruang kosong.";
    [SerializeField] private string completeMessage = "Puzzle lengkap! Syabas!";
    [SerializeField] private Button nextButton;
    [SerializeField] private bool lockNextUntilComplete;

    public bool IsComplete { get; private set; }

    private Vector3 _boardBaseScale = Vector3.one;
    private Coroutine _feedbackRoutine;

    private void Awake()
    {
        if (puzzleBoard != null)
        {
            _boardBaseScale = puzzleBoard.localScale;
        }
    }

    private void Start()
    {
        ResetPuzzle();
    }

    public void CompletePuzzle(DraggablePuzzlePiece piece, PuzzleDropZone slot)
    {
        if (IsComplete || piece == null || slot == null || slot != targetSlot)
        {
            return;
        }

        IsComplete = true;
        if (missingPieceCover != null)
        {
            missingPieceCover.enabled = false;
        }

        piece.PlaceInSlot(slot.SnapTarget);

        if (statusLabel != null)
        {
            statusLabel.text = completeMessage;
            statusLabel.color = new Color32(31, 132, 49, 255);
        }

        if (nextButton != null && lockNextUntilComplete)
        {
            nextButton.interactable = true;
        }

        if (_feedbackRoutine != null)
        {
            StopCoroutine(_feedbackRoutine);
        }

        _feedbackRoutine = StartCoroutine(PlayCompletionFeedback());
    }

    public void ResetPuzzle()
    {
        IsComplete = false;

        if (_feedbackRoutine != null)
        {
            StopCoroutine(_feedbackRoutine);
            _feedbackRoutine = null;
        }

        if (puzzleBoard != null)
        {
            puzzleBoard.localScale = _boardBaseScale;
        }

        if (missingPieceCover != null)
        {
            missingPieceCover.enabled = true;
        }

        if (loosePiece != null)
        {
            loosePiece.ResetPiece();
        }

        if (statusLabel != null)
        {
            statusLabel.text = readyMessage;
            statusLabel.color = new Color32(23, 91, 47, 255);
        }

        if (nextButton != null && lockNextUntilComplete)
        {
            nextButton.interactable = false;
        }
    }

    private IEnumerator PlayCompletionFeedback()
    {
        if (puzzleBoard == null)
        {
            yield break;
        }

        const float duration = 0.36f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float pulse = Mathf.Sin(t * Mathf.PI) * 0.035f;
            puzzleBoard.localScale = _boardBaseScale * (1f + pulse);
            yield return null;
        }

        puzzleBoard.localScale = _boardBaseScale;
        _feedbackRoutine = null;
    }
}
