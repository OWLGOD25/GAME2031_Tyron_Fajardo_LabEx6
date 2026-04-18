using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("References")]
    public MenuManger menuManger; // assign the MenuManger in the Inspector

    [Header("Levels / Targets")]
    public int[] levelTargets = new int[] { 5, 10, 20 };
    public int extraPerLevel = 10;
    public int startLevel = 0;

    [Header("Score UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI targetText;
    public TextMeshProUGUI remainingText;
    public TextMeshProUGUI levelText;

    [Header("UI Panels")]
    public GameObject gameUI;
    public GameObject gameOverUI;

    [Header("Options")]
    public bool stopTimeOnGameOver = true;

    const string HighScoreKey = "HighScore";
    const string HighTargetKey = "HighTarget";
    const string LastLevelKey = "LastLevel";

    int score;
    int currentLevel;
    int targetScore;

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
        currentLevel = Mathf.Max(0, startLevel);
        SetTargetForLevel(currentLevel);
        ResetScore();
        if (gameOverUI != null) gameOverUI.SetActive(false);
        UpdateScoreUI();
    }

    void SetTargetForLevel(int level)
    {
        if (levelTargets != null && levelTargets.Length > 0)
        {
            if (level < levelTargets.Length)
                targetScore = levelTargets[level];
            else
            {
                int last = levelTargets[levelTargets.Length - 1];
                targetScore = last + (level - (levelTargets.Length - 1)) * extraPerLevel;
            }
        }
        else
        {
            targetScore = 5;
        }

        if (targetText != null)
            targetText.text = $"Target: {targetScore}";

        if (levelText != null)
            levelText.text = $"Level: {currentLevel}";
    }

    void ResetScore()
    {
        score = 0;
        UpdateScoreUI();
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
            scoreText.text = $"Score: {score}";

        if (targetText != null)
            targetText.text = $"Target: {targetScore}";

        int remaining = Mathf.Max(0, targetScore - score);
        if (remainingText != null)
            remainingText.text = $"Remaining: {remaining}";

        if (levelText != null)
            levelText.text = $"Level: {currentLevel}";
    }

    void TriggerGameOver()
    {
        // update high score (best player score)
        int high = PlayerPrefs.GetInt(HighScoreKey, 0);
        if (score > high)
        {
            PlayerPrefs.SetInt(HighScoreKey, score);
            PlayerPrefs.Save();
        }

        // update highest target reached (largest targetScore seen)
        int highTarget = PlayerPrefs.GetInt(HighTargetKey, 0);
        if (targetScore > highTarget)
        {
            PlayerPrefs.SetInt(HighTargetKey, targetScore);
            PlayerPrefs.Save();
        }

        // save last completed level / progress
        PlayerPrefs.SetInt(LastLevelKey, currentLevel);
        PlayerPrefs.Save();

        if (gameUI != null) gameUI.SetActive(false);
        if (gameOverUI != null) gameOverUI.SetActive(true);
        if (stopTimeOnGameOver) Time.timeScale = 0f;
    }

    // Called by Game Over -> Next Level button
    public void NextLevel()
    {
        currentLevel++;
        SetTargetForLevel(currentLevel);
        ResetScore();

        // save progress (player now advanced to this level)
        PlayerPrefs.SetInt(LastLevelKey, currentLevel);
        PlayerPrefs.Save();

        if (gameOverUI != null) gameOverUI.SetActive(false);
        if (gameUI != null) gameUI.SetActive(true);
        Time.timeScale = 1f;
    }

    // Called by Game Over / Pause -> Main Menu button
    public void BackToMainMenu()
    {
        currentLevel = 0;
        SetTargetForLevel(currentLevel);
        ResetScore();

        if (menuManger != null)
        {
            menuManger.ShowTitleMenu();
            menuManger.UpdateHighScoreDisplay();
        }

        if (gameOverUI != null) gameOverUI.SetActive(false);
        if (gameUI != null) gameUI.SetActive(false);
        Time.timeScale = 1f;
    }

    // Quit application (works in build; stops play mode in editor)
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Restart a run at the specified level (resets score to start-of-level)
    public void RestartRun(int level = 0)
    {
        currentLevel = Mathf.Max(0, level);
        SetTargetForLevel(currentLevel);
        ResetScore();
        if (gameOverUI != null) gameOverUI.SetActive(false);
        if (gameUI != null) gameUI.SetActive(true);
        Time.timeScale = 1f;
    }

    // Helpers
    public int GetScore() => score;
    public int GetTarget() => targetScore;
    public int GetCurrentLevel() => currentLevel;
    public int GetHighScore() => PlayerPrefs.GetInt(HighScoreKey, 0);
    public int GetHighTarget() => PlayerPrefs.GetInt(HighTargetKey, 0);
    public int GetRemaining() => Mathf.Max(0, targetScore - score);
    public int GetSavedLastLevel() => PlayerPrefs.GetInt(LastLevelKey, 0);
}
