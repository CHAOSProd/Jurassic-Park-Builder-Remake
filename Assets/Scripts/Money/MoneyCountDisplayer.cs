using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyCountDisplayer : MonoBehaviour
{
    enum IconAlignment
    {
        Left,
        Right
    }

    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private string spriteName;
    [SerializeField] private IconAlignment iconAlignment = IconAlignment.Right;
    public void DisplayCount(int moneyCount)
    {
        if(iconAlignment == IconAlignment.Left) 
        {
            _text.text = $"<sprite name=\"{spriteName}\"> " + moneyCount.ToString();
        }
        else
        {
            _text.text = moneyCount.ToString() + $"<sprite name=\"{spriteName}\">";
        }
    }
}