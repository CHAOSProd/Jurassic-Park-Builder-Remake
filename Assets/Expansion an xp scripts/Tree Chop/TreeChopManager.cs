using UnityEngine;

// Manages the available tree chops and their state
public class TreeChopManager : Singleton<TreeChopManager>
{

    private int availableTreeChops = 10; // Number of available tree chops

    private int _currentXPAdder = 20;
    private int _currentCostAdder = 600;
    public int CurrentXP { get; private set; } = 16;
    public int CurrentCost { get; private set; } = 200;

    // Increases the number of available tree chops
    public void IncreaseTreeChops() {
        availableTreeChops = Mathf.Max(0, availableTreeChops + 1); // Ensure chops don't go below zero
        Debug.Log($"Tree chops: {availableTreeChops}");
    }
    public void Load()
    {
        CurrentXP = PlayerPrefs.GetInt("TreeExpansionXP", 16);
        _currentXPAdder = PlayerPrefs.GetInt("TreeExpansionXPAdder", 20);
        CurrentCost = PlayerPrefs.GetInt("TreeExpansionCost", 200);
        _currentCostAdder = PlayerPrefs.GetInt("TreeExpansionCostAdder", 600);
    }
    public void UpdateXP()
    {
        CurrentXP += _currentXPAdder;
        _currentXPAdder += 8;

        PlayerPrefs.SetInt("TreeExpansionXP", CurrentXP);
        PlayerPrefs.SetInt("TreeExpansionXPAdder", _currentXPAdder);
    }
    public void UpdadeCost()
    {
        CurrentCost += _currentCostAdder;
        _currentCostAdder += 400;

        PlayerPrefs.SetInt("TreeExpansionCost", CurrentCost);
        PlayerPrefs.SetInt("TreeExpansionCostAdder", _currentCostAdder);
    }
    // Decreases the number of available tree chops
    public void ChopTree() {
        availableTreeChops = Mathf.Max(0, availableTreeChops - 1); // Ensure chops don't go below zero
        Debug.Log($"Tree chops: {availableTreeChops}");
    }

    // Checks if there are any tree chops available
    public bool HasTreeChops() {
        return availableTreeChops > 0; // Return true if chops are available
    }
}
