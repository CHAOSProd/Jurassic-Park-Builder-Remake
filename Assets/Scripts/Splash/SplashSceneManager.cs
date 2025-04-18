using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using LootLocker.Requests;

public class SplashSceneManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject disclaimerPanel;
    public GameObject introVideoPanel;
    public GameObject loginUIContainer;
    public GameObject loadingPanel;

    [Header("Video")]
    public VideoPlayer introVideo;

    [Header("Durations")]
    public float disclaimerDuration = 3f;
    public float loadingDuration = 2f;

    private bool isLoggedIn = false;
    private bool sessionValid = false;
    private bool videoFinished = false;

    private void Start()
    {
        // 1) First check if there's already a valid session...
        LootLockerSDKManager.CheckWhiteLabelSession(valid =>
        {
            sessionValid = valid;

            // If session is already valid, we consider the user "logged in"
            if (sessionValid)
                isLoggedIn = true;

            // 2) Now start the splash sequence
            StartCoroutine(RunSplashSequence());
        });
    }

    private System.Collections.IEnumerator RunSplashSequence()
    {
        // --- 1: Disclaimer ---
        disclaimerPanel.SetActive(true);
        introVideoPanel.SetActive(false);
        loginUIContainer.SetActive(false);
        loadingPanel.SetActive(false);

        yield return new WaitForSeconds(disclaimerDuration);
        disclaimerPanel.SetActive(false);

        // --- 2: Intro Video ---
        videoFinished = false;
        introVideo.loopPointReached += OnVideoFinished;

        introVideoPanel.SetActive(true);
        introVideo.Prepare();
        yield return new WaitUntil(() => introVideo.isPrepared);

        introVideo.Play();
        yield return new WaitUntil(() => introVideo.isPlaying);
        yield return new WaitUntil(() => videoFinished);

        introVideo.loopPointReached -= OnVideoFinished;
        introVideoPanel.SetActive(false);

        // --- 3: Login UI or Skip ---
        if (!sessionValid)
        {
            loginUIContainer.SetActive(true);
            yield return new WaitUntil(() => isLoggedIn);
            loginUIContainer.SetActive(false);
        }
        // if sessionValid, we never show loginUIContainer

        // --- 4: Loading ---
        loadingPanel.SetActive(true);
        yield return new WaitForSeconds(loadingDuration);

        // --- 5: Load Main Game ---
        SceneManager.LoadScene("Game");
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        videoFinished = true;
    }

    /// <summary>
    /// Call this from your Login button callback (as you already do)
    /// once the user has successfully logged in.
    /// </summary>
    public void OnUserLoggedIn()
    {
        isLoggedIn = true;
    }
}
