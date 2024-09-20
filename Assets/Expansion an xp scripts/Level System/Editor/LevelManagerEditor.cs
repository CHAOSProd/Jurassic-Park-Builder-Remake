using UnityEngine;
using UnityEditor;

// Custom editor for LevelManager, provides testing buttons in the editor
[CustomEditor(typeof(LevelManager))]
public class LevelManagerEditor : Editor {

    public override void OnInspectorGUI() {
        DrawDefaultInspector(); // Draw the default inspector UI

        // Adds a button to give 50 XP for testing purposes
        if (GUILayout.Button("Give 50 XP (PLAY MODE ONLY)")) {
            EventManager.Instance.TriggerEvent(new XPAddedGameEvent(50));
        }

        // Adds a button to remove 50 XP for testing purposes
        if (GUILayout.Button("Remove 50 XP (PLAY MODE ONLY)")) {
            EventManager.Instance.TriggerEvent(new XPAddedGameEvent(-50));
        }

        // Adds a button to give 50 bucks for testing purposes
        if (GUILayout.Button("Give 50 bucks (PLAY MODE ONLY)")) {
            // Give 50 bucks to the player
            EventManager.Instance.TriggerEvent(new CurrencyChangeGameEvent(50, CurrencyType.Bucks)); 
        }

        // Adds a button to Remove 50 bucks for testing purposes
        if (GUILayout.Button("Remove 50 bucks (PLAY MODE ONLY)")) {
            // Remove 50 bucks from the player
            EventManager.Instance.TriggerEvent(new CurrencyChangeGameEvent(-50, CurrencyType.Bucks)); 
        }
    }

}
