using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopItemUnlock : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject lockPanel; // The panel that appears when the item is locked
    [SerializeField] private TMP_Text premiumCostText; // TMPRO field to display the calculated price

    [Header("Item Details")]
    [SerializeField] private int requiredLevel; // The level required to unlock the item

    [Header("Logic")]
    [SerializeField] private Button panelUnlockButton;
    [SerializeField] private string saveName;

    private bool isUnlocked; // Whether the item is unlocked or not

    private void Start()
    {
        if (lockPanel == null || panelUnlockButton == null || premiumCostText == null)
        {
            Debug.LogError("Some UI elements are missing in the ShopItemUnlock script!");
            return;
        }

        isUnlocked = Attributes.GetBool(saveName, false);

        // Don't do the rest if it's already unlocked
        if (isUnlocked) return;

        // Sets unlock item of the panel to this object
        panelUnlockButton.onClick.AddListener(() => ButtonUnlockHandler.Instance.SetUnlockItem(this));

        // Initialize the lock panel for this item based on the current level
        CheckLevelAndUnlock();
    }

    // Call this method when the UI panel is opened
    public void OnUIOpen()
    {
        Debug.Log("UI Panel opened. Updating premium cost display.");
        UpdatePremiumCostDisplay();
    }
    // Added in order to make the item not show while levelling up if the item was bought with bucks
    public void UnlockItemWithBucks()
    {
    UnlockItem();
    Attributes.SetBool(saveName + "_bucks", true);
    }
    private void UpdatePremiumCostDisplay()
    {
        int currentLevel = Attributes.GetInt("level", 1);

        // Debug: Log the current and required levels
        Debug.Log($"Current Level: {currentLevel}, Required Level: {requiredLevel}");

        // Calculate the bucks required based on level difference
        if (currentLevel < requiredLevel)
        {
            int bucks = (requiredLevel - currentLevel) * 2;
            premiumCostText.text = $"{bucks}?"; // Display cost with "?"
            Debug.Log($"Calculated Bucks: {bucks}");
        }
        else
        {
            premiumCostText.text = "0?";
            Debug.Log("Player level meets or exceeds required level. No cost needed.");
        }

        // Force TextMeshPro and Canvas to update
        premiumCostText.ForceMeshUpdate();
        Canvas.ForceUpdateCanvases();
    }

    // Check if the player meets level requirements and unlock the item accordingly
    public bool CheckLevelAndUnlock()
    {
        if (isUnlocked) return false;
        int currentLevel = Attributes.GetInt("level", 1);

        if (currentLevel >= requiredLevel)
        {
            UnlockItem(); // Unlock the item if the level is sufficient
            return true;
        }
        else
        {
            ShowLockPanel(); // Show the lock panel if the level is insufficient
            return false;
        }
    }

    // Called when the unlock button is pressed for this specific item.
public void OnUnlockButtonClicked()
{
    int currentLevel = Attributes.GetInt("level", 1);
    int bucks = currentLevel < requiredLevel ? (requiredLevel - currentLevel) * 2 : 0;

    if (bucks > 0)
    {
        CurrencyChangeGameEvent currencyChange = new(-bucks, CurrencyType.Bucks);

        bool transactionSuccessful = CurrencySystem.Instance.AddCurrency(currencyChange);

        if (transactionSuccessful)
        {
            UnlockItemWithBucks();
            Debug.Log("Bucks deducted and item unlocked.");
        }
        else
        {
            Debug.LogWarning($"Not enough bucks to unlock the item. Required: {bucks}, Available: {CurrencySystem.Instance.GetCurrencyAmount(CurrencyType.Bucks)}");
        }
    }
    else
    {
        Debug.Log("No bucks required to unlock this item.");
        UnlockItem();
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
            Attributes.SetBool(saveName, true);
            Debug.Log("UnlockItem: Lock panel deactivated and item unlocked.");
        }
        else
        {
            Debug.LogError("UnlockItem: lockPanel is not assigned in the Inspector!");
        }
    }
}















