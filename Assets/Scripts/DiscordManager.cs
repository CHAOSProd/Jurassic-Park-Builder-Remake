using UnityEngine;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PanelData
{
    public GameObject panelObject;
    public string activityText;
}

public class DiscordManager : MonoBehaviour
{
    public static string discordUsername = "UnknownPlayer";
    public static string discordAvatarUrl = "";
    public static string discordUserId = "";

    private Discord.Discord discord;
    private long startTimestamp;
    public TMP_Text levelText;
    private string previousLevelText;

    [Header("Panel Configuration")]
    public PanelData[] panelData;

    private Dictionary<GameObject, string> panelDictionary;
    private List<GameObject> panelList = new List<GameObject>();

    void Start()
    {
        try
        {
            // Use NoRequireDiscord here if you want the game to run even without a Discord client,
            // but note that full relationship data might not be available then.
            discord = new Discord.Discord(1342685934776221786, (ulong)Discord.CreateFlags.NoRequireDiscord);
        }
        catch (Exception e)
        {
            Debug.LogError($"Discord initialization failed: {e.Message}");
            return;
        }

        previousLevelText = levelText != null ? levelText.text : string.Empty;
        startTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        InitializePanels();
        InitializeDiscord();
    }

    void InitializePanels()
    {
        panelDictionary = new Dictionary<GameObject, string>();
        foreach (var data in panelData)
        {
            if (data.panelObject != null)
            {
                if (!panelDictionary.ContainsKey(data.panelObject))
                {
                    panelDictionary.Add(data.panelObject, data.activityText);
                    panelList.Add(data.panelObject);
                    Debug.Log($"Registered panel: {data.panelObject.name}");
                }
                else
                {
                    Debug.LogWarning($"Duplicate panel found: {data.panelObject.name}");
                }
            }
        }
    }

    void InitializeDiscord()
    {
        if (discord == null) return;

        try
        {
            var userManager = discord.GetUserManager();
            userManager.OnCurrentUserUpdate += UpdateUserInfo;

            // Subscribe to relationship updates to capture friend list updates.
            var relationshipManager = discord.GetRelationshipManager();
            relationshipManager.OnRelationshipUpdate += OnRelationshipUpdate;
            
            // Initial activity update.
            UpdateActivity();
        }
        catch (Exception e)
        {
            Debug.LogError($"Discord initialization error: {e.Message}");
        }
    }

    void OnDestroy()
    {
        if (discord != null)
        {
            discord.Dispose();
        }
    }

    void Update()
    {
        if (discord == null) return;

        try
        {
            discord.RunCallbacks();
        }
        catch (Exception e)
        {
            Debug.LogError($"Discord callback error: {e.Message}");
        }

        // Check for level text changes.
        if (levelText != null && levelText.text != previousLevelText)
        {
            previousLevelText = levelText.text;
            UpdateActivity();
        }

        // Continuous panel activity check.
        UpdateActivity();

        // Optionally, for debugging, press F (for example) to query and log the friend list.
        if (Input.GetKeyDown(KeyCode.F))
        {
            QueryFriendsList();
        }
    }

    void UpdateActivity()
    {
        if (discord == null) return;

        var activityManager = discord.GetActivityManager();
        var activity = new Discord.Activity();

        // Find first active panel.
        GameObject activePanel = null;
        foreach (var panel in panelList)
        {
            if (panel != null && panel.activeInHierarchy)
            {
                activePanel = panel;
                break;
            }
        }

        // Set activity details based on active panel.
        if (activePanel != null)
        {
            if (panelDictionary.TryGetValue(activePanel, out string panelText))
            {
                activity.Details = panelText;
            }
            else
            {
                activity.Details = $"Using {activePanel.name}";
            }
        }
        else
        {
            activity.Details = "In Jurassic Park";
        }

        // Set activity state.
        activity.State = "Level " + previousLevelText;
        activity.Timestamps = new Discord.ActivityTimestamps
        {
            Start = startTimestamp
        };

        // Update activity.
        try
        {
            activityManager.UpdateActivity(activity, result =>
            {
                if (result != Discord.Result.Ok)
                {
                    Debug.LogError($"Activity update failed: {result}");
                }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Activity update error: {e.Message}");
        }
    }

    private void UpdateUserInfo()
    {
        if (discord == null) return;

        try
        {
            var userManager = discord.GetUserManager();
            var currentUser = userManager.GetCurrentUser();

            if (currentUser.Id == 0)
            {
                Debug.LogWarning("No valid Discord user");
                return;
            }

            discordUsername = currentUser.Username;
            discordUserId = currentUser.Id.ToString();

            // Update avatar URL. If there's no avatar, use default based on discriminator.
            discordAvatarUrl = !string.IsNullOrEmpty(currentUser.Avatar)
                ? $"https://cdn.discordapp.com/avatars/{currentUser.Id}/{currentUser.Avatar}.png"
                : GetDefaultAvatar(currentUser.Discriminator);

            Debug.Log($"Discord user updated: {discordUsername}");
        }
        catch (Exception e)
        {
            Debug.LogError($"User update error: {e.Message}");
        }
    }

    private string GetDefaultAvatar(string discriminator)
    {
        int index = 0;
        if (ushort.TryParse(discriminator, out ushort result))
        {
            index = result % 5;
        }
        return $"https://cdn.discordapp.com/embed/avatars/{index}.png";
    }

    // This callback is triggered when the relationship data is updated.
    private void OnRelationshipUpdate(ref Discord.Relationship relationship)
    {
        // For now, simply log the relationship update.
        if (relationship.Type == Discord.RelationshipType.Friend)
        {
            Debug.Log($"Relationship update for friend: {relationship.User.Username}");
        }
    }

    // Method to query the friend list and log friend data.
    void QueryFriendsList()
    {
        if (discord == null) return;

        var relationshipManager = discord.GetRelationshipManager();

        int count = 0;
        try
        {
            count = (int)relationshipManager.Count();
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Error getting relationship count: " + ex.Message);
            return;
        }

        Debug.Log("Total relationships reported: " + count);

        for (uint i = 0; i < count; i++)
        {
            try
            {
                var relationship = relationshipManager.GetAt(i);
                if (relationship.Type == Discord.RelationshipType.Friend)
                {
                    Debug.Log($"Friend: {relationship.User.Username} (ID: {relationship.User.Id})");
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Error retrieving relationship at index {i}: {ex.Message}");
            }
        }
    }
}
