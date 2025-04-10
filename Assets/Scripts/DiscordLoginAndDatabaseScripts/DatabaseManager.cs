using UnityEngine;
using System;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;
using System.Collections.Generic;

public class DatabaseManager : MonoBehaviour
{
    private string connectionString;
    private string dbPath;

    void Awake()
    {
        InitializeDatabase();
    }

    void InitializeDatabase()
    {
        dbPath = System.IO.Path.Combine(Application.persistentDataPath, "ParkGame.db");
        connectionString = "URI=file:" + dbPath;

        if (!File.Exists(dbPath))
        {
            CreateDatabase();
        }
        else
        {
            Debug.Log("Database found at: " + dbPath);
        }
    }

    void CreateDatabase()
    {
        try
        {
            ExecuteNonQuery(
                @"CREATE TABLE IF NOT EXISTS Players (
                    discord_id TEXT PRIMARY KEY,
                    player_name TEXT,
                    last_login TEXT,
                    park_data BLOB,
                    coins INTEGER DEFAULT 1000
                )");

            ExecuteNonQuery(
                @"CREATE TABLE IF NOT EXISTS Friends (
                    friend_id TEXT PRIMARY KEY,
                    username TEXT,
                    avatar_url TEXT,
                    last_online TEXT,
                    park_version INTEGER
                )");

            ExecuteNonQuery(
                @"CREATE TABLE IF NOT EXISTS ParkVisits (
                    visit_id INTEGER PRIMARY KEY AUTOINCREMENT,
                    park_owner_id TEXT,
                    visitor_id TEXT,
                    visit_time TEXT
                )");

            Debug.Log("Database created successfully at: " + dbPath);
        }
        catch (Exception e)
        {
            Debug.LogError($"Database creation failed: {e.Message}");
        }
    }

    public void SavePlayerData(string discordId, string playerName, byte[] parkData)
    {
        ExecuteNonQuery(
            @"INSERT OR REPLACE INTO Players 
            (discord_id, player_name, park_data, last_login) 
            VALUES (@id, @name, @data, @time)",
            new Dictionary<string, object> {
                {"@id", discordId},
                {"@name", playerName},
                {"@data", parkData},
                {"@time", DateTime.UtcNow.ToString("o")}
            }
        );
        Debug.Log("Player data saved for discord_id: " + discordId);
    }

    public byte[] LoadPlayerData(string discordId)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT park_data FROM Players WHERE discord_id = @id";
                cmd.Parameters.AddWithValue("@id", discordId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Debug.Log("Player data loaded for discord_id: " + discordId);
                        return (byte[])reader["park_data"];
                    }
                    else
                    {
                        Debug.LogWarning("No player data found for discord_id: " + discordId);
                        return null;
                    }
                }
            }
        }
    }

    // Updated to remove usage of a non-existent discriminator property.
    public void UpdateFriendsCache(List<DiscordFriend> friends)
    {
        Debug.Log("Updating friends cache with " + friends.Count + " friends.");
        foreach (var friend in friends)
        {
            Debug.Log("Updating friend: " + friend.username + " (ID: " + friend.id + ")");
            ExecuteNonQuery(
                @"INSERT OR REPLACE INTO Friends 
                (friend_id, username, avatar_url, last_online) 
                VALUES (@id, @name, @avatar, @time)",
                new Dictionary<string, object> {
                    {"@id", friend.id},
                    {"@name", friend.username},
                    {"@avatar", GetAvatarUrl(friend)},
                    {"@time", DateTime.UtcNow.ToString("o")}
                }
            );
        }
        Debug.Log("Friends cache updated successfully.");
    }

    private string GetAvatarUrl(DiscordFriend friend)
    {
        return string.IsNullOrEmpty(friend.avatar) 
            ? "https://cdn.discordapp.com/embed/avatars/0.png" 
            : $"https://cdn.discordapp.com/avatars/{friend.id}/{friend.avatar}.png";
    }

    public List<CachedFriend> GetCachedFriends()
    {
        var friends = new List<CachedFriend>();
        
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Friends";
                
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CachedFriend cachedFriend = new CachedFriend {
                            discordId = reader["friend_id"].ToString(),
                            displayName = reader["username"].ToString(), // Changed from 'username' to 'displayName'
                            avatarUrl = reader["avatar_url"].ToString(),
                            lastOnline = DateTime.Parse(reader["last_online"].ToString()),
                            parkVersion = reader["park_version"] != DBNull.Value ? Convert.ToInt32(reader["park_version"]) : 0,
                            status = "" // Use empty string as a default or set accordingly
                        };
                        friends.Add(cachedFriend);
                        Debug.Log("Cached friend: " + cachedFriend.displayName + " with avatar URL: " + cachedFriend.avatarUrl);
                    }
                }
            }
        }
        Debug.Log("Total cached friends retrieved: " + friends.Count);
        return friends;
    }

    private void ExecuteNonQuery(string query, Dictionary<string, object> parameters = null)
    {
        try
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = query;
                    
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }
                    
                    cmd.ExecuteNonQuery();
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Database error: {e.Message}\nQuery: {query}");
        }
    }
}
