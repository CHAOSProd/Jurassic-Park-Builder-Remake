using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Discord;
using UnityEngine.SceneManagement;


[Serializable]
public class DiscordOAuth2TokenResponse {
    public string access_token;
    public string token_type;
    public int expires_in;
    public string refresh_token;
    public string scope;
}

public class DiscordAuthManager : MonoBehaviour
{
    public static DiscordAuthManager Instance;
    public bool isProcessing = false;
    public bool isDataReady = false;
    public static event Action OnDataReady;

    public long clientId = 1342685934776221786;
    public string clientSecret = "YOUR_CLIENT_SECRET";
    public string redirectUri = "https://nasty-rubie-jpbr-5e57689e.koyeb.app/";

    private Discord.Discord discord;
    private bool friendDataQueried = false;
    
    void Awake()
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

    void Start()
    {
        InitializeDiscord();
        string code = GetAuthorizationCodeFromURL();
        if (!string.IsNullOrEmpty(code))
        {
            StartCoroutine(ExchangeCodeForToken(code));
        }
    }

    void Update()
    {
        if (discord != null)
        {
            discord.RunCallbacks();
        }
    }

    void InitializeDiscord()
    {
        discord = new Discord.Discord(clientId, (ulong)Discord.CreateFlags.Default);
        var userManager = discord.GetUserManager();
        userManager.OnCurrentUserUpdate += OnCurrentUserUpdate;
    }

    public void Login()
    {
        Debug.Log("Login initiated.");
        OpenOAuth2URL();
    }

    public void OpenOAuth2URL()
    {
        string oauthURL = "https://discord.com/oauth2/authorize" +
                          "?client_id=" + clientId.ToString() +
                          "&response_type=code" +
                          "&redirect_uri=" + UnityWebRequest.EscapeURL(redirectUri) +
                          "&scope=identify+email+relationships.read";
        Application.OpenURL(oauthURL);
    }

    private string GetAuthorizationCodeFromURL()
    {
        string url = Application.absoluteURL;
        if (!string.IsNullOrEmpty(url) && url.Contains("code="))
        {
            Uri uri = new Uri(url);
            string query = uri.Query;
            string[] parameters = query.TrimStart('?').Split('&');
            foreach (string param in parameters)
            {
                string[] keyValue = param.Split('=');
                if (keyValue.Length == 2 && keyValue[0] == "code")
                {
                    return UnityWebRequest.UnEscapeURL(keyValue[1]);
                }
            }
        }
        return null;
    }

    IEnumerator ExchangeCodeForToken(string code)
    {
        isProcessing = true;
        WWWForm form = new WWWForm();
        form.AddField("client_id", clientId.ToString());
        form.AddField("client_secret", clientSecret);
        form.AddField("grant_type", "authorization_code");
        form.AddField("code", code);
        form.AddField("redirect_uri", redirectUri);

        UnityWebRequest www = UnityWebRequest.Post("https://discord.com/api/oauth2/token", form);
        yield return www.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
        if (www.result == UnityWebRequest.Result.Success)
#else
        if (!www.isNetworkError && !www.isHttpError)
#endif
        {
            DiscordOAuth2TokenResponse tokenResponse = JsonUtility.FromJson<DiscordOAuth2TokenResponse>(www.downloadHandler.text);
            ReinitializeDiscord(tokenResponse.access_token);
        }
        else
        {
            Debug.LogError("Token exchange failed: " + www.error);
            isProcessing = false;
        }
    }

    void ReinitializeDiscord(string accessToken)
    {
        if (discord != null)
        {
            discord.Dispose();
        }
        discord = new Discord.Discord(clientId, (ulong)Discord.CreateFlags.Default);
        var userManager = discord.GetUserManager();
        userManager.OnCurrentUserUpdate += OnCurrentUserUpdate;
    }

    private void OnCurrentUserUpdate()
    {
        var userManager = discord.GetUserManager();
        var currentUser = userManager.GetCurrentUser();

        if (PersistentData.Instance != null)
        {
            PersistentData.Instance.discordID = currentUser.Id.ToString();
            PersistentData.Instance.discordUsername = $"{currentUser.Username}#{currentUser.Discriminator}";
            PersistentData.Instance.isLoggedIn = true;
        }

        PlayerPrefs.SetString("DiscordID", currentUser.Id.ToString());
        PlayerPrefs.SetString("DiscordUsername", $"{currentUser.Username}#{currentUser.Discriminator}");

        var relationshipManager = discord.GetRelationshipManager();
        relationshipManager.OnRelationshipUpdate += OnRelationshipUpdate;
        StartCoroutine(FallbackQueryFriendsAfterDelay(20f));
    }

    private void OnRelationshipUpdate(ref Relationship relationship)
    {
        if (!friendDataQueried)
        {
            friendDataQueried = true;
            var relationshipManager = discord.GetRelationshipManager();
            relationshipManager.OnRelationshipUpdate -= OnRelationshipUpdate;

            QueryUnifiedFriendsList();
            isProcessing = false;
            isDataReady = true;
            OnDataReady?.Invoke();
        }
    }

    IEnumerator FallbackQueryFriendsAfterDelay(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        if (!friendDataQueried)
        {
            friendDataQueried = true;
            QueryUnifiedFriendsList();
            isProcessing = false;
            isDataReady = true;
            OnDataReady?.Invoke();
        }
    }

    void QueryUnifiedFriendsList()
    {
        var relationshipManager = discord.GetRelationshipManager();
        List<DiscordFriend> friends = new List<DiscordFriend>();

        int relationshipCount = 0;
        try
        {
            relationshipCount = (int)relationshipManager.Count();
        }
        catch (ResultException ex)
        {
            Debug.LogWarning("Failed to retrieve relationship count: " + ex.Message);
            relationshipCount = 0;
        }

        for (uint i = 0; i < relationshipCount; i++)
        {
            try
            {
                Relationship relationship = relationshipManager.GetAt(i);
                if (relationship.Type == RelationshipType.Friend)
                {
                    DiscordFriend friend = new DiscordFriend
                    {
                        id = relationship.User.Id.ToString(),
                        username = relationship.User.Username,
                        avatar = relationship.User.Avatar,
                        status = "Online",
                        lastOnline = DateTime.UtcNow
                    };
                    friends.Add(friend);
                }
            }
            catch (ResultException ex)
            {
                Debug.LogWarning($"Failed to retrieve relationship at index {i}: {ex.Message}");
            }
        }

        DatabaseManager dbManager = FindObjectOfType<DatabaseManager>();
        if (dbManager != null)
        {
            dbManager.UpdateFriendsCache(friends);
        }
    }

    [ContextMenu("Reset Login")]
    public void ResetLogin()
    {
        friendDataQueried = false;
        isProcessing = false;
        isDataReady = false;
        
        if (PersistentData.Instance != null)
        {
            PersistentData.Instance.discordID = "";
            PersistentData.Instance.discordUsername = "";
            PersistentData.Instance.isLoggedIn = false;
        }

        PlayerPrefs.DeleteKey("DiscordID");
        PlayerPrefs.DeleteKey("DiscordUsername");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}