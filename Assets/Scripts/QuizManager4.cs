using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizManager4 : MonoBehaviour
{
    [Header("Panels")]
    public GameObject startPanel;
    public GameObject quizPanel;
    public GameObject dragDropPanel;
    public GameObject reflectionPanel;
    public GameObject resultPanel;

    [Header("Quiz UI")]
    public Image questionImage;
    public TMP_Text questionText;
    public TMP_Text feedbackText;

    public Button answerButton1;
    public Button answerButton2;
    public Button answerButton3;

    public TMP_Text answerText1;
    public TMP_Text answerText2;
    public TMP_Text answerText3;

    [Header("Result UI")]
    public TMP_Text resultText;

    [Header("Question Images")]
    public Sprite breadMouldImage;
    public Sprite spoiledFoodImage;
    public Sprite microorganismImage;

    private int currentQuestionIndex = 0;
    private int score = 0;

    private QuizQuestion[] questions;

    private void Start()
    {
        CreateQuestions();
        ShowStartPanel();
    }

    private void CreateQuestions()
    {
        questions = new QuizQuestion[]
        {
            new QuizQuestion(
                "Which environment helps microorganisms grow faster?",
                new string[] { "Warm and wet place", "Dry place", "Very cold place" },
                0,
                microorganismImage
            ),

            new QuizQuestion(
                "Why does bread grow mould?",
                new string[] { "Because microorganisms grow on it", "Because it becomes plastic", "Because it has no air" },
                0,
                breadMouldImage
            ),

            new QuizQuestion(
                "Why does food spoil quickly?",
                new string[] { "Because there is moisture and air", "Because it is frozen", "Because it is dry" },
                0,
                spoiledFoodImage
            )
        };
    }

    public void ShowStartPanel()
    {
        startPanel.SetActive(true);
        quizPanel.SetActive(false);
        dragDropPanel.SetActive(false);
        reflectionPanel.SetActive(false);
        resultPanel.SetActive(false);
    }

    public void StartQuiz()
    {
        score = 0;
        currentQuestionIndex = 0;

        startPanel.SetActive(false);
        resultPanel.SetActive(false);

        ShowQuestion();
    }

    private void ShowQuestion()
{
    quizPanel.SetActive(true);
    dragDropPanel.SetActive(false);
    reflectionPanel.SetActive(false);
    resultPanel.SetActive(false);

    feedbackText.text = "";

    // Enable buttons again when new question loads
    SetAnswerButtonsInteractable(true);

    QuizQuestion q = questions[currentQuestionIndex];

    questionText.text = q.question;
    questionImage.sprite = q.questionSprite;

    answerText1.text = q.answers[0];
    answerText2.text = q.answers[1];
    answerText3.text = q.answers[2];

    answerButton1.onClick.RemoveAllListeners();
    answerButton2.onClick.RemoveAllListeners();
    answerButton3.onClick.RemoveAllListeners();

    answerButton1.onClick.AddListener(() => CheckAnswer(0));
    answerButton2.onClick.AddListener(() => CheckAnswer(1));
    answerButton3.onClick.AddListener(() => CheckAnswer(2));
}

private void CheckAnswer(int selectedAnswer)
{
    // Disable buttons immediately after selecting an answer
    SetAnswerButtonsInteractable(false);

    QuizQuestion q = questions[currentQuestionIndex];

    if (selectedAnswer == q.correctAnswerIndex)
    {
        score++;
        feedbackText.text = "Correct! Good job!";
    }
    else
    {
        feedbackText.text = "Try again next time!";
    }

    Invoke(nameof(NextQuestion), 1.2f);
}

private void SetAnswerButtonsInteractable(bool value)
{
    answerButton1.interactable = value;
    answerButton2.interactable = value;
    answerButton3.interactable = value;
}

    private void NextQuestion()
    {
        currentQuestionIndex++;

        if (currentQuestionIndex < questions.Length)
        {
            ShowQuestion();
        }
        else
        {
            ShowDragDropQuestion();
        }
    }

    private void ShowDragDropQuestion()
    {
        quizPanel.SetActive(false);
        dragDropPanel.SetActive(true);
        reflectionPanel.SetActive(false);
        resultPanel.SetActive(false);
    }

    public void CompleteDragDrop(bool isCorrect)
    {
        if (isCorrect)
        {
            score++;
        }

        Invoke(nameof(ShowReflectionPanel), 1.2f);
    }

    private void ShowReflectionPanel()
    {
        quizPanel.SetActive(false);
        dragDropPanel.SetActive(false);
        reflectionPanel.SetActive(true);
        resultPanel.SetActive(false);
    }

    public void ChooseReflection(string reflection)
    {
        Debug.Log("Student reflection: " + reflection);
        ShowResultPanel();
    }

    private void ShowResultPanel()
    {
        quizPanel.SetActive(false);
        dragDropPanel.SetActive(false);
        reflectionPanel.SetActive(false);
        resultPanel.SetActive(true);

        resultText.text = "Great job!\nYour score: " + score + " / 4";
    }

    public void RestartQuiz()
    {
        StartQuiz();
    }
}

[System.Serializable]
public class QuizQuestion
{
    public string question;
    public string[] answers;
    public int correctAnswerIndex;
    public Sprite questionSprite;

    public QuizQuestion(string question, string[] answers, int correctAnswerIndex, Sprite questionSprite)
    {
        this.question = question;
        this.answers = answers;
        this.correctAnswerIndex = correctAnswerIndex;
        this.questionSprite = questionSprite;
    }
}