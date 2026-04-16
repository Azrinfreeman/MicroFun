using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Aktiviti V3: Refleksi Kendiri Murid
// Icon scale: 😊 (Faham) | 😐 (Kurang Faham) | 😕 (Tidak Faham)
public class QuizV3 : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI promptText;
    public Button btnFaham; // happy icon button
    public Button btnKurangFaham; // neutral icon button
    public Button btnTidakFaham; // sad icon button
    public TextMeshProUGUI responseText;
    public GameObject summaryPanel;
    public TextMeshProUGUI summaryText;

    public enum ReflectionLevel
    {
        Faham,
        KurangFaham,
        TidakFaham,
    }

    private readonly string[] topics =
    {
        "Kegunaan yis dalam kehidupan seharian",
        "Proses penapaian mikroorganisma",
        "Peranan mikroorganisma dalam industri makanan",
    };

    private readonly string[] feedbackFaham =
    {
        "Bagus! Teruskan semangat belajar.",
        "Hebat! Kamu faham proses penapaian.",
        "Cemerlang! Kamu bersedia untuk topik seterusnya.",
    };
    private readonly string[] feedbackKurang =
    {
        "Okay! Cuba baca semula bahagian yis.",
        "Cuba tonton video penapaian sekali lagi.",
        "Ulangkaji rajah mikroorganisma dalam buku.",
    };
    private readonly string[] feedbackTidak =
    {
        "Jangan risau — tanya guru untuk penerangan lanjut.",
        "Minta guru tunjukkan demonstrasi penapaian.",
        "Luangkan masa bersama rakan untuk ulangkaji bersama.",
    };

    private int current = 0;
    private ReflectionLevel[] results;

    void Start()
    {
        results = new ReflectionLevel[topics.Length];
        summaryPanel.SetActive(false);
        btnFaham.onClick.AddListener(() => Reflect(ReflectionLevel.Faham));
        btnKurangFaham.onClick.AddListener(() => Reflect(ReflectionLevel.KurangFaham));
        btnTidakFaham.onClick.AddListener(() => Reflect(ReflectionLevel.TidakFaham));
        LoadTopic(0);
    }

    void LoadTopic(int i)
    {
        current = i;
        promptText.text = $"Topik {i + 1}: {topics[i]}\n\nBagaimana kefahaman kamu?";
        responseText.text = "";
        SetButtonsInteractable(true);
    }

    void Reflect(ReflectionLevel level)
    {
        results[current] = level;
        responseText.text = level switch
        {
            ReflectionLevel.Faham => feedbackFaham[current],
            ReflectionLevel.KurangFaham => feedbackKurang[current],
            _ => feedbackTidak[current],
        };
        SetButtonsInteractable(false);
        Invoke(nameof(GoNext), 2f);
    }

    void GoNext()
    {
        if (current + 1 < topics.Length)
        {
            LoadTopic(current + 1);
            return;
        }
        ShowSummary();
    }

    void ShowSummary()
    {
        summaryPanel.SetActive(true);
        var sb = new System.Text.StringBuilder("Ringkasan Refleksi:\n\n");
        for (int i = 0; i < topics.Length; i++)
            sb.AppendLine($"• {topics[i]}: {results[i]}");
        summaryText.text = sb.ToString();
        QuizManager.Instance.OnV3Complete(results);
    }

    void SetButtonsInteractable(bool state)
    {
        btnFaham.interactable = state;
        btnKurangFaham.interactable = state;
        btnTidakFaham.interactable = state;
    }
}
