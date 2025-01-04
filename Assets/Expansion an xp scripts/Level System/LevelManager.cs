using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Image XPFillImage;
    [SerializeField] private GameObject levelUpPanel; // Parent object for panels
    [SerializeField] private Animator panel1Animator; // Animator for Panel 1
    [SerializeField] private Animator panel2Animator; // Animator for Panel 2
    [SerializeField] private Animator panel3Animator; // Animator for Panel 3
    [SerializeField] private float[] xpPerLevel;

    private float level;
    private float XP;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            EventManager.Instance.AddListener<XPAddedGameEvent>(GiveXP);
            XP = Attributes.GetFloat("xp", 0);
            level = Attributes.GetInt("level", 1);
            UpdateUI();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (xpPerLevel == null || xpPerLevel.Length == 0)
        {
            Debug.LogError("XP per level array is not assigned or empty!");
            return;
        }

        if (levelUpPanel == null)
        {
            Debug.LogError("Level Up Panel is not assigned!");
            return;
        }

        levelUpPanel.SetActive(false); // Ensure panel is initially inactive
        UpdateUI();
    }

    private void OnLevelUp()
    {
        level++;
        XP = 0f;

        if (level <= xpPerLevel.Length)
        {
            StartCoroutine(PlayLevelUpAnimations());
        }
        else
        {
            Debug.LogWarning("Level exceeds the defined xpPerLevel array. Consider extending the xpPerLevel array.");
        }

        Save();
        ButtonUnlockHandler.Instance.UpdateUnlockItems();
        UpdateUI();
    }

    private IEnumerator PlayLevelUpAnimations()
    {
        // Activate the LevelUpPanel
        levelUpPanel.SetActive(true);

        // Reset and trigger animations for Panel 1 and Panel 2
        panel1Animator.ResetTrigger("PlayAnimation");
        panel2Animator.ResetTrigger("PlayAnimation");

        panel1Animator.SetTrigger("PlayAnimation");
        Debug.Log("Triggered Panel 1 Animation");

        panel2Animator.SetTrigger("PlayAnimation");
        Debug.Log("Triggered Panel 2 Animation");

        // Wait for both animations to finish
        yield return new WaitUntil(() =>
            panel1Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 &&
            !panel1Animator.IsInTransition(0) &&
            panel2Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 &&
            !panel2Animator.IsInTransition(0)
        );

        Debug.Log("Panels 1 and 2 animations finished.");

        // Reset and trigger animation for Panel 3
        panel3Animator.ResetTrigger("PlayAnimation");
        panel3Animator.SetTrigger("PlayAnimation");
        Debug.Log("Triggered Panel 3 Animation");

        // Wait for Panel 3 animation to finish
        yield return new WaitUntil(() =>
            panel3Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 &&
            !panel3Animator.IsInTransition(0)
        );

        Debug.Log("Panel 3 animation finished.");

        // Optional: Deactivate LevelUpPanel or reset to idle state
        levelUpPanel.SetActive(false);
    }

    private void UpdateXP()
    {
        Debug.Log($"XP: {XP}");
        CalculateLevel();
        UpdateUI();
        Save();
    }

    public bool GiveXP(XPAddedGameEvent xpEvent)
    {
        XP = Mathf.Max(0f, XP + xpEvent.Amount);
        UpdateXP();
        return true;
    }

    private void CalculateLevel()
    {
        if (level < xpPerLevel.Length && XP >= xpPerLevel[(int)level - 1])
        {
            OnLevelUp();
        }
    }

    private void UpdateUI()
    {
        levelText.text = $"{level}";
        XPFillImage.fillAmount = Mathf.Clamp01(XP / xpPerLevel[(int)level - 1]);
    }

    private void Save()
    {
        Attributes.SetFloat("xp", XP);
        Attributes.SetInt("level", (int)level);
    }
}








