using UnityEngine;

public class VisitorCenter : Selectable
{
    public BoundsInt Area;

    [Header("Selection")]
    [SerializeField] private bool _selectableFromBeginning;
    [SerializeField] private GameObject _selected;
    [SerializeField] public GameObject ResearchButton;

    [Header("Audio")]
    [SerializeField] private AudioClip _selectSound;
    private AudioSource _audioSource;

    public bool AllowSelection { get; private set; }

    private void Awake()
    {
        AllowSelection = _selectableFromBeginning;

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnMouseUp()
    {
        if (!AllowSelection || PointerOverUIChecker.Instance.IsPointerOverUIObject() ||
            GridBuildingSystem.Instance.TempPlaceableObject)
            return;

        if (SelectablesManager.Instance.CurrentSelectable != this)
        {
            Select();
        }
    }

    public override void Select()
    {
        if (!AllowSelection) return;

        _selected.SetActive(true);
        ResearchButton.SetActive(true);

        if (_selectSound != null)
        {
            _audioSource.PlayOneShot(_selectSound);
        }

        base.Select();
    }

    public override void Unselect()
    {
        _selected.SetActive(false);
        base.Unselect();
        ResearchButton.SetActive(false);
    }

    public void Unlock()
    {
        AllowSelection = true;
    }
}
