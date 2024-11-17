using System;
using System.Collections.Generic;
using Unity.Profiling;
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
        LoadDebris();
        CurrencySystem.Instance.Load();
        TreeChopManager.Instance.Load();
        DebrisManager.Instance.Load();
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

                TreeData td = new TreeData(i);
                SaveData.TreeData.Add(td);
                tree.GetComponent<TreeChopper>().SetData(td);
            }
        }
        else
        {
            List<GameObject> choppedTrees = new List<GameObject>();
            foreach (TreeData td in SaveData.TreeData)
            {
                if(td.Chopped)
                {
                    choppedTrees.Add(treesObject.transform.GetChild(td.InstanceIndex).gameObject);
                }
                else
                {
                    treesObject.transform.GetChild(td.InstanceIndex).GetComponent<TreeChopper>().SetData(td);
                }
            }

            foreach(GameObject go in choppedTrees)
            {
                DestroyImmediate(go);
            }
        }

        Dictionary<(int x, int y), TreeChopper> mappedTrees = new();
        for (int i = 0; i < treesObject.transform.childCount; i++)
        {
            Transform tree = treesObject.transform.GetChild(i);
            TreeChopper chopper = tree.GetComponent<TreeChopper>();

            //Take area of expansion
            BoundsInt treeArea = tree.GetComponent<TreeChopper>().Area;
            Vector3Int positionInt = GridBuildingSystem.Instance.GridLayout.LocalToCell(tree.position);
            treeArea.position = new Vector3Int(treeArea.position.x + positionInt.x, treeArea.position.y + positionInt.y, 0);

            GridBuildingSystem.Instance.TakeArea(treeArea);


            int mappedXPos = int.Parse((2f * tree.localPosition.x / TreeChopManager.Instance.CellSize.width).ToString());
            int mappedYPos = int.Parse((2f * tree.localPosition.y / TreeChopManager.Instance.CellSize.height).ToString());

            chopper.SetMappedPosition(mappedXPos, mappedYPos);

            mappedTrees.Add((mappedXPos, mappedYPos), chopper);
        }

        TreeChopManager.Instance.SetTreeMap(mappedTrees);
    }
    private void LoadDebris()
    {
        foreach(DebrisData dd in SaveData.DebrisData)
        {
            DebrisManager.Instance.LoadDebris(dd);
        }
    }
    private void OnApplicationQuit()
    {
        Attributes.SetAttribute("LastSaveTime", DateTime.UtcNow);
        SaveData.Attributes = Attributes.Export();
        SaveData.AnimalShopData = ShopManager.Instance.GetAnimalShopData();

        SaveSystem.Save(SaveData);
    }
}