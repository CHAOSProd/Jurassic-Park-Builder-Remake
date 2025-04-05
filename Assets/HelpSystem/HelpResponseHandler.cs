using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class HelpResponseHandler : MonoBehaviour
{
    [System.Serializable]
    private class DiscordUser
    {
        public string id;
        public string username;
        public string discriminator;
    }

    [System.Serializable]
    private class HelperResponse
    {
        public DiscordUser[] helpers;
    }

    [System.Serializable]
    private class HelpRequestResponse
    {
        public string requestId;
    }

    public string helpersEndpoint = "https://nasty-rubie-jpbr-5e57689e.koyeb.app/gethelpers/";
    public float checkInterval = 5f;
    public TextMeshProUGUI helpersText;
    
    private string currentRequestId;
    // Dictionary to track all helpers (id -> full username)
    private Dictionary<string, string> helperDictionary = new Dictionary<string, string>();

    void Start()
    {
        StartCoroutine(PeriodicHelperCheck());
    }

    public void SetActiveRequest(string requestId)
    {
        currentRequestId = requestId;
        helperDictionary.Clear();
        Debug.Log($"Tracking help request: {currentRequestId}");
        UpdateHelperDisplay();
    }

    IEnumerator PeriodicHelperCheck()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkInterval);
            
            if (!string.IsNullOrEmpty(currentRequestId))
            {
                yield return StartCoroutine(GetHelpers(currentRequestId));
            }
        }
    }

    IEnumerator GetHelpers(string requestId)
    {
        string url = $"{helpersEndpoint}{requestId}";
        UnityWebRequest www = UnityWebRequest.Get(url);
        
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            ProcessHelpers(www.downloadHandler.text);
        }
        else
        {
            Debug.Log($"Error checking helpers: {www.error}");
        }
    }

    void ProcessHelpers(string jsonResponse)
    {
        var response = JsonUtility.FromJson<HelperResponse>(jsonResponse);
        
        if (response == null || response.helpers == null)
        {
            Debug.Log("No helpers found or invalid response");
            return;
        }

        // Update helperDictionary with all helpers from the response
        foreach (var helper in response.helpers)
        {
            string fullUsername = $"{helper.username}#{helper.discriminator}";
            if (!helperDictionary.ContainsKey(helper.id))
            {
                helperDictionary.Add(helper.id, fullUsername);
                Debug.Log($"New helper: {fullUsername}");
            }
        }
        
        UpdateHelperDisplay();
    }

    void UpdateHelperDisplay()
    {
        if (helpersText != null)
        {
            List<string> helperList = new List<string>(helperDictionary.Values);
            helpersText.text = "Helpers:\n" + (helperList.Count > 0 ? string.Join("\n", helperList) : "No helpers yet");
        }
    }

    public IEnumerator SendHelpRequestUpdated(WWWForm form)
    {
        UnityWebRequest www = UnityWebRequest.Post(
            GetComponent<HelpRequest>().helpEndpoint, 
            form
        );
        
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            var response = JsonUtility.FromJson<HelpRequestResponse>(www.downloadHandler.text);
            if (response != null)
            {
                SetActiveRequest(response.requestId);
            }
            Debug.Log("Help request sent successfully.");
        }
        else
        {
            Debug.Log("Error sending help request: " + www.error);
        }
    }
}
