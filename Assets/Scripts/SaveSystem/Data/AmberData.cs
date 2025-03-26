using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class AmberData
{
    public int Index { get; private set; }
    public bool IsActivated { get; private set; }
    public bool IsDecoded { get; private set; }

    public AmberData(int index, bool isActivated = false, bool isDecoded = false)
    {
        Index = index;
        IsActivated = isActivated;
        IsDecoded = isDecoded;
    }
    public void Activate()
    {
        IsActivated = true;
    }
    public void SetDecoded(bool decoded)
    {
        IsDecoded = decoded;
    }
}
