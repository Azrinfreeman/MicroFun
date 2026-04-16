using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

// Aktiviti V1: Kuiz Objektif Bergambar
// Attach to a Canvas > Panel in your scene
public class QuizV1 : MonoBehaviour
{
    [Header("UI References")]
    public Image          questionImage;
    public TextMeshProUGUI questionText;
    public Button[]        choiceButtons;   // 4 buttons (A-D)
    public TextMeshProUGUI[] choiceLabels;
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI scoreText;
    public GameObject      matchingPanel;   // for Padanan mode

    [Header("Question Data")]
    public Sprite[]  questionImages;
    public string[]  questions;
    public string[,] choices = new string[3, 4]
    {
        { "Membuat roti",   "Membuat besi",   "Mencetak buku",  "Mengalir air"  },
        { "Penapaian",       "Pendidihan",      "Penyejukan",     "Penyaringan"   },
        { "Menaikkan doh",   "Mewarnakan roti", "Mengeraskan roti","Menyejukkan roti"}
    };
    public int[] correctIndex = { 0, 0, 0 }; // index of correct choice per question

    private int current = 0;
    private int score   = 0;

    void Start() => LoadQuestion(0);

    void LoadQuestion(int i)
    {
        current                = i;
        questionText.text      = questions[i];
        if (questionImages.Length > i) questionImage.sprite = questionImages[i];
        feedbackText.text      = "";
        for (int j = 0; j < choiceButtons.Length; j++)
        {
            choiceLabels[j].text = choices[i, j];
            int captured = j;
            choiceButtons[j].onClick.RemoveAllListeners();
            choiceButtons[j].onClick.AddListener(() => OnChoiceSelected(captured));
            choiceButtons[j].interactable = true;
        }
        scoreText.text = $"Skor: {score}";
    }

    void OnChoiceSelected(int chosen)
    {
        bool correct = chosen == correctIndex[current];
        if (correct) { score++; feedbackText.text = "Betul!"; }
        else         { feedbackText.text = $"Salah. Jawapan: {choices[current, correctIndex[current]]}"; }

        foreach (var b in choiceButtons) b.interactable = false;
        StartCoroutine(NextAfterDelay());
    }

    IEnumerator NextAfterDelay()
    {
        yield return new WaitForSeconds(1.5f);
        if (current + 1 < questions.Length) LoadQuestion(current + 1);
        else QuizManager.Instance.OnV1Complete(score, questions.Length);
    }
}