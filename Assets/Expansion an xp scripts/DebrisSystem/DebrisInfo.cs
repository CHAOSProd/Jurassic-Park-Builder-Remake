using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameObjects/DebrisInfo",fileName = "debrisInfo", order = 1)]
public class DebrisInfo : ScriptableObject
{
    public int AreaLength; // In Tiles
    public GameObject Prefab;
}
public enum DebrisType
{
    SmallGrass,
    SmallStones,
    MediumStones,
    Swamp,
    Marsh,
    BigStones,
    BigGrassWithTree,
    GiantStones
}