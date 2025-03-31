using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class CurrencySystem : Singleton<CurrencySystem>
{
    [SerializeField] private List<GameObject> _texts;
    [SerializeField] public PurchasePanel _notEnoughCropsPanel;
    [SerializeField] public PurchasePanel _notEnoughMeatPanel;
    [SerializeField] public PurchasePanel _notEnoughCoinsPanel;
    [SerializeField] public PurchasePanel _notEnoughBucksPanel;

    private static Dictionary<CurrencyType, int> _currencyAmounts = new Dictionary<CurrencyType, int>();

    private static Dictionary<CurrencyType, TextMeshProUGUI> _currencyTexts = new Dictionary<CurrencyType, TextMeshProUGUI>();
    private Dictionary<CurrencyType, Coroutine> _activeCoroutines = new Dictionary<CurrencyType, Coroutine>();

    // Define default values in code, if we want to, we can make a serialized dictionary (would require new classes)
    private static readonly Dictionary<CurrencyType, int> _defaultValues = new() 
    {
        { CurrencyType.Crops, 1000},
        { CurrencyType.Meat, 1000},
        { CurrencyType.Coins, 2000},
        { CurrencyType.Bucks, 10}
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
                    case CurrencyType.Crops:
                        _notEnoughCropsPanel.ShowNotEnoughCoinsPanel(-amount);
                        break;
                    case CurrencyType.Meat:
                        _notEnoughMeatPanel.ShowNotEnoughCoinsPanel(-amount);
                        break;
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
            if (_activeCoroutines.ContainsKey(currencyType))
            {
                StopCoroutine(_activeCoroutines[currencyType]);
                _activeCoroutines.Remove(currencyType);
            }
            if (amount > 0)
            {
                Coroutine coroutine = StartCoroutine(UpdateCurrencyTextDelayed(currencyType));
                _activeCoroutines[currencyType] = coroutine;
            }
            else
            {
                _currencyTexts[currencyType].text = _currencyAmounts[currencyType].ToString("N0", new CultureInfo("en-US"));
            }
            return true;
        }
        return false;
    }
    private IEnumerator UpdateCurrencyTextDelayed(CurrencyType currencyType)
    {
        yield return new WaitForSeconds(0.9f);

        int startValue = int.Parse(_currencyTexts[currencyType].text, NumberStyles.AllowThousands, CultureInfo.InvariantCulture);
        int endValue = _currencyAmounts[currencyType];
        float duration = 0.4f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            int currentValue = Mathf.RoundToInt(Mathf.Lerp(startValue, endValue, elapsed / duration));
            _currencyTexts[currencyType].text = currentValue.ToString("N0", new CultureInfo("en-US"));
            yield return null;
        }

        _currencyTexts[currencyType].text = endValue.ToString("N0", new CultureInfo("en-US"));
    }

    public int GetCurrencyAmount(CurrencyType currencyType)
    {
        return _currencyAmounts.ContainsKey(currencyType) ? _currencyAmounts[currencyType] : 0;
    }
}


public enum CurrencyType
{
    Crops,
    Meat,
    Coins,
    Bucks
}