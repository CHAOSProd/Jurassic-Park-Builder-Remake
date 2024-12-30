using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class CurrencySystem : Singleton<CurrencySystem>
{
    [SerializeField] private List<GameObject> _texts;
    [SerializeField] private PurchasePanel _notEnoughCoinsPanel;

    private static Dictionary<CurrencyType, int> _currencyAmounts = new Dictionary<CurrencyType, int>();

    private static Dictionary<CurrencyType, TextMeshProUGUI> _currencyTexts = new Dictionary<CurrencyType, TextMeshProUGUI>();

    // Define default values in code, if we want to, we can make a serialized dictionary (would require new classes)
    private static readonly Dictionary<CurrencyType, int> _defaultValues = new() 
    {
        { CurrencyType.Coins, 100},
        { CurrencyType.Bucks, 50}, 
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
            if((CurrencyType)i == CurrencyType.Coins)
            {
                _currencyAmounts[(CurrencyType)i] = 10000000;
            }

            // Initial UI Update
            _currencyTexts[(CurrencyType)i].text = _currencyAmounts[(CurrencyType)i].ToString("#,#", new CultureInfo("en-US"));
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
            if(amount < 0 && !HasEnoughCurrency(currencyType, -amount))
            {
                _notEnoughCoinsPanel.ShowNotEnoughCoinsPanel(-amount);
                return false;
            }
            _currencyAmounts[currencyType] += amount;
            Attributes.SetInt(currencyType.ToString(), _currencyAmounts[currencyType]);
            _currencyTexts[currencyType].text = _currencyAmounts[currencyType].ToString("#,#", new CultureInfo("en-US"));
            return true;
        }
        return false;
    }
}

public enum CurrencyType
{
    Coins,
    Bucks,
    Crystals
}