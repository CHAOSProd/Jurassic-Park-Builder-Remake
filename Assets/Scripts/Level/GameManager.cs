using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{

    public GameObject canvas;

    public void GetXP(int amount)
    {
        XPAddedGameEvent info = new XPAddedGameEvent(amount);
        EventManager.Instance.QueueEvent(info);
    }

    public void AddXPTest()
    {
        GetXP(25);
        Debug.Log("added 25 XP!");
    }
    public void ChangeLVLTest()
    {
        new LevelChangedGameEvent(1);
        Debug.Log("Level changed from 1 to 2!");
    }
}

