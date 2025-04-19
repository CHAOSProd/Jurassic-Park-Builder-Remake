using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LootLocker.Requests;
using System;

public class WhiteLabelUIManager : MonoBehaviour
{
    public static event Action OnSessionStarted;

    [Header("Panels")]
    public GameObject signUpPanel, loginPanel, resetPanel;

    [Header("Sign Up Fields")]
    public TMP_InputField suEmailInput, suPasswordInput, suUsernameInput;
    public Button signUpBtn, toLoginFromSuBtn;

    [Header("Login Fields")]
    public TMP_InputField liEmailInput, liPasswordInput;
    public Toggle rememberMeToggle;
    public Button loginBtn, toSignUpFromLiBtn, toResetBtn;

    [Header("Reset Fields")]
    public TMP_InputField rpEmailInput;
    public Button sendResetBtn, toLoginFromRpBtn;

    [Header("Leaderboard Settings")]
    public string leaderboardKey = "JPBR_Social";
    public int    maxResults     = 50;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        LootLockerSDKManager.CheckWhiteLabelSession(valid =>
        {
            if (valid)
                OnLoginSuccess();
            else
                ShowPanel(loginPanel);
        });
    }

    private void Start()
    {
        toLoginFromSuBtn.onClick.AddListener(() => ShowPanel(loginPanel));
        toSignUpFromLiBtn.onClick.AddListener(() => ShowPanel(signUpPanel));
        toResetBtn.onClick.AddListener(() => ShowPanel(resetPanel));
        toLoginFromRpBtn.onClick.AddListener(() => ShowPanel(loginPanel));

        signUpBtn.onClick.AddListener(OnSignUpClicked);
        loginBtn.onClick.AddListener(OnLoginClicked);
        sendResetBtn.onClick.AddListener(OnResetClicked);
    }

    private void ShowPanel(GameObject panel)
    {
        signUpPanel.SetActive(false);
        loginPanel.SetActive(false);
        resetPanel.SetActive(false);
        panel.SetActive(true);
    }

    private void OnSignUpClicked()
    {
        string email    = suEmailInput.text;
        string password = suPasswordInput.text;
        string username = suUsernameInput.text;

        LootLockerSDKManager.WhiteLabelSignUp(email, password, signUpResponse =>
        {
            if (!signUpResponse.success)
            {
                Debug.LogError($"SignUp failed: {signUpResponse.errorData}");
                return;
            }

            LootLockerSDKManager.WhiteLabelLoginAndStartSession(email, password, false, loginResponse =>
            {
                if (!loginResponse.success)
                {
                    Debug.LogError("Login/session start failed after sign‑up.");
                    return;
                }

                // Cache credentials and player ID
                PlayerPrefs.SetString("WL_Email", email);
                PlayerPrefs.SetString("WL_Password", password);
                PlayerPrefs.SetString("PlayerID", loginResponse.SessionResponse.player_id.ToString());
                PlayerPrefs.Save();

                SetPlayerName(username, FetchPlayerDataAndProceed);
            });
        });
    }

    private void OnLoginClicked()
    {
        string email      = liEmailInput.text;
        string password   = liPasswordInput.text;
        bool   rememberMe = rememberMeToggle.isOn;

        LootLockerSDKManager.WhiteLabelLoginAndStartSession(email, password, rememberMe, res =>
        {
            if (!res.success)
            {
                Debug.LogError("Login/session start failed: " +
                    (res.LoginResponse.success ? res.SessionResponse.errorData : res.LoginResponse.errorData));
                return;
            }

            PlayerPrefs.SetString("WL_Email", email);
            PlayerPrefs.SetString("WL_Password", password);
            PlayerPrefs.SetString("PlayerID", res.SessionResponse.player_id.ToString());
            PlayerPrefs.Save();

            FetchPlayerDataAndProceed();
        });
    }

    private void OnResetClicked()
    {
        string email = rpEmailInput.text;
        LootLockerSDKManager.WhiteLabelRequestPassword(email, response =>
        {
            if (!response.success)
            {
                Debug.LogError($"Reset request failed: {response.errorData}");
                return;
            }
            ShowPanel(loginPanel);
        });
    }

    private void CachePlayerId(string id)
    {
        PlayerPrefs.SetString("PlayerID", id);
    }

    private void SetPlayerName(string name, Action onComplete)
    {
        LootLockerSDKManager.SetPlayerName(name, setNameResponse =>
        {
            if (!setNameResponse.success)
                Debug.LogWarning("Failed to set player name: " + setNameResponse.errorData);
            onComplete?.Invoke();
        });
    }

    private void FetchPlayerDataAndProceed()
    {
        int randomScore = UnityEngine.Random.Range(0, 1000);
        LootLockerSDKManager.SubmitScore(
            "", randomScore, leaderboardKey,
            submitResp =>
            {
                if (!submitResp.success)
                {
                    Debug.LogError("Score submission failed: " + submitResp.errorData);
                    return;
                }

                PlayerPrefs.SetInt("PlayerScore", randomScore);
                PlayerPrefs.Save();
                OnLoginSuccess();
            }
        );
    }

    private void OnLoginSuccess()
    {
        signUpPanel.SetActive(false);
        loginPanel.SetActive(false);
        resetPanel.SetActive(false);

        OnSessionStarted?.Invoke();
        FindObjectOfType<SplashSceneManager>()?.OnUserLoggedIn();
    }
}
