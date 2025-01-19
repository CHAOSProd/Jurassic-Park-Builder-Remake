using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Rendering;
using static UnityEngine.GraphicsBuffer;
using UnityEditor;

public class PlaceableObject : MonoBehaviour
{
    public bool Placed;

    public BoundsInt Area;
    public int GridBuildingID;
    public bool ConstructionFinished = false;
    public int BuildTime; // time to build in SECONDS
    public int BuildXp;

    [SerializeField] private GameObject _xpNotification;
    [SerializeField] private GameObject _tapVFX;
    [SerializeField] private GameObject _xpCounter;
    [SerializeField] private MoneyCountDisplayer _xpCountDisplayer;
    [SerializeField] private AudioClip _xpCollectSound;
    [SerializeField] private AudioClip _constructionFinishedSound;


    private AudioSource _audioSource;

    [ReadOnly()] public PlaceableObjectData data = new PlaceableObjectData();

    [HideInInspector] public FadeInOut DisplayFadeInOut;

    [SerializeField] private GameObject _display;
    [SerializeField] private GameObject _construction;
    [SerializeField] private GameObject _main;

    [SerializeField] private GameObject _timerBarPrefab;

    private TimerBar _timerBarInstance;

    public GameObject Display
    {
        get { return _display; }
        set { _display = value; }
    }

    private PlaceableObjectItem _placeableObjectItem;
    [SerializeField] private Selectable _selectable;
    public Button _editButton;
    private Vector3 _origin;

    [Header("Dinos")]

    [HideInInspector] public bool _isPaddock;
    [HideInInspector] public GameObject Hatching;
    [HideInInspector] public GameObject Dino;

    private bool _isPointerMoving;
    private Vector3 _lastPointerPosition;

    #region Unity Methods

    private void Awake()
    {
        DisplayFadeInOut = _construction.GetComponent<FadeInOut>();
        _audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Start()
    {
        _editButton = EditButton.Instance.GetComponent<Button>();
        _editButton.onClick.AddListener(StartEditing);
    }

    #endregion

    #region Build Methods

    public bool CanBePlaced()
    {
        Vector3Int positionInt = GridBuildingSystem.Instance.GridLayout.LocalToCell(transform.position);
        BoundsInt areaTemp = Area;
        areaTemp.position = positionInt;

        if (GridBuildingSystem.Instance.CanTakeArea(areaTemp))
        {
            return true;
        }

        return false;
    }

    public void Place()
    {
        InitializeDisplayObjects(false);

        Vector3Int positionInt = GridBuildingSystem.Instance.GridLayout.LocalToCell(transform.position);
        BoundsInt areaTemp = Area;
        areaTemp.position = positionInt;
        transform.position = GridBuildingSystem.Instance.GridLayout.CellToLocalInterpolated(positionInt);
        GridBuildingSystem.Instance.TakeArea(areaTemp);
        _origin = transform.position;
        data.Position = (transform.position.x, transform.position.y, transform.position.z);
        CameraObjectFollowing.Instance.SetTarget(null);

        if (Placed) return;

        _selectable.PlayPlacementSound();
        Placed = true;

        SaveManager.Instance.SaveData.PlaceableObjects.Add(data);

        GetComponentInChildren<MoneyObject>(true).InitData(SaveManager.Instance.SaveData.PlaceableObjects.Count - 1);
        Debug.Log("Data initialised");

        data.Progress = new ProgressData(0, DateTime.Now);
        _timerBarInstance = Instantiate(_timerBarPrefab, transform).GetComponent<TimerBar>();
        _timerBarInstance.transform.position = _construction.transform.position;
        _timerBarInstance.transform.localScale = new Vector3(1f / transform.localScale.x, 1f / transform.localScale.y);

        // Update Progress every second and display XP icon when construction is finished
        _timerBarInstance.FillOverInterval(BuildTime, 1, UpdateProgress, OnConstructionFinished);
    }

    public void PlaceWithoutSave()
    {
        InitializeDisplayObjects(false);

        Vector3Int positionInt = GridBuildingSystem.Instance.GridLayout.LocalToCell(transform.position);
        BoundsInt areaTemp = Area;
        areaTemp.position = positionInt;
        Placed = true;

        transform.position = GridBuildingSystem.Instance.GridLayout.CellToLocalInterpolated(positionInt);

        GridBuildingSystem.Instance.TakeArea(areaTemp);

        _origin = transform.position;

        CameraObjectFollowing.Instance.SetTarget(null);
    }

    private void OnConstructionFinished()
    {
        _xpNotification.SetActive(true);
        if (_timerBarInstance != null)
        {
            Destroy(_timerBarInstance.gameObject);
            _timerBarInstance = null;
        }

        if (_selectable.IsSelected)
        {
            SelectablesManager.Instance.UnselectAll();
        }
    }

    private void UpdateProgress()
    {
        this.data.Progress.ElapsedTime += 1;
        this.data.Progress.LastTick = DateTime.Now;
    }

    #endregion

    #region Initialization Methods

    public void InitializeDisplayObjects(bool isBuildingEnabled)
    {
        if (!ConstructionFinished)
        {
            _construction.SetActive(true);

            // Ensure collider is correctly enabled/disabled based on whether the object can be edited
            Collider2D collider = _construction.GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = isBuildingEnabled; // Enable collider only if editing is allowed
            }
        }
        else
        {
            _main.SetActive(!isBuildingEnabled);
            _display.SetActive(isBuildingEnabled);
            _construction.SetActive(false);

            // If the object is finished, ensure the main display is interactable
            if (TryGetComponent(out Collider2D collider))
            {
                Destroy(collider);
            }

            DisplayFadeInOut = _display.GetComponent<FadeInOut>();
        }
    }



