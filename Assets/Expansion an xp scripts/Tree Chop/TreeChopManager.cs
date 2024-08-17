using UnityEngine;

// Manages the available tree chops and their state
public class TreeChopManager : MonoBehaviour {

    public static TreeChopManager Instance; // Singleton instance

    private int availableTreeChops; // Number of available tree chops

    // Ensures only one instance of TreeChopManager exists
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    // Increases the number of available tree chops
    public void IncreaseTreeChops() {
        availableTreeChops = Mathf.Max(0, availableTreeChops + 1); // Ensure chops don't go below zero
        Debug.Log($"Tree chops: {availableTreeChops}");
        Save();
    }

    // Decreases the number of available tree chops
    public void ChopTree() {
        availableTreeChops = Mathf.Max(0, availableTreeChops - 1); // Ensure chops don't go below zero
        Debug.Log($"Tree chops: {availableTreeChops}");
        Save();
    }

    // Checks if there are any tree chops available
    public bool HasTreeChops() {
        return availableTreeChops > 0; // Return true if chops are available
    }

    // Handle saving
    private void Save() {
        PlayerPrefs.SetInt("tree chops", availableTreeChops);
        // Un-comment the line below to save the player pref after modification. (You won't need the bottom line if you have a general saving script)
        // PlayerPrefs.Save();
    }

}
