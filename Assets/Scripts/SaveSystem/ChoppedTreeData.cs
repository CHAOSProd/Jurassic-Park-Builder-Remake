using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ChoppedTreeData : Data
{
    public int TreeInstanceID { get; set; }

    public ChoppedTreeData(int treeObjectName)
    {
        this.TreeInstanceID = treeObjectName;
    }
}
