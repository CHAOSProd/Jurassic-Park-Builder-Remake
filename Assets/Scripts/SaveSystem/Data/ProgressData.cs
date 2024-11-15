using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Create class to make it nullable (to save less data)
[Serializable]
public class ProgressData
{
    public int BuildTime { get; set; }
    public int ElapsedTime { get; set; }
    public DateTime LastTick { get; set; }
    public int XP { get; set; }

    public ProgressData(int buildTime, int elapsedTime, DateTime lastTick, int xp)
    {
        BuildTime = buildTime;
        ElapsedTime = elapsedTime;
        LastTick = lastTick;
        XP = xp;
    }
}