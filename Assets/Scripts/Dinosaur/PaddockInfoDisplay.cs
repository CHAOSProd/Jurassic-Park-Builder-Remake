using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PaddockInfoDisplay : Singleton<PaddockInfoDisplay>
{

    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _moneyPerTimeText;
    [SerializeField] private TextMeshProUGUI _nameText;

    public void Display(string dinosaurName, int currentLevel, int maximumMinutes, float maximumMoney)
    {
        if (currentLevel != 40)
        {
            _levelText.text = "LVL " + currentLevel;
        }
        else
        {
            _levelText.text = "MAX";
        }

        string timeUnit = "mn";
        int displayTime = maximumMinutes;

        if (maximumMinutes >= 60)
        {
            displayTime = maximumMinutes / 60;
            timeUnit = "hr";
        }

        _moneyPerTimeText.text = $"<sprite=0>{maximumMoney} / {displayTime} {timeUnit}";
        _nameText.text = dinosaurName;
    }
}
