using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class AmberData
{
    public int Index { get; private set; }
    public bool IsActivated { get; private set; }

    public AmberData(int index, bool isActivated = false)
    {
        Index = index;
        IsActivated = isActivated;
    }
    public void Activate()
    {
        IsActivated = true;
    }
}
