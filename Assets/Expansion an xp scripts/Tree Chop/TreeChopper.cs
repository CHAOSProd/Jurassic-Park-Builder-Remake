using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.Build.Reporting;
#endif

// Handles tree chopping mechanics and visual changes
public class TreeChopper : Selectable
{
    public BoundsInt Area;

    [Header("Display Objects")]
    [SerializeField] private GameObject _trees;
    [SerializeField] private GameObject _debris;
    [SerializeField] private GameObject _selected;
    [SerializeField] private GameObject _selectedDebris;

    [Header("XP Objects")]
    [SerializeField] private GameObject _xpNotification;
    [SerializeField] private GameObject _tapVFX;
    [SerializeField] private GameObject _xpCounter;
    [SerializeField] private MoneyCountDisplayer _xpCountDisplayer;

    [Header("Selection")]
    [SerializeField] private bool _selectableFromBeginning;

    [Header("Chopping Time")]
    [SerializeField] private GameObject _timerBarPrefab;
    public int chopTime;
    private bool isProgressUpdated = false;

    [Header("Audio")]
    [SerializeField] private AudioClip _xpCollectSound; // XP collect sound
    private AudioSource _audioSource;

    public bool AllowSelection { get; private set; }
    public (int x, int y) MappedPosition { get; private set; }

    private Vector3 _lastPointerPosition;

    public bool hasTreeDebris = false;
    public bool canSpeedUp = false;
    private bool chopped = false;
    private bool _isPointerMoving;

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

    // Removed Start() default initialization so that saved state (such as debris active)
    // is not overwritten when the scene is loaded.
    // If needed, you can uncomment the following code and add a check for _treeData.
    /*
    private void Start()
    {
        if (_treeData == null)
        {
            _trees.SetActive(true);
            _debris.SetActive(false);
        }
        else
        {
            if (_treeData.HasDebris)
            {
                _trees.SetActive(false);
                _debris.SetActive(true);
            }
            else
            {
                _trees.SetActive(true);
                _debris.SetActive(false);
            }
        }
    }
    */

    private void OnMouseDrag()
    {
        Vector3 delta = Input.mousePosition - _lastPointerPosition;
        _isPointerMoving = delta.magnitude > 15f;
    }

    private void OnMouseDown()
    {
        _lastPointerPosition = Input.mousePosition;
    }

    private void OnMouseUp()
    {
        if (chopped || PointerOverUIChecker.Instance.IsPointerOverUIObject() ||
            GridBuildingSystem.Instance.TempPlaceableObject || _isPointerMoving)
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
        canSpeedUp = true;
        _trees.SetActive(false);
        _debris.SetActive(true);

        _timerBarInstance = Instantiate(_timerBarPrefab, transform).GetComponent<TimerBar>();
        _timerBarInstance.transform.position = _debris.transform.position;

        _treeData.ChopTime = chopTime;
        _treeData.Progress = new ProgressData(0, DateTime.Now);
        _timerBarInstance.FillOverInterval(chopTime, 1, UpdateProgress, EnableDebris);
    }

    public void EnableDebris()
    {
        AllowSelection = true;
        _trees.SetActive(false);
        _debris.SetActive(true);
        _xpNotification.SetActive(true);
        hasTreeDebris = true;
        _treeData.HasDebris = true;
        canSpeedUp = true;
        if (SelectablesManager.Instance.CurrentSelectable == this)
        {
            SelectablesManager.Instance.UnselectAll();
        }

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
        canSpeedUp = false;
        SelectablesManager.Instance.UnselectAll();

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
        _selectedDebris.SetActive(false);
        _selected.SetActive(false);
        base.Unselect();
    }

    public override void Select()
    {
        if (canSpeedUp)
        {
            _selectedDebris.SetActive(true);
            _selected.SetActive(false);
        }
        else
        {
            _selected.SetActive(true);
            _selectedDebris.SetActive(true);
        }

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
        chopTime = td.ChopTime;
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
            canSpeedUp = true;

            _trees.SetActive(false);
            _debris.SetActive(true);

            int newTime = (int)Math.Floor((DateTime.Now - _treeData.Progress.LastTick).TotalSeconds) + _treeData.Progress.ElapsedTime;
            if (newTime >= chopTime)
            {
                EnableDebris();
            }
            else
            {
                _treeData.Progress.LastTick = DateTime.Now;
                _treeData.Progress.ElapsedTime = newTime;

                if (transform.Find("TimerBar(Clone)"))
                {
                    _timerBarInstance = transform.Find("TimerBar(Clone)").GetComponent<TimerBar>();
                }
                else
                {
                    _timerBarInstance = Instantiate(_timerBarPrefab, transform).GetComponent<TimerBar>();
                    _timerBarInstance.transform.position = _debris.transform.position;

                    _timerBarInstance.FillOverInterval(chopTime, 1, UpdateProgress, EnableDebris, newTime);
                }
            }
        }
    }

    public TreeData GetTreeData()
    {
        return _treeData;
    }

    private void UpdateProgress()
    {
        if (!isProgressUpdated)
        {
            _treeData.Progress.ElapsedTime += 1;
            _treeData.Progress.LastTick = DateTime.Now;
            isProgressUpdated = true;
        }
    }

    public void SetMappedPosition(int x, int y)
    {
        MappedPosition = (x, y);
    }
}

