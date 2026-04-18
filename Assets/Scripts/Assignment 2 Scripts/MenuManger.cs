using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManger : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject titleMenuUI;
    public GameObject gameUI;
    public GameObject pauseMenuUI; // pause menu panel

    [Header("Title Menu Controls")]
    public TMP_InputField nameInput;
    public Button confirmButton;
    public Button continueButton; // continue from last progress
    public TextMeshProUGUI highScoreText; // best player score (A)
    public TextMeshProUGUI highLevelText; // NEW: displays highest level reached

    [Header("Player Link")]
    public string playerTag = "Player";
    public TextMeshProUGUI hudNameText;

    const string PlayerNameKey = "PlayerName";
    const string LastLevelKey = "LastLevel"; // same key used by ScoreManager

    void Start()
    {
        ShowTitleMenu();

        string saved = PlayerPrefs.GetString(PlayerNameKey, "");
        if (!string.IsNullOrEmpty(saved) && nameInput != null)
            nameInput.text = saved;

        if (confirmButton != null)
            confirmButton.onClick.AddListener(ConfirmName);

        if (continueButton != null)
            continueButton.onClick.AddListener(ContinueFromLastProgress);

        UpdateHighScoreDisplay();
    }

    public void ShowTitleMenu()
    {
        if (titleMenuUI != null) titleMenuUI.SetActive(true);
        if (gameUI != null) gameUI.SetActive(false);
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
    }

    void StartGame()
    {
        Time.timeScale = 1f;

        if (titleMenuUI != null) titleMenuUI.SetActive(false);
        if (gameUI != null) gameUI.SetActive(true);

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.RestartRun(0);
    }

    public void ConfirmName()
    {
        if (nameInput == null) return;

        string playerName = nameInput.text.Trim();
        if (string.IsNullOrEmpty(playerName))
            playerName = "Player";

        PlayerPrefs.SetString(PlayerNameKey, playerName);
        PlayerPrefs.Save();

        ApplyNameToPlayer(playerName);

        StartGame();
    }

    void ApplyNameToPlayer(string name)
    {
        if (hudNameText != null)
            hudNameText.text = name;

        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player == null) return;

        var nameComp = player.GetComponent<DisplayHelper>();
        if (nameComp != null)
        {
            nameComp.SetName(name);
            return;
        }

        var childText = player.GetComponentInChildren<TextMeshProUGUI>();
        if (childText != null)
            childText.text = name;
    }

    // Continue button on title: resume at the last saved level (restart that level)
    public void ContinueFromLastProgress()
    {
        int lastLevel = PlayerPrefs.GetInt(LastLevelKey, 0);
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.RestartRun(lastLevel);

        // apply saved name to HUD/player if present
        string saved = PlayerPrefs.GetString(PlayerNameKey, "");
        if (!string.IsNullOrEmpty(saved))
            ApplyNameToPlayer(saved);

        // show game UI and hide title
        if (titleMenuUI != null) titleMenuUI.SetActive(false);
        if (gameUI != null) gameUI.SetActive(true);
    }

    // Pause controls
    public void PauseGame()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
    }

    // Update title high score text (called at Start and when returning to menu)
    public void UpdateHighScoreDisplay()
    {
        int highScore = 0;
        int highLevel = 0;
        if (ScoreManager.Instance != null)
        {
            highScore = ScoreManager.Instance.GetHighScore();
            highLevel = ScoreManager.Instance.GetHighLevel();
        }
        else
        {
            highScore = PlayerPrefs.GetInt("HighScore", 0);
            highLevel = PlayerPrefs.GetInt("HighLevel", 0);
        }

        if (highScoreText != null)
            highScoreText.text = $"High Score: {highScore}";

        if (highLevelText != null)
            highLevelText.text = $"Highest Level: {highLevel}";
    }
}
