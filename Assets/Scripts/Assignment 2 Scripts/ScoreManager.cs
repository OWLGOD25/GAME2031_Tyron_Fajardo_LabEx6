using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Score")]
    public int targetScore = 10;
    public TextMeshProUGUI scoreText;

    [Header("UI Panels")]
    public GameObject gameUI;      // gameplay HUD/panels (will be hidden on game over)
    public GameObject gameOverUI;  // assign your Game Over panel here

    [Header("Options")]
    public bool stopTimeOnGameOver = true;

    int score;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        score = 0;
        UpdateScoreUI();
        if (gameOverUI != null) gameOverUI.SetActive(false);
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();

        if (score >= targetScore)
            TriggerGameOver();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = score.ToString();
    }

    void TriggerGameOver()
    {
        if (gameUI != null) gameUI.SetActive(false);
        if (gameOverUI != null) gameOverUI.SetActive(true);
        if (stopTimeOnGameOver) Time.timeScale = 0f;
    }

    // optional helper
    public int GetScore() => score;
}
