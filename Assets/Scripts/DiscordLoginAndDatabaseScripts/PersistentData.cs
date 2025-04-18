using UnityEngine;

public class PersistentData : MonoBehaviour
{
    public static PersistentData Instance;

    public string discordID;
    public string discordUsername;
    public bool isLoggedIn;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Public method to reset data
    public void ResetData()
    {
        discordID = string.Empty;
        discordUsername = string.Empty;
        isLoggedIn = false;
    }
}