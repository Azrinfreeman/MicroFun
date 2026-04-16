using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// Singleton manager — orchestrates V1 → V2 → V3 flow
public class QuizManager : MonoBehaviour
{
    public static QuizManager Instance;

    [Header("Scene Panels")]
    public GameObject panelV1;
    public GameObject panelV2;
    public GameObject panelV3;
    public GameObject panelResult;

    [Header("Result UI")]
    public TextMeshProUGUI resultText;

    private int totalScore = 0;
    private int totalMax = 0;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start() => ShowPanel(panelV1);

    public void OnV1Complete(int score, int max)
    {
        totalScore += score;
        totalMax += max;
        ShowPanel(panelV2);
    }

    public void OnV2Complete(int score, int max)
    {
        totalScore += score;
        totalMax += max;
        ShowPanel(panelV3);
    }

    public void OnV3Complete(QuizV3.ReflectionLevel[] results)
    {
        ShowPanel(panelResult);
        resultText.text =
            $"Jumlah Skor: {totalScore} / {totalMax}\n\n"
            + $"Pencapaian kognitif dinilai.\nSemak refleksi anda untuk penambahbaikan.";
    }

    public void RestartQuiz()
    {
        totalScore = 0;
        totalMax = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void ShowPanel(GameObject panel)
    {
        panelV1.SetActive(false);
        panelV2.SetActive(false);
        panelV3.SetActive(false);
        if (panelResult != null)
            panelResult.SetActive(false);
        panel.SetActive(true);
    }
}
