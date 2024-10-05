using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TreeData : Data
{
    public int TreeInstanceID { get; set; }
    public bool Chopped { get; set; } = false;
    public bool Selectable { get; set; } = false;

    public TreeData(int treeObjectID)
    {
        this.TreeInstanceID = treeObjectID;
    }
}
