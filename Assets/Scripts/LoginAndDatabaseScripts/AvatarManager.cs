using UnityEngine;
using SimpleFileBrowser;         // SimpleFileBrowser namespace
using LootLocker.Requests;

public class AvatarManager : MonoBehaviour
{
    // Hook this up to your "Upload Avatar" button’s OnClick
    public void OnUploadAvatarClicked()
    {
        // 1) Optionally set file filters (images only)
        //    This uses the overload: SetFilters(bool showAllFilesFilter, params string[] filters)
        //    to show only .png, .jpg, .jpeg files in the dialog :contentReference[oaicite:1]{index=1}
        FileBrowser.SetFilters(true, ".png", ".jpg", ".jpeg");

        // 2) Show the load dialog (no filter argument here)
        FileBrowser.ShowLoadDialog(
            OnFilePicked,       // OnSuccess callback
            OnPickCanceled,     // OnCancel callback
            FileBrowser.PickMode.Files,
            false               // allowMultiSelection = false
            // initialPath, initialFilename, title, loadButtonText use defaults
        );
    }

    // Called when user selects one or more files
    private void OnFilePicked(string[] paths)
    {
        if (paths == null || paths.Length == 0)
            return;

        UploadCustomAvatar(paths[0]);
    }

    // Called when the user cancels the dialog
    private void OnPickCanceled()
    {
        Debug.Log("Avatar selection canceled.");
    }

    private void UploadCustomAvatar(string localFilePath)
    {
        // Ensure we have a valid LootLocker session
        LootLockerSDKManager.CheckWhiteLabelSession(valid =>
        {
            if (!valid)
            {
                Debug.LogError("No valid session—cannot upload avatar.");
                return;
            }

            // Upload the file as a public player file
            LootLockerSDKManager.UploadPlayerFile(
                localFilePath,
                "player_profile_picture",  // your file purpose
                true,                      // public
                resp =>
                {
                    if (!resp.success)
                    {
                        Debug.LogError("Avatar upload failed: " + resp.errorData);
                        return;
                    }

                    // Cache the URL locally for this client
                    PlayerPrefs.SetString("CustomAvatarUrl", resp.url);
                    PlayerPrefs.Save();

                    // Override the discord_avatar key-value so others see this custom avatar
                    LootLockerSDKManager.UpdateOrCreateKeyValue(
                        "discord_avatar",
                        resp.url,
                        metaResp =>
                        {
                            if (!metaResp.success)
                                Debug.LogError("Failed to override discord_avatar: " + metaResp.errorData);
                        }
                    );
                }
            );
        });
    }
}
