using UnityEngine;

public class PanelUIController : MonoBehaviour
{
    // Drag and drop the UI elements you want to hide when this panel is active.
    public GameObject[] uiElementsToHide;

    // Called when the panel becomes active.
    private void OnEnable()
    {
        // Hide each assigned UI element.
        foreach (GameObject element in uiElementsToHide)
        {
            if (element != null)
            {
                element.SetActive(false);
            }
        }
    }

    // Optional: Called when the panel is deactivated.
    // Uncomment if you want the UI elements to reappear.
    /*
    private void OnDisable()
    {
        // Show each assigned UI element.
        foreach (GameObject element in uiElementsToHide)
        {
            if (element != null)
            {
                element.SetActive(true);
            }
        }
    }
    */
}

