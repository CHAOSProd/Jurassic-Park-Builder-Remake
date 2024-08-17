using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }

    [SerializeField] private int defaultMoneyAmount = 1000;
    public TextMeshProUGUI coinsText; // Reference to the TextMeshProUGUI displaying player coins

    private int playerCoins;

    private void Awake()
    {
        if(Instance == null) Instance = this;
    }
    private void Start()
    {
        LoadPlayerCoins();
        UpdateCoinsText(); // Update the UI text on start
    }

    public void AddCoins(int amount)
    {
        playerCoins += amount;
        SavePlayerCoins();
        UpdateCoinsText(); // Update the UI text after adding coins
        Debug.Log($"Added {amount} coins. Total coins: {playerCoins}");
    }

    public bool RemoveCoins(int amount)
    {
        if (playerCoins >= amount)
        {
            playerCoins -= amount;
            SavePlayerCoins(); // Save after deducting coins
            UpdateCoinsText(); // Update the UI text after removing coins
            Debug.Log($"Removed {amount} coins. Total coins: {playerCoins}");
            return true; // Indicate successful coin removal
        }

        Debug.Log($"Attempted to remove {amount} coins, but not enough coins available. Total coins: {playerCoins}");
        return false; // Insufficient coins
    }

    public int GetPlayerCoins()
    {
        Debug.Log($"Current coins: {playerCoins}");
        return playerCoins;
    }

    public string FormatCoins(int coins)
    {
        return coins.ToString("#,##0");
    }

    private void SavePlayerCoins()
    {
        PlayerPrefs.SetInt(nameof(playerCoins), playerCoins);
        PlayerPrefs.Save();
        Debug.Log($"Saved coins to PlayerPrefs. Current coins: {playerCoins}");
    }

    private void LoadPlayerCoins()
    {
        playerCoins = PlayerPrefs.GetInt(nameof(playerCoins), defaultMoneyAmount);
        Debug.Log($"Loaded coins from PlayerPrefs. Current coins: {playerCoins}");
    }

    private void UpdateCoinsText()
    {
        if (coinsText != null)
        {
            coinsText.text = FormatCoins(playerCoins);
            Debug.Log($"Updated UI text. Current coins: {playerCoins}");
        }
    }
}



















