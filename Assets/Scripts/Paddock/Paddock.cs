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

    [HideInInspector] public bool is_hatching = false;
    [HideInInspector] public bool hatching_completed = false;

    private void Awake()
    {
        _moneyObject = GetComponent<MoneyObject>();
        _moneyObject.enabled = false;
    }

    private void Start()
    {
        _evolutionsChanger.SetActive(IsSelected);

        _dinosaurAnimator = _dinosaurAnimationEventsListener.GetComponent<Animator>();
    }

    public override void Select()
    {
        if (is_hatching)
        {
            base.Select();
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

            Debug.Log(_dinosaurAnimationEventsListener.IsAnimationEnded);

            if (_dinosaurAnimationEventsListener.IsAnimationEnded && _moneyObject.CurrentMoneyInteger != 0)
            {
                _dinosaurAnimator.SetTrigger("Fun");
                _dinosaurAnimationEventsListener.OnAnimationStarted();
                PlaySound(Sounds[2], 0.5f);
            }
        }
    }

    public override void Unselect()
    {
        base.Unselect();

        _evolutionsChanger.SetActive(IsSelected);
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