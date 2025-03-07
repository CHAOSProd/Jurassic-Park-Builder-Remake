using System.Collections;
using UnityEngine;

public class Paddock : Selectable
{
    [SerializeField] private GameObject _evolutionsChanger;
    [SerializeField] private AnimationEventsListener _dinosaurAnimationEventsListener;
    [SerializeField] private FoodType _foodType;
    [SerializeField] private DinoNumber _dinoNumber;

    [SerializeField] public HatchingTimer hatchingScript;

    private MoneyObject _moneyObject;
    private Animator _dinosaurAnimator;

    private FeedButton _feedButton;
    private static Paddock _selectedPaddock = null;

    [HideInInspector] public bool is_hatching = false;
    [HideInInspector] public bool should_earn_money = false;
    [HideInInspector] public bool hatching_completed = false;

    private void Awake()
    {
        _moneyObject = GetComponent<MoneyObject>();
        _moneyObject.enabled = false;
        StartCoroutine(FindFeedButtonWhenActive());
    }

    private IEnumerator FindFeedButtonWhenActive()
    {
        while (_feedButton == null)
        {
            _feedButton = FindAnyObjectByType<FeedButton>();
            yield return null;
        }

        if (_feedButton == null)
        {
            Debug.LogError("FeedButton not found even after waiting.");
        }
        else
        {
            Debug.Log("FeedButton found!");
            _feedButton.OnClickEvent.AddListener(OnFeedButtonClick);
        }
    }

    private void OnFeedButtonClick()
    {
        if (_dinosaurAnimationEventsListener.IsEatAnimationEnded && _dinosaurAnimator != null && _selectedPaddock == this && _moneyObject._maxMoneyReached == false)
        {
            StopSound(Sounds[2]);
            _dinosaurAnimator.SetTrigger("Eat");
            _dinosaurAnimationEventsListener.OnEatAnimationStarted();
            PlaySound(Sounds[3], 0.5f);
        }
    }

    private void Start()
    {
        _evolutionsChanger.SetActive(IsSelected);
        _dinosaurAnimator = _dinosaurAnimationEventsListener.GetComponent<Animator>();
    }

    private void Update()
    {
        if(!is_hatching && _dinosaurAnimator.gameObject.activeSelf)
        {
            _moneyObject.enabled = true;
        }
        else
        {
            _moneyObject.enabled = false;
        }
    }

    public override void Select()
    {
        bool isAlreadySelected = (_selectedPaddock == this);
        if (is_hatching)
        {
            base.Select();
            if (!isAlreadySelected)
            {
                PlaySound(Sounds[4]);
            }
        }
        else if (hatching_completed)
        {
            PlaySound(Sounds[1], 0.5f);
            hatchingScript.OnPaddockClicked();
            hatching_completed = false;
        }
        else
        {
            _moneyObject.enabled = true;
            base.Select();
            _evolutionsChanger.SetActive(IsSelected);
            FeedButton.Instance.UpdateButton(_foodType);

            if (_dinosaurAnimationEventsListener.IsAnimationEnded && _moneyObject.CurrentMoneyInteger != 0)
            {
                StopSound(Sounds[3]);
                _dinosaurAnimator.SetTrigger("Fun");
                _dinosaurAnimationEventsListener.OnAnimationStarted();
                PlaySound(Sounds[2], 0.5f);
            }
            else if (_moneyObject.CurrentMoneyInteger == 0)
            {
                if (!isAlreadySelected)
                {
                    PlaySound(Sounds[4]);
                }
            }
        }
        _selectedPaddock = this;
    }

    public override void Unselect()
    {
        base.Unselect();
        _evolutionsChanger.SetActive(IsSelected);

        if (_selectedPaddock == this)
        {
            _selectedPaddock = null;
        }
    }
}

public enum FoodType
{
    Herbivore,
    Carnivore
}

public enum DinoNumber
{
    Single,
    Multiple
}