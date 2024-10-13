using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeedButton : Singleton<FeedButton>
{
    [Header("Herbivore Sprites")]
    [SerializeField] private Sprite _herbivore;
    [SerializeField] private Sprite _herbivoreClick;
    [SerializeField] private Sprite _herbivoreDisabled;

    [Header("Carnivore Sprites")]
    [SerializeField] private Sprite _carnivore;
    [SerializeField] private Sprite _carnivoreClick;
    [SerializeField] private Sprite _carnivoreDisabled;

    [Header("On Click")]
    [SerializeField] private Button.ButtonClickedEvent _onClickEvent;

    private Image _buttonImage;
    private FoodType _currentType;

    public void Awake()
    {
        _buttonImage = GetComponent<Image>();
    }

    public void UpdateButton(FoodType foodType)
    {
        switch(foodType)
        {
            case FoodType.Herbivore:
                _buttonImage.sprite = _herbivore;
                break;
            case FoodType.Carnivore:
                _buttonImage.sprite = _carnivore;
                break;
        }

        _currentType = foodType;
    }

    public void ClickImage()
    {
        switch (_currentType)
        {
            case FoodType.Herbivore:
                _buttonImage.sprite = _herbivoreClick;
                break;
            case FoodType.Carnivore:
                _buttonImage.sprite = _carnivoreClick;
                break;
        }

        _onClickEvent.Invoke();
    }

    public void DefaultImage()
    {
        switch (_currentType)
        {
            case FoodType.Herbivore:
                _buttonImage.sprite = _herbivore;
                break;
            case FoodType.Carnivore:
                _buttonImage.sprite = _carnivore;
                break;
        }
    }
    private void OnDisable()
    {
        switch (_currentType)
        {
            case FoodType.Herbivore:
                _buttonImage.sprite = _herbivoreDisabled;
                break;
            case FoodType.Carnivore:
                _buttonImage.sprite = _carnivoreDisabled;
                break;
        }
    }
    private void OnEnable()
    {
        switch (_currentType)
        {
            case FoodType.Herbivore:
                _buttonImage.sprite = _herbivore;
                break;
            case FoodType.Carnivore:
                _buttonImage.sprite = _carnivore;
                break;
        }
    }
}
