using System;
using UnityEngine;
using UnityEngine.UI;

public class MoneyObject : MonoBehaviour
{
    [SerializeField] private GameObject _notification;
    [SerializeField] private GameObject _tapVFX;
    [SerializeField] private GameObject _moneyCounter;
    [SerializeField] private MoneyCountDisplayer _moneyCountDisplayer;
    [SerializeField] private float _moneyPerSecond = 0.33f;
    [SerializeField] private CollectMoneyDisplay _collectMoneyDisplay;
    [SerializeField] private Button _collectMoneyButton;
    [SerializeField] private Animator _animator;  // Reference to the Animator

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
    private bool _maxMoneyReached;

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
        if (_maxMoneyReached) return;

        if (_data.Money >= MaximumMoney && !_maxMoneyReached)
        {
            _currentMoneyFloated = MaximumMoney;
            _data.Money = Mathf.FloorToInt(_currentMoneyFloated);
            _notification.SetActive(true);

            if (_animator != null)
                _animator.SetTrigger("MaxMoneyReached");

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
        this._data = data;

        InitMoneyPerSecond();


        DateTime lastSaveTime = Attributes.GetAttribute("LastSaveTime", DateTime.Now);
        TimeSpan timePassed = DateTime.Now - lastSaveTime;
        int secondsPassed = (int)timePassed.TotalSeconds;

        _data.Money += Mathf.FloorToInt(_moneyPerSecond * secondsPassed);

        if (_data.Money > MaximumMoney)
        {
            _data.Money = Mathf.FloorToInt(MaximumMoney);
        }

        _currentMoneyFloated = _data.Money;
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
        if (_data.Money != 0 && !_moneyCounter.activeInHierarchy && !GridBuildingSystem.Instance.TempPlaceableObject)
        {
            if (_selectable)
            {
                _selectable.Select();
            }

            GetMoney();
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