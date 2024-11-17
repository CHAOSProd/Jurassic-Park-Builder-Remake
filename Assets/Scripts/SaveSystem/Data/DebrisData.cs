using System;

[Serializable]
public class DebrisData
{
    public DebrisType DebrisType { get; set; }
    public (float x, float y, float z) Position { get; set; }
    public ProgressData Progress { get; set; }
    public bool Removed { get; set; }

    public DebrisData(DebrisType debrisType, (float x, float y, float z) position)
    {
        this.DebrisType = debrisType;
        this.Position = position;
    }
}
