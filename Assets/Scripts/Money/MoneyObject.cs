using System;
using UnityEngine;
using UnityEngine.UI;

public class MoneyObject : MonoBehaviour
{
    [SerializeField] private GameObject _notification;
    [SerializeField] private GameObject _tapVFX;
    [SerializeField] private GameObject _moneyCounter;
    [SerializeField] private MoneyCountDisplayer _moneyCountDisplayer;
    [SerializeField] public float _moneyPerSecond = 0.33f;
    [SerializeField] private CollectMoneyDisplay _collectMoneyDisplay;
    [SerializeField] private Button _collectMoneyButton;
    [SerializeField] private Animator _BabyAnimator;
    [SerializeField] private Animator _AdultAnimator;
    private DinosaurFeedingSystem _dinosaurFeedingSystem;

    public int CurrentMoneyInteger
    {
        get
        {
            return _data.Money;
        }
    }

    public float MaximumMoney = 100;
    public int MaximumMinutes = 5;
    private DinosaurLevelManager _levelManager;  
    private Selectable _selectable;
    private Paddock _paddock;
    private float _maximumSeconds;
    private int _currentSecond;
    private float _currentMoneyFloated;
    private float _timeFromLastMoneyAdding;
    private bool _isPointerMoving;
    private Vector3 _lastPointerPosition;
    public bool _maxMoneyReached;

    private MoneyObjectData _data;

    private void Awake()
    {
        Paddock p = GetComponent<Paddock>();
        if (p != null)
        {
            _paddock = p;
            _selectable = p;
        }
        else
        {
            _selectable = GetComponentInParent<Building>();
        }
        _dinosaurFeedingSystem = GetComponentInChildren<DinosaurFeedingSystem>();
    }
    private void Start()
    {
        _collectMoneyDisplay = CollectMoneyDisplay.Instance;
        _collectMoneyButton = CollectMoneyButton.Instance.GetComponent<Button>();

        // Get the DinosaurLevelManager values
        _levelManager = GetComponentInParent<DinosaurLevelManager>();
        if (_levelManager != null)
        {
            if (_levelManager._dinosaurLevelResourcesManager == null)
            {
                _levelManager._dinosaurLevelResourcesManager = FindAnyObjectByType<DinosaurLevelResourcesManager>();
            }

            if (_levelManager._dinosaurLevelResourcesManager != null)
            {
                MaximumMoney = _levelManager._dinosaurLevelResourcesManager.GetMaximumMoneyByLevel(_levelManager.CurrentLevel);
            }
        }
        _collectMoneyButton.onClick.AddListener(GetMoneyIfAvaliableByButton);
    }

    private void Update()
    {
        bool isBabyActive = _dinosaurFeedingSystem.babyModel.activeSelf;
        bool isAdultActive = _dinosaurFeedingSystem.adultModel.activeSelf;

        if(_data == null)
        {
            return;
        }

        if (_maxMoneyReached)
        {
            if (!_notification.activeSelf)
            {
                _notification.SetActive(true);
                if (isBabyActive && _BabyAnimator != null)
                    _BabyAnimator.SetTrigger("MaxMoneyReached");
                else if (isAdultActive && _AdultAnimator != null)
                    _AdultAnimator.SetTrigger("MaxMoneyReached");
            }
            return;
        }

        if (_data.Money > MaximumMoney && !_maxMoneyReached)
        {
            _currentMoneyFloated = MaximumMoney;
            _data.Money = Mathf.FloorToInt(_currentMoneyFloated);
            _notification.SetActive(true);
            if (isBabyActive && _BabyAnimator != null)
                _BabyAnimator.SetTrigger("MaxMoneyReached");
            else if (isAdultActive && _AdultAnimator != null)
                _AdultAnimator.SetTrigger("MaxMoneyReached");

            _maxMoneyReached = true;

            return;
        }

        if (_selectable.IsSelected)
        {
            _collectMoneyDisplay.Display(_data.Money);
        }

        _timeFromLastMoneyAdding += Time.deltaTime;

        if (_timeFromLastMoneyAdding >= 1)
        {
            _currentMoneyFloated += _moneyPerSecond;

            // Reset the moneys to max cap when re entering the game
            if (_currentMoneyFloated > MaximumMoney)
            {
                _currentMoneyFloated = MaximumMoney;
                _maxMoneyReached = true;
            }

            _data.Money = Mathf.FloorToInt(_currentMoneyFloated);
            _currentSecond++;

            _timeFromLastMoneyAdding = 0;
        }
    }

