using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    // Persist across scenes
    public GameObject canvas;

    // New fields for login data
    public string discordID;
    public string discordUsername;
    public bool isLoggedIn = false;

    void Awake()
    {
        // If PersistentData exists, load the saved login data.
        if (PersistentData.Instance != null)
        {
            discordID = PersistentData.Instance.discordID;
            discordUsername = PersistentData.Instance.discordUsername;
            isLoggedIn = PersistentData.Instance.isLoggedIn;

            if (isLoggedIn)
            {
                Debug.Log("GameManager loaded login data: " + discordUsername);
            }
            else
            {
                Debug.Log("No login data found in PersistentData.");
            }
        }
        else
        {
            Debug.LogWarning("PersistentData instance not found. Make sure it is in your initial scene and marked DontDestroyOnLoad.");
        }
    }

    // Example functions for game events
    public void GetXP(float amount)
    {
        EventManager.Instance.TriggerEvent(new XPAddedGameEvent(amount));
    }

    public void AddXPTest()
    {
        GetXP(25);
        Debug.Log("added 25 XP!");
    }

    public void ChangeLVLTest()
    {
        new LevelChangedGameEvent(1);
        Debug.Log("Level changed from 1 to 2!");
    }

    // Call this once the login is successful to update both GameManager and PersistentData
    public void SetLoginData(string id, string username)
    {
        discordID = id;
        discordUsername = username;
        isLoggedIn = true;

        if (PersistentData.Instance != null)
        {
            PersistentData.Instance.discordID = id;
            PersistentData.Instance.discordUsername = username;
            PersistentData.Instance.isLoggedIn = true;
        }

        Debug.Log($"Logged in as: {discordUsername}");
    }
}


