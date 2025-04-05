using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class HelpRequest : MonoBehaviour
{
    // URL of your bot's endpoint that triggers the help message (update this with your actual URL)
    public string helpEndpoint = "https://nasty-rubie-jpbr-5e57689e.koyeb.app/triggerhelp";
    
    // Reference to the Get Help button in your UI
    public Button getHelpButton;
    
    // The player's name (or other identifier) to send along with the request
    public string playerName = "CHAOS";

    void Start()
    {
        if(getHelpButton != null)
        {
            getHelpButton.onClick.AddListener(OnGetHelpClicked);
        }
    }

    void OnGetHelpClicked()
    {
        // Start the coroutine to send the help request to your bot server
        StartCoroutine(SendHelpRequest());
    }

    IEnumerator SendHelpRequest()
    {
        // Create a form and add the player's name field
        WWWForm form = new WWWForm();
        form.AddField("playerName", playerName);
        
        // Create a POST request to your endpoint
        UnityWebRequest www = UnityWebRequest.Post(helpEndpoint, form);
        yield return www.SendWebRequest();

        if(www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Help request sent successfully.");
        }
        else
        {
            Debug.Log("Error sending help request: " + www.error);
        }
    }
}
