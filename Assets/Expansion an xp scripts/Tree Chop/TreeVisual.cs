using UnityEngine;

// Controls the visibility and destruction of the tree visual representation
public class TreeVisual : MonoBehaviour {

    // Enables the tree visual, making it visible in the scene
    public void EnableVisual() {
        gameObject.SetActive(true);
    }

    // Disables the tree visual, making it invisible in the scene
    public void DisableVisual() {
        gameObject.SetActive(false);
    }

}
