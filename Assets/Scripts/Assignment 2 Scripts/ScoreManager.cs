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
    public TextMeshProUGUI livesText;     // shows current lives

    [Header("Game Over UI")]
    public GameObject nextLevelButton;    // assign Next Level button object in inspector (will be hidden when game over by lives)

    [Header("Falling object / Spawner tuning")]
    public float baseFallLifetime = 5f;             // base destroy delay for landed objects
    public float lifetimeDecreasePerLevel = 0.3f;   // reduce lifetime each level
    public float minFallLifetime = 1.0f;

    public float baseSpawnRate = 5f;                // base spawn interval
    public float spawnRateDecreasePerLevel = 0.5f;  // reduce interval each level (faster spawns)
    public float minSpawnRate = 0.6f;

    [Header("Lives")]
    public int initialLives = 5;        // lives at level 0
    public int extraLivesPerLevel = 2;  // additional lives per level

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
    int currentLives;

    // flag to indicate why the game ended: true == ended because lives ran out
    bool gameOverByLives;

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

        // set lives for this level
        currentLives = ComputeLivesForLevel(currentLevel);
        UpdateScoreUI();

        if (gameOverUI != null) gameOverUI.SetActive(false);

        // update spawner with the current spawn rate
        UpdateSpawnerRate();
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

    int ComputeLivesForLevel(int level)
    {
        return Mathf.Max(0, initialLives + level * extraLivesPerLevel);
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
        // mark that this potential game over (if triggered) is from level completion
        gameOverByLives = false;

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

        if (livesText != null)
            livesText.text = $"Lives: {currentLives}";
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

        // hide or show Next Level depending on why the game ended
        if (nextLevelButton != null)
            nextLevelButton.SetActive(!gameOverByLives);

        if (gameUI != null) gameUI.SetActive(false);
        if (gameOverUI != null) gameOverUI.SetActive(true);
        if (stopTimeOnGameOver) Time.timeScale = 0f;
    }

    // Called by Game Over -> Next Level button
    public void NextLevel()
    {
        // ensure flag cleared when advancing
        gameOverByLives = false;

        currentLevel++;
        SetTargetForLevel(currentLevel);

        // give additional lives for the new level
        currentLives = ComputeLivesForLevel(currentLevel);
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

        // update spawner rate for new level
        UpdateSpawnerRate();

        if (gameOverUI != null) gameOverUI.SetActive(false);
        if (gameUI != null) gameUI.SetActive(true);
        Time.timeScale = 1f;
        UpdateScoreUI();
    }

    // Called by Game Over / Pause -> Main Menu button
    public void BackToMainMenu()
    {
        currentLevel = 0;
        SetTargetForLevel(currentLevel);
        ResetLevelScore();
        currentLives = ComputeLivesForLevel(currentLevel);

        if (menuManger != null)
        {
            menuManger.ShowTitleMenu();
            menuManger.UpdateHighScoreDisplay();
        }

        if (gameOverUI != null) gameOverUI.SetActive(false);
        if (gameUI != null) gameUI.SetActive(false);
        Time.timeScale = 1f;
        UpdateScoreUI();
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

        // set lives appropriately for the restarted level
        currentLives = ComputeLivesForLevel(currentLevel);

        if (gameOverUI != null) gameOverUI.SetActive(false);
        if (gameUI != null) gameUI.SetActive(true);

        // update spawner rate for restarted level
        UpdateSpawnerRate();

        Time.timeScale = 1f;
        UpdateScoreUI();
    }

    // Called by falling objects when they self-destruct after landing
    public void ReduceLife(int amount)
    {
        currentLives -= amount;
        currentLives = Mathf.Max(0, currentLives);
        UpdateScoreUI();

        if (currentLives <= 0)
        {
            // mark that this game over was caused by lives running out
            gameOverByLives = true;
            // treat running out of lives as game over
            TriggerGameOver();
        }
    }

    // Spawn/fall tuning helpers
    public float GetFallLifetime()
    {
        float lifetime = baseFallLifetime - currentLevel * lifetimeDecreasePerLevel;
        return Mathf.Max(minFallLifetime, lifetime);
    }

    public float GetSpawnRate()
    {
        float rate = baseSpawnRate - currentLevel * spawnRateDecreasePerLevel;
        return Mathf.Max(minSpawnRate, rate);
    }

    void UpdateSpawnerRate()
    {
        // find spawner and update its spawn rate
        var spawner = Object.FindAnyObjectByType<EnemySpawnerScript>();
        if (spawner != null)
            spawner.SetSpawnRate(GetSpawnRate());
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
