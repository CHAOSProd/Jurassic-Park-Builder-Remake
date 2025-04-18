using System;

[Serializable]
public class DiscordFriend
{
    public string id;
    public string username;  // use this field for the user’s name
    public string avatar;
    public string status;
    public DateTime lastOnline;
}

[Serializable]
public class DiscordUser
{
    public string id;
    public string username;
    public string avatar;
}

[Serializable]
public class DiscordTokenResponse
{
    public string access_token;
    public string token_type;
    public int expires_in;
    public string refresh_token;
    public string scope;
}

[Serializable]
public class CachedFriend
{
    public string discordId;
    public string displayName; // note: use displayName instead of username
    public string avatarUrl;
    public DateTime lastOnline;
    public int parkVersion;
    public string status;
}

[Serializable]
public class DiscordFriendListWrapper
{
    public DiscordFriend[] friends;
}