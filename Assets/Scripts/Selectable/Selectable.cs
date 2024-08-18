using UnityEngine;

public class Selectable : SoundObject
{
    public bool IsSelected;

    [SerializeField] private FadeInOut _objectsFadeInOut;

    public virtual void Select()
    {
        SelectablesManager.Instance.UnselectAll();

        IsSelected = true;

        _objectsFadeInOut.SetFade(IsSelected);

        SelectablesManager.Instance.SetIsSomethingSelected(IsSelected);
    }

    public virtual void Unselect()
    {
        IsSelected = false;

        _objectsFadeInOut.SetFade(IsSelected);

        SelectablesManager.Instance.SetIsSomethingSelected(IsSelected);
    }

    public void PlayPlacementSound()
    {
        PlaySound(Sounds[1]);
    }
}
