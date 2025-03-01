using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class AmberData
{
    public int Index { get; private set; }

    public AmberData(int index)
    {
        Index = index;
    }
}
