using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlaceableObject : MonoBehaviour
{
    public bool Placed;

    public BoundsInt Area;
    public int GridBuildingID;
    public bool PlacedFromBeginning = false;
    public bool isConstructed = false;
    public bool ConstructionFinished = false;
    public int BuildTime; //time to build in minutes(?)
    public int BuildXp;

    [SerializeField] private GameObject _xpNotification;
    [SerializeField] private GameObject _tapVFX;
    [SerializeField] private GameObject _xpCounter;
    [SerializeField] private MoneyCountDisplayer _xpCountDisplayer;



    [ReadOnly()] public PlaceableObjectData data = new PlaceableObjectData();

    [HideInInspector] public FadeInOut DisplayFadeInOut;

    [SerializeField] private GameObject _display;
    [SerializeField] private GameObject _construction;
    [SerializeField] private GameObject _main;



    public GameObject Display
    {
        get
        {
            return _display;
        }
        set
        {
            _display = value;
        }
    }

    private PlaceableObjectItem _placeableObjectItem;
    private Selectable _selectable;
    public Button _editButton;
    private Vector3 _origin;
    private bool _isEditing = false;
    private GameManager gameManager;
    
    #region Unity Methods

    private void Awake()
    {
        DisplayFadeInOut = _construction.GetComponent<FadeInOut>();
        gameManager = GameObject.FindObjectOfType<GameManager>();

    }

    private void Start()
    {
        //if (Placed && !PlacedFromBeginning)
        //{
        //    if (CanBePlaced())
        //    {
        //        Place();
        //    }
        //}

        if (PlacedFromBeginning && !PlayerPrefs.HasKey("IsDefaultObjectInitialized"))
        {
            PlaceableObjectItem defaultPlaceableObjectItem = Resources.Load<PlaceableObjectItem>("Placeables/TriceratopsItem");

            Initialize(defaultPlaceableObjectItem);

            if (CanBePlaced())
            {
                Place();
            }

            PlayerPrefs.SetInt("IsDefaultObjectInitialized", 1);
        }

        _editButton = FindObjectOfType<EditButton>(true).GetComponent<Button>();
        _editButton.onClick.AddListener(StartEditing);

        _selectable = GetComponent<Selectable>();
    }
    private void Update()
    {
        if (isConstructed)
        {
            _xpNotification.SetActive(!ConstructionFinished);
        }
    }

    #endregion

    #region Build Methods

    public bool CanBePlaced()
    {
        Vector3Int positionInt = GridBuildingSystem.Current.GridLayout.LocalToCell(transform.position);
        BoundsInt areaTemp = Area;
        areaTemp.position = positionInt;

        if (GridBuildingSystem.Current.CanTakeArea(areaTemp))
        {
            return true;
        }

        return false;
    }

    public void Place()
    {
        InitializeDisplayObjects(false);

        if (!Placed)
        GetComponent<Selectable>().PlayPlacementSound();

        Vector3Int positionInt = GridBuildingSystem.Current.GridLayout.LocalToCell(transform.position);
        BoundsInt areaTemp = Area;
        areaTemp.position = positionInt;
        Placed = true;

        transform.position = GridBuildingSystem.Current.GridLayout.CellToLocalInterpolated(positionInt);

        GridBuildingSystem.Current.TakeArea(areaTemp);
        SelectablesManager.Current.CheckForSelectables();

        _origin = transform.position;

        data.Position = transform.position;
        SaveManager.Current.SaveData.AddData(data);
        SaveManager.Current.SaveGame();

        CameraObjectFollowing.Current.SetTarget(null);
    }

    public void PlaceWithoutSave()
    {
        InitializeDisplayObjects(false);

        Vector3Int positionInt = GridBuildingSystem.Current.GridLayout.LocalToCell(transform.position);
        BoundsInt areaTemp = Area;
        areaTemp.position = positionInt;
        Placed = true;

        transform.position = GridBuildingSystem.Current.GridLayout.CellToLocalInterpolated(positionInt);

        GridBuildingSystem.Current.TakeArea(areaTemp);
        SelectablesManager.Current.CheckForSelectables();

        _origin = transform.position;

        CameraObjectFollowing.Current.SetTarget(null);
    }

    #endregion

    #region Initialization Methods

    public void InitializeDisplayObjects(bool isBuildingEnabled)
    {
       
        
        if (!isConstructed)
        {
            _construction.SetActive(true);
            if (!isBuildingEnabled)
            {
                _construction.GetComponent<Collider2D>().enabled = false;
            }
            
        }
        
        if (isConstructed)
        {
            _main.SetActive(!isBuildingEnabled);
            _display.SetActive(isBuildingEnabled);
            _construction.SetActive(false);
            if (TryGetComponent<Collider2D>(out Collider2D collider))
            {
                Destroy(collider);
            }
            DisplayFadeInOut = _display.GetComponent<FadeInOut>();
        }

    }
    public void InitializeConstructedBuilding()
    {
        Debug.Log("Constructed the building!");
        if (isConstructed && !ConstructionFinished)
        {
            GetXP(BuildXp);
            //play animations of xp and play sound effect
            gameManager.GetXP(BuildXp);
            ConstructionFinished = true;
        }
        _construction.SetActive(!isConstructed);
        _main.SetActive(isConstructed);
    }

    public void Initialize(PlaceableObjectItem placeableObjectItem)
    {
        _placeableObjectItem = placeableObjectItem;
        data.ItemName = placeableObjectItem.name;
        data.ID = SaveData.GenerateId();
        _main.name = _main.name + data.ID;
    }

    public void Initialize(PlaceableObjectItem placeableObjectItem, PlaceableObjectData placeableObjectData)
    {
        _placeableObjectItem = placeableObjectItem;
        data = placeableObjectData;
        _main.name = _main.name + data.ID;
        transform.position = data.Position;
    }

    #endregion

    #region Editing Mode

    public void StartEditing()
    {
        Debug.Log("started editing!");
        if (_selectable.IsSelected)
        {
            GridBuildingSystem.Current.TempPlaceableObject = this;
            InitializeDisplayObjects(true);
            CameraObjectFollowing.Current.SetTarget(transform);
            GridBuildingSystem.Current.TempTilemap.gameObject.SetActive(true);

            Vector3Int positionInt = GridBuildingSystem.Current.GridLayout.WorldToCell(transform.position);
            BoundsInt areaTemp = Area;
            areaTemp.position = positionInt;

            GridBuildingSystem.Current.SetAreaWhite(areaTemp, GridBuildingSystem.Current.MainTilemap);

            GridBuildingSystem.Current.FollowBuilding();
            GridBuildingSystem.Current.ReloadUI();

            _isEditing = true;
        }
    }

    public void CancelEditing()
    {
        transform.position = _origin;
        Place();
        _isEditing = false;
    }

    #endregion

    #region Construction Methods
    private bool _isPointerMoving;
    private Vector3 _lastPointerPosition;
    private void OnMouseDown()
    {
        _lastPointerPosition = Input.mousePosition;
    }
    private void OnMouseUp()
    {
        Debug.Log("mouseup");
        if (!PointerOverUIChecker.Current.IsPointerOverUIObject() && !_isPointerMoving && !GridBuildingSystem.Current.TempPlaceableObject)
        {

            if (ConstructionFinished == false)
            {
                if (isConstructed)
                {
                    Debug.Log("I want to initialize constructed building!");
                    InitializeConstructedBuilding();
                }
                else
                {
                    //select the construction site and show how much time is left/ bucks needed for speed up
                }
            }
            else
            {
                GetComponentInChildren<MoneyObject>().GetMoneyIfAvaliable();
                GetComponent<Selectable>().Select();
            }
            

            
        }
    }
    public void GetXP(int xp)
    {
        _xpNotification.SetActive(false);
        _tapVFX.SetActive(true);
        _xpCounter.SetActive(true);
        _xpCountDisplayer.DisplayCount(xp);
        Debug.Log("Xp added successfully");
    }

    #endregion
}
