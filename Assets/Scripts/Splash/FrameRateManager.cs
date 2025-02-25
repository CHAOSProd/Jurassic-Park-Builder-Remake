using UnityEngine;

public class FrameRateManager : MonoBehaviour
{
    [SerializeField]
    private int targetFrameRate = 60; // Set your desired FPS here

    private void Awake()
    {
        // Disable VSync to allow the target frame rate to take effect.
        QualitySettings.vSyncCount = 0;

        // Set the target frame rate.
        Application.targetFrameRate = targetFrameRate;

        // Optional: Keep this GameObject across scenes.
        DontDestroyOnLoad(gameObject);
    }
}

