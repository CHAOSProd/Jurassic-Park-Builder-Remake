using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Create class to make it nullable (to save less data)
[Serializable]
public class ProgressData
{
    public int ElapsedTime { get; set; }
    public DateTime LastTick { get; set; }

    public ProgressData(int elapsedTime, DateTime lastTick)
    {
        ElapsedTime = elapsedTime;
        LastTick = lastTick;
    }
}