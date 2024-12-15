using UnityEngine;
using UnityEngine.UI;

public class ButtonUnlockHandler : Singleton<ButtonUnlockHandler>
{
    [SerializeField] private Button _unlockButton;
    [SerializeField] private Transform _buildingContainer;
    private ShopItemUnlock _itemToUnlock; // The item this button unlocks

    private void Start()
    {
        if (_unlockButton != null)
        {
            _unlockButton.onClick.AddListener(OnButtonClicked);
        }
        else
        {
            Debug.LogError("Button reference is missing!");
        }
    }

    private void OnButtonClicked()
    {
        if (_itemToUnlock != null)
        {
            _itemToUnlock.OnUnlockButtonClicked(); // Only unlock the specific item this button refers to
        }
    }
    public void SetUnlockItem(ShopItemUnlock unlockItem)
    {
        _itemToUnlock = unlockItem;
    }
    public void UpdateUnlockItems()
    {
        int itemCount = _buildingContainer.childCount;
        for (int i = 0; i < itemCount; i++)
        {
            ShopItemUnlock item = _buildingContainer.GetChild(i).GetComponent<ShopItemUnlock>();
            if (item.CheckLevelAndUnlock()) return;
        }
    }
}

