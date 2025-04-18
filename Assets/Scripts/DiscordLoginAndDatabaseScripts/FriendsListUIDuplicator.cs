using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class FriendsListUIDuplicator : MonoBehaviour
{
    // Assign the friend item prefab from your hierarchy (template GameObject).
    public GameObject friendItemPrefab;

    // Assign the parent container (e.g., the Content GameObject of your Scroll View).
    public Transform contentPanel;

    // Awake is called even if the GameObject is deactivated.
    void Awake()
    {
        PopulateFriendsList();
    }

    void PopulateFriendsList()
    {
        // Retrieve cached friends from your DatabaseManager.
        DatabaseManager dbManager = FindObjectOfType<DatabaseManager>();
        if (dbManager == null)
        {
            Debug.LogWarning("DatabaseManager not found in scene.");
            return;
        }

        List<CachedFriend> friends = dbManager.GetCachedFriends();
        Debug.Log("Populating friends list with " + friends.Count + " friends.");

        foreach (CachedFriend friend in friends)
        {
            // Duplicate the assigned prefab as a child of the content panel.
            GameObject friendItem = Instantiate(friendItemPrefab, contentPanel);

            // Find the TMP child by name ("Username") and update the text.
            TMP_Text usernameTMP = friendItem.transform.Find("Username").GetComponent<TMP_Text>();
            if (usernameTMP != null)
            {
                usernameTMP.text = friend.displayName;
                Debug.Log("Friend username set: " + friend.displayName);
            }
            else
            {
                Debug.LogWarning("Username TMP_Text not found in friend item prefab.");
            }

            // Find the Image child by name ("Avatar") and load the Discord profile picture.
            Image avatarImage = friendItem.transform.Find("Avatar").GetComponent<Image>();
            if (avatarImage != null)
            {
                // Begin coroutine to download and set the avatar image.
                StartCoroutine(LoadAvatarImage(friend.avatarUrl, avatarImage));
            }
            else
            {
                Debug.LogWarning("Avatar Image component not found in friend item prefab.");
            }
        }
    }

    IEnumerator LoadAvatarImage(string url, Image imageComponent)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning("Failed to load avatar image: " + request.error);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                // Create a Sprite from the loaded texture.
                Sprite avatarSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                imageComponent.sprite = avatarSprite;
                Debug.Log("Avatar image loaded successfully from: " + url);
            }
        }
    }
}

