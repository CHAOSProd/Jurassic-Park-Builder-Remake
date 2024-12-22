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

    [Header("Logic")]
    [SerializeField] private Button panelUnlockButton;
    [SerializeField] private string saveName;

    private bool isUnlocked; // Whether the item is unlocked or not

    private void Start()
    {
        if (lockPanel == null || panelUnlockButton == null)
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

    // Check if the player meets level requirements and unlock the item accordingly
    public bool CheckLevelAndUnlock()
    {
        if(isUnlocked) return false;
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
        premiumCost = (requiredLevel-currentLevel)*2;
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
            Attributes.SetBool(saveName, true);
            Debug.Log("UnlockItem: Lock panel deactivated and item unlocked.");
        }
        else
        {
            Debug.LogError("UnlockItem: lockPanel is not assigned in the Inspector!");
        }
    }
}














