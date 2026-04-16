using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreInit : MonoBehaviour
{
    public TextMeshProUGUI titleText;

    public Button exitButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float totalScore;

        totalScore =
            (float)ScoreController.instance.scoreTotal
            / QuizController.instance.totalQuestions
            * 100f;
        titleText.text = totalScore.ToString("F0") + " %";

        Debug.Log(
            ScoreController.instance.scoreTotal
                + " / "
                + QuizController.instance.totalQuestions
                + " = "
                + totalScore
        );
    }

    // Update is called once per frame
    void Update() { }
}
