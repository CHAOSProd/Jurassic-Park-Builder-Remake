using UnityEngine;
using TMPro;
using LootLocker.Requests;

public class FriendsListUIDuplicator : MonoBehaviour
{
    [Header("Leaderboard Settings")]
    public string leaderboardKey = "JPBR_Social";
    public int maxResults = 50;

    [Header("UI References")]
    public GameObject friendItemPrefab;
    public Transform contentPanel;

    private string currentPlayerID;

    private void OnEnable()
    {
        WhiteLabelUIManager.OnSessionStarted += PopulateFriendsList;

        LootLockerSDKManager.CheckWhiteLabelSession(isValid =>
        {
            if (!isValid) return;

            string email    = PlayerPrefs.GetString("WL_Email", "");
            string password = PlayerPrefs.GetString("WL_Password", "");

            LootLockerSDKManager.WhiteLabelLoginAndStartSession(
                email, password, false,
                sessionResp =>
                {
                    if (!sessionResp.success)
                    {
                        Debug.LogError("Session restart failed: " + sessionResp.SessionResponse.errorData);
                        return;
                    }
                    PopulateFriendsList();
                }
            );
        });
    }

    private void OnDisable()
    {
        WhiteLabelUIManager.OnSessionStarted -= PopulateFriendsList;
    }

    public void PopulateFriendsList()
    {
        if (friendItemPrefab == null || contentPanel == null)
        {
            Debug.LogError("[FriendsList] Prefab or contentPanel not assigned.");
            return;
        }

        currentPlayerID = PlayerPrefs.GetString("PlayerID", "");
        foreach (Transform t in contentPanel)
            Destroy(t.gameObject);

        LootLockerSDKManager.GetScoreList(
            leaderboardKey,
            maxResults,
            0,
            response =>
            {
                if (!response.success)
                {
                    Debug.LogError($"[FriendsList] Leaderboard Error: {response.errorData?.message}");
                    return;
                }

                if (response.items == null || response.items.Length == 0)
                {
                    Debug.Log("[FriendsList] No entries to display.");
                    return;
                }

                foreach (var entry in response.items)
                {
                    if (entry.member_id == currentPlayerID)
                        continue;
                    CreateFriendEntry(entry);
                }
            }
        );
    }

    private void CreateFriendEntry(LootLockerLeaderboardMember entry)
    {
        var go = Instantiate(friendItemPrefab, contentPanel);
        if (go == null)
        {
            Debug.LogError("[FriendsList] Instantiate failed—check your prefab.");
            return;
        }

        // Use recursive find to locate deep children by name
        var nameTf  = FindDeepChild(go.transform, "Username");
        var scoreTf = FindDeepChild(go.transform, "Score");

        if (nameTf == null || scoreTf == null)
        {
            Debug.LogError("[FriendsList] Could not find 'Username' or 'Score' under the prefab.");
            return;
        }

        var nameText  = nameTf.GetComponent<TextMeshProUGUI>();
        var scoreText = scoreTf.GetComponent<TextMeshProUGUI>();

        if (nameText == null || scoreText == null)
        {
            Debug.LogError("[FriendsList] 'Username' or 'Score' missing TextMeshProUGUI component.");
            return;
        }

        nameText.text  = string.IsNullOrEmpty(entry.player.name)
                       ? $"Player {entry.member_id}"
                       : entry.player.name;
        scoreText.text = entry.score.ToString();
    }

    /// <summary>
    /// Recursively searches for a child transform with the given name.
    /// </summary>
    private Transform FindDeepChild(Transform parent, string childName)
    {
        if (parent.name == childName)
            return parent;

        foreach (Transform child in parent)
        {
            var result = FindDeepChild(child, childName);
            if (result != null)
                return result;
        }
        return null;
    }
}
