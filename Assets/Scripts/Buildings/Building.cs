using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Building : Selectable
{
    [SerializeField] private Animator _selectedAnimator;

    public override void Select()
    {
        base.Select();

        _selectedAnimator.SetBool("FadeInOut", true);
    }

    public override void Unselect()
    {
        base.Unselect();

        _selectedAnimator.SetBool("FadeInOut", false);
    }
}
