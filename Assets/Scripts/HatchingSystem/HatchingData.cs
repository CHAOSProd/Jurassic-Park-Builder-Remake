using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HatchingData : Data
{
    public string DinoName {  get; set; }
    public bool HatchingFinished { get; set; }
    public bool isHatching { get; set; }
    public ProgressData HatchingProgress { get; set; }
}
