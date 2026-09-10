using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MicroorganismGameManager : MonoBehaviour
{
    [Header("Screens")]
    public GameObject screenE1;
    public GameObject screenE2;
    public GameObject screenE3;

    [Header("Scenario Data")]
    public ScenarioData[] scenarios;
    private int currentScenarioIndex = 0;
    private ScenarioData currentScenario;

    [Header("Screen E1 UI")]
    public Image scenarioImage;
    public TMP_Text scenarioText;
    public TMP_Text e1FeedbackText;
    public Button e1NextButton;

    [Header("Screen E2 Sliders")]
    public Slider temperatureSlider;
    public Slider nutrientSlider;
    public Slider phSlider;

    [Header("Screen E2 Value Text")]
    public TMP_Text temperatureValueText;
    public TMP_Text nutrientValueText;
    public TMP_Text phValueText;
    public TMP_Text e2FeedbackText;
    public Button e2NextButton;

    [Header("Screen E3 UI")]
    public TMP_Text sentenceText;
    public TMP_Text e3FeedbackText;
    public Button finishButton;

    [Header("Settings")]
    public bool allowNextOnlyAfterCorrectAnswer = true;

    private FactorType selectedFactor;
    private GrowthLevel correctGrowthLevel;

    private void Start()
    {
        LoadScenario(0);
        ShowScreen(1);

        e1NextButton.gameObject.SetActive(false);
        e2NextButton.gameObject.SetActive(false);
        finishButton.gameObject.SetActive(false);

        temperatureSlider.onValueChanged.AddListener(delegate { UpdateSliderTexts(); });
        nutrientSlider.onValueChanged.AddListener(delegate { UpdateSliderTexts(); });
        phSlider.onValueChanged.AddListener(delegate { UpdateSliderTexts(); });

        UpdateSliderTexts();
    }

    private void LoadScenario(int index)
    {
        currentScenarioIndex = index;
        currentScenario = scenarios[currentScenarioIndex];

        scenarioText.text = currentScenario.scenarioDescription;
        scenarioImage.sprite = currentScenario.scenarioImage;

        e1FeedbackText.text = "";
        e2FeedbackText.text = "";
        e3FeedbackText.text = "";

        e1NextButton.gameObject.SetActive(false);
        e2NextButton.gameObject.SetActive(false);
        finishButton.gameObject.SetActive(false);

        sentenceText.text = "Pertumbuhan mikroorganisma meningkat apabila _____ kerana _____.";
    }

    private void ShowScreen(int screenNumber)
    {
        screenE1.SetActive(screenNumber == 1);
        screenE2.SetActive(screenNumber == 2);
        screenE3.SetActive(screenNumber == 3);
    }

    // -----------------------------
    // SCREEN E1: Choose factor
    // -----------------------------

    public void ChooseFactorTemperature()
    {
        CheckFactorAnswer(FactorType.Temperature);
    }

    public void ChooseFactorNutrient()
    {
        CheckFactorAnswer(FactorType.Nutrient);
    }

    public void ChooseFactorAcidity()
    {
        CheckFactorAnswer(FactorType.Acidity);
    }

    private void CheckFactorAnswer(FactorType chosenFactor)
    {
        selectedFactor = chosenFactor;

        if (chosenFactor == currentScenario.correctDominantFactor)
        {
            e1FeedbackText.text = "Betul! Faktor utama ialah " + chosenFactor.ToString() + ".";
            e1NextButton.gameObject.SetActive(true);
        }
        else
        {
            e1FeedbackText.text = "Cuba lagi. Fikirkan faktor yang paling mempengaruhi situasi ini.";

            if (!allowNextOnlyAfterCorrectAnswer)
            {
                e1NextButton.gameObject.SetActive(true);
            }
        }
    }

    public void GoToScreenE2()
    {
        ShowScreen(2);
        ResetPredictionScreen();
    }

    // -----------------------------
    // SCREEN E2: Slider prediction
    // -----------------------------

    private void ResetPredictionScreen()
    {
        temperatureSlider.value = 1;
        nutrientSlider.value = 1;
        phSlider.value = 1;

        e2FeedbackText.text = "";
        e2NextButton.gameObject.SetActive(false);

        UpdateSliderTexts();
    }

    private void UpdateSliderTexts()
    {
        temperatureValueText.text = GetTemperatureLabel();
        nutrientValueText.text = GetNutrientLabel();
        phValueText.text = GetPHLabel();

        correctGrowthLevel = CalculateGrowthLevel();
    }

    private string GetTemperatureLabel()
    {
        int value = Mathf.RoundToInt(temperatureSlider.value);

        if (value == 0) return "Sejuk";
        if (value == 1) return "Sederhana";
        return "Panas";
    }

    private string GetNutrientLabel()
    {
        int value = Mathf.RoundToInt(nutrientSlider.value);

        if (value == 0) return "Sedikit";
        if (value == 1) return "Sederhana";
        return "Banyak";
    }

    private string GetPHLabel()
    {
        int value = Mathf.RoundToInt(phSlider.value);

        if (value == 0) return "Berasid";
        if (value == 1) return "Neutral";
        return "Beralkali";
    }

    private GrowthLevel CalculateGrowthLevel()
    {
        int score = 0;

        int temperature = Mathf.RoundToInt(temperatureSlider.value);
        int nutrient = Mathf.RoundToInt(nutrientSlider.value);
        int ph = Mathf.RoundToInt(phSlider.value);

        // Temperature:
        // Cold slows growth, moderate is okay, warm/hot increases growth.
        if (temperature == 0) score += 0;
        if (temperature == 1) score += 1;
        if (temperature == 2) score += 2;

        // Nutrient:
        // More nutrients means more food for microorganisms.
        if (nutrient == 0) score += 0;
        if (nutrient == 1) score += 1;
        if (nutrient == 2) score += 2;

        // pH:
        // Neutral is better for many microorganisms.
        if (ph == 0) score += 0;
        if (ph == 1) score += 2;
        if (ph == 2) score += 1;

        if (score <= 2)
        {
            return GrowthLevel.Low;
        }
        else if (score <= 4)
        {
            return GrowthLevel.Medium;
        }
        else
        {
            return GrowthLevel.High;
        }
    }

    public void PredictLowGrowth()
    {
        CheckGrowthPrediction(GrowthLevel.Low);
    }

    public void PredictMediumGrowth()
    {
        CheckGrowthPrediction(GrowthLevel.Medium);
    }

    public void PredictHighGrowth()
    {
        CheckGrowthPrediction(GrowthLevel.High);
    }

    private void CheckGrowthPrediction(GrowthLevel chosenGrowth)
    {
        if (chosenGrowth == correctGrowthLevel)
        {
            e2FeedbackText.text = "Betul! Tahap pertumbuhan ialah " + GetGrowthMalayName(correctGrowthLevel) + ".";
            e2NextButton.gameObject.SetActive(true);
        }
        else
        {
            e2FeedbackText.text = "Cuba lagi. Perhatikan suhu, nutrien dan pH.";

            if (!allowNextOnlyAfterCorrectAnswer)
            {
                e2NextButton.gameObject.SetActive(true);
            }
        }
    }

    private string GetGrowthMalayName(GrowthLevel growthLevel)
    {
        if (growthLevel == GrowthLevel.Low) return "Rendah";
        if (growthLevel == GrowthLevel.Medium) return "Sederhana";
        return "Tinggi";
    }

    public void GoToScreenE3()
    {
        ShowScreen(3);
        PrepareJustificationScreen();
    }

    // -----------------------------
    // SCREEN E3: Justification
    // -----------------------------

    private void PrepareJustificationScreen()
    {
        e3FeedbackText.text = "";
        finishButton.gameObject.SetActive(false);

        sentenceText.text = "Pertumbuhan mikroorganisma meningkat apabila _____ kerana _____.";
    }

    public void ChooseReasonTemperature()
    {
        CheckReasonAnswer(
            FactorType.Temperature,
            "Pertumbuhan mikroorganisma meningkat apabila suhu sesuai kerana mikroorganisma dapat membiak lebih cepat."
        );
    }

    public void ChooseReasonNutrient()
    {
        CheckReasonAnswer(
            FactorType.Nutrient,
            "Pertumbuhan mikroorganisma meningkat apabila nutrien banyak kerana mikroorganisma mempunyai makanan untuk membiak."
        );
    }

    public void ChooseReasonAcidity()
    {
        CheckReasonAnswer(
            FactorType.Acidity,
            "Pertumbuhan mikroorganisma meningkat apabila pH sesuai kerana keadaan itu membantu mikroorganisma hidup dan membiak."
        );
    }

    private void CheckReasonAnswer(FactorType chosenReason, string completedSentence)
    {
        if (chosenReason == selectedFactor)
        {
            sentenceText.text = completedSentence;
            e3FeedbackText.text = "Betul! Kamu berjaya menjelaskan hubungan sebab dan akibat.";
            finishButton.gameObject.SetActive(true);
        }
        else
        {
            e3FeedbackText.text = "Cuba lagi. Pilih sebab yang berkaitan dengan faktor utama tadi.";
        }
    }

    // -----------------------------
    // Navigation
    // -----------------------------

    public void BackToScreenE1()
    {
        ShowScreen(1);
    }

    public void BackToScreenE2()
    {
        ShowScreen(2);
    }

    public void FinishActivity()
    {
        e3FeedbackText.text = "Tahap selesai! Bagus, kamu faham faktor pertumbuhan mikroorganisma.";
        finishButton.gameObject.SetActive(false);
    }
}