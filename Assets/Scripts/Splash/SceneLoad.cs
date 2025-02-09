using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoad : MonoBehaviour
{
    [Tooltip("Name of the main scene to load after the splash screen.")]
    public string mainSceneName;

    [Tooltip("Duration (in seconds) the splash screen is displayed.")]
    public float splashDelay = 3.0f;

    void Start()
    {
        // Start the coroutine to load the main scene after a delay
        StartCoroutine(LoadMainSceneAfterDelay());
    }

    private IEnumerator LoadMainSceneAfterDelay()
    {
        // Wait for the specified splash screen duration
        yield return new WaitForSeconds(splashDelay);
        // Load the main scene
        Load(mainSceneName);
    }

    public void Load(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
