using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Aktiviti V2: Soalan Aplikasi Situasi
// Shows a bakery/food image and asks why microorganisms are needed
public class QuizV2 : MonoBehaviour
{
    [Header("UI")]
    public Image situationImage; // bakery or food product image
    public TextMeshProUGUI situationCaption;
    public TextMeshProUGUI questionText;
    public Button[] answerButtons; // 4 MCQ options
    public TextMeshProUGUI[] answerLabels;
    public TextMeshProUGUI explanationText;
    public Button nextButton;

    [Header("Situations")]
    public Sprite[] situationSprites; // 0=kilang roti, 1=produk makanan

    private readonly string[] captions = { "Kilang Roti", "Produk Makanan" };
    private readonly string[] questions_q =
    {
        "Mengapa mikroorganisma diperlukan dalam proses ini?",
        "Apakah peranan yis dalam penghasilan produk ini?",
    };
    private readonly string[,] options = new string[2, 4]
    {
        {
            "Untuk penapaian — yis menghasilkan CO₂ menaikkan doh",
            "Untuk memberi warna pada roti",
            "Untuk mengeraskan tepung",
            "Untuk menyejukkan ketuhar",
        },
        {
            "Memecahkan gula kepada alkohol dan CO₂",
            "Menambah warna semula jadi",
            "Menyimpan makanan lebih lama",
            "Mengurangkan kandungan gula",
        },
    };
    private readonly string[] explanations =
    {
        "Yis menjalankan penapaian: C₆H₁₂O₆ → 2C₂H₅OH + 2CO₂. Gas CO₂ mengembangkan doh roti.",
        "Penapaian yis menukar gula kepada alkohol dan karbon dioksida, memberikan rasa dan tekstur khas.",
    };
    private readonly int[] correct = { 0, 0 };

    private int current = 0;
    private int score = 0;

    void Start()
    {
        nextButton.gameObject.SetActive(false);
        nextButton.onClick.AddListener(Next);
        LoadSituation(0);
    }

    void LoadSituation(int i)
    {
        current = i;
        if (situationSprites.Length > i)
            situationImage.sprite = situationSprites[i];
        situationCaption.text = captions[i];
        questionText.text = questions_q[i];
        explanationText.text = "";
        nextButton.gameObject.SetActive(false);

        for (int j = 0; j < answerButtons.Length; j++)
        {
            answerLabels[j].text = options[i, j];
            int cap = j;
            answerButtons[j].onClick.RemoveAllListeners();
            answerButtons[j].onClick.AddListener(() => Select(cap));
            answerButtons[j].interactable = true;
        }
    }

    void Select(int idx)
    {
        if (idx == correct[current])
            score++;
        explanationText.text = explanations[current];
        foreach (var b in answerButtons)
            b.interactable = false;
        nextButton.gameObject.SetActive(true);
    }

    void Next()
    {
        if (current + 1 < captions.Length)
            LoadSituation(current + 1);
        else
            QuizManager.Instance.OnV2Complete(score, captions.Length);
    }
}
