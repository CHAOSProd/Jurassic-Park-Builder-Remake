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

    public float MaximumMoney = 100;
    public int CurrentMoneyInteger = 0;
    public int MaximumMinutes = 5;

    private Selectable _selectable;
    private Paddock _paddock;
    private float _maximumSeconds;
    private int _currentSecond;
    private float _currentMoneyFloated;
    private float _timeFromLastMoneyAdding;
    private bool _isPointerMoving;
    private Vector3 _lastPointerPosition;
    private bool _maxMoneyReachedPreviously;  // To track if max money was reached before

    private void Awake() //Havik changed from start to awake to not miss any references
    {
        _collectMoneyDisplay = CollectMoneyDisplay.Instance;
        _collectMoneyButton = CollectMoneyButton.Instance.GetComponent<Button>();

        _maximumSeconds = MaximumMinutes * 60;
        _selectable = GetComponentInParent<Selectable>();
        _animator = GetComponent<Animator>();
        if (GetComponent<Paddock>())
            _paddock = GetComponent<Paddock>();

        if (Attributes.HaveKey("CurrentMoney" + gameObject.name))
        {
            CurrentMoneyInteger = Attributes.GetInt("CurrentMoney" + gameObject.name);
        }
        else
        {
            CurrentMoneyInteger = 0;
        }

        InitializeMoneyPerSecond();

        DateTime lastSaveTime = Attributes.GetAttribute("LastSaveTime", DateTime.UtcNow);
        TimeSpan timePassed = DateTime.UtcNow - lastSaveTime;
        int secondsPassed = (int)timePassed.TotalSeconds;

        Debug.Log(lastSaveTime);
        Debug.Log(timePassed);

        CurrentMoneyInteger += Mathf.FloorToInt(_moneyPerSecond * secondsPassed);


        _currentMoneyFloated = CurrentMoneyInteger;


        _collectMoneyButton.onClick.AddListener(GetMoneyIfAvaliableByButton);

        Debug.Log("MoneyObject initialized!");
    }

    private void Update()
    {
        if (CurrentMoneyInteger >= MaximumMoney)
        {
            _currentMoneyFloated = MaximumMoney;
            CurrentMoneyInteger = Mathf.FloorToInt(_currentMoneyFloated);
            _notification.SetActive(true);
            return;
        }

        _maxMoneyReachedPreviously = false;  // Reset this flag when money is below max

        if (_selectable.IsSelected)
        {
            _collectMoneyDisplay.Display(CurrentMoneyInteger);
        }

        _timeFromLastMoneyAdding += Time.deltaTime;

        if (_timeFromLastMoneyAdding >= 1)
        {
            _currentMoneyFloated += _moneyPerSecond;
            CurrentMoneyInteger = Mathf.FloorToInt(_currentMoneyFloated);
            Attributes.SetInt("CurrentMoney" + gameObject.name, CurrentMoneyInteger);
            Attributes.SetAttribute("LastSaveTime", DateTime.UtcNow);
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

    public void InitializeMoneyPerSecond()
    {
        _moneyPerSecond = MaximumMoney / _maximumSeconds;
    }

    public void GetMoneyIfAvaliable()
    {
        if (CurrentMoneyInteger != 0 && !_moneyCounter.activeInHierarchy && !GridBuildingSystem.Instance.TempPlaceableObject)
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
        if (CurrentMoneyInteger != 0 && _selectable.IsSelected && !GridBuildingSystem.Instance.TempPlaceableObject)
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
        Debug.Log("Getting money...");
        _notification.SetActive(false);
        _tapVFX.SetActive(true);
        _moneyCounter.SetActive(true);
        _moneyCountDisplayer.DisplayCount(CurrentMoneyInteger);

        // Add coins
        EventManager.Instance.TriggerEvent(new CurrencyChangeGameEvent(CurrentMoneyInteger, CurrencyType.Coins));

        _currentMoneyFloated = 0;
        CurrentMoneyInteger = 0;
        _selectable.PlaySound(_selectable.Sounds[0]);
        Debug.Log("Money added successfully!");
    }


}





