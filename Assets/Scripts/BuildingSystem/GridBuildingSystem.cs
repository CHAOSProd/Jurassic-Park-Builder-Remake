using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GridBuildingSystem : Singleton<GridBuildingSystem>
{

    public GridLayout GridLayout;

    [SerializeField] private Camera _camera;
    [SerializeField] private string _highestLayerName;

    public Tilemap MainTilemap;
    public Tilemap TempTilemap;

    private static readonly Dictionary<TileType, TileBase> tileBases = new Dictionary<TileType, TileBase>();

    [HideInInspector] public PlaceableObject TempPlaceableObject;

    private Vector3 _startTouchPosition;
    private float _deltaX, _deltaY;

    private Vector3 _prevPosition;
    private BoundsInt _prevArea;
    private bool _prevBuildable = false;

    private VoidCallback _onAccept;

    public void SetAcceptCallback(VoidCallback function)
    {
        _onAccept = function;
    }
    public void ResetAcceptCallback()
    {
        _onAccept = null;
    }

    #region Unity Methods
    private void Start()
    {
        string tilePath = @"Tiles\";
        tileBases.Add(TileType.Empty, null);
        tileBases.Add(TileType.White, Resources.Load<TileBase>(tilePath + "white"));
        tileBases.Add(TileType.Green, Resources.Load<TileBase>(tilePath + "green"));
        tileBases.Add(TileType.Red, Resources.Load<TileBase>(tilePath + "red"));

        ReloadUI();
    }

    private void Update()
    {
        if (!TempPlaceableObject)
            return;

        if (TempPlaceableObject.DisplayFadeInOut != null)
            TempPlaceableObject.DisplayFadeInOut.SetFade(TempPlaceableObject && TempPlaceableObject.CanBePlaced());
    }

    #endregion

    #region Tilemap Management

    private static TileBase[] GetTilesBlock(BoundsInt area, Tilemap tilemap)
    {
        TileBase[] array = new TileBase[area.size.x * area.size.y * area.size.z];
        int counter = 0;

        foreach (var v in area.allPositionsWithin)
        {
            Vector3Int position = new Vector3Int(v.x, v.y, 0);
            array[counter] = tilemap.GetTile(position);
            counter++;
        }

        return array;
    }

    private static void SetTilesBlock(BoundsInt area, TileType type, Tilemap tilemap)
    {
        area.size = new Vector3Int(area.size.x, area.size.y, 1);
        int size = area.size.x * area.size.y;
        TileBase[] tileArray = new TileBase[size];
        FillTiles(tileArray, type);
        tilemap.SetTilesBlock(area, tileArray);
    }

    private static void FillTiles(TileBase[] array, TileType type)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = tileBases[type];
        }
    }

    public void SetAreaWhite(BoundsInt area, Tilemap tilemap)
    {
        SetTilesBlock(area, TileType.White, tilemap);
    }
    #endregion

    #region Building Placement

    public GameObject InitializeWithBuilding(GameObject building)
    {
        TempPlaceableObject = Instantiate(building, Vector3.zero, Quaternion.identity).GetComponent<PlaceableObject>();
        TempPlaceableObject.InitializeDisplayObjects(true);

        TempTilemap.GetComponent<TilemapRenderer>().sortingOrder = 1;

        ReloadUI();

        Vector3 screenMiddlePoint = _camera.ScreenToWorldPoint(new Vector3(Screen.width >> 1, Screen.height >> 1));
        Vector3Int cellPosition = GridLayout.WorldToCell(screenMiddlePoint);

        TempPlaceableObject.transform.localPosition = GridLayout.CellToLocalInterpolated(new Vector3(cellPosition.x, cellPosition.y, 0f));
        _prevPosition = cellPosition;
        FollowBuilding();

        CameraObjectFollowing.Instance.SetTarget(TempPlaceableObject.transform);
        TempTilemap.gameObject.SetActive(true);

        return TempPlaceableObject.gameObject;
    }

    private void ClearArea()
    {
        TileBase[] toClear = new TileBase[_prevArea.size.x * _prevArea.size.y * _prevArea.size.z];
        FillTiles(toClear, TileType.Empty);
        TempTilemap.SetTilesBlock(_prevArea, toClear);
    }

    public void FollowBuilding()
    {
        ClearArea();

        TempPlaceableObject.Area.position = GridLayout.WorldToCell(TempPlaceableObject.transform.position);
        BoundsInt buildingArea = TempPlaceableObject.Area;

        TileBase[] baseArray = GetTilesBlock(buildingArea, MainTilemap);

        int size = baseArray.Length;
        TileBase[] tileArray = new TileBase[size];
        
        bool buildable = true;

        for (int i = 0; i < baseArray.Length; i++)
        {
            if (baseArray[i] == tileBases[TileType.White])
            {
                tileArray[i] = tileBases[TileType.Green];
            }
            else
            {
                FillTiles(tileArray, TileType.Red);
                buildable = false;
                break;
            }
        }

        if(_prevBuildable != buildable)
        {
            _prevBuildable = buildable;
            if(buildable)
            {
                TempTilemap.GetComponent<TilemapRenderer>().sortingOrder = -1;
                TempPlaceableObject.ResetSortingOrder();
            }
            else
            {
                TempTilemap.GetComponent<TilemapRenderer>().sortingOrder = 1;
                TempPlaceableObject.SortAtTop();
            }
        }

        TempTilemap.SetTilesBlock(buildingArea, tileArray);
        _prevArea = buildingArea;
    }

    public bool CanTakeArea(BoundsInt area)
    {
        TileBase[] baseArray = GetTilesBlock(area, MainTilemap);
        foreach (var tileBase in baseArray)
        {
            if (tileBase != tileBases[TileType.White])
            {
                return false;
            }
        }

        return true;
    }

    public void TakeArea(BoundsInt area)
    {
        SetTilesBlock(area, TileType.Empty, TempTilemap);
        SetTilesBlock(area, TileType.Green, MainTilemap);
    }

    #endregion

    #region Building Movement

    public void SaveObjectOffset()
    {
        if (!TempPlaceableObject)
            return;

        _startTouchPosition = Input.mousePosition;
        _startTouchPosition = Camera.main.ScreenToWorldPoint(_startTouchPosition);

        _deltaX = _startTouchPosition.x - TempPlaceableObject.transform.position.x;
        _deltaY = _startTouchPosition.y - TempPlaceableObject.transform.position.y;
    }

    public void MoveObjectWithOffset()
    {
        if (!TempPlaceableObject)
            return;

        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 position = new Vector3(touchPosition.x - _deltaX, touchPosition.y - _deltaY);

        Vector3Int cellPosition = GridLayout.WorldToCell(position);

        if (_prevPosition != cellPosition)
        {
            TempPlaceableObject.transform.position = GridLayout.CellToLocalInterpolated(cellPosition);
            _prevPosition = cellPosition;
            FollowBuilding();
        }
    }

    #endregion

    #region Interface

    public void ReloadUI()
    {
        if (TempPlaceableObject)
        {
            UIManager.Instance.ChangeTo("PlacementUI");
        }
        else
        {
            SelectablesManager.Instance.UnselectAll();
        }
    }

    #endregion

    #region Building Ending Buttons

    public void Decline()
    {
        if (!TempPlaceableObject)
            return;

        if (!TempPlaceableObject.Placed)
        {
            ClearArea();
            Destroy(TempPlaceableObject.gameObject);
        }
        else
        {
            TempPlaceableObject.CancelEditing();
            ClearArea();

        }

        TempPlaceableObject = null;
        ReloadUI();
        TempTilemap.gameObject.SetActive(false);

        UIManager.Instance.ChangeFixedTo("DefaultUI");
    }

    public void Accept()
    {
        if (!TempPlaceableObject)
        {
            return;
        }
            

        if (TempPlaceableObject.CanBePlaced())
        {
            TempPlaceableObject.Place();
            TempPlaceableObject = null;
            TempTilemap.gameObject.SetActive(false);

            ReloadUI();
            UIManager.Instance.ChangeFixedTo("DefaultUI");

            _onAccept?.Invoke();
        }
    }

    #endregion
}

public enum TileType
{
    Empty,
    White,
    Green,
    Red
}