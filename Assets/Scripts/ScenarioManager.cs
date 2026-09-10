// ============================================================
// ScenarioManager.cs  —  Scene V2 (Scenario Application)
// Shows a real-world scenario image + question.
// After answering, displays a detailed explanation panel.
// Attach to a GameObject in SceneV2.
// ============================================================
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioManager : MonoBehaviour
{
    // ── Inspector References ──────────────────────────────────
    [Header("Scenario Data")]
    public ScenarioQuestion[] scenarios; // Add your scenarios in Inspector

    [Header("Scenario UI")]
    public Image scenarioImage;
    public TextMeshProUGUI scenarioDescription;
    public TextMeshProUGUI questionText;
    public Button[] answerButtons;
    public TextMeshProUGUI[] buttonLabels;

    [Header("Explanation Panel")]
    public GameObject explanationPanel; // Panel hidden until answer selected
    public TextMeshProUGUI explanationText;
    public GameObject nextSceneButton; // "Proceed to Reflection" button

    [Header("Visual Feedback")]
    public Color correctColor = new Color(0.2f, 0.8f, 0.4f);
    public Color wrongColor = new Color(0.9f, 0.3f, 0.3f);
    public Color defaultColor = Color.white;

    [Header("Scene Transition")]
    public string sceneToLoadOnComplete = "SceneV3";
    public SceneLoader sceneLoader;
    public UIAnimator uiAnimator;

    // ── Private State ─────────────────────────────────────────
    private int currentScenarioIndex = 0;
    private bool hasAnswered = false;

    // ── Start ─────────────────────────────────────────────────
    private void Start()
    {
        explanationPanel.SetActive(false);
        nextSceneButton.SetActive(false);
        LoadScenario(currentScenarioIndex);
    }

    // ── Load Scenario ─────────────────────────────────────────
    private void LoadScenario(int index)
    {
        if (index >= scenarios.Length)
        {
            sceneLoader.LoadScene(sceneToLoadOnComplete);
            return;
        }

        ScenarioQuestion s = scenarios[index];
        hasAnswered = false;

        // Set text content
        scenarioDescription.text = s.scenarioDescription;
        questionText.text = s.questionText;

        // Load scenario image
        Sprite img = Resources.Load<Sprite>($"Images/{s.imageName}");
        if (img != null)
        {
            scenarioImage.sprite = img;
            scenarioImage.gameObject.SetActive(true);
        }
        else
        {
            scenarioImage.gameObject.SetActive(false);
        }

        // Set up answer buttons
        for (int i = 0; i < answerButtons.Length; i++)
        {
            buttonLabels[i].text = (i < s.options.Length) ? s.options[i] : "";
            answerButtons[i].interactable = true;

            // Reset color
            ColorBlock cb = answerButtons[i].colors;
            cb.normalColor = defaultColor;
            answerButtons[i].colors = cb;

            int capturedIndex = i;
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(capturedIndex));
        }

        explanationPanel.SetActive(false);
        nextSceneButton.SetActive(false);

        uiAnimator?.AnimateFadeIn(scenarioDescription.gameObject);
    }

    // ── Answer Selected ───────────────────────────────────────
    public void OnAnswerSelected(int selectedIndex)
    {
        if (hasAnswered)
            return;
        hasAnswered = true;

        ScenarioQuestion s = scenarios[currentScenarioIndex];
        bool isCorrect = (selectedIndex == s.correctAnswerIndex);

        GameManager.Instance.RegisterQuestion();
        if (isCorrect)
            GameManager.Instance.AddScore();

        // Colour correct/wrong buttons
        for (int i = 0; i < answerButtons.Length; i++)
        {
            ColorBlock cb = answerButtons[i].colors;
            if (i == s.correctAnswerIndex)
                cb.normalColor = correctColor;
            else if (i == selectedIndex)
                cb.normalColor = wrongColor;
            else
                cb.normalColor = defaultColor;
            answerButtons[i].colors = cb;
            answerButtons[i].interactable = false;
        }

        // Show explanation with animation
        explanationText.text = s.explanation;
        explanationPanel.SetActive(true);
        uiAnimator?.AnimateFadeIn(explanationPanel);

        StartCoroutine(ShowNextButtonAfterDelay(1.5f));
    }

    // ── "Next Scenario" or "Go to SceneV3" ───────────────────
    public void OnNextClicked()
    {
        currentScenarioIndex++;
        if (currentScenarioIndex >= scenarios.Length)
            sceneLoader.LoadScene(sceneToLoadOnComplete);
        else
            LoadScenario(currentScenarioIndex);
    }

    private IEnumerator ShowNextButtonAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        nextSceneButton.SetActive(true);
        uiAnimator?.AnimateBounce(nextSceneButton);
    }
}
