using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PurchasableItem : MonoBehaviour
{
    public int itemPrice = 10;
    public TextMeshProUGUI coinsText;
    public Button buyButton;
    public PurchasePanel notEnoughCoinsPanel;
    public DebugBuildingButton debugBuildingButton;
    public GameObject shopPanel; // Reference to the shop panel

    private bool isProcessingPurchase = false;
    public bool purchased;

    private void Start()
    {
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
            buyButton.interactable = CurrencySystem.Instance.HasEnoughCurrency(CurrencyType.Coins, itemPrice) && !isProcessingPurchase;
        }
    }

    public void TryPurchase()
    {
        if (!isProcessingPurchase)
        {
            if (CurrencySystem.Instance.HasEnoughCurrency(CurrencyType.Coins, itemPrice))
            {
                isProcessingPurchase = true;

                purchased = true;
                EnableBuildingSystem();
                shopPanel.SetActive(false); // Close the shop panel after a successful purchase
            }
            else
            {
                notEnoughCoinsPanel.ShowNotEnoughCoinsPanel(this);
                ShowDebugBuildingPanel();
            }

            isProcessingPurchase = false;
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