using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebrisObject : Selectable
{
    [Header("Properties")]
    [SerializeField] private int _cost;
    [SerializeField] public int _removeTime; // In seconds
    [SerializeField] private int _xp;

    [Header("XP Objects")]
    [SerializeField] private GameObject _xpNotification;
    [SerializeField] private GameObject _tapVFX;
    [SerializeField] private GameObject _xpCounter;
    [SerializeField] private MoneyCountDisplayer _xpCountDisplayer;

    [Header("Selection")]
    [SerializeField] private GameObject _selectableObject;
    [SerializeField] private GameObject _debrisVisual;

    [Header("UI")]
    [SerializeField] private GameObject _timerBarPrefab;

    [Header("Audio")]
    [SerializeField] private AudioClip _xpCollectSound; // XP collect sound clip
    private AudioSource _audioSource; // Audio source for sound playback

    private TimerBar _timerBarInstance;
    public bool removing = false;
    private bool _removed = false;
    private BoundsInt _size;
    public DebrisData _data;
    private Vector3 _lastPointerPosition;
    private bool _xpCollected = false;
    private bool isProgressUpdated = false;
    private bool _isPointerMoving;
    public bool HasAmber { get; set; }
    private int _amberIndex;

    private void Awake()
    {
        _xpNotification.SetActive(false);
        _tapVFX.SetActive(false);
        _xpCounter.SetActive(false);
        _selectableObject.SetActive(false);
        _debrisVisual.SetActive(true);

        // Initialize the AudioSource component
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.playOnAwake = false;
    }

    private void OnMouseDown()
    {
        _lastPointerPosition = Input.mousePosition;
    }

    private void OnMouseDrag()
    {
        Vector3 delta = Input.mousePosition - _lastPointerPosition;

        if (delta.magnitude > 15f)
        {
            _isPointerMoving = true;
        }
        else
        {
            _isPointerMoving = false;
        }
    }

    private void OnMouseUp()
    {
        if (_xpCollected || SelectablesManager.Instance.CurrentSelectable == this || PointerOverUIChecker.Instance.IsPointerOverUIObject() || GridBuildingSystem.Instance.TempPlaceableObject || _isPointerMoving) return;

        _selectableObject.GetComponent<Animator>().enabled = true;

        if (_removed)
        {
            CollectDebris();
        }
        else
        {
            _selectableObject.SetActive(true);
            Select();
            DebrisManager.Instance.UpdateCoinText(_cost);
            PlaySound(0);
        }
    }

    public override void Unselect()
    {
        base.Unselect();
        _selectableObject.SetActive(false);
    }

    public void OnRemoveClick()
    {
        if (!EventManager.Instance.TriggerEvent(new CurrencyChangeGameEvent(-_cost, CurrencyType.Coins))) return;

        _timerBarInstance = Instantiate(_timerBarPrefab, transform).GetComponent<TimerBar>();
        _timerBarInstance.transform.position = _debrisVisual.transform.position + .25f * _debrisVisual.transform.lossyScale.y * Vector3.down;
        _timerBarInstance.transform.localScale = new Vector3(1f / transform.localScale.x, 1f / transform.localScale.y);

        _data.Progress = new ProgressData(0, DateTime.Now);
        _timerBarInstance.FillOverInterval(_removeTime, 1, UpdateProgress, OnRemovalComplete);

        removing = true;

        SelectablesManager.Instance.UnselectAll();

        UIManager.Instance.ChangeTo("DefaultUI");
        Unselect();
    }

    private void OnRemovalComplete()
    {
        if (SelectablesManager.Instance.CurrentSelectable == this)
        {
            SelectablesManager.Instance.UnselectAll();
        }
        _xpNotification.SetActive(true);
        removing = false;
        _removed = true;

        if (_timerBarInstance != null)
        {
            Destroy(_timerBarInstance.gameObject);
            _timerBarInstance = null;
        }

        _data.Progress = null;
        _data.Removed = true;
    }

    private void CollectDebris()
    {
        if (_xpCollected) return;
        _xpCollected = true;

        _xpNotification.SetActive(false);
        _tapVFX.SetActive(true);
        _xpCounter.SetActive(true);
        _xpCountDisplayer.DisplayCount(_xp);
        EventManager.Instance.TriggerEvent(new XPAddedGameEvent(_xp));

        if (HasAmber)
        {
            Debug.Log($"Amber collected with index: {_amberIndex}");
            AmberManager.Instance.OpenPanel();
        }

        // Play XP collect sound
        if (_xpCollectSound != null)
        {
            _audioSource.clip = _xpCollectSound;
            _audioSource.Play();
        }

        // Give GridBuild Area back
        GridBuildingSystem.Instance.SetAreaWhite(_size, GridBuildingSystem.Instance.MainTilemap);

        removing = true;

        if (!HasAmber)
        {
            PlaySound(1);         
        }

        _debrisVisual.SetActive(false);
        SaveManager.Instance.SaveData.DebrisData.Remove(_data);
        Destroy(gameObject, 1.578f);
    }

    private void UpdateProgress()
    {
        if (!isProgressUpdated)
        {
            _data.Progress.ElapsedTime += 1;
            _data.Progress.LastTick = DateTime.Now;
            isProgressUpdated = true;
        }
    }

    /// <summary>
    /// Calculates and returns the remaining time for the debris removal.
    /// </summary>
    /// <returns>Remaining time in seconds, or 0 if no progress data is available.</returns>
    //public int GetRemainingTime()
    //{
    //    if (_data?.Progress == null)
    //        return 0;

    //    // Calculate elapsed time
    //    int elapsedTime = (int)(DateTime.Now - _data.Progress.LastTick).TotalSeconds + _data.Progress.ElapsedTime;

    //    // Calculate remaining time
    //    return Mathf.Max(0, _removeTime - elapsedTime);
    //}

    public void Initialize(int size, DebrisType type, bool hasAmber)
    {
        Vector3Int positionInt = GridBuildingSystem.Instance.GridLayout.WorldToCell(transform.position);
        _size = new BoundsInt(positionInt - new Vector3Int(size >> 1, size >> 1), new Vector3Int(size, size, 1));
        GridBuildingSystem.Instance.TakeArea(_size);
        HasAmber = hasAmber;

        _data = new DebrisData(type, (transform.position.x, transform.position.y, transform.position.z), HasAmber);
        SaveManager.Instance.SaveData.DebrisData.Add(_data);

        if (HasAmber)
        {
            _amberIndex = AmberManager.Instance.GetAmberList().Count;
            AmberManager.Instance.AddAmber(_amberIndex);
            _data.AmberIndex = _amberIndex;
        }
    }

    public void Load(DebrisData d, int size)
    {
        _data = d;
        HasAmber = d.HasAmber;
        if (HasAmber)
        {
            _amberIndex = d.AmberIndex;
            Debug.Log($"Amber loaded -> Index: {_amberIndex}, Position: {_data.Position}");
        }
        if (_data.Progress != null)
        {
            int newTime = (int)Math.Floor((DateTime.Now - _data.Progress.LastTick).TotalSeconds) + _data.Progress.ElapsedTime;
            if (newTime >= _removeTime)
            {
                OnRemovalComplete();
            }
            else
            {
                _data.Progress.LastTick = DateTime.Now;
                _data.Progress.ElapsedTime = newTime;

                _timerBarInstance = Instantiate(_timerBarPrefab, transform).GetComponent<TimerBar>();
                _timerBarInstance.transform.position = _debrisVisual.transform.position + .25f * _debrisVisual.transform.lossyScale.y * Vector3.down;
                _timerBarInstance.transform.localScale = new Vector3(1f / transform.localScale.x, 1f / transform.localScale.y);

                // Update Progress every second and display XP icon when construction is finished
                _timerBarInstance.FillOverInterval(_removeTime, 1, UpdateProgress, OnRemovalComplete, newTime);
                removing = true;
            }
        }
        else if (d.Removed)
        {
            OnRemovalComplete();
        }

        Vector3Int positionInt = GridBuildingSystem.Instance.GridLayout.WorldToCell(transform.position);
        _size = new BoundsInt(positionInt - new Vector3Int(size >> 1, size >> 1), new Vector3Int(size, size, 1));
        GridBuildingSystem.Instance.TakeArea(_size);
    }
}


