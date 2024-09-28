using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SaveManager : Singleton<SaveManager>
{

    public SaveData SaveData;
    [SerializeField] private string placeablesPath = "Placeables";
    [SerializeField] private GameObject treesObject;

    private void Awake()
    {
        SaveSystem.Initialize();
    }

    private void Start()
    {
        SaveData = SaveSystem.Load();

        LoadGame();
    }

    private void LoadGame()
    {
        LoadPlaceableObjects();
        LoadChoppedTrees();
        CurrencySystem.Instance.Load();
        TreeChopManager.Instance.Load();
        SelectablesManager.Instance.InitializeUI();
    }

    private void LoadPlaceableObjects()
    {
        foreach (PlaceableObjectData placeableObjectData in SaveData.PlaceableObjects) 
        {
            PlaceableObjectItem placeableObjectItem = Resources.Load<PlaceableObjectItem>(placeablesPath + "/" + placeableObjectData.ItemName);
            Debug.Log(placeablesPath + "/" + placeableObjectData.ItemName);
            GameObject obj = Instantiate(placeableObjectItem.Prefab, Vector3.zero, Quaternion.identity);

            PlaceableObject placeableObject = obj.GetComponent<PlaceableObject>();

            placeableObject.Initialize(placeableObjectItem, placeableObjectData);

            placeableObject.PlaceWithoutSave();
        }
    }
    private void LoadChoppedTrees()
    {
        foreach (ChoppedTreeData ctd in SaveData.ChoppedTrees)
        {
            // Immediately destroys the tree game object, so we can use treesObject.transform.childCount correctly
            DestroyImmediate(Resources.InstanceIDToObject(ctd.TreeInstanceID) as GameObject);
        }
        for (int i = 0; i < treesObject.transform.childCount; i++)
        {
            Transform tree = treesObject.transform.GetChild(i);

            Vector3Int positionInt = GridBuildingSystem.Instance.GridLayout.LocalToCell(tree.position);
            positionInt -= new Vector3Int((int)tree.lossyScale.x / 2, (int)tree.lossyScale.y / 2);

            BoundsInt areaTemp = new BoundsInt(Vector3Int.zero, Vector3Int.one * (int)tree.lossyScale.x);
            areaTemp.position = positionInt;
            tree.position = GridBuildingSystem.Instance.GridLayout.CellToLocalInterpolated(positionInt);

            GridBuildingSystem.Instance.TakeArea(areaTemp);
        }
    }
    private void OnApplicationQuit()
    {
        SaveSystem.Save(SaveData);
        Utils.SetDateTime("LastSaveTime", DateTime.UtcNow);
        PlayerPrefs.Save();
    }
}