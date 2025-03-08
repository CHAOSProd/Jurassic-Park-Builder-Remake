using System;
using UnityEngine;

[Serializable]
public class RoadData
{
    public int x;
    public int y;
    public int z;
    public int connectivity; // New: stores the connectivity mask used to choose the road prefab

    // Constructor accepting a grid position and its connectivity.
    public RoadData(Vector3Int gridPosition, int connectivity)
    {
        x = gridPosition.x;
        y = gridPosition.y;
        z = gridPosition.z;
        this.connectivity = connectivity;
    }

    // Returns the grid position as a Vector3Int.
    public Vector3Int gridPos
    {
        get { return new Vector3Int(x, y, z); }
    }

    // Optional: Convert to a Vector3 if needed.
    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}
