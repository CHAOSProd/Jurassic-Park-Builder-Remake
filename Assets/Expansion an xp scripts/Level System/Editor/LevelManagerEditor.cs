using UnityEngine;
using UnityEditor;

// Custom editor for LevelManager, provides testing buttons in the editor
[CustomEditor(typeof(LevelManager))]
public class LevelManagerEditor : Editor {

    public override void OnInspectorGUI() {
        DrawDefaultInspector(); // Draw the default inspector UI
        

        // Adds a button to give 10000 XP for testing purposes
        if (GUILayout.Button("Give 10000 XP (PLAY MODE ONLY)")) {
            EventManager.Instance.TriggerEvent(new XPAddedGameEvent(10000));
        }

        // Adds a button to give 150 XP for testing purposes
        if (GUILayout.Button("Give 150 XP (PLAY MODE ONLY)")) {
            EventManager.Instance.TriggerEvent(new XPAddedGameEvent(150));
        }

        // Adds a button to remove 150 XP for testing purposes
        if (GUILayout.Button("Remove 150 XP (PLAY MODE ONLY)")) {
            EventManager.Instance.TriggerEvent(new XPAddedGameEvent(-150));
        }

        // Adds a button to give 10 bucks for testing purposes
        if (GUILayout.Button("Give 10 bucks (PLAY MODE ONLY)")) {
            // Give 10 bucks to the player
            EventManager.Instance.TriggerEvent(new CurrencyChangeGameEvent(10, CurrencyType.Bucks)); 
        }

        // Adds a button to Remove 10 bucks for testing purposes
        if (GUILayout.Button("Remove 10 bucks (PLAY MODE ONLY)")) {
            // Remove 10 bucks from the player
            EventManager.Instance.TriggerEvent(new CurrencyChangeGameEvent(-10, CurrencyType.Bucks)); 
        }

        // Adds a button to Add 200000 coins for testing purposes
        if (GUILayout.Button("Add 200000 coins (PLAY MODE ONLY)")) {
            // Add 200000 bucks from the player
            EventManager.Instance.TriggerEvent(new CurrencyChangeGameEvent(200000, CurrencyType.Coins)); 
        }

        // Adds a button to Remove 200000 coins for testing purposes
        if (GUILayout.Button("Remove 200000 coins (PLAY MODE ONLY)")) {
            // Remove 200000 bucks from the player
            EventManager.Instance.TriggerEvent(new CurrencyChangeGameEvent(-200000, CurrencyType.Coins)); 
        }

        if (GUILayout.Button("Add 2000 crops (PLAY MODE ONLY)")) {
            // Add 2000 crops from the player
            EventManager.Instance.TriggerEvent(new CurrencyChangeGameEvent(2000, CurrencyType.Crops)); 
        }
        
        if (GUILayout.Button("Remove 2000 crops (PLAY MODE ONLY)")) {
            // Remove 2000 crops from the player
            EventManager.Instance.TriggerEvent(new CurrencyChangeGameEvent(-2000, CurrencyType.Crops)); 
        }

        if (GUILayout.Button("Add 2000 meat (PLAY MODE ONLY)")) {
            // Add 2000 meat from the player
            EventManager.Instance.TriggerEvent(new CurrencyChangeGameEvent(2000, CurrencyType.Meat)); 
        }

        if (GUILayout.Button("Remove 2000 meat (PLAY MODE ONLY)")) {
            // Remove 2000 meat from the player
            EventManager.Instance.TriggerEvent(new CurrencyChangeGameEvent(-2000, CurrencyType.Meat)); 
        }
    }

}
