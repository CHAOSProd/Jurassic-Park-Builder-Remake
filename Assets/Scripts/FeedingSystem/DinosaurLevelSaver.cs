using UnityEngine;

public class DinosaurLevelSaver : MonoBehaviour
{
    /// <summary>
    /// Finds every DinosaurLevelManager in the scene and saves its current level 
    /// to the Attributes dictionary using a key formatted as "CurrentLevel" + paddock name.
    /// </summary>
    public static void SaveAllDinosaurLevels()
    {
        DinosaurLevelManager[] dinoManagers = GameObject.FindObjectsOfType<DinosaurLevelManager>();
        foreach (DinosaurLevelManager manager in dinoManagers)
        {
            Paddock paddock = manager.GetComponentInParent<Paddock>();
            DinosaurFeedingSystem feedingSystem = manager.GetComponent<DinosaurFeedingSystem>();
            if (paddock != null && feedingSystem != null)
            {
                string levelKey = "CurrentLevel" + paddock.gameObject.name;
                string feedCountKey = "FeedCount" + paddock.gameObject.name;

                Attributes.SetInt(levelKey, manager.CurrentLevel);
                Attributes.SetInt(feedCountKey, feedingSystem.feedCount);

                Debug.Log($"Saved dinosaur level for {paddock.gameObject.name}: Level {manager.CurrentLevel}, FeedCount: {feedingSystem.feedCount}");
            }
            else
            {
                Debug.LogWarning($"DinosaurLevelManager on {manager.gameObject.name} did not find a parent Paddock or FeedingSystem.");
            }
        }
    }
}

