using UnityEngine;
using SimpleFileBrowser;         // SimpleFileBrowser namespace
using LootLocker.Requests;

public class AvatarManager : MonoBehaviour
{
    // Hook this up to your "Upload Avatar" button’s OnClick
    public void OnUploadAvatarClicked()
    {
        FileBrowser.SetFilters(true, ".png", ".jpg", ".jpeg");
        FileBrowser.ShowLoadDialog(
            OnFilePicked,
            OnPickCanceled,
            FileBrowser.PickMode.Files,
            false
        );
    }

    private void OnFilePicked(string[] paths)
    {
        if (paths == null || paths.Length == 0) return;
        UploadAvatarPublic(paths[0]);
    }

    private void OnPickCanceled()
    {
        Debug.Log("Avatar selection canceled.");
    }

    /// <summary>
    /// Ensures the file is uploaded as a public player file.
    /// </summary>
    private void UploadAvatarPublic(string localFilePath)
    {
        LootLockerSDKManager.CheckWhiteLabelSession(valid =>
        {
            if (!valid)
            {
                Debug.LogError("No valid session—cannot upload avatar.");
                return;
            }

            // purpose: "player_profile_picture", isPublic: true
            LootLockerSDKManager.UploadPlayerFile(
                localFilePath,
                "player_profile_picture",
                true,                    // ← force public
                resp =>
                {
                    if (!resp.success)
                    {
                        Debug.LogError("Avatar upload failed: " + resp.errorData);
                        return;
                    }

                    // cache and override Discord metadata
                    PlayerPrefs.SetString("CustomAvatarUrl", resp.url);
                    PlayerPrefs.Save();

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
