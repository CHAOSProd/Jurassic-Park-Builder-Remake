using System;
using UnityEngine;

public static class DailyRewardSystem
{
    private const string LastClaimKey = "LastDailyBuckClaim";
    private const int DailyRewardAmount = 1;

    public static void CheckAndGrantDailyBuck()
    {
        string lastClaimDate = PlayerPrefs.GetString(LastClaimKey, "");
        string todayDate = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
        
        if (lastClaimDate != todayDate)
        {
            CurrencySystem.Instance.AddCurrency(new CurrencyChangeGameEvent(DailyRewardAmount, CurrencyType.Bucks));
            PlayerPrefs.SetString(LastClaimKey, todayDate);
            PlayerPrefs.Save();
            Debug.Log($"Daily buck granted, New total: {CurrencySystem.Instance.GetCurrencyAmount(CurrencyType.Bucks)}");
        }
        else
        {
            Debug.Log("Daily buck already claimed today.");
        }
    }
}