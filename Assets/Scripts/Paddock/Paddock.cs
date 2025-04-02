using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Paddock : Selectable
{
    [SerializeField] private GameObject _evolutionsChanger;
    [SerializeField] private AnimationEventsListener _BabyAnimationEventsListener;
    [SerializeField] private AnimationEventsListener _AdultAnimationEventsListener;
    [SerializeField] private FoodType _foodType;
    [SerializeField] private DinoNumber _dinoNumber;
    [SerializeField] public HatchingTimer hatchingScript;

    private MoneyObject _moneyObject;
    private Animator _BabyDinosaurAnimator;
    private Animator _AdultDinosaurAnimator;

    private FeedButton _feedButton;
    private static Paddock _selectedPaddock = null;
    private DinosaurFeedingSystem _dinosaurFeedingSystem;

    public static Paddock SelectedPaddock => _selectedPaddock;

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

    private void Start()
    {
        _evolutionsChanger.SetActive(IsSelected);
        _BabyDinosaurAnimator = _BabyAnimationEventsListener.GetComponent<Animator>();
        _AdultDinosaurAnimator = _AdultAnimationEventsListener.GetComponent<Animator>();
        _dinosaurFeedingSystem = GetComponentInChildren<DinosaurFeedingSystem>();
    }

    private void OnFeedButtonClick()
    {
        if (_dinosaurFeedingSystem == null)
        {
            Debug.LogWarning("No DinosaurFeedingSystem found in this paddock.");
            return;
        }

        bool isBabyActive = _dinosaurFeedingSystem.babyModel.activeSelf;
        bool isAdultActive = _dinosaurFeedingSystem.adultModel.activeSelf;
        int currentLevel = _dinosaurFeedingSystem.levelManager.CurrentLevel;
        int feedCost = _dinosaurFeedingSystem.GetFeedCostForLevel(currentLevel);

        if (_dinosaurFeedingSystem.dinosaurDiet == Diet.Herbivore)
        {
            if (!CurrencySystem.Instance.HasEnoughCurrency(CurrencyType.Crops, feedCost))
            {
                return;
            }
        }
        else if (_dinosaurFeedingSystem.dinosaurDiet == Diet.Carnivore)
        {
            if (!CurrencySystem.Instance.HasEnoughCurrency(CurrencyType.Meat, feedCost))
            {
                return;
            }
        }

        if (isBabyActive && _BabyAnimationEventsListener.IsEatAnimationEnded && _selectedPaddock == this && !_moneyObject._maxMoneyReached)
        {
            StopSound(Sounds[2]);
            _BabyDinosaurAnimator.SetTrigger("Eat");
            _BabyAnimationEventsListener.OnEatAnimationStarted();
            PlaySound(Sounds[3], 0.5f);
        }
        else if (isAdultActive && _AdultAnimationEventsListener.IsEatAnimationEnded && _selectedPaddock == this && !_moneyObject._maxMoneyReached)
        {
            StopSound(Sounds[5]);
            _AdultDinosaurAnimator.SetTrigger("Eat");
            _AdultAnimationEventsListener.OnEatAnimationStarted();
            PlaySound(Sounds[6], 0.5f);
        }
    }

    private void Update()
    {
        if (!is_hatching && _dinosaurFeedingSystem.babyModel.activeSelf ||_dinosaurFeedingSystem.adultModel.activeSelf )
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

            bool isBabyActive = _dinosaurFeedingSystem.babyModel.activeSelf;
            bool isAdultActive = _dinosaurFeedingSystem.adultModel.activeSelf;

            if (isBabyActive && _BabyAnimationEventsListener.IsAnimationEnded && _moneyObject.CurrentMoneyInteger != 0)
            {
                StopSound(Sounds[3]);
                _BabyDinosaurAnimator.SetTrigger("Fun");
                _BabyAnimationEventsListener.OnAnimationStarted();
                PlaySound(Sounds[2], 0.5f);
            }
            else if (isAdultActive && _AdultAnimationEventsListener.IsAnimationEnded && _moneyObject.CurrentMoneyInteger != 0)
            {
                StopSound(Sounds[6]);
                _AdultDinosaurAnimator.SetTrigger("Fun");
                _AdultAnimationEventsListener.OnAnimationStarted();
                PlaySound(Sounds[5], 0.5f);
            }
            else if (_moneyObject.CurrentMoneyInteger == 0)
            {
                if (!isAlreadySelected)
                {
                    PlaySound(Sounds[4]);
                }
            }
            // Set the current dinosaur feeding system in the UI manager.
            DinosaurFeedingSystem dinoFeedSys = GetComponentInChildren<DinosaurFeedingSystem>();
            if (dinoFeedSys != null && DinosaurFeedingUIManager.Instance != null)
            {
                DinosaurFeedingUIManager.Instance.SetSelectedDinosaur(dinoFeedSys);
            }
            else
            {
                Debug.LogWarning("No DinosaurFeedingSystem found in this paddock or UI Manager not available.");
            }
        }
        _selectedPaddock = this;
        Debug.Log("Paddock selected: " + gameObject.name);
    }

    public override void Unselect()
    {
        base.Unselect();
        DinosaurFeedingUIManager.Instance.DisableEvolutionButton();
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

