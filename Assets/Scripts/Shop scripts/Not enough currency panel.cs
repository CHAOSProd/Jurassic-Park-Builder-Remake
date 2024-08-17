using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PurchasePanel : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public Image spriteImage; // Reference to the Image component for the sprite

    // Show the panel when not enough coins, using the price from PurchasableItem
    public void ShowNotEnoughCoinsPanel(PurchasableItem item)
    {
        if (item.itemPrice <= 0)
        {
            Debug.LogError("Item price is not set or is set to 0.");
            return;
        }

        gameObject.SetActive(true);
        // Assuming you have a different way to set the sprite, ensure it is set before calling this method
        messageText.text = "You need        "+ FormatPrice(item.itemPrice) + " to do this!";
    }

    // Method to format the price with commas for thousands
    private string FormatPrice(int price)
    {
        return price >= 1000 ? string.Format("{0:#,0}", price) : price.ToString();
    }

    // Hide the panel
    public void HidePanel()
    {
        gameObject.SetActive(false);
    }
}


















