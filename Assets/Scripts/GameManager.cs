// ============================================================
// GameManager.cs
// Singleton that persists across all 3 scenes.
// Tracks score, total questions, and reflection responses.
// Attach to an empty GameObject named "_GameManager" in SceneV1.
// ============================================================
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // ── Singleton ────────────────────────────────────────────
    public static GameManager Instance { get; private set; }

    // ── Score Tracking ───────────────────────────────────────
    public int Score { get; private set; }
    public int TotalQuestions { get; private set; }

    // ── Reflection Data ──────────────────────────────────────
    // Populated by ReflectionManager when player submits responses
    public List<ReflectionSaveEntry> ReflectionResponses { get; private set; } =
        new List<ReflectionSaveEntry>();

    // ── Awake: Singleton Setup ───────────────────────────────
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate on reload
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across all scenes
    }

    // ── Public API ───────────────────────────────────────────

    /// Call this whenever player answers correctly
    public void AddScore(int points = 1)
    {
        Score += points;
    }

    /// Call this for every question answered (right or wrong)
    public void RegisterQuestion()
    {
        TotalQuestions++;
    }

    /// Store one reflection response from SceneV3
    public void AddReflection(string statement, ReflectionResponse response)
    {
        ReflectionResponses.Add(
            new ReflectionSaveEntry { statement = statement, response = (int)response }
        );
    }

    /// Persist score and reflection data to PlayerPrefs
    public void SaveAll()
    {
        PlayerPrefs.SetInt("FinalScore", Score);
        PlayerPrefs.SetInt("TotalQuestions", TotalQuestions);

        // Serialise reflection list to JSON and store as string
        ReflectionSaveData data = new ReflectionSaveData
        {
            entries = ReflectionResponses.ToArray(),
        };
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("ReflectionData", json);
        PlayerPrefs.Save();

        Debug.Log($"[GameManager] Saved — Score: {Score}/{TotalQuestions}, JSON: {json}");
    }

    /// Reset everything (e.g. when restarting the game)
    public void ResetGame()
    {
        Score = 0;
        TotalQuestions = 0;
        ReflectionResponses.Clear();
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("SceneV1");
    }

    /// Convenience: score as percentage string (e.g. "80%")
    public string GetScorePercent()
    {
        if (TotalQuestions == 0)
            return "0%";
        int pct = Mathf.RoundToInt((float)Score / TotalQuestions * 100);
        return $"{pct}%";
    }
}
