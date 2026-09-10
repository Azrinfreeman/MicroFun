// ============================================================
// ReflectionManager.cs  —  Scene V3 (Self-Reflection)
// Shows reflection statements ONE AT A TIME.
// Player picks an emoji response for each statement.
// After the last statement, Submit button appears.
// On Submit: saves all data, shows summary screen.
// Attach to a GameObject in SceneV3.
// ============================================================
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReflectionManager : MonoBehaviour
{
    // ── Inspector References ──────────────────────────────────
    [Header("Reflection Statements")]
    public ReflectionStatement[] statements; // Fill in Inspector

    [Header("Statement Panels")]
    // One panel per statement — e.g. Statement1Panel, Statement2Panel
    public GameObject[] statementPanels;

    // StatementText (TMP) inside each panel
    public TextMeshProUGUI[] statementTexts;

    [Header("Emoji Buttons (per statement)")]
    // Order: [stmt0_understand, stmt0_unsure, stmt0_dontunderstand,
    //         stmt1_understand, stmt1_unsure, stmt1_dontunderstand, ...]
    // Total = 3 x number of statements
    public Button[] emojiButtons;

    public Color selectedEmojiColor = new Color(1f, 0.85f, 0.2f); // Gold
    public Color defaultEmojiColor = Color.white;

    [Header("Submit")]
    public Button submitButton;
    public TextMeshProUGUI submitWarningText; // Optional — can leave unassigned

    [Header("Summary Panel")]
    public GameObject summaryPanel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI reflectionSummaryText;
    public Button restartButton;

    [Header("Components (optional)")]
    public UIAnimator uiAnimator; // Leave empty if not using animations

    // ── Private State ─────────────────────────────────────────
    private int[] responses; // -1 = unanswered, 0/1/2 = emoji choice
    private int currentStatementIndex = 0; // Which statement is currently shown

    // ── Start ─────────────────────────────────────────────────
    private void Start()
    {
        // Initialise response array — all unanswered
        responses = new int[statements.Length];
        for (int i = 0; i < responses.Length; i++)
            responses[i] = -1;

        // Hide everything at start
        summaryPanel.SetActive(false);
        submitButton.gameObject.SetActive(false);
        if (submitWarningText != null)
            submitWarningText.gameObject.SetActive(false);

        // Hide all statement panels — we show them one by one
        foreach (var panel in statementPanels)
            if (panel != null)
                panel.SetActive(false);

        SetupStatements();
        SetupEmojiButtons();

        // Wire Submit and Restart buttons
        submitButton.onClick.AddListener(OnSubmit);
        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestart);

        // Show the first statement
        ShowStatement(0);
    }

    // ── Setup Text for All Statements ────────────────────────
    private void SetupStatements()
    {
        for (int i = 0; i < statements.Length; i++)
        {
            if (i < statementTexts.Length && statementTexts[i] != null)
                statementTexts[i].text = statements[i].statementText;
        }
    }

    // ── Wire All Emoji Button Listeners ──────────────────────
    private void SetupEmojiButtons()
    {
        // Each statement has 3 buttons: index 0=Understand, 1=Unsure, 2=DontUnderstand
        for (int s = 0; s < statements.Length; s++)
        {
            for (int e = 0; e < 3; e++)
            {
                int btnIndex = s * 3 + e;
                if (btnIndex >= emojiButtons.Length)
                    break;
                if (emojiButtons[btnIndex] == null)
                    continue;

                int capturedStatement = s;
                int capturedEmoji = e;

                emojiButtons[btnIndex].onClick.RemoveAllListeners();
                emojiButtons[btnIndex]
                    .onClick.AddListener(() => OnEmojiSelected(capturedStatement, capturedEmoji));
            }
        }
    }

    // ── Show One Statement Panel ──────────────────────────────
    private void ShowStatement(int index)
    {
        // Hide all panels first
        foreach (var panel in statementPanels)
            if (panel != null)
                panel.SetActive(false);

        if (index >= statementPanels.Length || statementPanels[index] == null)
        {
            Debug.LogError($"[ReflectionManager] statementPanels[{index}] is missing.");
            return;
        }

        // Make sure all 3 emoji buttons for this statement are interactable
        for (int e = 0; e < 3; e++)
        {
            int btnIndex = index * 3 + e;
            if (btnIndex < emojiButtons.Length && emojiButtons[btnIndex] != null)
            {
                emojiButtons[btnIndex].interactable = true;

                // Reset button colour to default
                Image img = emojiButtons[btnIndex].GetComponent<Image>();
                if (img != null)
                    img.color = defaultEmojiColor;
            }
        }

        // Show and animate the panel
        statementPanels[index].SetActive(true);
        uiAnimator?.AnimateFadeIn(statementPanels[index]);
    }

    // ── Emoji Button Pressed ──────────────────────────────────
    private void OnEmojiSelected(int statementIndex, int emojiType)
    {
        // Ignore taps on non-active statements
        if (statementIndex != currentStatementIndex)
            return;

        responses[statementIndex] = emojiType;

        // Highlight the selected button, reset the other two
        for (int e = 0; e < 3; e++)
        {
            int btnIndex = statementIndex * 3 + e;
            if (btnIndex >= emojiButtons.Length)
                break;
            if (emojiButtons[btnIndex] == null)
                continue;

            Image img = emojiButtons[btnIndex].GetComponent<Image>();
            if (img != null)
                img.color = (e == emojiType) ? selectedEmojiColor : defaultEmojiColor;
        }

        // Optional bounce animation on selected button
        int selectedBtnIndex = statementIndex * 3 + emojiType;
        if (selectedBtnIndex < emojiButtons.Length)
            uiAnimator?.AnimateBounce(emojiButtons[selectedBtnIndex].gameObject);

        // Disable all buttons in this row so player cannot change answer
        for (int e = 0; e < 3; e++)
        {
            int btnIndex = statementIndex * 3 + e;
            if (btnIndex < emojiButtons.Length && emojiButtons[btnIndex] != null)
                emojiButtons[btnIndex].interactable = false;
        }

        // Advance to next statement after a short pause
        StartCoroutine(AdvanceAfterDelay(0.6f));
    }

    // ── Advance to Next Statement or Show Submit ──────────────
    private IEnumerator AdvanceAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        currentStatementIndex++;

        if (currentStatementIndex < statementPanels.Length)
        {
            // More statements remaining — show the next one
            ShowStatement(currentStatementIndex);
        }
        else
        {
            // All statements answered — hide panels and reveal Submit
            foreach (var panel in statementPanels)
                if (panel != null)
                    panel.SetActive(false);

            submitButton.gameObject.SetActive(true);
            uiAnimator?.AnimateFadeIn(submitButton.gameObject);
            OnSubmit();
        }
    }

    // ── Submit Button Pressed ─────────────────────────────────
    private void OnSubmit()
    {
        // Safety check — all responses must be filled
        // (should always pass since Submit only appears after all are answered)
        foreach (int r in responses)
        {
            if (r == -1)
            {
                Debug.LogWarning(
                    "[ReflectionManager] Unanswered statement found — this should not happen."
                );
                return;
            }
        }

        // Save each reflection response to GameManager
        for (int i = 0; i < statements.Length; i++)
        {
            GameManager.Instance.AddReflection(
                statements[i].statementText,
                (ReflectionResponse)responses[i]
            );
        }

        // Persist score + reflection data to PlayerPrefs
        GameManager.Instance.SaveAll();

        // Hide submit, show summary
        submitButton.gameObject.SetActive(false);
        ShowSummary();
    }

    // ── Build and Show Summary Screen ────────────────────────
    private void ShowSummary()
    {
        summaryPanel.SetActive(true);
        uiAnimator?.AnimateFadeIn(summaryPanel);

        // Final score line
        if (finalScoreText != null)
        {
            finalScoreText.text =
                $"Skor Akhir: {GameManager.Instance.Score} / {GameManager.Instance.TotalQuestions}\n"
                + $"({GameManager.Instance.GetScorePercent()})";
        }

        // Reflection summary — one line per statement
        if (reflectionSummaryText != null)
        {
            string[] emojiLabels = { "😊 Faham", "😐 Tidak Pasti", "😟 Tidak Faham" };
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("Refleksi Anda:");

            for (int i = 0; i < statements.Length; i++)
            {
                string label =
                    (responses[i] >= 0 && responses[i] < 3) ? emojiLabels[responses[i]] : "—";
                sb.AppendLine($"• {statements[i].statementText}: {label}");
            }

            reflectionSummaryText.text = sb.ToString();
        }
    }

    // ── Restart Game ──────────────────────────────────────────
    private void OnRestart()
    {
        // Resets score and reloads SceneV1 via GameManager
        GameManager.Instance.ResetGame();
    }
}
