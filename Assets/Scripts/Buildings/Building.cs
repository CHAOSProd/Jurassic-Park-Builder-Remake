using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Building : Selectable
{
    [SerializeField] private Animator _selectedAnimator;
    [SerializeField] private Animator _selectedConstructionAnimator;
    private static Building _selectedBuilding = null; 

    public override void Select()
    {
        bool isAlreadySelected = (_selectedBuilding == this);
        base.Select();
        _selectedAnimator.SetBool("FadeInOut", true);
        _selectedConstructionAnimator.SetBool("FadeInOut", true);
        MoneyObject moneyObject = GetComponentInChildren<MoneyObject>();
        if (moneyObject != null && moneyObject.CurrentMoneyInteger == 0)
        {
            if (!isAlreadySelected)
            {
                PlaySound(Sounds[2]);
            }
        }
        else if (moneyObject == null)
        {
            if (!isAlreadySelected)
            {
                PlaySound(Sounds[2]);
            }
        }
        _selectedBuilding = this;
    }

    public override void Unselect()
    {
        base.Unselect();

        _selectedAnimator.SetBool("FadeInOut", false);
        _selectedConstructionAnimator.SetBool("FadeInOut", false);

        if (_selectedBuilding == this)
        {
            _selectedBuilding = null;
        }
    }
}
