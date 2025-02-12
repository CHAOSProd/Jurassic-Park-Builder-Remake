using UnityEngine;
using UnityEngine.UI;

public class OptionsPanel : MonoBehaviour
{
    [Header("UI Elements")]
    public Button soundButton; // Button to toggle SFX
    public Button musicButton; // Button to toggle music

    [Header("Sprites")]
    public Sprite soundOnSprite;  // Sprite when SFX is enabled
    public Sprite soundOffSprite; // Sprite when SFX is disabled
    public Sprite musicOnSprite;  // Sprite when music is enabled
    public Sprite musicOffSprite; // Sprite when music is disabled

    void OnEnable()
    {
        // Update button sprites to reflect the current settings.
        UpdateSoundButtonSprite();
        UpdateMusicButtonSprite();

        // Add button listeners.
        soundButton.onClick.AddListener(OnSoundButtonClicked);
        musicButton.onClick.AddListener(OnMusicButtonClicked);
    }

    void OnDisable()
    {
        // Remove listeners to avoid duplicates.
        soundButton.onClick.RemoveListener(OnSoundButtonClicked);
        musicButton.onClick.RemoveListener(OnMusicButtonClicked);
    }

    // Called when the sound button is clicked.
    private void OnSoundButtonClicked()
    {
        AudioSettingsManager.Instance.ToggleSound();
        UpdateSoundButtonSprite();
    }

    // Called when the music button is clicked.
    private void OnMusicButtonClicked()
    {
        AudioSettingsManager.Instance.ToggleMusic();
        UpdateMusicButtonSprite();
    }

    private void UpdateSoundButtonSprite()
    {
        bool isSoundEnabled = AudioSettingsManager.Instance.IsSoundEnabled;
        soundButton.GetComponent<Image>().sprite = isSoundEnabled ? soundOnSprite : soundOffSprite;
    }

    private void UpdateMusicButtonSprite()
    {
        bool isMusicEnabled = AudioSettingsManager.Instance.IsMusicEnabled;
        musicButton.GetComponent<Image>().sprite = isMusicEnabled ? musicOnSprite : musicOffSprite;
    }
}



