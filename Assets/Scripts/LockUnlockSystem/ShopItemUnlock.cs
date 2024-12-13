using UnityEngine;
using TMPro;
using System.Globalization;
using UnityEngine.UI;

public class ShopItemUnlock : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject lockPanel; // The panel that appears when the item is locked

    [Header("Item Details")]
    [SerializeField] private int requiredLevel; // The level required to unlock the item
    [SerializeField] private int premiumCost; // The amount of bucks required to unlock the item
    [SerializeField] private TMP_Text levelText; // The TMP_Text component displaying the current level on the UI

    [Header("Logic")]
    [SerializeField] private Button panelUnlockButton;

    private bool isUnlocked = false; // Whether the item is unlocked or not

    private void Start()
    {
        if (lockPanel == null || levelText == null || panelUnlockButton == null)
        {
            Debug.LogError("Some UI elements are missing in the ShopItemUnlock script!");
            return;
        }

        // Sets unlock item of the panel to this object
        panelUnlockButton.onClick.AddListener(() => ButtonUnlockHandler.Instance.SetUnlockItem(this));

        // Initialize the lock panel for this item based on the current level
        CheckLevelAndUnlock();
    }

    // Check if the player meets level requirements and unlock the item accordingly
    private void CheckLevelAndUnlock()
    {
        if (isUnlocked) return; // Avoid checking again if already unlocked

        int currentLevel = GetPlayerLevel();

        if (currentLevel >= requiredLevel)
        {
            UnlockItem(); // Unlock the item if the level is sufficient
        }
        else
        {
            ShowLockPanel(); // Show the lock panel if the level is insufficient
        }
    }

    // Called when the unlock button is pressed for this specific item.
    public void OnUnlockButtonClicked()
    {
        CurrencyChangeGameEvent currencyChange = new(-premiumCost, CurrencyType.Bucks);

        // The CurrencyChangeEvent method already handles showing the not enough coins panel and returns true if the transaction was succesful and false if not
        if (EventManager.Instance.TriggerEvent(currencyChange)) 
        {
            //UpdateCurrencyUI();
            UnlockItem(); // Unlock the specific item that was clicked
            Debug.Log("Bucks deducted and item unlocked.");
        }
        else
        {
            Debug.LogError("Failed to deduct bucks.");
        }
    }

    // Show the lock panel only if the item is not unlocked
    private void ShowLockPanel()
    {
        if (lockPanel != null && !isUnlocked)
        {
            lockPanel.SetActive(true); // Show lock panel for this item
        }
    }

    // Unlock the item and hide the lock panel
    public void UnlockItem()
    {
        if (lockPanel != null)
        {
            lockPanel.SetActive(false); // Hide the lock panel for this item
            isUnlocked = true; // Mark this item as unlocked
            Debug.Log("UnlockItem: Lock panel deactivated and item unlocked.");
        }
        else
        {
            Debug.LogError("UnlockItem: lockPanel is not assigned in the Inspector!");
        }
    }

    // Get the player's current level from the levelText UI
    private int GetPlayerLevel()
    {
        if (levelText != null && int.TryParse(levelText.text, out int level))
        {
            return level;
        }
        else
        {
            Debug.LogError("Level text is not properly set or can't be parsed.");
            return 1; // Default to level 1 if parsing fails
        }
    }
}














