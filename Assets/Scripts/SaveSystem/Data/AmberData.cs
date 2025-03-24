using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class AmberData
{
    public int Index { get; private set; }
    public bool IsActivated { get; private set; }
    public int LastDecodedIndex { get; private set; }
    public bool IsDecoded { get; private set; }

    public AmberData(int index, bool isActivated = false, int lastDecodedIndex = -1, bool isDecoded = false)
    {
        Index = index;
        IsActivated = isActivated;
        LastDecodedIndex = lastDecodedIndex;
        IsDecoded = isDecoded;
    }
    public void Activate()
    {
        IsActivated = true;
    }
    public void SetLastDecodedIndex(int index)
    {
        LastDecodedIndex = index;
    }
    public void SetDecoded(bool decoded)
    {
        IsDecoded = decoded;
    }
}
