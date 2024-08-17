using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PaddockInfoDisplay : MonoBehaviour
{
    //Singleton to prevent search methods
    public static PaddockInfoDisplay Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _moneyPerTimeText;
    [SerializeField] private TextMeshProUGUI _nameText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    public void Display(string dinosaurName, int currentLevel, int maximumMinutes, float maximumMoney)
    {
        _levelText.text = "LVL " + currentLevel;
        _moneyPerTimeText.text = "<sprite=0>" + maximumMoney + " / " + maximumMinutes + " min";
        _nameText.text = dinosaurName;
    }
}
