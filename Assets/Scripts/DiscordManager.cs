using UnityEngine;
using TMPro;
using System;
using Discord;
using LootLocker.Requests;

public class DiscordManager : MonoBehaviour
{
    public static string discordUsername  = "UnknownPlayer";
    public static string discordAvatarUrl = "";
    public static string discordUserId    = "";

    private Discord.Discord discord;
    public TMP_Text levelText;  // optional

    private bool isSessionStarted = false;

    private void Start()
    {
        try
        {
            discord = new Discord.Discord(
                1342685934776221786,
                (ulong)Discord.CreateFlags.NoRequireDiscord
            );
            discord.GetUserManager().OnCurrentUserUpdate += UpdateUserInfo;
        }
        catch (Exception e)
        {
            Debug.LogError($"Discord init failed: {e.Message}");
        }

        InitializeLootLockerSession();
    }

    private void Update()
    {
        discord?.RunCallbacks();
    }

    private void InitializeLootLockerSession()
    {
        // Replace with your actual email and password retrieval logic
        string email = PlayerPrefs.GetString("WL_Email", "");
        string password = PlayerPrefs.GetString("WL_Password", "");

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Debug.LogError("Email or password not found in PlayerPrefs.");
            return;
        }

        LootLockerSDKManager.WhiteLabelLoginAndStartSession(email, password, false, response =>
        {
            if (!response.success)
            {
                Debug.LogError("Failed to start LootLocker session: " + response.errorData.message);
                return;
            }

            isSessionStarted = true;

            // If Discord user info is already available, update metadata
            if (!string.IsNullOrEmpty(discordAvatarUrl))
            {
                UpdateLootLockerMetadata();
            }
        });
    }

    private void UpdateUserInfo()
    {
        var user = discord.GetUserManager().GetCurrentUser();
        discordUsername  = user.Username;
        discordUserId    = user.Id.ToString();
        discordAvatarUrl = !string.IsNullOrEmpty(user.Avatar)
            ? $"https://cdn.discordapp.com/avatars/{user.Id}/{user.Avatar}.png"
            : $"https://cdn.discordapp.com/embed/avatars/{(int.Parse(user.Discriminator) % 5)}.png";

        PlayerPrefs.SetString("DiscordAvatarUrl", discordAvatarUrl);
        PlayerPrefs.Save();

        // Update metadata if session is already started
        if (isSessionStarted)
        {
            UpdateLootLockerMetadata();
        }
    }

    private void UpdateLootLockerMetadata()
    {
        LootLockerSDKManager.UpdateOrCreateKeyValue(
            "discord_avatar",
            discordAvatarUrl,
            metaResp =>
            {
                if (!metaResp.success)
                {
                    Debug.LogError("Failed to upload avatar metadata: " + metaResp.errorData.message);
                }
                else
                {
                    Debug.Log("Successfully updated LootLocker metadata with Discord avatar URL.");
                }
            }
        );
    }

    private void OnDestroy()
    {
        discord?.Dispose();
    }
}

