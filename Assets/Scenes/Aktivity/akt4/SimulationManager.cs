using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SimulationManager : MonoBehaviour
{
    [Header("Selected Factors")]
    public bool hasWater = true;
    public bool enoughAir = true;
    public int selectedDays = 1;

    [Header("UI Text")]
    public TMP_Text waterText;
    public TMP_Text airText;
    public TMP_Text timeText;
    public TMP_Text progressText;
    public TMP_Text resultTextA;
    public TMP_Text resultTextB;
    public TMP_Text feedbackText;

    [Header("Progress")]
    public Slider progressSlider;
    public float simulationDuration = 3f;

    [Header("Microbe Visuals")]
    public GameObject microbePrefab;
    public Transform microbeContainerA;
    public Transform microbeContainerB;

    [Header("Microbe Spawn Settings")]
    public int startingMicrobes = 3;
    public int maxMicrobes = 18;
    public Vector2 spawnAreaMin = new Vector2(-150f, -60f);
    public Vector2 spawnAreaMax = new Vector2(150f, 60f);

    private bool isSimulating = false;

    void Start()
    {
        UpdateFactorUI();
        ResetSimulationVisual();
    }

    public void SelectWaterAda()
    {
        hasWater = true;
        UpdateFactorUI();
    }

    public void SelectWaterTiada()
    {
        hasWater = false;
        UpdateFactorUI();
    }

    public void SelectAirCukup()
    {
        enoughAir = true;
        UpdateFactorUI();
    }

    public void SelectAirKurang()
    {
        enoughAir = false;
        UpdateFactorUI();
    }

    public void SelectTime1()
    {
        selectedDays = 1;
        UpdateFactorUI();
    }

    public void SelectTime2()
    {
        selectedDays = 2;
        UpdateFactorUI();
    }

    public void SelectTime3()
    {
        selectedDays = 3;
        UpdateFactorUI();
    }

    void UpdateFactorUI()
    {
        if (waterText != null)
            waterText.text = hasWater ? "Air: Ada" : "Air: Tiada";

        if (airText != null)
            airText.text = enoughAir ? "Udara: Cukup" : "Udara: Kurang";

        if (timeText != null)
            timeText.text = "Masa: " + selectedDays + " Hari";
    }

    public void PlayTimeSimulation()
    {
        if (isSimulating) return;
        StartCoroutine(SimulationRoutine());
    }

    IEnumerator SimulationRoutine()
    {
        isSimulating = true;

        feedbackText.text = "Simulasi sedang berjalan...";
        progressSlider.value = 0;
        progressText.text = "Mula";

        ClearMicrobes(microbeContainerA);
        ClearMicrobes(microbeContainerB);

        SpawnMicrobes(microbeContainerA, startingMicrobes);
        SpawnMicrobes(microbeContainerB, startingMicrobes);

        float timer = 0f;

        while (timer < simulationDuration)
        {
            timer += Time.deltaTime;

            float progress = timer / simulationDuration;
            progressSlider.value = progress;

            int currentDay = Mathf.Clamp(Mathf.CeilToInt(progress * selectedDays), 1, selectedDays);
            progressText.text = "Hari " + currentDay;

            yield return null;
        }

        progressSlider.value = 1;
        progressText.text = "Hari " + selectedDays;

        int growthA = CalculateGrowth(true, true, selectedDays);
        int growthB = CalculateGrowth(hasWater, enoughAir, selectedDays);

        ClearMicrobes(microbeContainerA);
        ClearMicrobes(microbeContainerB);

        SpawnMicrobes(microbeContainerA, growthA);
        SpawnMicrobes(microbeContainerB, growthB);

        resultTextA.text = "Pertumbuhan: Banyak";
        resultTextB.text = GetGrowthDescription(growthB);

        feedbackText.text = "Simulasi selesai! Sekarang lengkapkan rakaman pemerhatian.";

        isSimulating = false;
    }

    int CalculateGrowth(bool water, bool air, int days)
    {
        int growth = startingMicrobes;

        if (water && air)
        {
            growth += days * 5;
        }
        else if (water && !air)
        {
            growth += days * 2;
        }
        else if (!water && air)
        {
            growth += days * 1;
        }
        else
        {
            growth -= 1;
        }

        return Mathf.Clamp(growth, 1, maxMicrobes);
    }

    string GetGrowthDescription(int microbeCount)
    {
        if (microbeCount >= 13)
            return "Pertumbuhan: Banyak";
        else if (microbeCount >= 7)
            return "Pertumbuhan: Sederhana";
        else if (microbeCount >= 3)
            return "Pertumbuhan: Sedikit";
        else
            return "Pertumbuhan: Hampir Tiada";
    }

    void SpawnMicrobes(Transform container, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject newMicrobe = Instantiate(microbePrefab, container);

            RectTransform rect = newMicrobe.GetComponent<RectTransform>();

            float randomX = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
            float randomY = Random.Range(spawnAreaMin.y, spawnAreaMax.y);

            rect.anchoredPosition = new Vector2(randomX, randomY);

            float randomScale = Random.Range(0.7f, 1.2f);
            rect.localScale = new Vector3(randomScale, randomScale, 1f);
        }
    }

    void ClearMicrobes(Transform container)
    {
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
    }

    void ResetSimulationVisual()
    {
        progressSlider.value = 0;
        progressText.text = "Mula";

        ClearMicrobes(microbeContainerA);
        ClearMicrobes(microbeContainerB);

        SpawnMicrobes(microbeContainerA, startingMicrobes);
        SpawnMicrobes(microbeContainerB, startingMicrobes);

        resultTextA.text = "";
        resultTextB.text = "";
        feedbackText.text = "Pilih faktor dan tekan Mainkan Masa.";
    }
}