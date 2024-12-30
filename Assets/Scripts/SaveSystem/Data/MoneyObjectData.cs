using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MoneyObjectData : Data
{
    public int Money { get; set; }
    public int PlaceableObjectIndex { get; set; }
    public MoneyObjectData(int money)
    {
        this.Money = money;
    }
    public MoneyObjectData(int money, int placeableObjectIndex) : this(money)
    {
        PlaceableObjectIndex = placeableObjectIndex;
    }

}
