using UnityEngine;
using UnityEngine.Tilemaps;

public class SingleTilePrefabConverter : MonoBehaviour
{
    // This static flag controls whether tile reversion on prefab removal is enabled.
    public static bool EnableTileReversionOnDestroy = false;

    void Start()
    {
        ConvertTileUnder();
    }

    /// <summary>
    /// Converts the tile under this prefab from white to green.
    /// </summary>
    public void ConvertTileUnder()
    {
        if (GridBuildingSystem.Instance == null)
        {
            Debug.LogError("GridBuildingSystem instance not found!");
            return;
        }

        // Get the grid layout and determine the cell position based on the prefab's world position.
        GridLayout gridLayout = GridBuildingSystem.Instance.GridLayout;
        Vector3Int cellPosition = gridLayout.WorldToCell(transform.position);

        // Define the bounds for a single tile.
        BoundsInt tileBounds = new BoundsInt(cellPosition.x, cellPosition.y, 0, 1, 1, 1);

        // Check if the tile is available (white) and convert it to green.
        if (GridBuildingSystem.Instance.CanTakeArea(tileBounds))
        {
            GridBuildingSystem.Instance.TakeArea(tileBounds);
            Debug.Log("Tile converted from white to green at cell " + cellPosition);
        }
        else
        {
            Debug.LogWarning("Tile at cell " + cellPosition + " is not available for conversion.");
        }
    }

    /// <summary>
    /// Reverts the tile under this prefab from green back to white when the prefab is removed.
    /// This only occurs if the static flag EnableTileReversionOnDestroy is set to true.
    /// </summary>
    void OnDestroy()
    {
        // Only revert the tile if tile reversion has been enabled.
        if (!EnableTileReversionOnDestroy)
            return;

        if (GridBuildingSystem.Instance == null)
            return;

        GridLayout gridLayout = GridBuildingSystem.Instance.GridLayout;
        Vector3Int cellPosition = gridLayout.WorldToCell(transform.position);
        BoundsInt tileBounds = new BoundsInt(cellPosition.x, cellPosition.y, 0, 1, 1, 1);

        // Revert the tile back to white using the grid system method.
        GridBuildingSystem.Instance.SetAreaWhite(tileBounds, GridBuildingSystem.Instance.MainTilemap);
        Debug.Log("Tile reverted from green to white at cell " + cellPosition);
    }

    /// <summary>
    /// Call this method from your UI button to enable tile reversion for prefab removals.
    /// </summary>
    public static void EnableTileReversion()
    {
        EnableTileReversionOnDestroy = true;
        Debug.Log("Tile reversion on prefab removal has been enabled.");
    }

    /// <summary>
    /// Optionally, you can use this to disable tile reversion again.
    /// </summary>
    public static void DisableTileReversion()
    {
        EnableTileReversionOnDestroy = false;
        Debug.Log("Tile reversion on prefab removal has been disabled.");
    }
}



