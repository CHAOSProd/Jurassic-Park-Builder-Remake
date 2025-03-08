using System;

[Serializable]
public class DebrisData
{
    public DebrisType DebrisType { get; set; }
    public (float x, float y, float z) Position { get; set; }
    public ProgressData Progress { get; set; }
    public bool Removed { get; set; }
    public bool HasAmber { get; set; }
    public int AmberIndex;

    public DebrisData(DebrisType debrisType, (float x, float y, float z) position, bool hasAmber)
    {
        this.DebrisType = debrisType;
        this.Position = position;
        this.HasAmber = hasAmber;
        AmberIndex = -1;
    }
}
