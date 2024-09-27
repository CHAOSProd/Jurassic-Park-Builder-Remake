using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugBuildingButton : MonoBehaviour
{
    private Button _button;

    public PlaceableObjectItem PlaceableObjectItem;
    public GameObject panelToShow; // Reference to the panel to show
    public PurchasableItem myPurchasableItem;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void Start()
    {
        _button.onClick.AddListener(TryPurchaseOrCreateObject);
    }

    void Update()
    {
        // Adjust the interactability based on your conditions
        _button.interactable = !GridBuildingSystem.Instance.TempPlaceableObject;
    }

    private void TryPurchaseOrCreateObject()
    {
        if (PlaceableObjectItem != null)
        {
            // Check if the item is purchasable
            if (PlaceableObjectItem.IsPurchasable())
            {
                // Purchase logic
                PlaceableObjectItem.PurchasableItem.TryPurchase();
            }
            else
            {
                // Create object if not purchasable but is purchased so the money is gone from the player coins
                if (myPurchasableItem.purchased)
                {
                    CreateObject();
                    myPurchasableItem.purchased = false;
                }
                else
                {
                    Debug.Log("Object could not be created because the .purchased flag was not set to true at the buying system script");
                }
                
            }
        }
    }

    private void CreateObject()
    {
        // Your existing code to create an object
        var obj = GridBuildingSystem.Instance.InitializeWithBuilding(PlaceableObjectItem.Prefab);
        PlaceableObject placeableObj = obj.GetComponent<PlaceableObject>();

        placeableObj.Initialize(PlaceableObjectItem);
        placeableObj.data.SellRefund = (int)Mathf.Round(myPurchasableItem.itemPrice * .5f);
    }

    // Additional method to show a panel
    public void ShowPanel()
    {
        // Implement your logic to show the panelToShow
        if (panelToShow != null)
        {
            panelToShow.SetActive(true);
        }
    }
}

