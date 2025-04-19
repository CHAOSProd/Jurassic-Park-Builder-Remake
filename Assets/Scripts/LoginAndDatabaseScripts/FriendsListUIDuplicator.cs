using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections;
using LootLocker.Requests;

public class FriendsListUIDuplicator : MonoBehaviour
{
    [Header("Leaderboard Settings")]
    public string leaderboardKey = "JPBR_Social";
    public int    maxResults     = 50;

    [Header("UI References")]
    public GameObject friendItemPrefab;  // Must contain deep children:
                                         //  - "Username"    (TextMeshProUGUI)
                                         //  - "Score"       (TextMeshProUGUI)
                                         //  - "AvatarImage" (UnityEngine.UI.Image)
    public Transform  contentPanel;      // ScrollView Content w/ VerticalLayoutGroup & ContentSizeFitter

    private string currentPlayerID;

    private void OnEnable()
    {
        // Called on both fresh login and returning sessions
        WhiteLabelUIManager.OnSessionStarted += PopulateFriendsList;

        LootLockerSDKManager.CheckWhiteLabelSession(isValid =>
        {
            if (!isValid) return;

            var email    = PlayerPrefs.GetString("WL_Email", "");
            var password = PlayerPrefs.GetString("WL_Password", "");

            LootLockerSDKManager.WhiteLabelLoginAndStartSession(
                email, password, false,
                sessionResp =>
                {
                    if (!sessionResp.success)
                        Debug.LogError("Session restart failed: " + sessionResp.errorData);
                    else
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
            Debug.LogError("[FriendsListUIDuplicator] Assign prefab & contentPanel in Inspector.");
            return;
        }

        currentPlayerID = PlayerPrefs.GetString("PlayerID", "");
        foreach (Transform t in contentPanel) Destroy(t.gameObject);

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

                foreach (var entry in response.items)
                {
                    if (entry.member_id == currentPlayerID) continue;
                    CreateFriendEntry(entry);
                }
            }
        );
    }

    private void CreateFriendEntry(LootLockerLeaderboardMember entry)
    {
        // Instantiate under layout group (worldPositionStays = false)
        var go = Instantiate(friendItemPrefab, contentPanel, false);

        // Set Username & Score
        var nameTf  = FindDeepChild(go.transform, "Username");
        var scoreTf = FindDeepChild(go.transform, "Score");
        nameTf .GetComponent<TextMeshProUGUI>().text =
            string.IsNullOrEmpty(entry.player.name)
                ? $"Player {entry.member_id}"
                : entry.player.name;
        scoreTf.GetComponent<TextMeshProUGUI>().text = entry.score.ToString();

        // 1) Try fetching a custom-uploaded avatar file
        int friendPlayerId = entry.player.id;  // int for file API :contentReference[oaicite:2]{index=2}
        LootLockerSDKManager.GetAllPlayerFiles(
            friendPlayerId,
            filesResp =>
            {
                if (filesResp.success)
                {
                    var custom = filesResp.items
                        .FirstOrDefault(f => f.purpose == "player_profile_picture");
                    if (custom != null)
                    {
                        StartCoroutine(LoadTexture(custom.url, go));
                        return;
                    }
                }
                // 2) Fallback to public key‑value metadata ("discord_avatar")
                string friendUid = entry.player.public_uid;  // string for key‑value API :contentReference[oaicite:3]{index=3}
                LootLockerSDKManager.GetOtherPlayersPublicKeyValuePairs(
                    friendUid,
                    kvResp =>
                    {
                        if (kvResp.success && kvResp.payload != null)
                        {
                            var kv = kvResp.payload
                                .FirstOrDefault(k => k.key == "discord_avatar");
                            if (kv != null && !string.IsNullOrEmpty(kv.value))
                                StartCoroutine(LoadTexture(kv.value, go));
                        }
                    }
                );
            }
        );
    }

    private IEnumerator LoadTexture(string url, GameObject entryGO)
    {
        using (var www = UnityEngine.Networking.UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
                yield break;

            var tex    = UnityEngine.Networking.DownloadHandlerTexture.GetContent(www);
            var sprite = Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f)
            );  // Create sprite from texture 

            var imgTf = FindDeepChild(entryGO.transform, "AvatarImage");
            if (imgTf != null && imgTf.TryGetComponent<Image>(out var uiImg))
                uiImg.sprite = sprite;
        }
    }

    /// Recursively find a child transform by name anywhere in the hierarchy
    private Transform FindDeepChild(Transform parent, string name)
    {
        if (parent.name == name) return parent;
        foreach (Transform child in parent)
        {
            var found = FindDeepChild(child, name);
            if (found != null) return found;
        }
        return null;
    }
}
