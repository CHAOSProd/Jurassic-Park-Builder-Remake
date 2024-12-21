using UnityEngine;
using UnityEngine.UI;

public class OptionsPanel : MonoBehaviour
{
    [Header("UI Elements")]
    public Button soundButton; // Button to toggle sound
    public Button musicButton; // Button to toggle music

    [Header("Sprites")]
    public Sprite soundOnSprite;  // Sprite to show when sound is enabled
    public Sprite soundOffSprite; // Sprite to show when sound is disabled
    public Sprite musicOnSprite;  // Sprite to show when music is enabled
    public Sprite musicOffSprite; // Sprite to show when music is disabled

    [Header("Audio Sources")]
    public AudioSource musicSource; // Main AudioSource for music

    private const string SoundPrefsKey = "SoundEnabled";
    private const string MusicPrefsKey = "MusicEnabled";

    private bool isSoundEnabled;
    private bool isMusicEnabled;

    void Start()
    {
        // Load saved preferences
        isSoundEnabled = PlayerPrefs.GetInt(SoundPrefsKey, 1) == 1; // Default to enabled
        isMusicEnabled = PlayerPrefs.GetInt(MusicPrefsKey, 1) == 1; // Default to enabled

        // Apply settings
        UpdateSound(isSoundEnabled);
        UpdateMusic(isMusicEnabled);

        // Update button sprites
        UpdateSoundButtonSprite();
        UpdateMusicButtonSprite();

        // Add button listeners
        soundButton.onClick.AddListener(ToggleSound);
        musicButton.onClick.AddListener(ToggleMusic);
    }

    private void ToggleSound()
    {
        isSoundEnabled = !isSoundEnabled; // Toggle state
        UpdateSound(isSoundEnabled); // Apply changes
        PlayerPrefs.SetInt(SoundPrefsKey, isSoundEnabled ? 1 : 0); // Save preference
        UpdateSoundButtonSprite(); // Update sprite
    }

    private void ToggleMusic()
    {
        isMusicEnabled = !isMusicEnabled; // Toggle state
        UpdateMusic(isMusicEnabled); // Apply changes
        PlayerPrefs.SetInt(MusicPrefsKey, isMusicEnabled ? 1 : 0); // Save preference
        UpdateMusicButtonSprite(); // Update sprite
    }

    private void UpdateSound(bool isEnabled)
    {
        // Mute/unmute all AudioSources except music
        AudioSource[] allAudioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (AudioSource audioSource in allAudioSources)
        {
            if (audioSource != musicSource)
            {
                audioSource.mute = !isEnabled;
            }
        }
    }

    private void UpdateMusic(bool isEnabled)
    {
        if (musicSource != null)
        {
            musicSource.mute = !isEnabled;
        }
    }

    private void UpdateSoundButtonSprite()
    {
        Image buttonImage = soundButton.GetComponent<Image>();
        buttonImage.sprite = isSoundEnabled ? soundOnSprite : soundOffSprite;
    }

    private void UpdateMusicButtonSprite()
    {
        Image buttonImage = musicButton.GetComponent<Image>();
        buttonImage.sprite = isMusicEnabled ? musicOnSprite : musicOffSprite;
    }

    void OnDestroy()
    {
        // Remove listeners to avoid memory leaks
        soundButton.onClick.RemoveListener(ToggleSound);
        musicButton.onClick.RemoveListener(ToggleMusic);
    }
}


