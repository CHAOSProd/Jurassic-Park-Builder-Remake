using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoadPlacementSystem : MonoBehaviour
{
    [Header("Tilemap")]
    public Tilemap roadTilemap;

    [Header("Road Prefab Variants")]
    public GameObject prefabEndUp;
    public GameObject prefabEndRight;
    public GameObject prefabEndDown;
    public GameObject prefabEndLeft;

    public GameObject prefabStraightVertical;
    public GameObject prefabStraightHorizontal;

    public GameObject prefabCurveUpRight;
    public GameObject prefabCurveRightDown;
    public GameObject prefabCurveDownLeft;
    public GameObject prefabCurveLeftUp;

    public GameObject prefabTJunctionMissingUp;
    public GameObject prefabTJunctionMissingRight;
    public GameObject prefabTJunctionMissingDown;
    public GameObject prefabTJunctionMissingLeft;

    public GameObject prefabFourWay;

    [Header("Preview Settings")]
    public GameObject previewObject;
    public Sprite straightPreviewSprite;

    [Header("UI Buttons for Mode Switching")]
    public Button addRoadButton;
    public Button removeRoadButton;

    [Header("UI Button for Exiting Placement Mode")]
    public Button exitPlacementModeButton;

    [Header("Button Sprites")]
    public Sprite addButtonPressedSprite;
    public Sprite addButtonNormalSprite;
    public Sprite removeButtonPressedSprite;
    public Sprite removeButtonNormalSprite;

    [Header("Additional UI Objects to Deactivate")]
    public GameObject[] uiObjectsToDeactivate;

    [Header("Interactive Prefabs to Disable During Placement")]
    public GameObject[] interactivePrefabs;

    [Header("Interactive Prefab Tag for Clones")]
    [Tooltip("Any cloned interactive prefab should be tagged with this tag.")]
    public string interactivePrefabTag = "InteractivePrefab";

    [Header("Camera")]
    public Camera mainCamera;

    [Header("Sound Settings")]
    public AudioSource audioSource;
    public AudioClip placementSound;
    public AudioClip removalSound;

    [Header("Placement Blocking")]
    public LayerMask placementBlockingLayers;
    private bool isDragging = false;
    private Vector3 lastMousePosition;
    public BoundsInt Area;

    // Internal data for road placement.
    private bool isPlacing = false;
    private List<Vector3Int> placedPositions = new List<Vector3Int>();
    private Dictionary<Vector3Int, GameObject> placedRoadObjects = new Dictionary<Vector3Int, GameObject>();
    private Dictionary<Vector3Int, int> tileConnectivity = new Dictionary<Vector3Int, int>();

    private enum PlacementMode { Add, Remove }
    private PlacementMode currentMode = PlacementMode.Add;

    // Directional bitmask constants.
    private const int UP = 1;
    private const int RIGHT = 2;
    private const int DOWN = 4;
    private const int LEFT = 8;

    void Start()
    {
        if (addRoadButton != null)
            addRoadButton.onClick.AddListener(() => SetPlacementMode(PlacementMode.Add));
        else
            Debug.LogError("Add Road Button not assigned!");

        if (removeRoadButton != null)
            removeRoadButton.onClick.AddListener(() => SetPlacementMode(PlacementMode.Remove));
        else
            Debug.LogError("Remove Road Button not assigned!");

        if (exitPlacementModeButton != null)
            exitPlacementModeButton.onClick.AddListener(ExitPlacementMode);
        else
            Debug.LogError("Exit Placement Mode Button not assigned!");

        if (previewObject != null)
            previewObject.SetActive(false);
        else
            Debug.LogError("Preview Object not assigned!");

        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not assigned! Using Camera.main instead.");
            mainCamera = Camera.main;
        }

        LoadRoads();
    }

    void Update()
    {
        if (!isPlacing)
            return;

        // Deactivate extra UI objects during placement.
        foreach (GameObject uiObj in uiObjectsToDeactivate)
        {
            if (uiObj != null && uiObj.activeSelf)
                uiObj.SetActive(false);
        }

        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int roadGridPos = roadTilemap.WorldToCell(mouseWorldPos);
        Vector3 cellCenter = roadTilemap.GetCellCenterWorld(roadGridPos);
        if (previewObject != null)
            previewObject.transform.position = cellCenter;

        if (Input.GetMouseButtonDown(0))
        {
            isDragging = false;
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            if (Vector3.Distance(lastMousePosition, Input.mousePosition) > 10f)
            {
                isDragging = true;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (!isDragging && !EventSystem.current.IsPointerOverGameObject())
            {
                if (currentMode == PlacementMode.Add)
                {
                    if (placedPositions.Contains(roadGridPos))
                    {
                        Debug.Log("Tile already occupied at: " + roadGridPos);
                        return;
                    }

                    Collider2D hit = Physics2D.OverlapPoint(cellCenter, placementBlockingLayers);
                    if (hit != null)
                    {
                        Debug.Log("Cannot place road at " + roadGridPos + " because of blocking object: " + hit.gameObject.name);
                        return;
                    }

                    // Check with grid system if the area can be taken.
                    if (GridBuildingSystem.Instance != null &&
                        GridBuildingSystem.Instance.GridLayout != null &&
                        GridBuildingSystem.Instance.MainTilemap != null)
                    {
                        Vector3Int mainCellPos = GridBuildingSystem.Instance.GridLayout.WorldToCell(cellCenter);
                        var area = new BoundsInt(mainCellPos, new Vector3Int(1, 1, 1));
                        // Directly check if tile is white. (Assuming white means free.)
                        if (!GridBuildingSystem.Instance.CanTakeArea(area))
                        {
                            Debug.Log("Cannot place road at " + mainCellPos + " because the tile is not white.");
                            return;
                        }
                    }
                    else
                    {
                        Debug.LogWarning("GridBuildingSystem or its required components are missing!");
                    }

                    PlaceRoadTile(roadGridPos);
                }
                else if (currentMode == PlacementMode.Remove)
                {
                    if (placedPositions.Contains(roadGridPos))
                    {
                        RemoveRoadTile(roadGridPos);
                    }
                    else
                    {
                        Debug.Log("No road tile to remove at: " + roadGridPos);
                    }
                }
            }
        }
    }

    public void EnterPlacementMode()
    {
        isPlacing = true;
        SetPlacementMode(PlacementMode.Add);

        if (previewObject != null)
            previewObject.SetActive(true);

        foreach (GameObject uiObj in uiObjectsToDeactivate)
        {
            if (uiObj != null)
                uiObj.SetActive(false);
        }

        if (interactivePrefabs != null)
        {
            foreach (GameObject obj in interactivePrefabs)
            {
                if (obj != null)
                    DisableColliders(obj);
            }
        }
        GameObject[] clones = GameObject.FindGameObjectsWithTag(interactivePrefabTag);
        foreach (GameObject clone in clones)
        {
            DisableColliders(clone);
        }
        Debug.Log("Entered placement mode.");
    }

    public void ExitPlacementMode()
    {
        isPlacing = false;
        if (previewObject != null)
            previewObject.SetActive(false);

        foreach (GameObject uiObj in uiObjectsToDeactivate)
        {
            if (uiObj != null)
                uiObj.SetActive(true);
        }

        if (interactivePrefabs != null)
        {
            foreach (GameObject obj in interactivePrefabs)
            {
                if (obj != null)
                    EnableColliders(obj);
            }
        }
        GameObject[] clones = GameObject.FindGameObjectsWithTag(interactivePrefabTag);
        foreach (GameObject clone in clones)
        {
            EnableColliders(clone);
        }
        SaveRoads();
        Debug.Log("Exited placement mode and saved road layout.");
    }

    void SetPlacementMode(PlacementMode mode)
    {
        currentMode = mode;
        if (currentMode == PlacementMode.Add)
        {
            if (addRoadButton != null)
                addRoadButton.image.sprite = addButtonPressedSprite;
            if (removeRoadButton != null)
                removeRoadButton.image.sprite = removeButtonNormalSprite;
        }
        else if (currentMode == PlacementMode.Remove)
        {
            if (addRoadButton != null)
                addRoadButton.image.sprite = addButtonNormalSprite;
            if (removeRoadButton != null)
                removeRoadButton.image.sprite = removeButtonPressedSprite;
        }
    }

    private int CalculateConnectivity(Vector3Int pos)
    {
        int connectivity = 0;
        if (placedPositions.Contains(pos + new Vector3Int(0, 1, 0))) connectivity |= UP;
        if (placedPositions.Contains(pos + new Vector3Int(1, 0, 0))) connectivity |= RIGHT;
        if (placedPositions.Contains(pos + new Vector3Int(0, -1, 0))) connectivity |= DOWN;
        if (placedPositions.Contains(pos + new Vector3Int(-1, 0, 0))) connectivity |= LEFT;
        return connectivity;
    }

    private void UpdateRoadAt(Vector3Int pos)
    {
        if (!placedPositions.Contains(pos))
            return;

        int connectivity = CalculateConnectivity(pos);
        tileConnectivity[pos] = connectivity;

        GameObject prefabToUse = GetPrefabFromConnectivity(connectivity);
        if (prefabToUse == null)
        {
            Debug.LogWarning("No prefab assigned for connectivity mask: " + connectivity);
            return;
        }

        if (placedRoadObjects.ContainsKey(pos))
        {
            Destroy(placedRoadObjects[pos]);
            placedRoadObjects.Remove(pos);
        }

        Vector3 worldPos = roadTilemap.GetCellCenterWorld(pos);
        GameObject newTile = Instantiate(prefabToUse, worldPos, Quaternion.identity, roadTilemap.transform);
        newTile.SetActive(true);
        placedRoadObjects[pos] = newTile;
    }

    private void UpdateAdjacentRoads(Vector3Int centerPos)
    {
        List<Vector3Int> positionsToUpdate = new List<Vector3Int>()
        {
            centerPos,
            centerPos + new Vector3Int(0, 1, 0),
            centerPos + new Vector3Int(1, 0, 0),
            centerPos + new Vector3Int(0, -1, 0),
            centerPos + new Vector3Int(-1, 0, 0)
        };

        foreach (var pos in positionsToUpdate)
        {
            UpdateRoadAt(pos);
        }
    }

    void PlaceRoadTile(Vector3Int gridPos)
    {
        placedPositions.Add(gridPos);

        if (audioSource != null && placementSound != null)
            audioSource.PlayOneShot(placementSound);

        UpdateAdjacentRoads(gridPos);

        // Update grid occupancy by directly updating the MainTilemap to GREEN.
        if (GridBuildingSystem.Instance != null &&
            GridBuildingSystem.Instance.GridLayout != null &&
            GridBuildingSystem.Instance.MainTilemap != null)
        {
            Vector3 cellCenter = roadTilemap.GetCellCenterWorld(gridPos);
            Vector3Int gridBuildingPos = GridBuildingSystem.Instance.GridLayout.WorldToCell(cellCenter);
            BoundsInt area = new BoundsInt(gridBuildingPos, new Vector3Int(1, 1, 1));

            // Load the green tile directly from Resources.
            TileBase greenTile = Resources.Load<TileBase>("Tiles/green");
            if (greenTile != null)
            {
                GridBuildingSystem.Instance.MainTilemap.SetTile(gridBuildingPos, greenTile);
                GridBuildingSystem.Instance.MainTilemap.RefreshTile(gridBuildingPos);
                Debug.Log("Directly updated MainTilemap: Cell " + gridBuildingPos + " set to GREEN for road at grid position: " + gridPos);
            }
            else
            {
                Debug.LogError("Green tile not found in Resources/Tiles/green");
            }
        }
        else
        {
            Debug.LogWarning("GridBuildingSystem or its required components are missing!");
        }

        SaveRoads();
    }

    void RemoveRoadTile(Vector3Int gridPos)
    {
        if (!placedPositions.Contains(gridPos))
            return;

        placedPositions.Remove(gridPos);
        if (placedRoadObjects.ContainsKey(gridPos))
        {
            Destroy(placedRoadObjects[gridPos]);
            placedRoadObjects.Remove(gridPos);
        }
        if (tileConnectivity.ContainsKey(gridPos))
            tileConnectivity.Remove(gridPos);

        if (audioSource != null && removalSound != null)
            audioSource.PlayOneShot(removalSound);

        // Free grid occupancy by directly updating the MainTilemap to WHITE.
        if (GridBuildingSystem.Instance != null &&
            GridBuildingSystem.Instance.GridLayout != null &&
            GridBuildingSystem.Instance.MainTilemap != null)
        {
            Vector3 cellCenter = roadTilemap.GetCellCenterWorld(gridPos);
            Vector3Int gridBuildingPos = GridBuildingSystem.Instance.GridLayout.WorldToCell(cellCenter);
            BoundsInt area = new BoundsInt(gridBuildingPos, new Vector3Int(1, 1, 1));

            // Load the white tile directly from Resources.
            TileBase whiteTile = Resources.Load<TileBase>("Tiles/white");
            if (whiteTile != null)
            {
                GridBuildingSystem.Instance.MainTilemap.SetTile(gridBuildingPos, whiteTile);
                GridBuildingSystem.Instance.MainTilemap.RefreshTile(gridBuildingPos);
                Debug.Log("Directly updated MainTilemap: Cell " + gridBuildingPos + " set to WHITE for road at grid position: " + gridPos);
            }
            else
            {
                Debug.LogError("White tile not found in Resources/Tiles/white");
            }
        }
        else
        {
            Debug.LogWarning("GridBuildingSystem or its required components are missing!");
        }

        UpdateAdjacentRoads(gridPos);
        SaveRoads();
    }

    GameObject GetPrefabFromConnectivity(int connectivity)
    {
        if (connectivity == 0)
            return prefabEndUp;
        if (connectivity == UP)    return prefabEndUp;
        if (connectivity == DOWN)  return prefabEndDown;
        if (connectivity == RIGHT) return prefabEndRight;
        if (connectivity == LEFT)  return prefabEndLeft;

        switch (connectivity)
        {
            case UP | DOWN:
                return prefabStraightVertical;
            case LEFT | RIGHT:
                return prefabStraightHorizontal;
            case UP | RIGHT:
                return prefabCurveUpRight;
            case RIGHT | DOWN:
                return prefabCurveRightDown;
            case DOWN | LEFT:
                return prefabCurveDownLeft;
            case LEFT | UP:
                return prefabCurveLeftUp;
            case UP | RIGHT | DOWN:
                return prefabTJunctionMissingLeft;
            case RIGHT | DOWN | LEFT:
                return prefabTJunctionMissingUp;
            case DOWN | LEFT | UP:
                return prefabTJunctionMissingRight;
            case LEFT | UP | RIGHT:
                return prefabTJunctionMissingDown;
            case UP | RIGHT | DOWN | LEFT:
                return prefabFourWay;
            default:
                Debug.LogWarning("Unexpected connectivity: " + connectivity);
                return prefabEndUp;
        }
    }

    void SaveRoads()
    {
        SaveManager.Instance.SaveData.RoadData.Clear();
        foreach (Vector3Int pos in placedPositions)
        {
            int connectivity = 0;
            if (tileConnectivity.TryGetValue(pos, out connectivity))
                SaveManager.Instance.SaveData.RoadData.Add(new RoadData(pos, connectivity));
            else
                SaveManager.Instance.SaveData.RoadData.Add(new RoadData(pos, 0));
        }
        Debug.Log("Saved road layout. Total roads: " + placedPositions.Count);
    }

    void LoadRoads()
    {
        if (SaveManager.Instance.SaveData.RoadData == null)
            return;

        foreach (RoadData road in SaveManager.Instance.SaveData.RoadData)
        {
            Vector3Int gridPos = road.gridPos;
            if (!placedPositions.Contains(gridPos))
            {
                placedPositions.Add(gridPos);
                tileConnectivity[gridPos] = road.connectivity;
                GameObject prefabToUse = GetPrefabFromConnectivity(road.connectivity);
                if (prefabToUse != null)
                {
                    Vector3 worldPos = roadTilemap.GetCellCenterWorld(gridPos);
                    GameObject newTile = Instantiate(prefabToUse, worldPos, Quaternion.identity, roadTilemap.transform);
                    newTile.SetActive(true);
                    placedRoadObjects[gridPos] = newTile;
                }
            }
        }
        Debug.Log("Loaded road layout. Total roads: " + placedPositions.Count);
    }

    void DisableColliders(GameObject obj)
    {
        Collider2D[] colliders2D = obj.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in colliders2D)
            col.enabled = false;

        Collider[] colliders3D = obj.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders3D)
            col.enabled = false;
    }

    void EnableColliders(GameObject obj)
    {
        Collider2D[] colliders2D = obj.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in colliders2D)
            col.enabled = true;

        Collider[] colliders3D = obj.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders3D)
            col.enabled = true;
    }
}
