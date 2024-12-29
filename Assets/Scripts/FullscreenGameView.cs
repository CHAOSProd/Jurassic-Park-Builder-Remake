#if UNITY_EDITOR

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Little utility for opening a "Game" view in fullscreen. Will be opened on whatever Unity thinks is the "main"
/// monitor at the moment. The hotkey will toggle the window; however, if for some reason this breaks, fullscreen
/// windows can be closed via Alt+F4 as long as the editor is not in play mode.
/// </summary>
/// <remarks>
/// Confirmed to work in Unity 2019 and 2020. May work in earlier and later versions. No promises.
/// </remarks>
public static class FullscreenGameView
{
    static readonly Type GameViewType = Type.GetType("UnityEditor.GameView,UnityEditor");
    static readonly PropertyInfo ShowToolbarProperty = GameViewType.GetProperty("showToolbar", BindingFlags.Instance | BindingFlags.NonPublic);
    static readonly object False = false; // Only box once. This is a matter of principle.

    static EditorWindow instance;

    [MenuItem("Window/General/Game (Fullscreen) %#&2", priority = 2)]
    public static void Toggle()
    {
        if (GameViewType == null)
        {
            Debug.LogError("GameView type not found.");
            return;
        }

        if (ShowToolbarProperty == null)
        {
            Debug.LogWarning("GameView.showToolbar property not found.");
        }

        if (instance != null)
        {
            instance.Close();
            instance = null;
        }
        else
        {
            instance = (EditorWindow) ScriptableObject.CreateInstance(GameViewType);

            ShowToolbarProperty?.SetValue(instance, False);

            var desktopResolution = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
            var fullscreenRect = new Rect(Vector2.zero, desktopResolution);
            instance.ShowPopup();
            instance.position = fullscreenRect;
            instance.Focus();
        }
    }
}

#endif