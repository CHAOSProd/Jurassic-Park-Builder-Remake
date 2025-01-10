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
            Debug.Log($"Item {gameObject.name} was unlocked with bucks. Visibility update skipped.");
            return;
        }
        bool isActive = currentLevel == requiredLevel;
        Debug.Log($"Item {gameObject.name}: Required Level = {requiredLevel}, Current Level = {currentLevel}, Active = {isActive}");
        gameObject.SetActive(isActive);
    }
}
