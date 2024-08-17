using System;
using TMPro;
using UnityEngine;

public class CollectMoneyDisplay : MonoBehaviour
{
    public static CollectMoneyDisplay Instance { get; private set; }

    private TextMeshProUGUI _text;

    private void Awake()
    {
        if(Instance == null) Instance = this;
    }
    private void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    public void Display(int money)
    {
        if (_text)
            _text.text = "<sprite index=0>" + money;
    }

    internal void DisplayCount(int v)
    {
        throw new NotImplementedException();
    }
}