    public void InitializeConstructedBuilding()
    {
        Debug.Log("Constructed the building!");

        if (_isPaddock)
        {
            Hatching.SetActive(true);
            Dino.SetActive(false);
        }

        _xpNotification.SetActive(false);
        _tapVFX.SetActive(true);
        _xpCounter.SetActive(true);
        _xpCountDisplayer.DisplayCount(BuildXp);
        EventManager.Instance.TriggerEvent(new XPAddedGameEvent(BuildXp));
        _construction.SetActive(false);
        _main.SetActive(true);

        // Play both sounds at the same time
        if (_xpCollectSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(_xpCollectSound);
        }

        if (_constructionFinishedSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(_constructionFinishedSound);
        }
    }

    public void Initialize(PlaceableObjectItem placeableObjectItem)
    {
        _placeableObjectItem = placeableObjectItem;
        data.ItemName = placeableObjectItem.name;
    }

    public void Initialize(PlaceableObjectItem placeableObjectItem, PlaceableObjectData placeableObjectData)
    {
        _placeableObjectItem = placeableObjectItem;
        data = placeableObjectData;
        transform.position = new Vector3(data.Position.x, data.Position.y, data.Position.z);
        this.ConstructionFinished = placeableObjectData.ConstructionFinished;

        if (ConstructionFinished)
        {
            _xpNotification.SetActive(false);
            _tapVFX.SetActive(false);
            _xpCounter.SetActive(false);
            _construction.SetActive(false);
            _main.SetActive(true);
        }
        else if (data.Progress != null)
        {
            int newTime = (int)Math.Floor((DateTime.Now - placeableObjectData.Progress.LastTick).TotalSeconds) + placeableObjectData.Progress.ElapsedTime;

            if (newTime >= BuildTime)
            {
                OnConstructionFinished();
            }
            else
            {
                data.Progress = new ProgressData(BuildTime - newTime, DateTime.Now);

                _timerBarInstance = Instantiate(_timerBarPrefab, transform).GetComponent<TimerBar>();
                _timerBarInstance.transform.position = _construction.transform.position;
                _timerBarInstance.transform.localScale = new Vector3(1f / transform.localScale.x, 1f / transform.localScale.y);

                // Update Progress every second and display XP icon when construction is finished
                _timerBarInstance.FillOverInterval(BuildTime, 1, UpdateProgress, OnConstructionFinished, newTime);
            }
        }
    }

    #endregion

