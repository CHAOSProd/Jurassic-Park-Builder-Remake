using UnityEngine;

// Manages the available tree chops and their state
public class TreeChopManager : Singleton<TreeChopManager>
{

    private int availableTreeChops = 10; // Number of available tree chops

    private int _currentAdder = 20;
    public int CurrentXP { get; private set; } = 16;

    // Increases the number of available tree chops
    public void IncreaseTreeChops() {
        availableTreeChops = Mathf.Max(0, availableTreeChops + 1); // Ensure chops don't go below zero
        Debug.Log($"Tree chops: {availableTreeChops}");
        Save();
    }
    public void Load()
    {
        CurrentXP = PlayerPrefs.GetInt("TreeExpansionXP", 16);
        _currentAdder = PlayerPrefs.GetInt("TreeExpansionAdder", 20);
    }
    public void UpdateXP()
    {
        CurrentXP += _currentAdder;
        _currentAdder += 8;

        PlayerPrefs.SetInt("TreeExpansionXP", CurrentXP);
        PlayerPrefs.SetInt("TreeExpansionAdder", _currentAdder);
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
    private void Save() 
    {

    }
}
