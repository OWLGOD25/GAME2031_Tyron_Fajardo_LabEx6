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
    public TextMeshProUGUI scoreText;     // shows overall total score
    public TextMeshProUGUI targetText;
    public TextMeshProUGUI remainingText; // shows remaining for current level
    public TextMeshProUGUI levelText;

    [Header("UI Panels")]
    public GameObject gameUI;
    public GameObject gameOverUI;

    [Header("Options")]
    public bool stopTimeOnGameOver = true;

    const string HighScoreKey = "HighScore";
    const string HighLevelKey = "HighLevel";
    const string LastLevelKey = "LastLevel";

    int totalScore;   // overall score (persists across levels)
    int levelScore;   // per-level score (reset at level start)
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
        ResetLevelScore();
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

    // Only reset per-level score (levelScore). totalScore remains unless explicitly reset.
    void ResetLevelScore()
    {
        levelScore = 0;
        UpdateScoreUI();
    }

    // Call to explicitly reset entire run (total + level)
    public void ResetAllScores()
    {
        totalScore = 0;
        ResetLevelScore();
    }

    // Called when player collects/earns points
    public void AddScore(int amount)
    {
        levelScore += amount;
        totalScore += amount;
        UpdateScoreUI();

        if (levelScore >= targetScore)
            TriggerGameOver();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {totalScore}";

        if (targetText != null)
            targetText.text = $"Target: {targetScore}";

        int remaining = Mathf.Max(0, targetScore - levelScore);
        if (remainingText != null)
            remainingText.text = $"Remaining: {remaining}";

        if (levelText != null)
            levelText.text = $"Level: {currentLevel}";
    }

    void TriggerGameOver()
    {
        // update high score (best total player score)
        int high = PlayerPrefs.GetInt(HighScoreKey, 0);
        if (totalScore > high)
        {
            PlayerPrefs.SetInt(HighScoreKey, totalScore);
            PlayerPrefs.Save();
        }

        // update highest level reached (largest level index seen)
        int highLevel = PlayerPrefs.GetInt(HighLevelKey, 0);
        if (currentLevel > highLevel)
        {
            PlayerPrefs.SetInt(HighLevelKey, currentLevel);
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
        ResetLevelScore();

        // update highest level when advancing
        int highLevel = PlayerPrefs.GetInt(HighLevelKey, 0);
        if (currentLevel > highLevel)
        {
            PlayerPrefs.SetInt(HighLevelKey, currentLevel);
            PlayerPrefs.Save();
        }

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
        ResetLevelScore();

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

    // Restart a run at the specified level (resets levelScore, optionally reset total)
    public void RestartRun(int level = 0, bool resetTotal = false)
    {
        currentLevel = Mathf.Max(0, level);
        SetTargetForLevel(currentLevel);
        if (resetTotal) totalScore = 0;
        ResetLevelScore();
        if (gameOverUI != null) gameOverUI.SetActive(false);
        if (gameUI != null) gameUI.SetActive(true);
        Time.timeScale = 1f;
    }

    // Helpers
    public int GetTotalScore() => totalScore;
    public int GetLevelScore() => levelScore;
    public int GetTarget() => targetScore;
    public int GetCurrentLevel() => currentLevel;
    public int GetHighScore() => PlayerPrefs.GetInt(HighScoreKey, 0);
    public int GetHighLevel() => PlayerPrefs.GetInt(HighLevelKey, 0);
    public int GetRemaining() => Mathf.Max(0, targetScore - levelScore);
    public int GetSavedLastLevel() => PlayerPrefs.GetInt(LastLevelKey, 0);
}
