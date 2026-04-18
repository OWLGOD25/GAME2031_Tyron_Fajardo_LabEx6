using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManger : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject titleMenuUI; // panel containing title menu (input + confirm)
    public GameObject gameUI;      // panel(s) used during gameplay (HUD, etc.)

    [Header("Title Menu Controls")]
    public TMP_InputField nameInput;
    public Button confirmButton;
    public TextMeshProUGUI highScoreText; // show high score on title

    [Header("Player Link")]
    public string playerTag = "Player"; // tag to find player GameObject
    public TextMeshProUGUI hudNameText; // optional: HUD text to show player name

    const string PlayerNameKey = "PlayerName";

    void Start()
    {
        // default state: show title, hide game UI
        ShowTitleMenu();

        // restore previous name to input (optional)
        string saved = PlayerPrefs.GetString(PlayerNameKey, "");
        if (!string.IsNullOrEmpty(saved) && nameInput != null)
            nameInput.text = saved;

        if (confirmButton != null)
            confirmButton.onClick.AddListener(ConfirmName);

        UpdateHighScoreDisplay();
    }

    // made public so ScoreManager can call it
    public void ShowTitleMenu()
    {
        if (titleMenuUI != null) titleMenuUI.SetActive(true);
        if (gameUI != null) gameUI.SetActive(false);
    }

    void StartGame()
    {
        // Ensure time is running (in case Game Over paused it)
        Time.timeScale = 1f;

        if (titleMenuUI != null) titleMenuUI.SetActive(false);
        if (gameUI != null) gameUI.SetActive(true);

        // Ensure ScoreManager restarts the run at level 0 when starting from title
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
        // Update HUD if assigned
        if (hudNameText != null)
            hudNameText.text = name;

        // Find player by tag
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player == null) return;

        // Prefer a DisplayHelper component
        var nameComp = player.GetComponent<DisplayHelper>();
        if (nameComp != null)
        {
            nameComp.SetName(name);
            return;
        }

        // Otherwise try to find any TextMeshProUGUI in player's children
        var childText = player.GetComponentInChildren<TextMeshProUGUI>();
        if (childText != null)
            childText.text = name;
    }

    // Update title high score text (called from Start and when returning to menu)
    public void UpdateHighScoreDisplay()
    {
        int high = 0;
        if (ScoreManager.Instance != null)
            high = ScoreManager.Instance.GetHighScore();
        else
            high = PlayerPrefs.GetInt("HighScore", 0);

        if (highScoreText != null)
            highScoreText.text = $"High Score: {high}";
    }
}
