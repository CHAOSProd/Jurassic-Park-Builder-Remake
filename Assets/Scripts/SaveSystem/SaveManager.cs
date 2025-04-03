using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveManager : Singleton<SaveManager>
{
    public SaveData SaveData;
    [SerializeField] private string placeablesPath = "Placeables";
    [SerializeField] private GameObject treesObject;

    private void Awake()
    {
        // Initialize the save system
        SaveSystem.Initialize();

        // Check for reset flag and delete the save file and RoadData if needed
        if (PlayerPrefs.GetInt("DeleteSaveOnStart", 0) == 1)
        {
            Debug.Log("Deleting save file on startup...");

            if (File.Exists(SaveSystem.FilePath))
            {
                File.Delete(SaveSystem.FilePath);
                Debug.Log("File deleted successfully");
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
            }
            else
            {
                Debug.LogWarning("No save file found.");
            }

            // Clear RoadData if it already exists (if not, it will be initialized later)
            if (SaveData != null && SaveData.RoadData != null)
            {
                SaveData.RoadData.Clear();
                Debug.Log("Cleared RoadData on startup.");
            }

            PlayerPrefs.SetInt("DeleteSaveOnStart", 0);
            PlayerPrefs.Save();
        }

        // Load saved data (or create new data if no file exists)
        SaveData = SaveSystem.Load();
        Attributes.SetAttributes(SaveData.Attributes);
        int futureSaveCount = PlayerPrefs.GetInt("FutureSaveCount", 0);
        if (Attributes.HaveKey("LastSaveTime"))
        {
            DateTime lastSaveTime = Attributes.GetAttribute("LastSaveTime", DateTime.MinValue);
            DateTime now = DateTime.Now;
            if (lastSaveTime > now)
            {
                futureSaveCount++;
                if (futureSaveCount >= 2) 
                {
                    Debug.LogWarning("Cheated after the warning, deleting data...");
                    PlayerPrefs.SetInt("DeleteSaveOnStart", 1);
                    PlayerPrefs.Save();
                    #if UNITY_EDITOR
                    Debug.Log("Closing the game in the Unity Editor.");
                    UnityEditor.EditorApplication.isPlaying = false;
                    #else
                    Debug.Log("Closing the game in build.");
                    Application.Quit();
                    #endif
                }
                else
                {
                    Debug.LogWarning("Last save time is in the future! Next times cheating, the data will be deleted");
                    PlayerPrefs.SetInt("FutureSaveCount", futureSaveCount);
                    PlayerPrefs.Save();
                }
            }
        }
        else
        {
            Debug.Log("No previous save time found.");
        }

        // Ensure that RoadData is initialized so it works with the RoadData class
        if (SaveData.RoadData == null)
        {
            SaveData.RoadData = new List<RoadData>();
        }
    }

    private void Start()
    {
        LoadPlaceableObjects();
        LoadChoppedTrees();
        LoadDebris();
        CurrencySystem.Instance.Load();
        TreeChopManager.Instance.Load();
        DebrisManager.Instance.Load();
        ResearchManager.Instance.Load();
        AmberManager.Instance.Load();
        ShopManager.Instance.InitalizeAnimals(SaveData.AnimalShopData);
    }

    private void LoadPlaceableObjects()
    {
        int placableIndex = 0;
        int moneyIndex = 0;

        if (!Attributes.HaveKey("IsDefaultObjectInitialized"))
        {
            PlaceableObjectItem defaultPlaceableObjectItem = Resources.Load<PlaceableObjectItem>("Placeables/Triceratops");
            PlaceableObject placeableTriceratops = Instantiate(defaultPlaceableObjectItem.Prefab).GetComponent<PlaceableObject>();

            SaveData.MoneyData.Add(new MoneyObjectData(0, 0));
            placeableTriceratops.GetComponentInChildren<MoneyObject>(true).Initialise(SaveData.MoneyData[0]);

            placeableTriceratops.Initialize(defaultPlaceableObjectItem, new PlaceableObjectData()
            {
                ItemName = "Triceratops",
                ConstructionFinished = true,
                Position = (0.2625f, 0f, 0f),
                SellRefund = 100,
                Progress = null,
                AnimalIndex = 0
            });

            string parentName = placeableTriceratops.gameObject.name;

            var levelManager = placeableTriceratops.GetComponentInChildren<DinosaurLevelManager>();
            if (levelManager != null)
            {
                Attributes.SetInt("CurrentLevel" + parentName, 4);
                levelManager.CurrentLevel = 4;
            }

            var feedingSystem = placeableTriceratops.GetComponentInChildren<DinosaurFeedingSystem>();
            if (feedingSystem != null)
            {
                Attributes.SetInt("FeedCount" + parentName, 2);
                feedingSystem.feedCount = 2;
            }

            HatchingTimer hatchingTimer = placeableTriceratops.GetComponentInChildren<HatchingTimer>(true);
            MoneyObject moneyObject = placeableTriceratops.GetComponentInChildren<MoneyObject>(true);

            if (placeableTriceratops._isPaddock)
            {
                hatchingTimer.InitializeTriceratops();
                if (moneyObject != null)
                {
                    moneyObject.Initialise(new MoneyObjectData(0, 0) { Money = (int)moneyObject.MaximumMoney });
                }
            }

            placeableTriceratops.PlaceWithoutSave();
            SaveData.PlaceableObjects.Add(placeableTriceratops.data);

            Attributes.SetBool("IsDefaultObjectInitialized", true);

            placableIndex++;
            moneyIndex++;
        }

        for (; placableIndex < SaveData.PlaceableObjects.Count; placableIndex++)
        {
            PlaceableObjectData placeableObjectData = SaveData.PlaceableObjects[placableIndex];

            PlaceableObjectItem placeableObjectItem = Resources.Load<PlaceableObjectItem>(placeablesPath + "/" + placeableObjectData.ItemName);
            GameObject obj = Instantiate(placeableObjectItem.Prefab, Vector3.zero, Quaternion.identity);

            PlaceableObject placeableObject = obj.GetComponent<PlaceableObject>();

            placeableObject.Initialize(placeableObjectItem, placeableObjectData);
            SaveData.MoneyData[placableIndex].PlaceableObjectIndex = placableIndex;

            placeableObject.PlaceWithoutSave();

            HatchingTimer hatchingTimer = obj.GetComponentInChildren<HatchingTimer>(true);
            PaddockInfo paddockInfo = placeableObject.GetComponentInChildren<PaddockInfo>(true);
            HatchingData matchingHatchingData = null;
            if (placeableObject._isPaddock)
            {
                matchingHatchingData = SaveData.HatchingData.Find(hd => hd.DinoName == paddockInfo._dinosaurName);
            }
            if (placeableObject._isPaddock && matchingHatchingData != null)
            {
                hatchingTimer.Load(matchingHatchingData);
            }
            if (placeableObject._construction || (placeableObject._isPaddock && hatchingTimer != null && hatchingTimer.paddockScript.hatching_completed))
            {
                MoneyObject moneyObject = obj.GetComponentInChildren<MoneyObject>(true);
                if (moneyObject != null && moneyIndex < SaveData.MoneyData.Count && SaveData.MoneyData[moneyIndex].PlaceableObjectIndex == placableIndex)
                {
                    moneyObject.Initialise(SaveData.MoneyData[moneyIndex]);
                    moneyIndex++;
                }
                else
                {
                    moneyObject.Initialise(new MoneyObjectData(placableIndex, 0));
                }
            }
        }
    }

    private void LoadChoppedTrees()
    {
        if (SaveData.TreeData.Count == 0)
        {
            for (int i = 0; i < treesObject.transform.childCount; i++)
            {
                Transform tree = treesObject.transform.GetChild(i);
                TreeData td = new TreeData(i)
                {
                    ChopTime = tree.GetComponent<TreeChopper>().chopTime
                };
                SaveData.TreeData.Add(td);
                tree.GetComponent<TreeChopper>().SetData(td);
            }
        }
        else
        {
            List<GameObject> choppedTrees = new List<GameObject>();
            foreach (TreeData td in SaveData.TreeData)
            {
                var treeChopper = treesObject.transform.GetChild(td.InstanceIndex).GetComponent<TreeChopper>();
                treeChopper.SetData(td);
                treeChopper.chopTime = td.ChopTime;

                if (td.HasDebris)
                {
                    treeChopper.EnableDebris();
                }

                if (td.Chopped)
                {
                    choppedTrees.Add(treesObject.transform.GetChild(td.InstanceIndex).gameObject);
                }
                else
                {
                    treesObject.transform.GetChild(td.InstanceIndex).GetComponent<TreeChopper>().SetData(td);
                }
            }
            foreach (GameObject go in choppedTrees)
            {
                DestroyImmediate(go);
            }
        }

        Dictionary<(int x, int y), TreeChopper> mappedTrees = new Dictionary<(int, int), TreeChopper>();
        for (int i = 0; i < treesObject.transform.childCount; i++)
        {
            Transform tree = treesObject.transform.GetChild(i);
            TreeChopper chopper = tree.GetComponent<TreeChopper>();
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
        foreach (DebrisData dd in SaveData.DebrisData)
        {
            DebrisManager.Instance.LoadDebris(dd);
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveGameData();
        }
    }

    private void OnApplicationQuit()
    {
        SaveGameData();
    }

   private void SaveGameData()
{
    // Update dinosaur levels for all prefab clones
    DinosaurLevelSaver.SaveAllDinosaurLevels();
    Attributes.SetAttribute("LastSaveTime", DateTime.Now);
    SaveData.Attributes = Attributes.Export();
    SaveData.AnimalShopData = ShopManager.Instance.GetAnimalShopData();
    SaveSystem.Save(SaveData);
}

}

