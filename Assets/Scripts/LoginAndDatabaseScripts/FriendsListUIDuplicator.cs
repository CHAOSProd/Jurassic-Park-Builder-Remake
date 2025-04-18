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

            // Fetch cached credentials
            string email    = PlayerPrefs.GetString("WL_Email", "");
            string password = PlayerPrefs.GetString("WL_Password", "");

            // Re‑login and start a fresh game session
            LootLockerSDKManager.WhiteLabelLoginAndStartSession(email, password, false, sessionResp =>
            {
                if (!sessionResp.success)
                {
                    Debug.LogError("Failed to re‑start session for returning player: " + sessionResp.SessionResponse.errorData);
                    return;
                }
                PopulateFriendsList();
            });
        });
    }

    private void OnDisable()
    {
        WhiteLabelUIManager.OnSessionStarted -= PopulateFriendsList;
    }

    public void PopulateFriendsList()
    {
        currentPlayerID = PlayerPrefs.GetString("PlayerID", "");

        // Clear old entries
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
                    Debug.LogError($"Leaderboard Error: {response.errorData?.message}");
                    return;
                }

                if (response.items == null || response.items.Length == 0)
                {
                    Debug.Log("Leaderboard is empty");
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

        var nameText  = go.transform.Find("Username")?.GetComponent<TMP_Text>();
        var scoreText = go.transform.Find("Score")?.GetComponent<TMP_Text>();

        if (nameText != null)
            nameText.text = string.IsNullOrEmpty(entry.player.name)
                ? $"Player {entry.member_id}"
                : entry.player.name;

        if (scoreText != null)
            scoreText.text = entry.score.ToString();
    }
}
