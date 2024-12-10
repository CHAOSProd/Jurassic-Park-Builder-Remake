using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUnlock : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject lockPanel; // The panel that appears when the item is locked
    [SerializeField] private Button unlockButton;  // The button that triggers the unlocking process
    [SerializeField] private GameObject unlockConfirmationPanel; // The panel asking for premium currency
    [SerializeField] private GameObject insufficientCurrencyPanel; // The panel shown if the player doesn't have enough currency
    [SerializeField] private TMP_Text premiumCurrencyText; // Text showing the current amount of premium currency

    [Header("Item Details")]
    [SerializeField] private int requiredLevel; // The level required to unlock the item
    [SerializeField] private int premiumCost; // The amount of premium currency required to unlock the item
    [SerializeField] private TMP_Text levelText; // The TMP_Text component displaying the current level on the UI

    private int currentPremiumCurrency; // The player's current premium currency
    private bool isUnlocked = false; // Whether the item is unlocked or not

    private void Start()
    {
        // Check if the necessary references have been set
        if (lockPanel == null || unlockButton == null || unlockConfirmationPanel == null || insufficientCurrencyPanel == null || premiumCurrencyText == null || levelText == null)
        {
            Debug.LogError("Some UI elements are missing in the ShopItemUnlock script!");
            return;
        }

        // Initialize with the current player's premium currency
        currentPremiumCurrency = Attributes.GetInt("premiumCurrency", 0);
        premiumCurrencyText.text = currentPremiumCurrency.ToString();

        // Setup button listener
        unlockButton.onClick.AddListener(OnUnlockButtonClicked);

        // Initially check the player's level
        CheckLevelAndUnlock();
    }

    private void Update()
    {
        // Continuously check the player's level in case it changes
        CheckLevelAndUnlock();
    }

    private void CheckLevelAndUnlock()
    {
        int currentLevel = GetPlayerLevel(); // Get the current player level from the TMP_Text component

        // If the player's level meets or exceeds the required level, unlock the item
        if (currentLevel >= requiredLevel && !isUnlocked)
        {
            UnlockItem();
        }
        else if (currentLevel < requiredLevel)
        {
            ShowLockPanel(); // Show the lock panel if level is insufficient
        }
    }

    private void OnUnlockButtonClicked()
    {
        // Show the confirmation panel for unlocking the item
        unlockConfirmationPanel.SetActive(true);
    }

    public void OnUnlockConfirmButtonClicked()
    {
        // Check if the player has enough premium currency
        if (currentPremiumCurrency >= premiumCost)
        {
            currentPremiumCurrency -= premiumCost;
            Attributes.SetInt("premiumCurrency", currentPremiumCurrency); // Save updated premium currency
            premiumCurrencyText.text = currentPremiumCurrency.ToString(); // Update UI

            UnlockItem(); // Unlock the item
            unlockConfirmationPanel.SetActive(false); // Hide confirmation panel
        }
        else
        {
            insufficientCurrencyPanel.SetActive(true); // Show insufficient currency panel
        }
    }

    public void OnUnlockCancelButtonClicked()
    {
        // Hide the confirmation panel without unlocking
        unlockConfirmationPanel.SetActive(false);
    }

    public void OnInsufficientCurrencyCloseButtonClicked()
    {
        // Hide the insufficient currency panel
        insufficientCurrencyPanel.SetActive(false);
    }

    private void ShowLockPanel()
    {
        lockPanel.SetActive(true); // Show the lock UI panel
    }

    private void UnlockItem()
    {
        // Item is unlocked; no lock panel needed anymore
        lockPanel.SetActive(false);
        isUnlocked = true;
    }

    private int GetPlayerLevel()
    {
        // Get the player's level from the levelText component
        if (levelText != null && int.TryParse(levelText.text, out int level))
        {
            return level; // Return the level parsed from the levelText
        }
        else
        {
            Debug.LogError("Level text is not properly set or can't be parsed.");
            return 1; // Default to level 1 if the level is not valid
        }
    }
}





