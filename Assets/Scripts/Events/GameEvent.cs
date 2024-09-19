using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameEvent
{

}

public struct XPAddedGameEvent
{
    public float Amount { get; set; }

    public XPAddedGameEvent(float amount)
    {
        this.Amount = amount;
    }
}

public struct LevelChangedGameEvent
{
    public int NewLVL { get; set; }

    public LevelChangedGameEvent(int currLvl)
    {
        NewLVL = currLvl;
    }
}