using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SaveManager))]
public class SaveManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(20);
        if(GUILayout.Button("Delete save file"))
        {
            SaveSystem.Initialize();
            if (File.Exists(SaveSystem.FilePath))
            {
                File.Delete(SaveSystem.FilePath);
            }
        }
        if (GUILayout.Button("Clear player prefs"))
        {
            PlayerPrefs.DeleteAll();
        }
    }
}
