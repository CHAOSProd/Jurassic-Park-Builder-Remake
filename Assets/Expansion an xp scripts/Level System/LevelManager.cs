using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.VFX;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance; // Singleton instance
    [SerializeField] private TMP_Text levelText; // UI element displaying the current level
    [SerializeField] private TMP_Text EarnedUnlockedText; // UI element displaying the you earning/you unlocked text
    [SerializeField] private Image XPFillImage; // UI element displaying the XP bar
    [SerializeField] private GameObject ScrollRect; // UI element displaying the scroll rect
    [SerializeField] private Image BuckImage; // UI element displaying the buck image
    [SerializeField] private TMP_Text BuckAmountText; // UI element displaying the buck amount text
    [SerializeField] private Button CollectButton;
    [SerializeField] private Button OkButton;
    [SerializeField] private GameObject levelUpPanel; // Panel that pops up on level up
    [SerializeField] private GameObject levelUpSound;
    [SerializeField] private GameObject levelUpAppearSound;
    [SerializeField] private float[] xpPerLevel; // Array to store XP required for each level
    [SerializeField] private Image[] levelImages; // Array to store number images
    [SerializeField] private List<LevelReqItem> levelRequiredItems; // List for item's levelReq Filter
    [SerializeField] private List<GameObject> ToggledUI;
    [SerializeField] private GameObject appearVFXPrefab;


    private float level; // Current player level
    private float XP; // Current player XP

    // Ensures only one instance of LevelManager exists
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
    // Initializes XP and UI, and subscribes to events
    private void Start()
    {
        // Check if xpPerLevel array is properly assigned
        if (xpPerLevel == null || xpPerLevel.Length == 0)
        {
            Debug.LogError("XP per level array is not assigned or empty!");
            return;
        }
        // Check if levelUpPanel is assigned
        if (levelUpPanel == null)
        {
            Debug.LogError("Level Up Panel is not assigned!");
            return;
        }

        if (levelImages == null || levelImages.Length == 0)
        {
            Debug.LogError("Level images array is not assigned or empty!");
            return;
        }

        foreach (var image in levelImages)
        {
            if (image != null) image.gameObject.SetActive(false);
        }

        CollectButton.onClick.AddListener(OnCollectButtonClicked);
        OkButton.onClick.AddListener(OnOkButtonClicked);
        EarnedUnlockedText.text = "You earned";
        UpdateUnlockItems();
        UpdateUI();
    }
    // Handles logic for leveling up
    private void OnLevelUp()
    {
        level++; // Increase the player level
        // Ensure there is a next level to level up to
        if (level <= xpPerLevel.Length)
        {
            ShowLevelUpPanel();
            UpdateLevelImages();
        }
        else
        {
            Debug.LogWarning("Level exceeds the defined xpPerLevel array. Consider extending the xpPerLevel array.");
        }

        if (TreeChopManager.Instance != null)
        {
            TreeChopManager.Instance.IncreaseTreeChops(); // Increase available tree chops
        }
        else
        {
            Debug.LogError("TreeChopManager.Instance is null. Make sure TreeChopManager is initialized properly.");
        }

        Save(); // Save progress
        ButtonUnlockHandler.Instance.UpdateUnlockItems();
        UpdateUnlockItems(); // Update the unlocked item filter
        UpdateUI(); // Update the UI to reflect new level
    }

    private void UpdateUnlockItems()
    {
        foreach (var item in levelRequiredItems)
        {
            if (item != null)
            {
                item.UpdateItemVisibility((int)level);
            }
        }
    }


    private void UpdateLevelImages()
    {
        for (int i = 0; i < levelImages.Length; i++)
        {
            if (levelImages[i] != null)
            {
                levelImages[i].gameObject.SetActive(i == level - 1);
            }
        }
    }
    // Updates the level and UI when XP changes
    private void UpdateXP()
    {
        Debug.Log($"XP: {XP}"); // Log the current XP
        CalculateLevel(); // Check if the player should level up
        UpdateUI(); // Update the UI to reflect new XP
        Save(); // Save progress
    }
    // Adds XP and triggers the XP changed event
    public bool GiveXP(XPAddedGameEvent xpEvent)
    {
        XP = Mathf.Max(0f, XP + xpEvent.Amount); // Ensure XP doesn't go below zero
        UpdateXP();
        return true;
    }
    // Calculates if the player should level up based on current XP
    private void CalculateLevel()
    {
        if (level < xpPerLevel.Length && XP >= xpPerLevel[(int)level - 1])
        {
            XP -= xpPerLevel[(int)level - 1];
            OnLevelUp();
        }
    }
    // Updates the UI elements for level and XP bar
    private void UpdateUI()
    {
        levelText.text = $"{level}";
        if (level <= xpPerLevel.Length)
        {
            XPFillImage.fillAmount = Mathf.Clamp01(XP / xpPerLevel[(int)level - 1]);
        }
        else
        {
            XPFillImage.fillAmount = 1f;
        }
    }
    // Shows the level up panel
    private void ShowLevelUpPanel()
    {
        foreach(GameObject ui in ToggledUI)
        {
            ui.SetActive(false);
        }

        levelUpPanel.SetActive(true);
        levelUpSound.GetComponent<AudioSource>().Play();
        StartCoroutine(PlayLevelUpAnimations());
    }

    private IEnumerator PlayLevelUpAnimations()
    {
        EnableVFX(true);

        Transform textTransform = levelUpPanel.transform.Find("Text");
        Transform panelTransform = levelUpPanel.transform.Find("Panel");

        textTransform.gameObject.SetActive(true);

        Animator textAnimator = textTransform.GetComponent<Animator>();
        Animator panelAnimator = panelTransform.GetComponent<Animator>();

        textAnimator.Play("LevelUpTextAnim1");
        yield return WaitForAnimation("LevelUpTextAnim1", textAnimator);

        textTransform.gameObject.SetActive(false);
        panelTransform.gameObject.SetActive(true);

        panelAnimator.Play("LevelUpPanelAnim");
        yield return WaitForAnimation("LevelUpPanelAnim", panelAnimator);

        EnableVFX(false);
    }

    private void EnableVFX(bool enabled)
    {
        foreach (Transform vfx in levelUpPanel.transform.Find("Panel").transform)
        {
            if (vfx.CompareTag("LevelUpVFX"))
            {
                vfx.gameObject.SetActive(enabled);
            }
        }
    }

    private IEnumerator WaitForAnimation(string animationName, Animator animator)
    {
        // Wait until the animation starts playing
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
        {
            yield return null;
        }

        // Wait until the animation is done
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }
    }

    private IEnumerator PlayAppearVFX()
    {
        foreach (Transform item in ScrollRect.transform.GetChild(0).transform.GetChild(0).transform)
        {
            if (item.GetComponent<Image>() != null)
            {
                if (item.gameObject.activeSelf)
                {
                    levelUpAppearSound.GetComponent<AudioSource>().Play();

                    // Instantiate VFX
                    GameObject vfx = Instantiate(appearVFXPrefab, item.gameObject.transform);
                    vfx.transform.localPosition = Vector3.zero;

                    // Play the VFX animation and wait for it to complete
                    Animator vfxAnimator = vfx.GetComponent<Animator>();
                    vfxAnimator.Play("Appear");

                    // Play item animation if it has one
                    Animator itemAnimator = item.GetComponent<Animator>();
                    if (itemAnimator)
                    {
                        itemAnimator.enabled = true;
                        itemAnimator.Play("ImageAppear");
                    }

                    yield return WaitForAnimation("Appear", vfxAnimator);
                    Destroy(vfx);

                    yield return WaitForAnimation("ImageAppear", itemAnimator);

                    itemAnimator.enabled = false; 

                }
            }
        }
    }

    // Hides the level up panel
    private void HideLevelUpPanel()
    {
        Transform textTransform = levelUpPanel.transform.Find("Text");
        Transform panelTransform = levelUpPanel.transform.Find("Panel");

        foreach (GameObject ui in ToggledUI)
        {
            ui.SetActive(true);
        }

        textTransform.gameObject.SetActive(false);
        panelTransform.gameObject.SetActive(false);
        levelUpPanel.SetActive(false);
    }
    // Update the UI and add bucks
    private void OnCollectButtonClicked()
    {
        EarnedUnlockedText.text = "You unlocked";

        if (CurrencySystem.Instance != null)
        {
            CurrencySystem.Instance.AddCurrency(new CurrencyChangeGameEvent
            {
                CurrencyType = CurrencyType.Bucks,
                Amount = 2
            });
        }
        else
        {
            Debug.LogError("CurrencySystem.Instance is null. Make sure CurrencySystem is initialized properly.");
        }

        Animator animator = levelUpPanel.transform.Find("Panel").GetComponent<Animator>();

        animator.Play("ChangeAnim");

        EnableVFX(false);
        StartCoroutine(WaitForCollectAnimation(animator));
    }

    private IEnumerator WaitForCollectAnimation(Animator animator)
    {
        EnableVFX(false);
        yield return WaitForAnimation("ChangeAnim", animator);
        EnableVFX(false);
        CollectButton.gameObject.SetActive(false);
        BuckImage.gameObject.SetActive(false);
        BuckAmountText.gameObject.SetActive(false);
        ScrollRect.gameObject.SetActive(true);
        StartCoroutine(PlayAppearVFX());
        int itemCount = 0;
        foreach (Transform item in ScrollRect.transform.GetChild(0).transform.GetChild(0).transform)
        {
            if (item.gameObject.activeSelf)
            {
                itemCount++;
            }
        }
        float delay = itemCount * 0.65f;
        yield return new WaitForSeconds(delay);
        OkButton.gameObject.SetActive(true);
    }

    // Hide the level up panel and reset the UI
    private void OnOkButtonClicked()
    {
        HideLevelUpPanel();
        EarnedUnlockedText.text = "You earned";
        CollectButton.gameObject.SetActive(true);
        OkButton.gameObject.SetActive(false);
        BuckImage.gameObject.SetActive(true);
        BuckAmountText.gameObject.SetActive(true);
        ScrollRect.gameObject.SetActive(false);
    }
    // Handles saving
    private void Save()
    {
        Attributes.SetFloat("xp", XP);
        Attributes.SetInt("level", (int)level);
    }
}