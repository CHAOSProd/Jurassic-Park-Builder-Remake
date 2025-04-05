using UnityEngine;
using TMPro;

public class DiscordManager : MonoBehaviour
{
    public static string discordUsername = "UnknownPlayer";
    public static string discordAvatarUrl = "";
    public static string discordUserId = ""; // New user ID field

    private Discord.Discord discord;
    private long startTimestamp;
    public TMP_Text levelText;
    private string previousLevelText;

    void Start()
    {
        discord = new Discord.Discord(1342685934776221786, (ulong)Discord.CreateFlags.NoRequireDiscord);
        previousLevelText = levelText != null ? levelText.text : string.Empty;
        startTimestamp = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        var userManager = discord.GetUserManager();
        userManager.OnCurrentUserUpdate += UpdateUserInfo;
        UpdateActivity();
    }

    void OnDestroy()
    {
        discord.Dispose();
    }

    void Update()
    {
        discord.RunCallbacks();
        if (levelText != null && levelText.text != previousLevelText)
        {
            previousLevelText = levelText.text;
            UpdateActivity();
        }
    }

    private void UpdateUserInfo()
    {
        var userManager = discord.GetUserManager();
        var currentUser = userManager.GetCurrentUser();
        discordUsername = currentUser.Username;
        discordUserId = currentUser.Id.ToString(); // Store user ID

        if (!string.IsNullOrEmpty(currentUser.Avatar))
        {
            discordAvatarUrl = $"https://cdn.discordapp.com/avatars/{currentUser.Id}/{currentUser.Avatar}.png";
        }
        else
        {
            if (ushort.TryParse(currentUser.Discriminator, out ushort discriminator))
            {
                int defaultAvatarIndex = discriminator % 5;
                discordAvatarUrl = $"https://cdn.discordapp.com/embed/avatars/{defaultAvatarIndex}.png";
            }
            else
            {
                discordAvatarUrl = "https://cdn.discordapp.com/embed/avatars/0.png";
            }
        }

        Debug.Log($"Discord username: {discordUsername}");
        Debug.Log($"Discord avatar URL: {discordAvatarUrl}");
        Debug.Log($"Discord user ID: {discordUserId}");
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
}