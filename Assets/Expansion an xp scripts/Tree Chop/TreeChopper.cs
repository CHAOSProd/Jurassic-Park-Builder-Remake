using UnityEngine;

// Handles tree chopping mechanics and visual changes
public class TreeChopper : MonoBehaviour {

    private SelectedTreeVisual selectedVisual; // Visual indicator for selected tree
    private TreeDebris treeDebris; // Debris object for the tree
    private TreeVisual treeVisual; // Main tree visual object

    [SerializeField] private GameObject _xpNotification;
    [SerializeField] private GameObject _tapVFX;
    [SerializeField] private GameObject _xpCounter;
    [SerializeField] private MoneyCountDisplayer _xpCountDisplayer;

    private bool hasTreeDebris = false; // Tracks if tree has debris after being chopped
    private bool allowSelection = true; // Determines if tree can be selected

    // Initializes references to visual components
    private void Awake() {
        // Get references to the child components for visual indicators and debris
        selectedVisual = GetComponentInChildren<SelectedTreeVisual>();
        treeDebris = GetComponentInChildren<TreeDebris>(true);
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
    private void OnMouseDown() 
    {
        if (PointerOverUIChecker.Instance.IsPointerOverUIObject()) return;
        // ARBITARY VALUES -- figure out how much bucks coins are equivalent to --
        int chopCost = 50;
        // -----------------------------------------------------------------------

        Debug.Log("Chopping... " + CurrencySystem.Instance.HasEnoughCurrency(CurrencyType.Coins, 200));
        // If the tree already has debris, collect it
        if (hasTreeDebris) {
            CollectDebris();
        } 
        else if (TreeChopManager.Instance.HasTreeChops() && CurrencySystem.Instance.HasEnoughCurrency(CurrencyType.Coins, TreeChopManager.Instance.CurrentCost))
        {
            PerformChopAction();
            EventManager.Instance.TriggerEvent(new CurrencyChangeGameEvent(-TreeChopManager.Instance.CurrentCost, CurrencyType.Coins));
            // Reduce the player's available tree chops
            TreeChopManager.Instance.ChopTree();
            TreeChopManager.Instance.UpdadeCost();
        } 
        else if (CurrencySystem.Instance.HasEnoughCurrency(CurrencyType.Bucks, chopCost)) 
        {
            // Perform chopping action
            PerformChopAction();

            // Deduct currency for chopping
            EventManager.Instance.TriggerEvent(new CurrencyChangeGameEvent(-chopCost, CurrencyType.Bucks));
        }
    }

    private void PerformChopAction() {
        // Prevent further selection
        allowSelection = false;

        // Update visuals to reflect the chopped state
        selectedVisual.DeselectVisual(); // Hide selection visuals
        treeVisual.DisableVisual(); // Hide tree visuals
        treeDebris.EnableDebris(); // Show debris visuals

        _xpNotification.SetActive(true);
        // Mark that the tree now has debris
        hasTreeDebris = true;
    }

    // Highlights the tree when mouse is over it
    private void OnMouseEnter() {
        if (PointerOverUIChecker.Instance.IsPointerOverUIObject()) return;

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
        _xpNotification.SetActive(false);
        _tapVFX.SetActive(true);
        _xpCounter.SetActive(true);
        _xpCountDisplayer.DisplayCount(TreeChopManager.Instance.CurrentXP);
        EventManager.Instance.TriggerEvent(new XPAddedGameEvent(TreeChopManager.Instance.CurrentXP)); // Award XP to the player
        TreeChopManager.Instance.UpdateXP();

        treeDebris.DisableDebris(); // Show debris visuals
        SaveManager.Instance.SaveData.ChoppedTrees.Add(new ChoppedTreeData(gameObject.name));
        Destroy(gameObject, .5f); // Destroy the tree object after some time, to ensure the xp effect still plays
    }

}
