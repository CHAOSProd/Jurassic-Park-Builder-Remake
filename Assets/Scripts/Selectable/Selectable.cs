using UnityEngine;

public class Selectable : SoundObject
{
    public bool IsSelected;

    [SerializeField] private FadeInOut _objectsFadeInOut;
    private void Start()
    {
        Unselect();
    }
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

        SelectablesManager.Instance.SetSelected(this);
    }

    public void PlayPlacementSound()
    {
        PlaySound(Sounds[1]);
    }
}
