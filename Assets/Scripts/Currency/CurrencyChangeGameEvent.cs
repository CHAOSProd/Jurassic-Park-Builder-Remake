using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CurrencyChangeGameEvent
{
    public int Amount { get; set; }
    public CurrencyType CurrencyType { get; set; }

    public CurrencyChangeGameEvent(int amount, CurrencyType currencyType)
    {
        Amount = amount;
        CurrencyType = currencyType;
    }
}