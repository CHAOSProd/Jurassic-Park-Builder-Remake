using UnityEngine;

public class Selectable : SoundObject
{
    public bool IsSelected = false;

    [SerializeField] private FadeInOut _objectsFadeInOut;
    public virtual void Select()
    {
        SelectablesManager.Instance.UnselectAll();

        IsSelected = true;

        _objectsFadeInOut.SetFade(IsSelected);

        SelectablesManager.Instance.SetSelected(this);
    }

    public virtual void Unselect()
    {
        IsSelected = false;

        _objectsFadeInOut.SetFade(IsSelected);
    }

    public void PlayPlacementSound()
    {
        PlaySound(Sounds[1]);
    }
    public void PlaySound(int index)
    {
        PlaySound(Sounds[index]);
    }
}
