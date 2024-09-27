using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[Serializable]
public class PlaceableObjectData : Data
{
    // Create class to make it nullable (to save less data)
    public class ProgressData
    {
        public int BuildTime { get; set; }
        public int ElapsedTime { get; set; }
        public DateTime LastTick { get; set; }
        public int XP { get; set; }

        public ProgressData(int buildTime, int elapsedTime, DateTime lastTick ,int xp)
        {
            BuildTime = buildTime;
            ElapsedTime = elapsedTime;
            LastTick = lastTick;
            XP = xp;
        }
    }
    public string ItemName { get; set;}
    public Vector3 Position { get; set; }
    public ProgressData Progress { get; set; }
    public bool ConstructionFinished { get; set; }
    public int SellRefund { get; set; }
}
