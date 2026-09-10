// ============================================================
// QuizManager.cs  —  Scene V1 (Objective Quiz)
// Controls question flow, answer checking, score updates,
// feedback display, and transition to SceneV2.
//
// Setup: Attach to a GameObject in SceneV1.
// Fill all Inspector fields by dragging UI objects in.
// ============================================================
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    // ── Inspector References ──────────────────────────────────
    [Header("Question Data")]
    public MCQQuestion[] questions; // Fill this array in the Inspector

    [Header("UI Elements")]
    public TextMeshProUGUI questionText;
    public Image questionImage;
    public Button[] answerButtons; // Exactly 4 buttons
    public TextMeshProUGUI[] buttonLabels; // One label per button (A/B/C/D text)
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI scoreText;
    public GameObject nextButton; // "Next Question" button GameObject

    [Header("Visual Feedback Colors")]
    public Color correctColor = new Color(0.2f, 0.8f, 0.4f); // Green
    public Color wrongColor = new Color(0.9f, 0.3f, 0.3f); // Red
    public Color defaultColor = Color.white;

    [Header("Scene Transition")]
    public string sceneToLoadOnComplete = "SceneV2";
    public float feedbackDisplaySeconds = 1.5f;

    [Header("Components")]
    public SceneLoader sceneLoader; // Drag SceneLoader component here
    public UIAnimator uiAnimator; // Optional — for button animations

    // ── Private State ─────────────────────────────────────────
    private int currentIndex = 0;
    private bool hasAnswered = false;

    // ── Lifecycle ─────────────────────────────────────────────
    private void Start()
    {
        feedbackText.gameObject.SetActive(false);
        nextButton.SetActive(false);

        // ✅ ADD THIS LINE — wires the button click in code, no Inspector linking needed
        nextButton.GetComponent<Button>().onClick.AddListener(OnNextButtonClicked);

        DisplayQuestion(currentIndex);
        UpdateScoreUI();
    }

    // ── Display a Question ────────────────────────────────────
    private void DisplayQuestion(int index)
    {
        if (index >= questions.Length)
        {
            // All questions answered — move to next scene
            sceneLoader.LoadScene(sceneToLoadOnComplete);
            return;
        }

        // ✅ Always reset these at the start of each new question
        hasAnswered = false;
        feedbackText.gameObject.SetActive(false);
        nextButton.SetActive(false); // ← move it here (remove from bottom)

        MCQQuestion q = questions[index];
        hasAnswered = false;

        // Set question text
        questionText.text = q.questionText;

        // Load image from Resources/Images/
        Sprite img = Resources.Load<Sprite>($"Images/{q.imageName}");
        if (img != null)
        {
            questionImage.sprite = img;
            questionImage.gameObject.SetActive(true);
        }
        else
        {
            questionImage.gameObject.SetActive(false);
            if (!string.IsNullOrEmpty(q.imageName))
                Debug.LogWarning($"[QuizManager] Image not found: Resources/Images/{q.imageName}");
        }

        // Set button labels and reset colours
        for (int i = 0; i < answerButtons.Length; i++)
        {
            buttonLabels[i].text = (i < q.options.Length) ? q.options[i] : "";
            ResetButtonColor(i);
            answerButtons[i].interactable = true;

            // Capture index for lambda closure
            int capturedIndex = i;
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(capturedIndex));
        }

        // Hide feedback from previous question
        feedbackText.gameObject.SetActive(false);
        nextButton.SetActive(false);

        // Animate question in (optional)
        uiAnimator?.AnimateFadeIn(questionText.gameObject);
    }

    // ── Answer Selected ───────────────────────────────────────
    public void OnAnswerSelected(int selectedIndex)
    {
        if (hasAnswered)
            return; // Prevent double-tapping
        hasAnswered = true;

        MCQQuestion q = questions[currentIndex];
        bool isCorrect = (selectedIndex == q.correctAnswerIndex);

        // Tell GameManager to log this question
        GameManager.Instance.RegisterQuestion();
        if (isCorrect)
            GameManager.Instance.AddScore();

        // Highlight correct and (if wrong) selected button
        HighlightButtons(selectedIndex, q.correctAnswerIndex);

        // Show feedback text
        feedbackText.text = isCorrect ? q.feedbackCorrect : q.feedbackWrong;
        feedbackText.color = isCorrect ? correctColor : wrongColor;
        feedbackText.gameObject.SetActive(true);

        // Disable all buttons so player can't change answer
        foreach (var btn in answerButtons)
            btn.interactable = false;

        UpdateScoreUI();

        // Auto-advance or show Next button
        StartCoroutine(ShowNextAfterDelay());
    }

    // ── Next Button (called from Inspector or coroutine) ──────
    public void OnNextButtonClicked()
    {
        currentIndex++;
        DisplayQuestion(currentIndex);
    }

    // ── Helpers ───────────────────────────────────────────────
    private void HighlightButtons(int selected, int correct)
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            ColorBlock cb = answerButtons[i].colors;
            if (i == correct)
                cb.normalColor = correctColor;
            else if (i == selected)
                cb.normalColor = wrongColor;
            else
                cb.normalColor = defaultColor;
            answerButtons[i].colors = cb;
        }

        // Bounce the selected button (optional)
        uiAnimator?.AnimateBounce(answerButtons[selected].gameObject);
    }

    private void ResetButtonColor(int i)
    {
        ColorBlock cb = answerButtons[i].colors;
        cb.normalColor = defaultColor;
        answerButtons[i].colors = cb;
    }

    private void UpdateScoreUI()
    {
        scoreText.text =
            $"Skor: {GameManager.Instance.Score} / {GameManager.Instance.TotalQuestions}";
    }

    private IEnumerator ShowNextAfterDelay()
    {
        yield return new WaitForSeconds(feedbackDisplaySeconds);
        nextButton.SetActive(true);
    }
}
