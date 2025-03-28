using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementApi : MonoBehaviour
{

    public void init()
    {
        Debug.LogWarning("DONT CALL ME IF THE SAVE IS ALREADY LOADED WITH VALID DATA");

        if (SaveData.AchievementData.Keys.Count > 0)
        {
            Debug.Log("overwrite att cathced");
            return;
        }
        
        for (int i = 0; i < Enum.GetValues(typeof(AchievementClass.Achievements)).Length; i++)
        {
            SaveData.AchievementData.Add((AchievementClass.Achievements)i,false);
        }
        return;
    }
    public void setachievementstate(bool state,AchievementClass.Achievements achievement)
    {
        if (SaveData.AchievementData.Keys.Count < 1)
        {
            Debug.Log("null");
            return;
        }

        foreach (var item in SaveData.AchievementData) //ugly
        {
            if (item.Key == achievement)
            {
                SaveData.AchievementData.Remove(item.Key);
                SaveData.AchievementData.Add(achievement,state);
                break;
            }
        }
        return;
    }
}
