using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AudioSettingsManager : MonoBehaviour
{
    public static AudioSettingsManager Instance { get; private set; }

    private const string SoundPrefsKey = "SoundEnabled";
    private const string MusicPrefsKey = "MusicEnabled";

    // Exposed so that other scripts (like an OptionsPanel) can read these values.
    public bool IsSoundEnabled { get; private set; } = true;
    public bool IsMusicEnabled { get; private set; } = true;

    [Header("Audio Sources")]
    // Assign your dedicated music AudioSource in the Inspector.
    public AudioSource musicSource;

    private void Awake()
    {
        // Singleton pattern: If an instance already exists (and it isn’t this one), destroy this GameObject.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Load saved settings (default to enabled if not set).
        IsSoundEnabled = PlayerPrefs.GetInt(SoundPrefsKey, 1) == 1;
        IsMusicEnabled = PlayerPrefs.GetInt(MusicPrefsKey, 1) == 1;

        // Apply settings to currently existing AudioSources.
        ApplySound();
        ApplyMusic();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        // Start a coroutine to periodically update the mute state
        // (this helps catch new or dynamically created AudioSources).
        StartCoroutine(UpdateAudioSourcesRoutine());
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Called when a new scene loads.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplySound();
        ApplyMusic();
    }

    // Coroutine that periodically reapplies the SFX mute setting.
    private IEnumerator UpdateAudioSourcesRoutine()
    {
        while (true)
        {
            ApplySound();
            // Check every 0.5 seconds (adjust this interval if needed).
            yield return new WaitForSeconds(0.5f);
        }
    }

    // Call this method to toggle SFX (all sounds except the dedicated music).
    public void ToggleSound()
    {
        IsSoundEnabled = !IsSoundEnabled;
        PlayerPrefs.SetInt(SoundPrefsKey, IsSoundEnabled ? 1 : 0);
        PlayerPrefs.Save();
        ApplySound();
    }

    // Call this method to toggle the dedicated music AudioSource.
    public void ToggleMusic()
    {
        IsMusicEnabled = !IsMusicEnabled;
        PlayerPrefs.SetInt(MusicPrefsKey, IsMusicEnabled ? 1 : 0);
        PlayerPrefs.Save();
        ApplyMusic();
    }

    // Mute/unmute all AudioSources (except the dedicated music source) based on the SFX setting.
    private void ApplySound()
    {
        // Find all AudioSource components in the scene.
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audioSource in allAudioSources)
        {
            // Skip the dedicated music AudioSource.
            if (audioSource != musicSource)
            {
                audioSource.mute = !IsSoundEnabled;
            }
        }
    }

    // Mute/unmute the dedicated music AudioSource.
    private void ApplyMusic()
    {
        if (musicSource != null)
        {
            musicSource.mute = !IsMusicEnabled;
        }
    }
}



