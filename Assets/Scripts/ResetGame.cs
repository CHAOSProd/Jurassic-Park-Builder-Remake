using UnityEngine;
using UnityEngine.UI;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DeleteSaveButton : MonoBehaviour
{
    public Button deleteButton;
    void Start()
    {
        if (deleteButton != null)
        {
            deleteButton.onClick.AddListener(DeleteSaveFile);
        }
    }

    void DeleteSaveFile()
    {
        Debug.Log("Setting to delete saves on next opening...");
        
        PlayerPrefs.SetInt("DeleteSaveOnStart", 1);
        PlayerPrefs.Save();
        
        #if UNITY_EDITOR
            Debug.Log("Closing the game in the Unity Editor.");
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Debug.Log("Closing the game in build.");
            Application.Quit();
        #endif
    }
}
