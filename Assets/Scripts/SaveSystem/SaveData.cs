using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData 
{ 
    public List<PlaceableObjectData> PlaceableObjects { get; set; } = new List<PlaceableObjectData>();
    public List<HatchingData> HatchingData { get; set; } = new List<HatchingData>();
    public List<TreeData> TreeData { get; set; } = new List<TreeData>();
    public List<DebrisData> DebrisData { get; set; } = new List<DebrisData>();
    public List<AmberData> AmberData { get; set; } = new List<AmberData>();
    public List<AnimalShopData> AnimalShopData { get; set; } = new List<AnimalShopData>();
    public List<MoneyObjectData> MoneyData { get; set; } = new List<MoneyObjectData>();
    public List<RoadData> RoadData { get; set; } = new List<RoadData>();

    public Dictionary<string, object> Attributes { get; set; }
}
