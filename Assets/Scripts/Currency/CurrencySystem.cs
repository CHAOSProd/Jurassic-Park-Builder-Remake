using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class CurrencySystem : Singleton<CurrencySystem>
{
    [SerializeField] private List<GameObject> _texts;
    [SerializeField] public PurchasePanel _notEnoughCoinsPanel;

    [SerializeField] public PurchasePanel _notEnoughBucksPanel;

    private static Dictionary<CurrencyType, int> _currencyAmounts = new Dictionary<CurrencyType, int>();

    private static Dictionary<CurrencyType, TextMeshProUGUI> _currencyTexts = new Dictionary<CurrencyType, TextMeshProUGUI>();

    // Define default values in code, if we want to, we can make a serialized dictionary (would require new classes)
    private static readonly Dictionary<CurrencyType, int> _defaultValues = new() 
    {
        { CurrencyType.Coins, 500000},
        { CurrencyType.Bucks, 10}, 
        { CurrencyType.Crystals, 0}
    };


    private void Awake()
    {
        for (int i = 0; i < _texts.Count; i++)
        {
            _currencyAmounts.Add((CurrencyType)i, 0);
            _currencyTexts.Add((CurrencyType)i, _texts[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>());
        }
    }

    private void Start()
    {
        EventManager.Instance.AddListener<CurrencyChangeGameEvent>(AddCurrency);
    }
    public void Load()
    {
        for(int i = 0; i < _texts.Count; i++)
        {
            _currencyAmounts[(CurrencyType)i] = Attributes.GetInt(((CurrencyType)i).ToString(), _defaultValues[(CurrencyType)i]);

            // For testing purposes
            //if((CurrencyType)i == CurrencyType.Coins)
            //{
            //    _currencyAmounts[(CurrencyType)i] = 1000000;
            //}

            // Initial UI Update
            _currencyTexts[(CurrencyType)i].text = _currencyAmounts[(CurrencyType)i].ToString("N0", CultureInfo.InvariantCulture);
        }
    }
    public bool HasEnoughCurrency(CurrencyType currencyType, int amount)
    {
        return _currencyAmounts.ContainsKey(currencyType) && _currencyAmounts[currencyType] >= amount;
    }

    public bool AddCurrency(CurrencyChangeGameEvent currencyChange)
    {
        CurrencyType currencyType = currencyChange.CurrencyType;
        int amount = currencyChange.Amount;

        if (_currencyAmounts.ContainsKey(currencyType))
        {
            if (amount < 0 && !HasEnoughCurrency(currencyType, -amount))
            {
                switch (currencyType)
                {
                    case CurrencyType.Coins:
                        _notEnoughCoinsPanel.ShowNotEnoughCoinsPanel(-amount);
                        break;
                    case CurrencyType.Bucks:
                        _notEnoughBucksPanel.ShowNotEnoughCoinsPanel(-amount);
                        break;
                }
                return false;
            }

            _currencyAmounts[currencyType] += amount;
            Attributes.SetInt(currencyType.ToString(), _currencyAmounts[currencyType]);
            _currencyTexts[currencyType].text = _currencyAmounts[currencyType].ToString("N0", new CultureInfo("en-US"));
            return true;
        }
        return false;
    }

    public int GetCurrencyAmount(CurrencyType currencyType)
    {
        return _currencyAmounts.ContainsKey(currencyType) ? _currencyAmounts[currencyType] : 0;
    }
}


public enum CurrencyType
{
    Coins,
    Bucks,
    Crystals
}