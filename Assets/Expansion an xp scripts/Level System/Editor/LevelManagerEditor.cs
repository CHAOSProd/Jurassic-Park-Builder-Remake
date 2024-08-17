using UnityEngine;
using UnityEditor;

// Custom editor for LevelManager, provides testing buttons in the editor
[CustomEditor(typeof(LevelManager))]
public class LevelManagerEditor : Editor {

    public override void OnInspectorGUI() {
        DrawDefaultInspector(); // Draw the default inspector UI
        LevelManager levelManager = (LevelManager)target; // Get reference to the target LevelManager

        // Adds a button to give 50 XP for testing purposes
        if (GUILayout.Button("Give 50 XP (Play Mode Suggested)")) {
            levelManager.GiveXP(50f); // Give 50 XP to the player
        }

        // Adds a button to remove 50 XP for testing purposes
        if (GUILayout.Button("Remove 50 XP (Play Mode Suggested)")) {
            levelManager.RemoveXP(50f); // Remove 50 XP from the player
        }

        // Adds a button to give 50 bucks for testing purposes
        if (GUILayout.Button("Give 50 bucks (PLAY MODE ONLY)")) {
            CurrencySystem.AddCurrency(CurrencyType.Bucks, 50); // Give 50 bucks to the player
        }

        // Adds a button to Remove 50 bucks for testing purposes
        if (GUILayout.Button("Remove 50 bucks (PLAY MODE ONLY)")) {
            CurrencySystem.DeductCurrency(CurrencyType.Bucks, 50); // Remove 50 bucks from the player
        }
    }

}
