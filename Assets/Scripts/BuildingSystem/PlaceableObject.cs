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
    public bool ConstructionFinished = false;
    public int BuildTime; //time to build in SECONDS
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

    private bool _isEditing = false;

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
    [SerializeField] private Selectable _selectable;
    public Button _editButton;
    private Vector3 _origin;
    
    #region Unity Methods

    private void Awake()
    {
        DisplayFadeInOut = _construction.GetComponent<FadeInOut>();
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

        if (PlacedFromBeginning && !Attributes.HaveKey("IsDefaultObjectInitialized"))
        {
            PlaceableObjectItem defaultPlaceableObjectItem = Resources.Load<PlaceableObjectItem>("Placeables/TriceratopsItem");
            Initialize(defaultPlaceableObjectItem);
            data.SellRefund = 0;
            data.ItemName = "Triceratops";
            

            if (CanBePlaced())
            {
                Place();
            }

            ConstructionFinished = true;
            data.Progress = null;
            _construction.SetActive(false);

            Attributes.SetBool("IsDefaultObjectInitialized", true);
        }

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
        data.Position = transform.position;
        CameraObjectFollowing.Instance.SetTarget(null); 

        if (Placed) return;

        _selectable.PlayPlacementSound();
        Placed = true;
        
        SaveManager.Instance.SaveData.PlaceableObjects.Add(data);

        

        if (PlacedFromBeginning) return;

        data.Progress = new PlaceableObjectData.ProgressData(BuildTime, 0, DateTime.Now, BuildXp);
        //Update Progress every second and display xp icon when construction is finished
        UnityTimer.Instance.Tick(BuildTime, 1, UpdateProgress, OnConstructionFinished);
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
            if (!isBuildingEnabled)
            {
                _construction.GetComponent<Collider2D>().enabled = false;
            }
            
        }
        else
        {
            _main.SetActive(!isBuildingEnabled);
            _display.SetActive(isBuildingEnabled);
            _construction.SetActive(false);
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
        _xpNotification.SetActive(false);
        _tapVFX.SetActive(true);
        _xpCounter.SetActive(true);
        _xpCountDisplayer.DisplayCount(BuildXp);
        EventManager.Instance.TriggerEvent(new XPAddedGameEvent(BuildXp));
        _construction.SetActive(false);
        _main.SetActive(true);
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
        transform.position = data.Position;
        this.ConstructionFinished = placeableObjectData.ConstructionFinished;

        if(ConstructionFinished)
        {
            // Make sure building is displayed
            _xpNotification.SetActive(false);
            _tapVFX.SetActive(false);
            _xpCounter.SetActive(false);
            _construction.SetActive(false);
            _main.SetActive(true);
        }
        else if(data.Progress != null)
        {
            this.BuildTime = placeableObjectData.Progress.BuildTime;
            this.BuildXp = placeableObjectData.Progress.XP;

            int newTime = (int)Math.Floor((DateTime.Now - placeableObjectData.Progress.LastTick).TotalSeconds) + placeableObjectData.Progress.ElapsedTime;

            Debug.LogWarning(newTime);

            if(newTime >= BuildTime)
            {
                OnConstructionFinished();
            }
            else
            {
                data.Progress.ElapsedTime = newTime;
                data.Progress.LastTick = DateTime.Now;
                data.Progress = new PlaceableObjectData.ProgressData(BuildTime, BuildTime - newTime, DateTime.Now, BuildXp);
                //Update Progress every second and display xp icon when construction is finished
                UnityTimer.Instance.Tick(BuildTime - newTime, 1, UpdateProgress, OnConstructionFinished);
            }
        }
    }

    #endregion

    #region Editing Mode

    public void StartEditing()
    {
        Debug.Log("started editing!");
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
        if (!PointerOverUIChecker.Instance.IsPointerOverUIObject() && !_isPointerMoving && !GridBuildingSystem.Instance.TempPlaceableObject)
        {
            if (ConstructionFinished)
            {
                GetComponentInChildren<MoneyObject>().GetMoneyIfAvaliable();
                _selectable.Select();
                
            }
            else
            {
                if (!_xpNotification.activeSelf)
                {
                    //TODO: Select the construction site and show how much time is left/bucks needed for speed up
                }
                else
                {
                    InitializeConstructedBuilding();
                    ConstructionFinished = true;
                    data.ConstructionFinished = true;
                    data.Progress = null;
                }
            }
        }
    }
    #endregion
}
