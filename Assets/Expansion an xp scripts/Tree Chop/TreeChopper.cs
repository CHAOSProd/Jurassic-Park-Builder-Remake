using System;
using UnityEditor.Build.Reporting;
using UnityEngine;

// Handles tree chopping mechanics and visual changes
public class TreeChopper : Selectable
{
    public BoundsInt Area;

    [Header("Display Objects")]
    [SerializeField] private GameObject _trees;
    [SerializeField] private GameObject _debris;
    [SerializeField] private GameObject _selected;

    [Header("XP Objects")]
    [SerializeField] private GameObject _xpNotification;
    [SerializeField] private GameObject _tapVFX;
    [SerializeField] private GameObject _xpCounter;
    [SerializeField] private MoneyCountDisplayer _xpCountDisplayer;

    [Header("Selection")]
    [SerializeField] private bool _selectableFromBeginning;

    [Header("Chopping Time")]
    [SerializeField] private GameObject _timerBarPrefab;
    [SerializeField] private int _chopTime;

    [Header("Audio")]
    [SerializeField] private AudioClip _xpCollectSound; // XP collect sound
    private AudioSource _audioSource;

    public bool AllowSelection { get; private set; }
    public (int x, int y) MappedPosition { get; private set; }

    public bool hasTreeDebris = false;
    private bool chopped = false;

    private TreeData _treeData;

    private TimerBar _timerBarInstance;

    private void Awake()
    {
        AllowSelection = _selectableFromBeginning;
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            // Add an AudioSource component if not present
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    private void Start()
    {
        _trees.SetActive(true);
        _debris.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (chopped || PointerOverUIChecker.Instance.IsPointerOverUIObject() || GridBuildingSystem.Instance.TempPlaceableObject)
            return;

        if (hasTreeDebris)
        {
            CollectDebris();
            return;
        }

        if (AllowSelection && SelectablesManager.Instance.CurrentSelectable != this)
        {
            TreeChopManager.Instance.SetExpansionCostText();
            Select();
        }
    }

    public void PerformChopAction()
    {
        AllowSelection = false;
        _trees.SetActive(false);
        _debris.SetActive(true);

        _timerBarInstance = Instantiate(_timerBarPrefab, transform).GetComponent<TimerBar>();
        _timerBarInstance.transform.position = _debris.transform.position;

        //Initialize Progress
        _treeData.Progress = new ProgressData(0, DateTime.Now);
        _timerBarInstance.FillOverInterval(_chopTime, 1, UpdateProgress, EnableDebris);
    }
    public void EnableDebris()
    {
        _trees.SetActive(false);
        _debris.SetActive(true);
        _xpNotification.SetActive(true);
        hasTreeDebris = true;
        _treeData.HasDebris = true;

        if (_timerBarInstance != null)
            Destroy(_timerBarInstance.gameObject);

        _timerBarInstance = null;

        _treeData.Progress = null;
    }
    private void CollectDebris()
    {
        hasTreeDebris = false;
        _treeData.HasDebris = false;
        _xpNotification.SetActive(false);
        _tapVFX.SetActive(true);
        _xpCounter.SetActive(true);
        _xpCountDisplayer.DisplayCount(TreeChopManager.Instance.CurrentXP);

        // Play XP collect sound
        if (_xpCollectSound != null)
        {
            _audioSource.PlayOneShot(_xpCollectSound);
        }

        EventManager.Instance.TriggerEvent(new XPAddedGameEvent(TreeChopManager.Instance.CurrentXP));
        TreeChopManager.Instance.UpdateXP();
        _debris.SetActive(false);

        _treeData.Chopped = true;
        TreeChopManager.Instance.UnlockAdjacentTrees(this);

        BoundsInt tempArea = Area;
        Vector3Int positionInt = GridBuildingSystem.Instance.GridLayout.LocalToCell(transform.position);
        tempArea.position = new Vector3Int(tempArea.position.x + positionInt.x, tempArea.position.y + positionInt.y, 0);
        GridBuildingSystem.Instance.SetAreaWhite(tempArea, GridBuildingSystem.Instance.MainTilemap);

        DebrisManager.Instance.SpawnDebris(this);

        chopped = true;
        Destroy(gameObject, .5f);
    }

    public override void Unselect()
    {
        _selected.SetActive(false);
        base.Unselect();
    }
    public override void Select()
    {
        _selected.SetActive(true);
        PlaySound(0);
        base.Select();
    }

    public void Unlock()
    {
        AllowSelection = true;
        _treeData.Selectable = true;
    }
    public void SetData(TreeData td)
    {
        _treeData = td;
        InitializeProgress();

        if (_treeData.HasDebris)
        {
            EnableDebris();
        }
        else if (_selectableFromBeginning)
        {
            AllowSelection = true;
            _treeData.Selectable = true;
            return;
        }

        AllowSelection = td.Selectable;
    }
    private void InitializeProgress()
    {
        if (_treeData.Progress != null)
        {
            AllowSelection = false;

            _trees.SetActive(false);
            _debris.SetActive(true);

            int newTime = (int)Math.Floor((DateTime.Now - _treeData.Progress.LastTick).TotalSeconds) + _treeData.Progress.ElapsedTime;
            if (newTime >= _chopTime)
            {
                EnableDebris();
            }
            else
            {
                _treeData.Progress.LastTick = DateTime.Now;
                _treeData.Progress.ElapsedTime = newTime;

                _timerBarInstance = Instantiate(_timerBarPrefab, transform).GetComponent<TimerBar>();
                _timerBarInstance.transform.position = _debris.transform.position;

                //Update Progress every second and display xp icon when construction is finished
                _timerBarInstance.FillOverInterval(_chopTime, 1, UpdateProgress, EnableDebris, newTime);
            }
        }
    }
    public TreeData GetTreeData()
    {
        return _treeData;
    }
    private void UpdateProgress()
    {
        _treeData.Progress.ElapsedTime += 1;
        _treeData.Progress.LastTick = DateTime.Now;
    }
    public void SetMappedPosition(int x, int y)
    {
        MappedPosition = (x, y);
    }
}
