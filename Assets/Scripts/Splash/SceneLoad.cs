using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoad : MonoBehaviour
{
    public string mainSceneName;
    public float splashDelay = 3.0f;

    void Start()
    {
        var splashManager = FindObjectOfType<SplashScreenManager>();
        if (splashManager != null)
        {
            splashManager.OnSplashSequenceCompleted += StartSceneLoad;
        }
        else
        {
            StartSceneLoad();
        }
    }

    void StartSceneLoad()
    {
        StartCoroutine(LoadMainSceneAfterDelay());
    }

    private IEnumerator LoadMainSceneAfterDelay()
    {
        float timer = 0;
        while (timer < splashDelay)
        {
            if (!DiscordAuthManager.Instance.isProcessing)
            {
                timer += Time.deltaTime;
            }
            yield return null;
        }

        while (!DiscordAuthManager.Instance.isDataReady)
        {
            yield return null;
        }

        Load(mainSceneName);
    }

    public void Load(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}