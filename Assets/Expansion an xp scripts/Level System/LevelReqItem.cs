using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelReqItem : MonoBehaviour
{
    [Header("Item Details")]
    [SerializeField] public int requiredLevel;
    [SerializeField] private string saveName;
    public void UpdateItemVisibility(int currentLevel)
    {
        bool isUnlockedWithBucks = Attributes.GetBool(saveName + "_bucks", false);
        if (isUnlockedWithBucks)
        {
            gameObject.SetActive(false);
            return;
        }
        bool isActive = currentLevel == requiredLevel;
        gameObject.SetActive(isActive);
    }
}
