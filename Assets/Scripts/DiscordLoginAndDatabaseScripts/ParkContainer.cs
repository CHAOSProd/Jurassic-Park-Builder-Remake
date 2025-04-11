using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class ParkContainer : MonoBehaviour
{
    [System.Serializable]
    public class ParkState
    {
        public string parkName;
        public List<ParkItem> items = new List<ParkItem>();
    }

    [System.Serializable]
    public class ParkItem
    {
        public int prefabIndex;
        public Vector3 position;
        public Quaternion rotation;
    }

    [Header("Settings")]
    public GameObject[] buildablePrefabs;
    public Transform container;

    private ParkState currentPark;
    private ParkState savedPark;

    void Awake()
    {
        InitializeNewPark();
    }

    public void EnterVisitingMode(string friendParkData)
    {
        // Save current state
        savedPark = currentPark;
        
        // Load friend's park
        LoadParkFromData(friendParkData);
    }

    public void ExitVisitingMode()
    {
        // Restore original park
        if(savedPark != null)
        {
            LoadParkFromData(JsonUtility.ToJson(savedPark));
            savedPark = null;
        }
    }

    public void PlaceItem(GameObject prefab, Vector3 position)
    {
        int prefabIndex = System.Array.IndexOf(buildablePrefabs, prefab);
        if(prefabIndex == -1) return;

        currentPark.items.Add(new ParkItem {
            prefabIndex = prefabIndex,
            position = position,
            rotation = Quaternion.identity
        });

        Instantiate(prefab, position, Quaternion.identity, container);
    }

    public string GetCurrentParkData()
    {
        return JsonUtility.ToJson(currentPark);
    }

    public void LoadParkFromData(string jsonData)
    {
        ClearPark();
        currentPark = JsonUtility.FromJson<ParkState>(jsonData);

        foreach(ParkItem item in currentPark.items)
        {
            if(item.prefabIndex >= 0 && item.prefabIndex < buildablePrefabs.Length)
            {
                Instantiate(
                    buildablePrefabs[item.prefabIndex],
                    item.position,
                    item.rotation,
                    container
                );
            }
        }
    }

    void InitializeNewPark()
    {
        currentPark = new ParkState {
            parkName = "My Park",
            items = new List<ParkItem>()
        };
    }

    void ClearPark()
    {
        currentPark.items.Clear();
        foreach(Transform child in container)
        {
            Destroy(child.gameObject);
        }
    }

    // Editor tools
    [ContextMenu("Save Temporary Park")]
    void SaveTempPark()
    {
        PlayerPrefs.SetString("TempPark", GetCurrentParkData());
        Debug.Log("Park saved to temporary storage");
    }

    [ContextMenu("Load Temporary Park")]
    void LoadTempPark()
    {
        LoadParkFromData(PlayerPrefs.GetString("TempPark"));
    }
}