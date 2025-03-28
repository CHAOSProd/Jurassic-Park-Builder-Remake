using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TutorialDebrisSpawner : MonoBehaviour
{
    public Tilemap tilemap;
    [SerializeField] private GameObject smallGrassPrefab;
    [SerializeField] private Transform debrisParent;

    public static bool tutorialDebrisSpawned;
    Vector2[] possiblePositions = new Vector2[]
    {
        new Vector2(0.92f, 0.07f),
        new Vector2(-0.66f, 0.21f),
        new Vector2(0.00f, 0.01f),
        new Vector2(0.13f, -0.32f),
        new Vector2(0.39f, -0.06f),
        new Vector2(0.79f, 0.01f),
        new Vector2(1.05f, 0.14f),
        new Vector2(-0.13f, -0.19f)
    };

    private void Start()
    {
        if (!tutorialDebrisSpawned)
        {
            SpawnSmallGrassDebris();
            tutorialDebrisSpawned = true;
            Debug.Log($"Tutorial debris spawn set to: {tutorialDebrisSpawned}");
            ResearchManager.Instance.SaveResearchProgress();
        }
    }

    private void SpawnSmallGrassDebris()
    {
        if (smallGrassPrefab == null)
        {
            Debug.LogError("SmallGrass not assigned");
            return;
        }

        Vector2 chosenPosition = possiblePositions[Random.Range(0, possiblePositions.Length)];
        
        int size = 2;

        GameObject debrisObject = Instantiate(smallGrassPrefab, chosenPosition, Quaternion.identity, debrisParent);

        if (debrisObject.TryGetComponent(out DebrisObject debris))
        {
            debris.Initialize(size, DebrisType.SmallGrass, true);
            debris._removeTime = 5;
        }  

        Debug.Log($"Tutorial debris spawned at position: {chosenPosition}");
    }
}