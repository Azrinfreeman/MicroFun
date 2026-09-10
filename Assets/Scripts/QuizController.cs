using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizController : MonoBehaviour
{
    public static QuizController instance;
    public int totalQuestions { get { return questions == null ? 0 : questions.Count; } }

    [System.Serializable]
    public class Question
    {
        [TextArea(2, 5)] public string question;
        public List<string> choices = new List<string>();
        public int correctIndex;
        public List<Sprite> choiceIcons = new List<Sprite>();
        public AudioClip narrationClip;
    }

    [Header("Quiz content")]
    public List<Question> questions = new List<Question>();

    [Header("Scene references")]
    public List<Transform> buttonAnswers = new List<Transform>();
    public Transform questionText;
    public Transform WinPanel;
    public TextMeshProUGUI indicatorText;
    public AudioSource wrong;
    public AudioSource correct;
    [SerializeField] private Button nextButton;

    [Header("Kuiz 8 presentation")]
    [SerializeField] private bool useSubmitConfirmation;
    [SerializeField] private TextMeshProUGUI actionButtonLabel;
    [SerializeField] private List<Image> choiceIconImages = new List<Image>();
    [SerializeField] private Button narrationButton;
    [SerializeField] private AudioSource narrationSource;

    private int currentQuestionIndex;
    private int pendingAnswerIndex = -1;
    private bool answerResolved;

    private readonly Color idleCardColor = new Color(1f, 0.98f, 0.87f, 1f);
    private readonly Color selectedCardColor = new Color(0.90f, 0.98f, 0.68f, 1f);
    private readonly Color correctCardColor = new Color(0.72f, 0.94f, 0.55f, 1f);
    private readonly Color wrongCardColor = new Color(1f, 0.72f, 0.64f, 1f);

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (nextButton == null)
        {
            GameObject foundButton = GameObject.Find("PilihJawapanButton");
            if (foundButton == null) foundButton = GameObject.Find("SeterusnyaButton");
            if (foundButton != null) nextButton = foundButton.GetComponent<Button>();
        }

        if (nextButton != null)
        {
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(HandleActionButton);
        }

        if (narrationButton != null)
        {
            narrationButton.onClick.RemoveAllListeners();
            narrationButton.onClick.AddListener(PlayCurrentQuestionAudio);
        }

        if (WinPanel != null) WinPanel.gameObject.SetActive(false);
        currentQuestionIndex = 0;
        LoadQuestion();
    }

    private void LoadQuestion()
    {
        if (questions == null || questions.Count == 0) return;

        answerResolved = false;
        pendingAnswerIndex = -1;
        Question currentQuestion = questions[currentQuestionIndex];

        if (questionText != null)
        {
            TextMeshProUGUI questionLabel = questionText.GetComponent<TextMeshProUGUI>();
            if (questionLabel != null)
            {
                questionLabel.text = string.IsNullOrWhiteSpace(currentQuestion.question)
                    ? "Apakah definisi mikroorganisma?"
                    : currentQuestion.question;
            }
        }

        if (indicatorText != null)
            indicatorText.text = "Soalan " + (currentQuestionIndex + 1) + "/" + questions.Count;

        for (int i = 0; i < buttonAnswers.Count; i++)
        {
            if (buttonAnswers[i] == null) continue;

            bool visible = i < currentQuestion.choices.Count;
            buttonAnswers[i].gameObject.SetActive(visible);
            if (!visible) continue;

            Button answerButton = buttonAnswers[i].GetComponent<Button>();
            Image answerImage = buttonAnswers[i].GetComponent<Image>();
            TextMeshProUGUI answerLabel = buttonAnswers[i].GetComponentInChildren<TextMeshProUGUI>(true);

            if (answerImage != null) answerImage.color = idleCardColor;
            if (answerLabel != null) answerLabel.text = currentQuestion.choices[i];

            Transform letterLabel = buttonAnswers[i].Find("LetterBadge/LetterLabel");
            if (letterLabel != null)
            {
                TextMeshProUGUI letterText = letterLabel.GetComponent<TextMeshProUGUI>();
                if (letterText != null) letterText.text = ((char)('A' + i)).ToString();
            }

            if (answerButton != null)
            {
                answerButton.interactable = true;
                answerButton.onClick.RemoveAllListeners();
                int answerIndex = i;
                answerButton.onClick.AddListener(delegate
                {
                    if (useSubmitConfirmation) SelectPendingAnswer(answerIndex);
                    else SelectAnswer(answerIndex);
                });
            }
        }

        RefreshChoiceIcons(currentQuestion);

        if (nextButton != null) nextButton.interactable = !useSubmitConfirmation && answerResolved;
        SetActionLabel(useSubmitConfirmation ? "PILIH JAWAPAN" : "SETERUSNYA");

        if (narrationButton != null)
            narrationButton.interactable = currentQuestion.narrationClip != null && narrationSource != null;
    }

    public void SelectPendingAnswer(int answerIndex)
    {
        if (answerResolved || questions == null || currentQuestionIndex >= questions.Count) return;

        Question currentQuestion = questions[currentQuestionIndex];
        if (answerIndex < 0 || answerIndex >= currentQuestion.choices.Count) return;

        pendingAnswerIndex = answerIndex;
        for (int i = 0; i < buttonAnswers.Count; i++)
        {
            if (buttonAnswers[i] == null || !buttonAnswers[i].gameObject.activeSelf) continue;
            SetAnswerColor(i, i == pendingAnswerIndex ? selectedCardColor : idleCardColor);
        }

        if (nextButton != null) nextButton.interactable = true;
    }

    public void SubmitPendingAnswer()
    {
        if (!useSubmitConfirmation || answerResolved || pendingAnswerIndex < 0) return;
        EvaluateAnswer(pendingAnswerIndex);
    }

    public void SelectAnswer(int answerIndex)
    {
        if (useSubmitConfirmation)
        {
            SelectPendingAnswer(answerIndex);
            return;
        }

        EvaluateAnswer(answerIndex);
    }

    private void HandleActionButton()
    {
        if (answerResolved)
        {
            NextQuestion();
            return;
        }

        if (useSubmitConfirmation) SubmitPendingAnswer();
    }

    private void EvaluateAnswer(int answerIndex)
    {
        if (answerResolved || questions == null || currentQuestionIndex >= questions.Count) return;

        Question currentQuestion = questions[currentQuestionIndex];
        if (answerIndex < 0 || answerIndex >= currentQuestion.choices.Count) return;

        if (answerIndex == currentQuestion.correctIndex)
        {
            answerResolved = true;
            PlaySound(correct);

            if (ScoreController.instance != null) ScoreController.instance.AddScore(1);

            for (int i = 0; i < buttonAnswers.Count; i++)
            {
                Button answerButton = buttonAnswers[i] == null ? null : buttonAnswers[i].GetComponent<Button>();
                if (answerButton != null) answerButton.interactable = false;
            }

            SetAnswerColor(answerIndex, correctCardColor);
            if (nextButton != null) nextButton.interactable = true;
            SetActionLabel("SETERUSNYA");
        }
        else
        {
            PlaySound(wrong);
            StartCoroutine(ShowWrongAnswer(answerIndex));
        }
    }

    private IEnumerator ShowWrongAnswer(int answerIndex)
    {
        if (answerIndex < 0 || answerIndex >= buttonAnswers.Count || buttonAnswers[answerIndex] == null)
            yield break;

        Button pressedButton = buttonAnswers[answerIndex].GetComponent<Button>();
        if (pressedButton != null) pressedButton.interactable = false;

        SetAnswerColor(answerIndex, wrongCardColor);
        yield return new WaitForSeconds(0.45f);

        Color restore = useSubmitConfirmation && pendingAnswerIndex == answerIndex
            ? selectedCardColor
            : idleCardColor;
        SetAnswerColor(answerIndex, restore);

        if (!answerResolved && pressedButton != null) pressedButton.interactable = true;
    }

    public void NextQuestion()
    {
        if (!answerResolved) return;

        if (currentQuestionIndex < questions.Count - 1)
        {
            currentQuestionIndex++;
            LoadQuestion();
            return;
        }

        if (WinPanel != null) WinPanel.gameObject.SetActive(true);
    }

    public void PlayCurrentQuestionAudio()
    {
        if (questions == null || currentQuestionIndex < 0 || currentQuestionIndex >= questions.Count)
            return;

        AudioClip clip = questions[currentQuestionIndex].narrationClip;
        if (clip == null || narrationSource == null) return;

        narrationSource.Stop();
        narrationSource.clip = clip;
        narrationSource.Play();
    }

    private void RefreshChoiceIcons(Question currentQuestion)
    {
        for (int i = 0; i < choiceIconImages.Count; i++)
        {
            Image icon = choiceIconImages[i];
            if (icon == null) continue;

            Sprite sprite = currentQuestion.choiceIcons != null && i < currentQuestion.choiceIcons.Count
                ? currentQuestion.choiceIcons[i]
                : null;
            icon.sprite = sprite;
            icon.enabled = sprite != null;
            icon.gameObject.SetActive(sprite != null);
        }
    }

    private void SetActionLabel(string value)
    {
        if (actionButtonLabel != null) actionButtonLabel.text = value;
    }

    private void SetAnswerColor(int answerIndex, Color color)
    {
        if (answerIndex < 0 || answerIndex >= buttonAnswers.Count || buttonAnswers[answerIndex] == null) return;
        Image answerImage = buttonAnswers[answerIndex].GetComponent<Image>();
        if (answerImage != null) answerImage.color = color;
    }

    private void PlaySound(AudioSource source)
    {
        if (source != null) source.Play();
    }
}
