using UnityEngine;

public class DinosaurLevelManager : MonoBehaviour
{
    [SerializeField] private int _currentMaximumMoneyForTime;
    public MoneyObject _moneyObject;
    public DinosaurLevelResourcesManager _dinosaurLevelResourcesManager;
    public DinosaurFeedingSystem _feedingSystem;
    public int CurrentLevel;
    public int FeedsPerLevel = 5;

    private void Awake()
    {
        _moneyObject = GetComponent<MoneyObject>();
        _dinosaurLevelResourcesManager = GetComponent<DinosaurLevelResourcesManager>();
        _feedingSystem = GetComponent<DinosaurFeedingSystem>();

        string parentName = GetComponentInParent<Paddock>().gameObject.name;
        if (Attributes.HaveKey("CurrentLevel" + parentName))
        {
            CurrentLevel = Attributes.GetInt("CurrentLevel" + parentName);
        }
        else
        {
            CurrentLevel = 1;
            Attributes.SetInt("CurrentLevel" + parentName, CurrentLevel);
        }
        if (Attributes.HaveKey("FeedCount" + parentName))
        {
            _feedingSystem.feedCount = Attributes.GetInt("FeedCount" + parentName);
        }
        else
        {
            _feedingSystem.feedCount = 0;
            Attributes.SetInt("FeedCount" + parentName, _feedingSystem.feedCount);
        }
        Initialize();
    }

    public void LevelUp()
    {
        CurrentLevel++;
        string parentName = GetComponentInParent<Paddock>().gameObject.name;
        Attributes.SetInt("CurrentLevel" + parentName, CurrentLevel);
        Initialize();
    }

    public void Initialize()
    {
        _currentMaximumMoneyForTime = _dinosaurLevelResourcesManager.GetMaximumMoneyByLevel(CurrentLevel);
        _moneyObject.MaximumMoney = _currentMaximumMoneyForTime;
        _moneyObject.InitMoneyPerSecond();
    }
}
