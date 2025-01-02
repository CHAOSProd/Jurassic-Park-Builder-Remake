using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelReqItem : MonoBehaviour
{
    [Header("Item Details")]
    [SerializeField] public int requiredLevel;
    public void UpdateItemVisibility(int currentLevel)
    {
        bool isActive = currentLevel == requiredLevel;
        Debug.Log($"Item {gameObject.name}: Required Level = {requiredLevel}, Current Level = {currentLevel}, Active = {isActive}");
        gameObject.SetActive(isActive);
    }
}
