using UnityEngine;
using TMPro;

public class DiscordManager : MonoBehaviour
{
    private Discord.Discord discord;
    public TMP_Text levelText;
    private string previousLevelText;
    private long startTimestamp;

    void Start()
    {
        // Initialize the Discord instance
        discord = new Discord.Discord(1342685934776221786, (ulong)Discord.CreateFlags.NoRequireDiscord);

        // Store the initial text value
        previousLevelText = levelText != null ? levelText.text : string.Empty;

        // Set the initial start timestamp
        startTimestamp = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // Initialize the activity
        UpdateActivity();
    }

    void OnDestroy()
    {
        // Dispose of the Discord instance when the script is destroyed
        discord.Dispose();
    }

    void UpdateActivity()
    {
        var activityManager = discord.GetActivityManager();

        var activity = new Discord.Activity
        {
            Details = "In Jurassic Park",
            State = "Level " + previousLevelText,
            Timestamps = { Start = startTimestamp }
        };

        activityManager.UpdateActivity(activity, (result) =>
        {
            Debug.Log("Activity updated: " + result);
        });
    }

    void Update()
    {
        // Run Discord callbacks
        discord.RunCallbacks();

        // Check if the level text has changed
        if (levelText != null && levelText.text != previousLevelText)
        {
            // Update the stored previous text
            previousLevelText = levelText.text;

            // Update the activity state
            UpdateActivity();
        }
    }
}








