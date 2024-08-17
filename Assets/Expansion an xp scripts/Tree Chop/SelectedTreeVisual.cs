using UnityEngine;

// Controls the visual indicator for selected tree
public class SelectedTreeVisual : MonoBehaviour {

    // Enables the selected tree visual, making it visible in the scene
    public void SelectVisual() {
        gameObject.SetActive(true);
    }

    // Disables the selected tree visual, making it invisible in the scene
    public void DeselectVisual() {
        gameObject.SetActive(false);
    }

}
