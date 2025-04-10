using UnityEngine;
using System.Collections;

public class VisitorCenter : Selectable
{
    public BoundsInt Area;

    [Header("Selection")]
    [SerializeField] private bool _selectableFromBeginning;
    [SerializeField] private GameObject _selected;
    [SerializeField] public GameObject ResearchButton;

    [Header("Audio")]
    [SerializeField] private AudioClip _selectSound;
    [SerializeField] private AudioClip _visitorCenterSound;
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
        _audioSource.volume = 0.2f;
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

        if (_visitorCenterSound != null)
        {
            _audioSource.clip = _visitorCenterSound;
            _audioSource.loop = true;
            _audioSource.Play();
        }

        base.Select();
    }
    private Coroutine fadeOutCoroutine;

    private IEnumerator FadeOutSound(float duration)
    {
        float startVolume = _audioSource.volume;

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            _audioSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        _audioSource.Stop();
        _audioSource.volume = startVolume;
    }

    public override void Unselect()
    {
        _selected.SetActive(false);
        if (_audioSource.isPlaying)
        {
            if (fadeOutCoroutine != null)
                StopCoroutine(fadeOutCoroutine);

            fadeOutCoroutine = StartCoroutine(FadeOutSound(0.2f));
        }
        base.Unselect();
        ResearchButton.SetActive(false);
    }

    public void Unlock()
    {
        AllowSelection = true;
    }
}
