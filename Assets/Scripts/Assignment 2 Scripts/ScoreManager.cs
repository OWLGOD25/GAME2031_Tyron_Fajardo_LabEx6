using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Levels / Targets")]
    public int[] levelTargets = new int[] { 5, 10, 20 }; // level 0,1,2...
    public int extraPerLevel = 10; // added per level beyond defined array
    public int startLevel = 0;

    [Header("Score UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI targetText;

    [Header("UI Panels")]
    public GameObject gameUI;      // gameplay HUD root (will be hidden on game over)
    public GameObject gameOverUI;  // Game Over panel (shows Next / Menu / Quit buttons)

    [Header("Options")]
    public bool stopTimeOnGameOver = true;

    const string HighScoreKey = "HighScore";

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
    }

    void TriggerGameOver()
    {
        // update high score
        int high = PlayerPrefs.GetInt(HighScoreKey, 0);
        if (score > high)
        {
            PlayerPrefs.SetInt(HighScoreKey, score);
            PlayerPrefs.Save();
        }

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

        if (gameOverUI != null) gameOverUI.SetActive(false);
        if (gameUI != null) gameUI.SetActive(true);
        Time.timeScale = 1f;
    }

    // Called by Game Over -> Main Menu button
    public void BackToMainMenu()
    {
        // reset run to first level (one-digit target expected in levelTargets[0])
        currentLevel = 0;
        SetTargetForLevel(currentLevel);
        ResetScore();

        // show title menu
        var menu = Object.FindAnyObjectByType<MenuManger>();
        if (menu != null)
        {
            menu.ShowTitleMenu();
            menu.UpdateHighScoreDisplay(); // refresh high score on title
        }

        // ensure game UI / game over UI hidden and time running
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

    // Optional: restart run at given level (used by MenuManger when starting from title)
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
}
