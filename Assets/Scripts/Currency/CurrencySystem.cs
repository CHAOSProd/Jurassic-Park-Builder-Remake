using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class CurrencySystem : Singleton<CurrencySystem>
{
    [SerializeField] private List<GameObject> _texts;
    
    private static Dictionary<CurrencyType, int> _currencyAmounts = new Dictionary<CurrencyType, int>();

    private static Dictionary<CurrencyType, TextMeshProUGUI> _currencyTexts = new Dictionary<CurrencyType, TextMeshProUGUI>();

    // Define default values in code, if we want to, we can make a serialized dictionary (would require new classes)
    private static readonly Dictionary<CurrencyType, int> _defaultValues = new() { 
        { CurrencyType.Bucks, 5}, 
        { CurrencyType.Crystals, 0}, 
        { CurrencyType.Coins, 100} 
    };


    private void Awake()
    {
        for (int i = 0; i < _texts.Count; i++)
        {
            _currencyAmounts.Add((CurrencyType)i, 0);
            _currencyTexts.Add((CurrencyType)i, _texts[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>());

            if(!_defaultValues.ContainsKey((CurrencyType)i))
            {
                _defaultValues.Add((CurrencyType)i, 0);
            }
        }
    }

    private void Start()
    {
        EventManager.Instance.AddListener<CurrencyChangeGameEvent>(AddCurrency);
        EventManager.Instance.AddListener<NotEnoughCurrencyGameEvent>(OnNotEnoughCurrency);
    }
    public void Load()
    {
        for(int i = 0; i < _texts.Count; i++)
        {
            _currencyAmounts[(CurrencyType)i] = PlayerPrefs.GetInt(((CurrencyType)i).ToString(), _defaultValues[(CurrencyType)i]);
            if((CurrencyType)i == CurrencyType.Coins)
            {
                _currencyAmounts[(CurrencyType)i] = 100000;
            }
            _currencyTexts[(CurrencyType)i].text = _currencyAmounts[(CurrencyType)i].ToString("#,#", new CultureInfo("en-US"));
        }
    }
    private void OnNotEnoughCurrency(NotEnoughCurrencyGameEvent info)
    {
        Debug.Log($"You dont have enough amount of {info.Amount} {info.CurrencyType}");
    }
    public bool HasEnoughCurrency(CurrencyType currencyType, int amount)
    {
        return _currencyAmounts.ContainsKey(currencyType) && _currencyAmounts[currencyType] >= amount;
    }

    private void AddCurrency(CurrencyChangeGameEvent currencyChange)
    {
        CurrencyType currencyType = currencyChange.CurrencyType;
        int amount = currencyChange.Amount;

        if (_currencyAmounts.ContainsKey(currencyType))
        {
            if(amount < 0 && !HasEnoughCurrency(currencyType, -amount))
            {
                Debug.Log($"Can't deduct more {currencyType} than you have. (You had {_currencyAmounts[currencyType]} and wanted to deduct {-amount})");
                return;
            }
            _currencyAmounts[currencyType] += amount;
            PlayerPrefs.SetInt(currencyType.ToString(), amount);
            _currencyTexts[currencyType].text = _currencyAmounts[currencyType].ToString("#,#", new CultureInfo("en-US"));
        }
    }
}

public enum CurrencyType
{
    Coins,
    Bucks,
    Crystals
}