    #region Editing Mode

public void StartEditing()
{
    Debug.Log("StartEditing triggered");
    Animator mainObjectAnimator = GetComponentInChildren<Animator>();
    if (mainObjectAnimator != null)
    {
        mainObjectAnimator.enabled = false;
    }
    Transform dinoTransform = Dino?.transform;
    Vector3 dinoOriginalPosition = Vector3.zero;

    if (dinoTransform != null)
    {
        dinoOriginalPosition = dinoTransform.position;
        dinoTransform.SetParent(null);
    }

    Animator dinoAnimator = Dino?.GetComponentInChildren<Animator>();
    if (dinoAnimator != null)
    {
        dinoAnimator.enabled = true;
    }

    if (_selectable.IsSelected)
    {
        GridBuildingSystem.Instance.TempPlaceableObject = this;
        InitializeDisplayObjects(true);
        CameraObjectFollowing.Instance.SetTarget(transform);
        GridBuildingSystem.Instance.TempTilemap.gameObject.SetActive(true);

        Vector3Int positionInt = GridBuildingSystem.Instance.GridLayout.WorldToCell(transform.position);
        BoundsInt areaTemp = Area;
        areaTemp.position = positionInt;

        GridBuildingSystem.Instance.SetAreaWhite(areaTemp, GridBuildingSystem.Instance.MainTilemap);

        GridBuildingSystem.Instance.FollowBuilding();
        GridBuildingSystem.Instance.ReloadUI();
        UIManager.Instance.DisableCurrentFixed();
    }
    else
    {
        Debug.LogWarning("StartEditing called, but object is not selected.");
    }

    if (mainObjectAnimator != null)
    {
        mainObjectAnimator.enabled = true;
    }

    if (dinoTransform != null)
    {
        dinoTransform.SetParent(transform);
        dinoTransform.position = dinoOriginalPosition;
    }
}

    public void CancelEditing()
    {
        transform.position = _origin;
        Place();
    }

    public void SortAtTop()
    {
        if (ConstructionFinished)
        {
            _display.GetComponent<SortingGroup>().sortingOrder = 2;
        }
        else
        {
            _construction.GetComponent<SortingGroup>().sortingOrder = 2;
        }
    }

    public void ResetSortingOrder()
    {
        if (ConstructionFinished)
        {
            _display.GetComponent<SortingGroup>().sortingOrder = 0;
        }
        else
        {
            _construction.GetComponent<SortingGroup>().sortingOrder = 0;
        }
    }

    #endregion

    #region Construction Methods

    private void OnMouseDown()
    {
        _lastPointerPosition = Input.mousePosition;
    }

    private void OnMouseUp()
    {
        if (!PointerOverUIChecker.Instance.IsPointerOverUIObject() &&
            !_isPointerMoving &&
            !GridBuildingSystem.Instance.TempPlaceableObject)
        {
            Debug.Log("OnMouseUp triggered");

            if (!ConstructionFinished)
            {
                Debug.Log("Object under construction");

                if (_xpNotification.activeSelf)
                {
                    // Collect XP if the notification is active
                    Debug.Log("Collecting XP during construction");
                    InitializeConstructedBuilding();
                    ConstructionFinished = true;
                    data.ConstructionFinished = true;
                    data.Progress = null;
                }
                else
                {
                    // Allow selection but do not collect XP
                    _selectable.Select();
                    Debug.Log("Selected object under construction");
                    UIManager.Instance.ChangeTo("BuildingsSelectedUI");
                }
            }
            else
            {
                Debug.Log("Object is already constructed");
                GetComponentInChildren<MoneyObject>().GetMoneyIfAvaliable();
                _selectable.Select();
            }
        }
    }



    #endregion
}

// This just makes it so that the Hatching and Dino variables appear when _isPaddock is true
[CustomEditor(typeof(PlaceableObject))]
public class PlaceableObjectEditor : Editor
{
    private SerializedProperty isPaddockProperty;
    private SerializedProperty hatchingProperty;
    private SerializedProperty dinoProperty;

    private void OnEnable()
    {
        isPaddockProperty = serializedObject.FindProperty("_isPaddock");
        hatchingProperty = serializedObject.FindProperty("Hatching");
        dinoProperty = serializedObject.FindProperty("Dino");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        EditorGUILayout.PropertyField(isPaddockProperty, new GUIContent("Is Paddock"));

        if (isPaddockProperty.boolValue)
        {
            EditorGUILayout.PropertyField(hatchingProperty, new GUIContent("Hatching"));
            EditorGUILayout.PropertyField(dinoProperty, new GUIContent("Dino"));
        }
        serializedObject.ApplyModifiedProperties();
    }
}


