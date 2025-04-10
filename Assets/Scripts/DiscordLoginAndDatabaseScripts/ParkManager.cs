using UnityEngine;
using System.Collections.Generic;

public class ParkManager : MonoBehaviour
{
    [System.Serializable]
    public class ParkData
    {
        public List<PrefabLayout> layouts = new List<PrefabLayout>();
    }

    [System.Serializable]
    public class PrefabLayout
    {
        public int prefabIndex;
        public List<Vector3> positions = new List<Vector3>();
    }

    [Header("Settings")]
    public GameObject[] buildablePrefabs; // Assign prefabs in inspector
    public Transform parkContainer; // Parent object for all placed items

    // Save current park layout
    public string SaveParkLayout()
    {
        ParkData data = new ParkData();

        // Check each prefab type
        for (int prefabIndex = 0; prefabIndex < buildablePrefabs.Length; prefabIndex++)
        {
            PrefabLayout layout = new PrefabLayout();
            layout.prefabIndex = prefabIndex;

            // Find all instances of this prefab
            foreach (Transform child in parkContainer)
            {
                if (IsInstanceOfPrefab(child.gameObject, buildablePrefabs[prefabIndex]))
                {
                    layout.positions.Add(child.position);
                }
            }

            if (layout.positions.Count > 0) data.layouts.Add(layout);
        }

        return JsonUtility.ToJson(data);
    }

    // Load park layout from saved data
    public void LoadParkLayout(string jsonData)
    {
        ClearPark();
        ParkData data = JsonUtility.FromJson<ParkData>(jsonData);

        foreach (PrefabLayout layout in data.layouts)
        {
            if (layout.prefabIndex >= buildablePrefabs.Length) continue;

            GameObject prefab = buildablePrefabs[layout.prefabIndex];
            foreach (Vector3 pos in layout.positions)
            {
                Instantiate(prefab, pos, Quaternion.identity, parkContainer);
            }
        }
    }

    // Check if instance matches prefab (works in builds)
    private bool IsInstanceOfPrefab(GameObject instance, GameObject prefab)
    {
        return instance.name.StartsWith(prefab.name); // Simple name-based check
    }

    // Clear existing park objects
    public void ClearPark()
    {
        foreach (Transform child in parkContainer)
        {
            Destroy(child.gameObject);
        }
    }

    // Editor test button
    [ContextMenu("Print Layout JSON")]
    private void PrintLayout()
    {
        Debug.Log(SaveParkLayout());
    }
}
