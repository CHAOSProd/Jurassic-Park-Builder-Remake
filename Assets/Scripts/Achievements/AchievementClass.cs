using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementClass
{
    public enum Achievements
    {
        testach = 0
    }
    public Achievements achievement {get;set;}
    public bool isunlocked {get;set;}

    public AchievementClass(bool isunlocked_,Achievements achievements)
    {   
        this.achievement = achievements;
        this.isunlocked = isunlocked_;
        return;
    }
    
}
