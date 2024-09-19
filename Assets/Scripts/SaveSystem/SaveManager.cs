using System;
using System.Collections.Generic;
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
    }

    private void LoadPlaceableObjects()
    {
        foreach (PlaceableObjectData placeableObjectData in SaveData.PlaceableObjects) 
        {
            PlaceableObjectItem placeableObjectItem = Resources.Load<PlaceableObjectItem>(placeablesPath + "/" + placeableObjectData.ItemName);
            GameObject obj = Instantiate(placeableObjectItem.Prefab, Vector3.zero, Quaternion.identity);

            PlaceableObject placeableObject = obj.GetComponent<PlaceableObject>();

            placeableObject.Initialize(placeableObjectItem, placeableObjectData);

            placeableObject.PlaceWithoutSave();
        }
    }
    private void LoadChoppedTrees()
    {
        for(int i = 0;i < treesObject.transform.childCount; i++)
        {
            GameObject trees = treesObject.transform.GetChild(i).gameObject;
            foreach (ChoppedTreeData ctd in SaveData.ChoppedTrees)
            {
                if (trees.name == ctd.TreeObjectName)
                {
                    Destroy(trees);
                    break;
                }
            }
        }
    }
    private void OnApplicationQuit()
    {
        SaveSystem.Save(SaveData);
        Utils.SetDateTime("LastSaveTime", DateTime.UtcNow);
        PlayerPrefs.Save();
    }
}