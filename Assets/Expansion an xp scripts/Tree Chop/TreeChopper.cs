using UnityEngine;

// Handles tree chopping mechanics and visual changes
public class TreeChopper : MonoBehaviour {

    private SelectedTreeVisual selectedVisual; // Visual indicator for selected tree
    private TreeDebris treeDebris; // Debris object for the tree
    private TreeVisual treeVisual; // Main tree visual object

    private bool hasTreeDebris; // Tracks if tree has debris after being chopped
    private bool allowSelection = true; // Determines if tree can be selected

    // Initializes references to visual components
    private void Awake() {
        // Get references to the child components for visual indicators and debris
        selectedVisual = GetComponentInChildren<SelectedTreeVisual>();
        treeDebris = GetComponentInChildren<TreeDebris>();
        treeVisual = GetComponentInChildren<TreeVisual>();
    }

    // Sets initial state of visual components
    private void Start() {
        // Deselect the tree visual and disable debris at the start
        selectedVisual.DeselectVisual();
        treeDebris.DisableDebris();
        treeVisual.EnableVisual();
    }

    // Handles tree chopping and debris collection logic on mouse click
    private void OnMouseDown() {
        int chopCost = 50;

        // If the tree already has debris, collect it
        if (hasTreeDebris) {
            CollectDebris();
        } else if (TreeChopManager.Instance.HasTreeChops()) { // Otherwise, if tree chops are available, chop the tree
            PerformChopAction();
            // Reduce the player's available tree chops
            TreeChopManager.Instance.ChopTree();
        } else if (CurrencySystem.HasEnoughCurrency(CurrencyType.Bucks, chopCost)) {
            // Perform chopping action
            PerformChopAction();

            // Deduct currency for chopping
            CurrencySystem.DeductCurrency(CurrencyType.Bucks, chopCost);
        }
    }

    private void PerformChopAction() {
        // Prevent further selection
        allowSelection = false;

        // Update visuals to reflect the chopped state
        selectedVisual.DeselectVisual(); // Hide selection visuals
        treeVisual.DisableVisual(); // Hide tree visuals
        treeDebris.EnableDebris(); // Show debris visuals

        // Mark that the tree now has debris
        hasTreeDebris = true;
    }

    // Highlights the tree when mouse is over it
    private void OnMouseEnter() {
        if (allowSelection) {
            selectedVisual.SelectVisual();
        }
    }

    // Removes highlight from the tree when mouse leaves it
    private void OnMouseExit() {
        if (allowSelection) {
            selectedVisual.DeselectVisual();
        }
    }

    // Collects tree debris and awards XP to the player
    private void CollectDebris() {
        hasTreeDebris = false; // Mark that the tree no longer has debris
        Destroy(gameObject); // Destroy the tree object
        float randomXP = Mathf.Ceil(Random.Range(10f, 20f)); // Generate random XP amount and round it to the top integer
        LevelManager.Instance.GiveXP(randomXP); // Award XP to the player

        Debug.Log($"Got {randomXP} from chopping trees!");
    }

}
