using UnityEngine;

// Controls the visibility and destruction of tree debris
public class TreeDebris : MonoBehaviour {

    // Enables the tree debris, making it visible in the scene
    public void EnableDebris() {
        gameObject.SetActive(true);
    }

    // Disables the tree debris, making it invisible in the scene
    public void DisableDebris() {
        gameObject.SetActive(false);
    }

}
