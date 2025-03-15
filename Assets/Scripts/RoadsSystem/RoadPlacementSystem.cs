using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class RoadPlacementSystem : MonoBehaviour
{
    [Header("Tilemap")]
    public Tilemap roadTilemap;

    [Header("Road Prefab Variants")]
    // End pieces – assign a separate prefab for each facing direction
    public GameObject prefabEndUp;
    public GameObject prefabEndRight;
    public GameObject prefabEndDown;
    public GameObject prefabEndLeft;

    // Straight pieces
    public GameObject prefabStraightVertical;
    public GameObject prefabStraightHorizontal;

    // Curve pieces
    public GameObject prefabCurveUpRight;   // connectivity mask: UP + RIGHT
    public GameObject prefabCurveRightDown; // connectivity mask: RIGHT + DOWN
    public GameObject prefabCurveDownLeft;  // connectivity mask: DOWN + LEFT
    public GameObject prefabCurveLeftUp;    // connectivity mask: LEFT + UP

    // T-junctions (each missing one direction)
    public GameObject prefabTJunctionMissingUp;    // missing Up => connectivity: RIGHT+DOWN+LEFT
    public GameObject prefabTJunctionMissingRight; // missing Right => connectivity: UP+DOWN+LEFT
    public GameObject prefabTJunctionMissingDown;  // missing Down => connectivity: UP+RIGHT+LEFT
    public GameObject prefabTJunctionMissingLeft;  // missing Left => connectivity: UP+RIGHT+DOWN

    // 4-way intersection
    public GameObject prefabFourWay; // connectivity mask: UP+RIGHT+DOWN+LEFT

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
    public GameObject[] uiObjectsToDeactivate; // Ensure these DO NOT include the add/remove buttons

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

    // Internal data for road placement.
    private bool isPlacing = false;
    private List<Vector3Int> placedPositions = new List<Vector3Int>();
    private Dictionary<Vector3Int, GameObject> placedRoadObjects = new Dictionary<Vector3Int, GameObject>();

    // Dictionary to store each tile’s connectivity (used for saving road type)
    private Dictionary<Vector3Int, int> tileConnectivity = new Dictionary<Vector3Int, int>();

    // Use an enum for the two modes.
    private enum PlacementMode { Add, Remove }
    private PlacementMode currentMode = PlacementMode.Add;

    // Define directional bitmask constants.
    private const int UP = 1;
    private const int RIGHT = 2;
    private const int DOWN = 4;
    private const int LEFT = 8;

    void Start()
    {
        // Set up mode buttons.
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

        // Attempt to load saved road placements.
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

        // Update preview position.
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int roadGridPos = roadTilemap.WorldToCell(mouseWorldPos);
        Vector3 cellCenter = roadTilemap.GetCellCenterWorld(roadGridPos);
        if (previewObject != null)
            previewObject.transform.position = cellCenter;

        if (Input.GetMouseButtonDown(0))
        {
            if (currentMode == PlacementMode.Add)
            {
                // In add mode, check if tile is already occupied.
                if (placedPositions.Contains(roadGridPos))
                {
                    Debug.Log("Tile already occupied at: " + roadGridPos);
                    return;
                }

                // Check for blocking objects.
                Collider2D hit = Physics2D.OverlapPoint(cellCenter, placementBlockingLayers);
                if (hit != null)
                {
                    Debug.Log("Cannot place road at " + roadGridPos + " because of blocking object: " + hit.gameObject.name);
                    return;
                }

                // Optional: Use GridBuildingSystem to check the tile.
                if (GridBuildingSystem.Instance != null &&
                    GridBuildingSystem.Instance.GridLayout != null &&
                    GridBuildingSystem.Instance.MainTilemap != null)
                {
                    Vector3Int mainCellPos = GridBuildingSystem.Instance.GridLayout.WorldToCell(cellCenter);
                    var area = new BoundsInt(mainCellPos, new Vector3Int(1, 1, 1));
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
                // In remove mode, if there is a road tile at the clicked cell, remove it.
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

    public void EnterPlacementMode()
    {
        isPlacing = true;
        // Default mode when entering is Add mode.
        SetPlacementMode(PlacementMode.Add);

        if (previewObject != null)
            previewObject.SetActive(true);

        foreach (GameObject uiObj in uiObjectsToDeactivate)
        {
            if (uiObj != null)
                uiObj.SetActive(false);
        }

        // Disable colliders on interactive prefabs.
        if (interactivePrefabs != null)
        {
            foreach (GameObject obj in interactivePrefabs)
            {
                if (obj != null)
                    DisableColliders(obj);
            }
        }
        // Also disable colliders on any clones in the scene.
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

        // Re-enable colliders on interactive prefabs.
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

    // Switches the current mode and updates the button visuals.
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

    // Helper: Calculate connectivity based on adjacent tiles.
    private int CalculateConnectivity(Vector3Int pos)
    {
        int connectivity = 0;
        if (placedPositions.Contains(pos + new Vector3Int(0, 1, 0))) connectivity |= UP;
        if (placedPositions.Contains(pos + new Vector3Int(1, 0, 0))) connectivity |= RIGHT;
        if (placedPositions.Contains(pos + new Vector3Int(0, -1, 0))) connectivity |= DOWN;
        if (placedPositions.Contains(pos + new Vector3Int(-1, 0, 0))) connectivity |= LEFT;
        return connectivity;
    }

    // Update a road tile at a given position.
    private void UpdateRoadAt(Vector3Int pos)
    {
        if (!placedPositions.Contains(pos))
            return;

        // Calculate connectivity using the helper.
        int connectivity = CalculateConnectivity(pos);
        tileConnectivity[pos] = connectivity;

        GameObject prefabToUse = GetPrefabFromConnectivity(connectivity);
        if (prefabToUse == null)
        {
            Debug.LogWarning("No prefab assigned for connectivity mask: " + connectivity);
            return;
        }

        // Remove any existing instance before instantiating the updated one.
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

    // Update the tile and its immediate neighbors.
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

        UpdateAdjacentRoads(gridPos);
        SaveRoads();
    }

    // Returns the prefab corresponding to the connectivity mask.
    GameObject GetPrefabFromConnectivity(int connectivity)
    {
        // For isolated tiles, choose a default.
        if (connectivity == 0)
            return prefabEndUp;

        // For single-connection tiles, use an identity mapping:
        // • Only neighbor ABOVE → return End Up.
        // • Only neighbor BELOW → return End Down.
        // • Only neighbor RIGHT → return End Right.
        // • Only neighbor LEFT → return End Left.
        if (connectivity == UP)    return prefabEndUp;
        if (connectivity == DOWN)  return prefabEndDown;
        if (connectivity == RIGHT) return prefabEndRight;
        if (connectivity == LEFT)  return prefabEndLeft;

        // For multiple connections, use explicit mappings.
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
                return prefabTJunctionMissingLeft; // missing LEFT
            case RIGHT | DOWN | LEFT:
                return prefabTJunctionMissingUp;   // missing UP
            case DOWN | LEFT | UP:
                return prefabTJunctionMissingRight; // missing RIGHT
            case LEFT | UP | RIGHT:
                return prefabTJunctionMissingDown;  // missing DOWN
            case UP | RIGHT | DOWN | LEFT:
                return prefabFourWay;
            default:
                Debug.LogWarning("Unexpected connectivity: " + connectivity);
                return prefabEndUp;
        }
    }

    // Save the road layout using grid positions and stored connectivity.
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

    // Load the road layout using saved grid positions and connectivity values.
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
