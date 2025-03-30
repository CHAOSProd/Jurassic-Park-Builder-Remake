using System;
using UnityEngine;
using UnityEngine.UI;

public class HomeManager : Singleton<HomeManager>
{
    [SerializeField] private GameObject DailyBuck;
    [SerializeField] private GameObject HomeNotification;
    [SerializeField] private GameObject LetterNotification;
    [SerializeField] private Button dailyBuckCollectButton;
    private const string LastClaimKey = "LastDailyBuckClaim";

    private void Start()
    {
        CheckAndToggleDailyBuck();
        if (dailyBuckCollectButton != null)
        {
            dailyBuckCollectButton.onClick.AddListener(ClaimDailyBuck);
        }
    }

    private void CheckAndToggleDailyBuck()
    {
        string lastClaimDate = PlayerPrefs.GetString(LastClaimKey, "");
        string todayDate = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
        
        if (lastClaimDate != todayDate)
        {
            DailyBuck.SetActive(true);
            HomeNotification.SetActive(true);
            LetterNotification.SetActive(true);
        }
        else
        {
            DailyBuck.SetActive(false);
            HomeNotification.SetActive(false);
            LetterNotification.SetActive(false);
        }
    }

    public void ClaimDailyBuck()
    {
        string todayDate = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
        EventManager.Instance.TriggerEvent(new CurrencyChangeGameEvent(1, CurrencyType.Bucks));
        PlayerPrefs.SetString(LastClaimKey, todayDate);
        PlayerPrefs.Save();
        
        DailyBuck.SetActive(false);
        HomeNotification.SetActive(false);
        LetterNotification.SetActive(false);
        Debug.Log("Daily buck claimed and hidden.");
    }
}