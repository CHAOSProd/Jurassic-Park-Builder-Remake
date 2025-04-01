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
    private const string FirstLaunchKey = "FirstLaunchDate";
    private const int DaysLimit = 45;

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
        string firstLaunchDateString = PlayerPrefs.GetString(FirstLaunchKey, "");
        DateTime firstLaunchDate;

        if (string.IsNullOrEmpty(firstLaunchDateString))
        {
            firstLaunchDate = DateTime.UtcNow.Date;
            PlayerPrefs.SetString(FirstLaunchKey, firstLaunchDate.ToString("yyyy-MM-dd"));
            PlayerPrefs.Save();
        }
        else
        {
            firstLaunchDate = DateTime.Parse(firstLaunchDateString);
        }

        DateTime today = DateTime.UtcNow.Date;
        int daysSinceFirstLaunch = (today - firstLaunchDate).Days;

        if (daysSinceFirstLaunch >= DaysLimit)
        {
            DailyBuck.SetActive(false);
            HomeNotification.SetActive(false);
            LetterNotification.SetActive(false);
            return;
        }

        string lastClaimDate = PlayerPrefs.GetString(LastClaimKey, "");
        string todayDate = today.ToString("yyyy-MM-dd");

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