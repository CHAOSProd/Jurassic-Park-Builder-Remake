using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[Serializable]
public class PlaceableObjectData : Data
{
    public string ItemName { get; set;}
    public (float x, float y, float z) Position { get; set; }
    public ProgressData Progress { get; set; }
    public bool ConstructionFinished { get; set; }
    public bool ConstructionReady { get; set; }
    public int SellRefund { get; set; }
    public int? AnimalIndex { get; set; }
}
