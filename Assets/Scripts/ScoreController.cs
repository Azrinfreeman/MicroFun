using UnityEngine;

public class ScoreController : MonoBehaviour
{
    public static ScoreController instance;

    public int scoreTotal;

    public void AddScore(int scoreAdd)
    {
        scoreTotal += scoreAdd;
    }

    void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update() { }
}
