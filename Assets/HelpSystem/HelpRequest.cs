using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class HelpRequest : MonoBehaviour
{
    public string helpEndpoint = "https://nasty-rubie-jpbr-5e57689e.koyeb.app/triggerhelp";
    public Button getHelpButton;
    private HelpResponseHandler responseHandler;

    [System.Serializable]
    private class HelpRequestResponse
    {
        public string requestId;
    }

    void Start()
    {
        responseHandler = GetComponent<HelpResponseHandler>();
        if(getHelpButton != null)
        {
            getHelpButton.onClick.AddListener(OnGetHelpClicked);
        }
    }

    void OnGetHelpClicked()
    {
        string playerName = DiscordManager.discordUsername;
        string playerAvatar = DiscordManager.discordAvatarUrl;
        string userId = DiscordManager.discordUserId;
        
        Debug.Log($"Sending help request from user ID: {userId}");
        StartCoroutine(SendHelpRequest(playerName, playerAvatar, userId));
    }

    IEnumerator SendHelpRequest(string playerName, string playerAvatar, string userId)
    {
        WWWForm form = new WWWForm();
        form.AddField("playerName", playerName);
        form.AddField("playerAvatar", playerAvatar);
        form.AddField("discordUserId", userId);
        
        UnityWebRequest www = UnityWebRequest.Post(helpEndpoint, form);
        yield return www.SendWebRequest();

        if(www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Help request sent successfully.");
            
            // Parse the response to get the request ID
            var response = JsonUtility.FromJson<HelpRequestResponse>(www.downloadHandler.text);
            if(response != null && !string.IsNullOrEmpty(response.requestId))
            {
                responseHandler.SetActiveRequest(response.requestId);
            }
            else
            {
                Debug.LogError("Failed to get request ID from response");
            }
        }
        else
        {
            Debug.Log("Error sending help request: " + www.error);
        }
    }
}
