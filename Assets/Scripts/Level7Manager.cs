using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Level7Manager : MonoBehaviour
{
    [System.Serializable]
    public class QuestionData
    {
        public string situationText;

        public string correctMicroorganism;
        public string correctHarm;
        public string correctJustification;

        [TextArea]
        public string correctFeedback;

        [TextArea]
        public string wrongFeedback;
    }

    [Header("Question Data")]
    public QuestionData[] questions;
    private int currentQuestionIndex = 0;

    [Header("UI Text")]
    public TextMeshProUGUI situationText;
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI progressText;

    [Header("Drop Slots")]
    public DropSlot microorganismSlot;
    public DropSlot harmSlot;
    public DropSlot justificationSlot;

    [Header("Cards")]
    public Transform answerCardPanel;
    public DraggableCard[] allCards;

    [Header("Buttons")]
    public Button checkAnswerButton;
    public Button nextLevelButton;

    [Header("Level Settings")]
    public string nextSceneName = "Level7";

    private int score = 0;

    private void Start()
    {
        nextLevelButton.gameObject.SetActive(false);

        checkAnswerButton.onClick.AddListener(CheckAnswer);

        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.AddListener(GoToNextLevel);
        }

        foreach (DraggableCard card in allCards)
        {
            card.SetOriginalPosition();
        }

        LoadQuestion();
    }

    private void LoadQuestion()
    {
        if (currentQuestionIndex >= questions.Length)
        {
            CompleteLevel();
            return;
        }

        QuestionData currentQuestion = questions[currentQuestionIndex];

        situationText.text = "Situasi: " + currentQuestion.situationText;

        if (progressText != null)
        {
            progressText.text = "Soalan " + (currentQuestionIndex + 1) + " / " + questions.Length;
        }

        feedbackText.text = "";

        ResetSlots();
        ResetCards();
    }

    private void CheckAnswer()
    {
        QuestionData currentQuestion = questions[currentQuestionIndex];

        bool microorganismCorrect = microorganismSlot.HasCorrectCard(
            currentQuestion.correctMicroorganism
        );
        bool harmCorrect = harmSlot.HasCorrectCard(currentQuestion.correctHarm);
        bool justificationCorrect = justificationSlot.HasCorrectCard(
            currentQuestion.correctJustification
        );

        if (microorganismCorrect && harmCorrect && justificationCorrect)
        {
            score++;
            feedbackText.text = currentQuestion.correctFeedback;

            currentQuestionIndex++;

            Invoke(nameof(LoadQuestion), 1.5f);
        }
        else
        {
            feedbackText.text = currentQuestion.wrongFeedback;
        }
    }

    private void ResetSlots()
    {
        microorganismSlot.ClearSlot();
        harmSlot.ClearSlot();
        justificationSlot.ClearSlot();
    }

    private void ResetCards()
    {
        foreach (DraggableCard card in allCards)
        {
            card.ResetCardToPanel(answerCardPanel);
        }
    }

    private void CompleteLevel()
    {
        situationText.text = "Tahniah! Anda telah melengkapkan Level 6.";
        feedbackText.text = "Markah: " + score + " / " + questions.Length;

        checkAnswerButton.gameObject.SetActive(false);

        if (nextLevelButton != null)
        {
            nextLevelButton.gameObject.SetActive(true);
        }
    }

    private void GoToNextLevel()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
