using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuildingInfoDisplay : MonoBehaviour
{
    //Singleton pattern to avoid search functions
    public static BuildingInfoDisplay Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI _moneyPerTimeText;
    [SerializeField] private TextMeshProUGUI _nameText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    public void Display(string buildingName, int maximumMinutes, float maximumMoney)
    {
        _moneyPerTimeText.text = "<sprite=0>" + maximumMoney + " / " + maximumMinutes + " min";
        _nameText.text = buildingName;
    }
}
