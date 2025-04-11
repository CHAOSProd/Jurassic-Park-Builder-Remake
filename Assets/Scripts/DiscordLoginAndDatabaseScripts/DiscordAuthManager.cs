using UnityEngine;
using UnityEngine.SceneManagement;
using Discord; // Ensure the Discord Social SDK package is imported
using System;
using System.Collections;
using System.Collections.Generic;

public class DiscordAuthManager : MonoBehaviour
{
    
    public long clientId = 1342685934776221786;
    private Discord.Discord discord;
    private bool friendDataQueried = false;

    void Start()
    {
        InitializeDiscord();
    }

    void InitializeDiscord()
    {
        // Initialize the Discord SDK with your client ID.
        discord = new Discord.Discord(clientId, (ulong)Discord.CreateFlags.Default);

        // Subscribe to the current user update event.
        var userManager = discord.GetUserManager();
        userManager.OnCurrentUserUpdate += OnCurrentUserUpdate;
    }

    void Update()
    {
        // Process Discord callbacks every frame.
        if (discord != null)
        {
            discord.RunCallbacks();
        }
    }

    // Trigger login from UI if needed.
    public void Login()
    {
        Debug.Log("Login initiated.");
        // Login work is handled in callbacks.
    }

    // Called when the current user data is updated.
    private void OnCurrentUserUpdate()
    {
        var userManager = discord.GetUserManager();
        var currentUser = userManager.GetCurrentUser();

        // Save user data to PersistentData so it persists between scenes.
        if (PersistentData.Instance != null)
        {
            PersistentData.Instance.discordID = currentUser.Id.ToString();
            
            PersistentData.Instance.discordUsername = $"{currentUser.Username}#{currentUser.Discriminator}";
            PersistentData.Instance.isLoggedIn = true;
        }
        else
        {
            Debug.LogWarning("PersistentData instance not found!");
        }

        // Optionally store data in PlayerPrefs.
        PlayerPrefs.SetString("DiscordID", currentUser.Id.ToString());
        PlayerPrefs.SetString("DiscordUsername", $"{currentUser.Username}#{currentUser.Discriminator}");

        // Subscribe to relationship updates.
        var relationshipManager = discord.GetRelationshipManager();
        relationshipManager.OnRelationshipUpdate += OnRelationshipUpdate;

        // Start fallback coroutine in case no update event is fired.
        // Increased fallback delay (for example, 10 seconds) to give more time for the SDK to populate relationships.
        StartCoroutine(FallbackQueryFriendsAfterDelay(20f));
    }

    // Called when a relationship update is received.
    private void OnRelationshipUpdate(ref Relationship relationship)
    {
        if (!friendDataQueried)
        {
            friendDataQueried = true;
            // Unsubscribe so this callback isn't called repeatedly.
            var relationshipManager = discord.GetRelationshipManager();
            relationshipManager.OnRelationshipUpdate -= OnRelationshipUpdate;

            // Query friend list.
            QueryUnifiedFriendsList();

            // Load the main game scene.
            SceneManager.LoadScene("Game");
        }
    }

    // Fallback coroutine: if no relationship update is received within delaySeconds, query directly.
    IEnumerator FallbackQueryFriendsAfterDelay(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        if (!friendDataQueried)
        {
            friendDataQueried = true;
            Debug.Log("Fallback: No relationship update received. Querying friend list directly.");
            QueryUnifiedFriendsList();
            SceneManager.LoadScene("Game");
        }
    }

    // Retrieves the unified friends list from Discord.
    void QueryUnifiedFriendsList()
    {
        var relationshipManager = discord.GetRelationshipManager();
        List<DiscordFriend> friends = new List<DiscordFriend>();

        // Wrap the Count() call in try/catch in case SDK data isn't ready.
        int relationshipCount = 0;
        try
        {
            relationshipCount = (int)relationshipManager.Count();
        }
        catch (ResultException ex)
        {
            Debug.LogWarning("Failed to retrieve relationship count: " + ex.Message + ". Setting count to 0.");
            relationshipCount = 0;
        }

        Debug.Log("Querying friend list. Total relationships reported: " + relationshipCount);

        if (relationshipCount == 0)
        {
            Debug.LogWarning("No relationships available. Ensure the Discord account has friends and that permissions are set correctly.");
        }

        // Iterate over available relationships.
        for (uint i = 0; i < relationshipCount; i++)
        {
            try
            {
                Relationship relationship = relationshipManager.GetAt(i);
                Debug.Log($"Relationship {i}: Type = {relationship.Type}, Username = {relationship.User.Username}");

                // Filter for friend relationships.
                if (relationship.Type == RelationshipType.Friend)
                {
                    // Create a friend using the new DiscordFriend class.
                    // For fields not provided by the Discord SDK, assign default values.
                    DiscordFriend friend = new DiscordFriend
                    {
                        id = relationship.User.Id.ToString(),
                        username = relationship.User.Username,
                        avatar = relationship.User.Avatar,
                        status = "Online", // Default value; update if you have actual presence data.
                        lastOnline = DateTime.UtcNow // Default to current time; adjust as needed.
                    };
                    friends.Add(friend);
                }
            }
            catch (ResultException ex)
            {
                Debug.LogWarning($"Failed to retrieve relationship at index {i}: {ex.Message}");
            }
        }
        Debug.Log("Total friends found after filtering: " + friends.Count);

        // Update the friend cache using the DatabaseManager.
        DatabaseManager dbManager = FindObjectOfType<DatabaseManager>();
        if (dbManager != null)
        {
            dbManager.UpdateFriendsCache(friends);
        }
        else
        {
            Debug.LogWarning("DatabaseManager not found in scene.");
        }
    }

    // Adds a context menu item in the Inspector to reset login.
    [ContextMenu("Reset Login")]
    public void ResetLogin()
    {
        Debug.Log("Resetting login state.");

        // Reset friend retrieval flag and persistent data.
        friendDataQueried = false;
        if (PersistentData.Instance != null)
        {
            PersistentData.Instance.discordID = "";
            PersistentData.Instance.discordUsername = "";
            PersistentData.Instance.isLoggedIn = false;
        }

        // Optionally, reset PlayerPrefs.
        PlayerPrefs.DeleteKey("DiscordID");
        PlayerPrefs.DeleteKey("DiscordUsername");

        // Reload the current scene so the login process can restart.
        // In the Editor, if not in Play mode, use the EditorSceneManager API.
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            string scenePath = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path;
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
#else
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
#endif
    }
}
