using UnityEngine;
using UnityEngine.UI;

public class ButtonUnlockHandler : MonoBehaviour
{
    [SerializeField] private ShopItemUnlock itemToUnlock; // The item this button unlocks

    private void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null && itemToUnlock != null)
        {
            button.onClick.AddListener(OnButtonClicked);
        }
        else
        {
            Debug.LogError("Button or itemToUnlock reference is missing!");
        }
    }

    private void OnButtonClicked()
    {
        if (itemToUnlock != null)
        {
            itemToUnlock.OnUnlockButtonClicked(); // Only unlock the specific item this button refers to
        }
    }
}