    private void OnMouseDown()
    {
        _lastPointerPosition = Input.mousePosition;
    }

    private void OnMouseUp()
    {
        if (!PointerOverUIChecker.Instance.IsPointerOverUIObject() && !_isPointerMoving && !GridBuildingSystem.Instance.TempPlaceableObject)
        {
            GetMoneyIfAvaliable();
        }
    }

    private void OnMouseDrag()
    {
        Vector3 delta = Input.mousePosition - _lastPointerPosition;

        if (delta.magnitude > 15f)
        {
            _isPointerMoving = true;
        }
        else
        {
            _isPointerMoving = false;
        }
    }
    public void Initialise(MoneyObjectData data)
    {
        _data = data;

        InitMoneyPerSecond();
        PlaceableObject placeableObject = GetComponentInParent<PlaceableObject>();
        if (_paddock != null && _paddock.is_hatching || _paddock != null && _paddock.hatching_completed || placeableObject != null && !placeableObject.ConstructionFinished)
        {
            _data.Money=0;
            return;
        }

        DateTime lastSaveTime = Attributes.GetAttribute("LastSaveTime", DateTime.Now);
        TimeSpan timePassed = DateTime.Now - lastSaveTime;
        int secondsPassed = (int)timePassed.TotalSeconds;

        _data.Money += Mathf.FloorToInt(_moneyPerSecond * secondsPassed);

        if (_data.Money > MaximumMoney)
        {
            if (_paddock != null && _paddock.is_hatching || _paddock != null && _paddock.hatching_completed)
            {
                _data.Money=0;
                return;
            }
            _data.Money = Mathf.FloorToInt(MaximumMoney);
        }

        _currentMoneyFloated = _data.Money;
        if (_data.Money >= MaximumMoney)
        {
            if (_paddock != null && _paddock.is_hatching || _paddock != null && _paddock.hatching_completed)
            {
                _data.Money=0;
                return;
            }
            _maxMoneyReached = true;
            _notification.SetActive(true);
            bool isBabyActive = _dinosaurFeedingSystem.babyModel.activeSelf;
            bool isAdultActive = _dinosaurFeedingSystem.adultModel.activeSelf;
            if (isBabyActive && _BabyAnimator != null)
                _BabyAnimator.SetTrigger("MaxMoneyReached");
            else if (isAdultActive && _AdultAnimator != null)
                _AdultAnimator.SetTrigger("MaxMoneyReached");
        }
    }
    public void InitData(int placeableIndex)
    {
        _data = new MoneyObjectData(0, placeableIndex);
        SaveManager.Instance.SaveData.MoneyData.Add(_data);
    }
    public void InitMoneyPerSecond()
    {
        _maximumSeconds = MaximumMinutes * 60;
        _moneyPerSecond = MaximumMoney / _maximumSeconds;
    }

    public void GetMoneyIfAvaliable()
    {
        if (!_moneyCounter.activeInHierarchy && !GridBuildingSystem.Instance.TempPlaceableObject)
        {
            if (_selectable)
            {
                _selectable.Select();
                if (_data.Money != 0)
                {
                    GetMoney();
                }
                else
                {
                    return;
                }
            }
        }
        else
        {
            if (_selectable)
            {
                _selectable.Select();
            }
        }
    }

    private void GetMoneyIfAvaliableByButton()
    {
        if (_data.Money != 0 && _selectable.IsSelected && !GridBuildingSystem.Instance.TempPlaceableObject)
        {
            if (_selectable)
            {
                _selectable.Select();
            }

            GetMoney();
        }
    }

    private void GetMoney()
    {
        _notification.SetActive(false);
        _tapVFX.SetActive(true);
        _moneyCounter.SetActive(true);
        _moneyCountDisplayer.DisplayCount(_data.Money);

        // Add coins
        EventManager.Instance.TriggerEvent(new CurrencyChangeGameEvent(_data.Money, CurrencyType.Coins));

        _currentMoneyFloated = 0;
        _data.Money = 0;
        _maxMoneyReached = false;
        _selectable.PlaySound(_selectable.Sounds[0]);
    }
}