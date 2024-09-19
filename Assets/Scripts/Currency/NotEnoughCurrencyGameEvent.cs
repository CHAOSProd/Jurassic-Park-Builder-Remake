using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct NotEnoughCurrencyGameEvent
{
    public int Amount { get; set; }
    public CurrencyType CurrencyType { get; set; }

    public NotEnoughCurrencyGameEvent(int amount, CurrencyType currencyType)
    {
        Amount = amount;
        CurrencyType = currencyType;
    }
}