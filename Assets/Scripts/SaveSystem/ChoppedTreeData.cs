using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoppedTreeData : Data
{
    public string TreeObjectName { get; set; }

    public ChoppedTreeData(string treeObjectName)
    {
        this.TreeObjectName = treeObjectName;
    }
}
