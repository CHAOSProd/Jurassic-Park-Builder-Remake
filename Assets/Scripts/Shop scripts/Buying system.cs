using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PurchasableItem : MonoBehaviour
{
    public int itemPrice = 10;
    public TextMeshProUGUI coinsText;
    public MoneyManager moneyManager;
    public Button buyButton;
    public PurchasePanel notEnoughCoinsPanel;
    public DebugBuildingButton debugBuildingButton;
    public GameObject shopPanel; // Reference to the shop panel

    private bool isProcessingPurchase = false;
    public bool purchased;

    private void Start()
    {
        moneyManager = MoneyManager.Instance;
        if (moneyManager == null)
        {
            Debug.LogError("MoneyManager script not found in the scene.");
        }

        if (buyButton != null)
        {
            buyButton.onClick.AddListener(TryPurchase);
        }
        else
        {
            Debug.LogError("Button reference not found.");
        }
    }

    private void Update()
    {
        UpdateBuyButton();
    }

    private void UpdateBuyButton()
    {
        if (buyButton != null)
        {
            buyButton.interactable = moneyManager.GetPlayerCoins() >= itemPrice && !isProcessingPurchase;
        }
    }

    public void TryPurchase()
    {
        if (moneyManager != null && !isProcessingPurchase)
        {
            int playerCoinsBeforePurchase = moneyManager.GetPlayerCoins();

            if (playerCoinsBeforePurchase >= itemPrice)
            {
                isProcessingPurchase = true;

                if (moneyManager.RemoveCoins(itemPrice))
                {
                    Debug.Log(gameObject.name + " purchased!");
                    purchased = true;
                    EnableBuildingSystem();
                    shopPanel.SetActive(false); // Close the shop panel after a successful purchase
                }
                else
                {
                    Debug.LogError("Unexpected failure during coin deduction.");
                }
            }
            else
            {
                Debug.Log("Not enough coins to buy " + gameObject.name);
                notEnoughCoinsPanel.ShowNotEnoughCoinsPanel(this);
                ShowDebugBuildingPanel();
            }

            isProcessingPurchase = false;
        }
        else
        {
            Debug.LogError("MoneyManager reference not found.");
        }
    }

    private void ShowDebugBuildingPanel()
    {
        // Add logic here to show the DebugBuildingButton panel over the shop panel
        if (debugBuildingButton != null && shopPanel != null)
        {
            debugBuildingButton.panelToShow = notEnoughCoinsPanel.gameObject;
            debugBuildingButton.ShowPanel();
            shopPanel.SetActive(true); // Ensure the shop panel is still open
        }
    }

    private void EnableBuildingSystem()
    {
        if (debugBuildingButton != null)
        {
            debugBuildingButton.panelToShow = shopPanel; // Set the shop panel as the panel to show
            debugBuildingButton.ShowPanel();
        }
    }
}






































































