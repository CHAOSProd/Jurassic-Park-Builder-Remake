using UnityEngine;

public class ShopVisibility : MonoBehaviour
{
    public bool isBuildingOrDecorations;
    public bool isDinosaur;
    [SerializeField] private int requiredLevelOrAmber;
    private void Start()
    {
        CheckVisibility();
    }

    public void CheckVisibility()
    {
        int currentLevel = LevelManager.Instance != null ? (int)LevelManager.Instance.GetCurrentLevel() : 1;

        if (isBuildingOrDecorations)
        {
            if (currentLevel >= requiredLevelOrAmber - 4)
            {
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        else if (isDinosaur)
        {
            if(AmberManager.Instance.GetLastCollectedAmberIndex() >= requiredLevelOrAmber)
            {
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    public static void UpdateShopVisibility()
    {
        ShopVisibility[] allShops = FindObjectsOfType<ShopVisibility>(true);
        foreach (ShopVisibility shop in allShops)
        {
            shop.CheckVisibility();
        }
    }
}