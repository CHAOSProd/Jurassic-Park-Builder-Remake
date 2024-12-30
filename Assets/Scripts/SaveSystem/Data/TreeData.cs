using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TreeData : Data
{
    public int InstanceIndex { get; set; }
    public bool Chopped { get; set; } = false;
    public bool Selectable { get; set; } = false;
    public bool HasDebris { get; set; } = false;
    public ProgressData Progress { get; set; }
    public TreeData(int instanceIndex)
    {
        this.InstanceIndex = instanceIndex;
    }
}
