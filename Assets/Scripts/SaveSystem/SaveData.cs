using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData 
{ 
    public List<PlaceableObjectData> PlaceableObjects { get; set; } = new List<PlaceableObjectData>();
    public List<TreeData> TreeData { get; set; } = new List<TreeData>();
    public List<AnimalShopData> AnimalShopData { get; set; } = new List<AnimalShopData>();
    public Dictionary<string, object> Attributes { get; set; }
}
