using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Manages player levels and experience points (XP)
public class LevelManager : MonoBehaviour
{

    public event EventHandler OnLevelUp; // Event triggered when leveling up
    public event EventHandler<float> OnXPChanged; // Event triggered when XP changes

    public static LevelManager Instance; // Singleton instance

    [SerializeField] private TMP_Text levelText; // UI element displaying the current level
    [SerializeField] private Image XPFillImage; // UI element displaying the XP bar
    [SerializeField] private GameObject levelUpPanel; // Panel that pops up on level up
    [SerializeField] private float[] xpPerLevel; // Array to store XP required for each level

    private float level; // Current player level
    private float XP; // Current player XP

    // Ensures only one instance of LevelManager exists
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Initializes XP and UI, and subscribes to events
    private void Start()
    {
        // Check if xpPerLevel array is properly assigned
        if (xpPerLevel == null || xpPerLevel.Length == 0)
        {
            Debug.LogError("XP per level array is not assigned or empty!");
            return;
        }

        // Check if levelUpPanel is assigned
        if (levelUpPanel == null)
        {
            Debug.LogError("Level Up Panel is not assigned!");
            return;
        }

        level = 1; // Start at level 1
        OnXPChanged += LevelManager_OnXPChanged; // Subscribe to XP changed event
        OnLevelUp += LevelManager_OnLevelUp; // Subscribe to level up event
        UpdateUI(); // Update the UI to reflect initial values
    }

    // Handles logic for leveling up
    private void LevelManager_OnLevelUp(object sender, EventArgs e)
    {
        Debug.Log($"TreeChopManager.Instance is {(TreeChopManager.Instance == null ? "null" : "not null")}");

        level++; // Increase the player level
        XP = 0f; // Reset XP to zero

        // Ensure there is a next level to level up to
        if (level <= xpPerLevel.Length)
        {
            ShowLevelUpPanel(); // Display the level up panel
        }
        else
        {
            Debug.LogWarning("Level exceeds the defined xpPerLevel array. Consider extending the xpPerLevel array.");
        }

        if (TreeChopManager.Instance != null)
        {
            TreeChopManager.Instance.IncreaseTreeChops(); // Increase available tree chops
        }
        else
        {
            Debug.LogError("TreeChopManager.Instance is null. Make sure TreeChopManager is initialized properly.");
        }

        UpdateUI(); // Update the UI to reflect new level
        Save(); // Save progress
    }

    // Updates the level and UI when XP changes
    private void LevelManager_OnXPChanged(object sender, float amount)
    {
        Debug.Log($"XP: {XP}"); // Log the current XP
        CalculateLevel(); // Check if the player should level up
        UpdateUI(); // Update the UI to reflect new XP
        Save(); // Save progress
    }

    // Adds XP and triggers the XP changed event
    public void GiveXP(float amount)
    {
        XP = Mathf.Max(0f, XP + amount); // Ensure XP doesn't go below zero
        OnXPChanged?.Invoke(this, XP); // Trigger XP changed event
    }

    // Removes XP and triggers the XP changed event
    public void RemoveXP(float amount)
    {
        XP = Mathf.Max(0f, XP - amount); // Ensure XP doesn't go below zero
        OnXPChanged?.Invoke(this, XP); // Trigger XP changed event
    }

    // Calculates if the player should level up based on current XP
    private void CalculateLevel()
    {
        if (level < xpPerLevel.Length && XP >= xpPerLevel[(int)level - 1])
        {
            OnLevelUp?.Invoke(this, EventArgs.Empty); // Trigger level up event
        }
    }

    // Updates the UI elements for level and XP bar
    private void UpdateUI()
    {
        levelText.text = $"{level}"; // Update the level text
        XPFillImage.fillAmount = Mathf.Clamp01(XP / xpPerLevel[(int)level - 1]); // Update the XP fill amount
    }

    // Shows the level up panel
    private void ShowLevelUpPanel()
    {
        levelUpPanel.SetActive(true);
        // Hide the panel after a delay (optional)
        Invoke(nameof(HideLevelUpPanel), 2f); // Hide after 2 seconds, adjust as needed
    }

    // Hides the level up panel
    private void HideLevelUpPanel()
    {
        levelUpPanel.SetActive(false);
    }

    // Handles saving
    private void Save()
    {
        PlayerPrefs.SetFloat("xp", XP);
        PlayerPrefs.SetInt("level", (int)level);
        // Un-comment the line below to save the player pref after modification. (You won't need the bottom line if you have a general saving script)
        // PlayerPrefs.Save();
    }
}


