using System.Collections;
using UnityEngine;
using System;

public class SplashScreenManager : MonoBehaviour
{
    [System.Serializable]
    public class SplashItem
    {
        public GameObject splashObject;
        public float displayTime;
    }

    public SplashItem[] splashItems;
    private int currentSplashIndex = 0;
    private GameObject activeSplash;
    
    public event Action OnSplashSequenceCompleted;

    void Start()
    {
        if (splashItems.Length > 0)
        {
            StartCoroutine(PlaySplashSequence());
        }
        else
        {
            OnSplashSequenceCompleted?.Invoke();
        }
    }

    IEnumerator PlaySplashSequence()
    {
        while (currentSplashIndex < splashItems.Length)
        {
            ShowSplashItem(currentSplashIndex);

            float timer = 0;
            while (timer < splashItems[currentSplashIndex].displayTime)
            {
                if (!DiscordAuthManager.Instance.isProcessing)
                {
                    timer += Time.deltaTime;
                }
                yield return null;
            }

            HideSplashItem(currentSplashIndex);
            currentSplashIndex++;
        }
        OnSplashSequenceCompleted?.Invoke();
    }

    void ShowSplashItem(int index)
    {
        if (activeSplash != null)
        {
            activeSplash.SetActive(false);
        }
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