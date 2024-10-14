using System.Collections;
using UnityEngine;

public class SplashScreenManager : MonoBehaviour
{
    [System.Serializable]
    public class SplashItem
    {
        public GameObject splashObject;   // The GameObject to display
        public float displayTime;         // How long the object stays on screen (in seconds)
    }

    public SplashItem[] splashItems;      // Array to store splash items in the order they should appear

    private int currentSplashIndex = 0;   // Track the current splash item
    private GameObject activeSplash;      // Store the currently active splash object

    void Start()
    {
        if (splashItems.Length > 0)
        {
            StartCoroutine(PlaySplashSequence());
        }
        else
        {
            Debug.LogWarning("No splash items set in SplashScreenManager.");
        }
    }

    IEnumerator PlaySplashSequence()
    {
        // Loop through each splash item and display it
        while (currentSplashIndex < splashItems.Length)
        {
            ShowSplashItem(currentSplashIndex);

            // Wait for the specified display time
            yield return new WaitForSeconds(splashItems[currentSplashIndex].displayTime);

            HideSplashItem(currentSplashIndex);
            currentSplashIndex++;
        }

        // Optionally, load the main menu or another scene here
        // Example: SceneManager.LoadScene("MainMenu");
    }

    void ShowSplashItem(int index)
    {
        // Make sure no other splash object is visible
        if (activeSplash != null)
        {
            activeSplash.SetActive(false);
        }

        // Activate the current splash object
        activeSplash = splashItems[index].splashObject;
        activeSplash.SetActive(true);
    }

    void HideSplashItem(int index)
    {
        if (splashItems[index].splashObject != null)
        {
            splashItems[index].splashObject.SetActive(false);
        }
    }
}

