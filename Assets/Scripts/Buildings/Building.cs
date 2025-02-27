using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Building : Selectable
{
    [SerializeField] private Animator _selectedAnimator;
    [SerializeField] private Animator _selectedConstructionAnimator;

    public override void Select()
    {
        base.Select();

        _selectedAnimator.SetBool("FadeInOut", true);
        _selectedConstructionAnimator.SetBool("FadeInOut", true);
        PlaySound(Sounds[2]);
    }

    public override void Unselect()
    {
        base.Unselect();

        _selectedAnimator.SetBool("FadeInOut", false);
        _selectedConstructionAnimator.SetBool("FadeInOut", false);
    }
}
