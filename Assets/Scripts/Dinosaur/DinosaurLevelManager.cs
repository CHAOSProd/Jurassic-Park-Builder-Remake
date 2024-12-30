using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DinosaurLevelManager : MonoBehaviour
{
    [SerializeField] private int _currentMaximumMoneyForTime;

    public MoneyObject _moneyObject;
    public DinosaurLevelResourcesManager _dinosaurLevelResourcesManager;

    public int CurrentLevel;

    private void Start()
    {
        _moneyObject = GetComponent<MoneyObject>();
        _dinosaurLevelResourcesManager = GetComponent<DinosaurLevelResourcesManager>();

        string parrentName = GetComponentInParent<Paddock>().gameObject.name;
        if (Attributes.HaveKey("CurrentLevel" + parrentName))
        {
            CurrentLevel = Attributes.GetInt("CurrentLevel" + parrentName);
        }
        else
        {
            CurrentLevel = 1;
        }

        Initialize();
    }

    public void LevelUp()
    {
        CurrentLevel++;
        Initialize();
    }

    public void SetLevel(int level)
    {
        CurrentLevel = level;
        Initialize();
    }

    public void Initialize()
    {
    _currentMaximumMoneyForTime = _dinosaurLevelResourcesManager.GetMaximumMoneyByLevel(CurrentLevel);

    _moneyObject.MaximumMoney = _currentMaximumMoneyForTime;

    _moneyObject.InitializeMoneyPerSecond();
    }   
}
