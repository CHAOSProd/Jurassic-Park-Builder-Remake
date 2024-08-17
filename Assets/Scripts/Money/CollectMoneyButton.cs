using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectMoneyButton : MonoBehaviour
{
    public static CollectMoneyButton Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}
