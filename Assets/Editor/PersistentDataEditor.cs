#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PersistentData))]
public class PersistentDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw default inspector
        DrawDefaultInspector();

        // Add reset button
        PersistentData persistentData = (PersistentData)target;
        if (GUILayout.Button("Reset Data"))
        {
            persistentData.ResetData();
            EditorUtility.SetDirty(persistentData); // Mark object as dirty to save changes
        }
    }
}
#endif