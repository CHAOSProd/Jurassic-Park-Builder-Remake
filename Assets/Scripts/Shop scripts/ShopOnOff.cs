using UnityEngine;

public class ShopOnOff : MonoBehaviour
{
    public GameObject pic;

    public void ToggleShop()
    {
        pic.SetActive(!pic.activeInHierarchy);
    }
}


