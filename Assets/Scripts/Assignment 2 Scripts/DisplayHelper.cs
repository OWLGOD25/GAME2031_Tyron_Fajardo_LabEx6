using TMPro;
using UnityEngine;

public class DisplayHelper : MonoBehaviour
{
    public TextMeshProUGUI nameText; // assign in inspector (child text showing name)

    string currentName;

    void Start()
    {
        // restore previously saved name if none was set programmatically
        if (string.IsNullOrEmpty(currentName))
        {
            currentName = PlayerPrefs.GetString("PlayerName", "");
            if (!string.IsNullOrEmpty(currentName))
                ApplyNameToText(currentName);
        }
    }

    public void SetName(string name)
    {
        currentName = name;
        ApplyNameToText(name);
    }

    void ApplyNameToText(string name)
    {
        if (nameText == null)
            nameText = GetComponentInChildren<TextMeshProUGUI>();

        if (nameText != null)
            nameText.text = name;
    }
}
