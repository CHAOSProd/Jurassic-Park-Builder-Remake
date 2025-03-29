using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CheatMenuUi : MonoBehaviour
{
    public Button closeUI,Moneygib,xpgib,meatgib,cropsgib,bucksgib;
    enum clickevent
    {
        exit,
        Moneygib,
        Xpgib,
        MeatGib,
        Cropsgib,
        Bucksgib
    }
    void clicked(clickevent e)
    {
        switch (e)
        {
            case clickevent.exit:
                this.gameObject.SetActive(false);
                break;
            case clickevent.Moneygib:
                EventManager.Instance.TriggerEvent(new CurrencyChangeGameEvent(200000, CurrencyType.Coins)); 
                break;
            case clickevent.Xpgib:
                EventManager.Instance.TriggerEvent(new XPAddedGameEvent(10000));
                break;
            case clickevent.MeatGib:
                EventManager.Instance.TriggerEvent(new CurrencyChangeGameEvent(2000, CurrencyType.Meat)); 
                break;
            case clickevent.Cropsgib:
                EventManager.Instance.TriggerEvent(new CurrencyChangeGameEvent(2000, CurrencyType.Crops)); 
                break;
            case clickevent.Bucksgib:
                EventManager.Instance.TriggerEvent(new CurrencyChangeGameEvent(10, CurrencyType.Bucks)); 
                break;
            default:
                break;
        }
    }

    void Start()
    {
        closeUI.onClick.AddListener(() => clicked(clickevent.exit));
    }
}
