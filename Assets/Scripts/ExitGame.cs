using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ExitGame : MonoBehaviour
{
    // This method can be linked to a UI Button's OnClick event.
    public void ExitGameFunction()
    {
        Debug.Log("Exit button pressed. Quitting game...");

#if UNITY_EDITOR
        // If running in the Unity Editor, stop play mode.
        EditorApplication.isPlaying = false;
#else
            // If running in a build, quit the application.
            Application.Quit();
#endif
    }
}

