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
            // Get the parent paddock from which to derive a unique key.
            Paddock paddock = manager.GetComponentInParent<Paddock>();
            if (paddock != null)
            {
                string key = "CurrentLevel" + paddock.gameObject.name;
                Attributes.SetInt(key, manager.CurrentLevel);
                Debug.Log("Saved dinosaur level for " + paddock.gameObject.name + ": " + manager.CurrentLevel);
            }
            else
            {
                Debug.LogWarning("DinosaurLevelManager on " + manager.gameObject.name + " did not find a parent Paddock.");
            }
        }
    }
}

