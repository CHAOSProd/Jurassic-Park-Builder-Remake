using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuildingInfoDisplay : Singleton<BuildingInfoDisplay>
{
    [SerializeField] private TextMeshProUGUI _moneyPerTimeText;
    [SerializeField] private TextMeshProUGUI _nameText;

    public void Display(string buildingName, int maximumMinutes, float maximumMoney)
    {
        string timeUnit = "mn";
        int displayTime = maximumMinutes;

        if (maximumMinutes >= 60)
        {
            displayTime = maximumMinutes / 60;
            timeUnit = "hr";
        }

        _moneyPerTimeText.text = $"<sprite=0>{maximumMoney} / {displayTime} {timeUnit}";
        _nameText.text = buildingName;
    }
}
