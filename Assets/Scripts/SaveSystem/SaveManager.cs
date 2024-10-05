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
        SaveData = SaveSystem.Load();

        Attributes.SetAttributes(SaveData.Attributes);
    }
    private void Start()
    {
        LoadPlaceableObjects();
        LoadChoppedTrees();
        CurrencySystem.Instance.Load();
        TreeChopManager.Instance.Load();
        ShopManager.Instance.InitalizeAnimals(SaveData.AnimalShopData);
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

        if(!Attributes.HaveKey("IsDefaultObjectInitialized")) {
            PlaceableObjectItem defaultPlaceableObjectItem = Resources.Load<PlaceableObjectItem>("Placeables/Triceratops");
            PlaceableObject placeableTriceratops = Instantiate(defaultPlaceableObjectItem.Prefab).GetComponent<PlaceableObject>();

            placeableTriceratops.Initialize(defaultPlaceableObjectItem, new PlaceableObjectData()
            {
                ItemName = "Triceratops",
                ConstructionFinished = true,
                Position = (0.2625f, 0f, 0f),
                SellRefund = 100,
                Progress = null,
                AnimalIndex = 0
            });

            placeableTriceratops.PlaceWithoutSave();
            SaveData.PlaceableObjects.Add(placeableTriceratops.data);

            Attributes.SetBool("IsDefaultObjectInitialized", true);
        }
    }
    private void LoadChoppedTrees()
    {
        if (SaveData.TreeData.Count == 0)
        {
            for(int i = 0; i < treesObject.transform.childCount; i++)
            {
                Transform tree = treesObject.transform.GetChild(i);

                TreeData td = new TreeData(tree.gameObject.GetInstanceID());
                SaveData.TreeData.Add(td);
                tree.GetComponent<TreeChopper>().SetData(td);
            }
        }
        else
        {
            foreach (TreeData td in SaveData.TreeData)
            {
                if (td.Chopped)
                {
                    DestroyImmediate(Resources.InstanceIDToObject(td.TreeInstanceID) as GameObject);
                    continue;
                }

                (Resources.InstanceIDToObject(td.TreeInstanceID) as GameObject).GetComponent<TreeChopper>().SetData(td);
            }
        }

        Dictionary<float, Dictionary<float, TreeChopper>> mappedTrees = new();
        for (int i = 0; i < treesObject.transform.childCount; i++)
        {
            Transform tree = treesObject.transform.GetChild(i);
            TreeChopper chopper = tree.GetComponent<TreeChopper>();

            //Take area of expansion
            BoundsInt treeArea = tree.GetComponent<TreeChopper>().Area;
            Vector3Int positionInt = GridBuildingSystem.Instance.GridLayout.LocalToCell(tree.position);
            treeArea.position = new Vector3Int(treeArea.position.x + positionInt.x, treeArea.position.y + positionInt.y, 0);

            GridBuildingSystem.Instance.TakeArea(treeArea);

            //Add trees to map
            if (mappedTrees.ContainsKey(tree.localPosition.y))
            {
                mappedTrees[tree.localPosition.y].Add(tree.localPosition.x, chopper);
            }
            else
            {
                mappedTrees.Add(tree.localPosition.y, new Dictionary<float, TreeChopper>()
                {
                    { tree.localPosition.x, chopper }
                });
            }

        }
        TreeChopManager.Instance.SetTreeMap(mappedTrees);
    }
    private void OnApplicationQuit()
    {
        Attributes.SetAttribute("LastSaveTime", DateTime.UtcNow);
        SaveData.Attributes = Attributes.Export();
        SaveData.AnimalShopData = ShopManager.Instance.GetAnimalShopData();

        SaveSystem.Save(SaveData);
    }
}