using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData 
{ 
    public List<PlaceableObjectData> PlaceableObjects { get; set; } = new List<PlaceableObjectData>();
    public List<ChoppedTreeData> ChoppedTrees { get; set; } = new List<ChoppedTreeData>();

    public Dictionary<string, object> Attributes { get; set; }
}
