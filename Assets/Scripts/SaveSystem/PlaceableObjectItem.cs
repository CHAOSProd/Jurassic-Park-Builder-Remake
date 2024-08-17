using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Game Objects/Placeable Object Item", order = 0)]
public class PlaceableObjectItem : ScriptableObject
{
    public GameObject Prefab;
    public PurchasableItem PurchasableItem; // Reference to the PurchasableItem script

    public bool IsPurchasable()
    {
        return PurchasableItem != null; // Add any additional conditions for purchasability
    }
}

