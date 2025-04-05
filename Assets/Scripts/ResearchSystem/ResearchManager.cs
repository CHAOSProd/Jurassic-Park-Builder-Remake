using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class ResearchManager : Singleton<ResearchManager>
{
    [Header("UI")]
    [SerializeField] private GameObject ResearchPanel;
    [SerializeField] private GameObject NoAmberPanel;
    [SerializeField] private GameObject AmberNotification;
    [SerializeField] private TMP_Text attemptCostText;

    [Header("Sound")]
    [SerializeField] private GameObject PanelOpeningSound;
        
    [Header("Research Settings")]
    private float successRate;
    private int requiredAttempts;
    private int attemptCost;

    private int currentAmberIndex = -1;
    private int currentAttempts = 0;
    // true if the segment outcome is DNA; false if XP.
    bool[] researchSegments = new bool[3];

    [Header("DNA & XP Segments")]
    [SerializeField] private GameObject[] dnaSegments;   // Shown when segment outcome is true.
    [SerializeField] private GameObject[] xpSegments;      // Shown when segment outcome is false.
    [SerializeField] private GameObject[] VFX;
    [SerializeField] private GameObject[] FlashVFX;

    [Header("Retry Buttons")]
    [SerializeField] private GameObject[] retryButtons;    // One per segment.

    [Header("Display Settings")]
    [SerializeField] private float displayDelay = 0.5f;        // Delay between displaying each segment (initial attempt).
    [SerializeField] private float resetDelay = 0.2f;          // Delay to force a visual refresh.
    [SerializeField] private float xpDisplayDuration = 1.0f;   // How long the XP segment is shown before switching to a Retry button.

    // Flags for the current research attempt.
    private bool attemptInProgress = false;
    private bool attemptSuccessful = false;

    private void Start()
    {
        // Ensure that all segments and retry buttons are deactivated at the start.
        ResetSegmentsDisplay();
    }

    /// <summary>
    /// Deactivates all DNA, XP, and Retry button objects.
    /// </summary>
    private void ResetSegmentsDisplay()
    {
        if (dnaSegments != null)
        {
            foreach (var segment in dnaSegments)
            {
                if (segment != null)
                    segment.SetActive(false);
            }
        }
        if (xpSegments != null)
        {
            foreach (var segment in xpSegments)
            {
                if (segment != null)
                    segment.SetActive(false);
            }
        }
        if (retryButtons != null)
        {
            foreach (var button in retryButtons)
            {
                if (button != null)
                    button.SetActive(false);
            }
        }
    }

    public void UpdateAttemptCostText()
    {
        if (attemptCostText != null)
        {
            attemptCostText.text = attemptCost.ToString();
        }
        else
        {
            Debug.LogWarning("Attempt Cost Text is not assigned in the ResearchManager.");
        }
    }

    public void SetAmberIndex(int index)
    {
        currentAmberIndex = index;
        DinoAmber selectedAmber = FindObjectsOfType<DinoAmber>(true)
            .FirstOrDefault(a => a.DinoAmberIndex == index);
        if (selectedAmber != null)
        {
            successRate = selectedAmber.GetSuccessRate();
            requiredAttempts = selectedAmber.GetRequiredAttempts();
            attemptCost = selectedAmber.GetAttemptCost();
            Debug.Log($"ResearchManager - Stats taken from DinoAmber {index}:");
            Debug.Log($"Success Rate: {successRate}%");
            Debug.Log($"Required Attempts: {requiredAttempts}");
            Debug.Log($"Attempt Cost: {attemptCost} coins");
            UpdateAttemptCostText();
        }
    }

    public int GetCurrentAmberIndex()
    {
        return currentAmberIndex;
    }

    public void OpenPanel()
    {
        if (PanelOpeningSound != null)
        {
            PanelOpeningSound.GetComponent<AudioSource>().Play();
        }

        Debug.Log($"Currently researching dino amber index: {currentAmberIndex}");
        
        ResearchPanel.SetActive(true);
        UIManager.Instance.ChangeFixedTo("PanelUI");
        UIManager.Instance.DisableCurrent();
        UIManager.Instance.ChangeCameraPanningStatus(false);
    }

    public void OpenNoAmberPanel()
    {
        if (PanelOpeningSound != null)
        {
            PanelOpeningSound.GetComponent<AudioSource>().Play();
        }

        Debug.Log("No amber to research");
        
        NoAmberPanel.SetActive(true);
        UIManager.Instance.ChangeFixedTo("PanelUI");
        UIManager.Instance.DisableCurrent();
        UIManager.Instance.ChangeCameraPanningStatus(false);
    }

    public void ActivateAmberNotification()
    {
        if (AmberManager.Instance.HasUndecodedActivatedAmber())
        {
            AmberNotification.SetActive(true);
        }
    }

    public void DeactivateAmberNotification()
    {
        if (!AmberManager.Instance.HasUndecodedActivatedAmber())
        {
            AmberNotification.SetActive(false);
        }
    }

    /// <summary>
    /// Closes the research panel and deactivates the amber notification.
    /// </summary>
    public void ClosePanel()
    {
        ResearchPanel.SetActive(false);
        UIManager.Instance.ChangeFixedTo("DefaultUI");
        UIManager.Instance.EnableCurrent();
        UIManager.Instance.ChangeCameraPanningStatus(true);
        DeactivateAmberNotification();
    }

    /// <summary>
    /// Begins a new research attempt.
    /// </summary>
    public void AttemptResearch()
    {
        // Deduct coins for this research attempt.
        EventManager.Instance.TriggerEvent(new CurrencyChangeGameEvent(-attemptCost, CurrencyType.Coins));

        attemptInProgress = true;
        attemptSuccessful = false;

        // Reset visual display.
        ResetSegmentsDisplay();

        float roll = Random.Range(0f, 100f);

        // Determine outcome for each segment.
        if (roll <= successRate)
        {
            // All segments are DNA.
            researchSegments[0] = true;
            researchSegments[1] = true;
            researchSegments[2] = true;
            attemptSuccessful = true;
            Debug.Log("3V, Research attempt succeeded");
            currentAttempts++;
            SaveResearchProgress();
        }
        else
        {
            // Randomly choose a failure type.
            int failureType = Random.Range(0, 3);
            switch (failureType)
            {
                case 0:
                    Debug.Log("2V, 1X");
                    researchSegments[0] = true;
                    researchSegments[1] = true;
                    researchSegments[2] = false;
                    break;
                case 1:
                    Debug.Log("1V, 2X");
                    researchSegments[0] = true;
                    researchSegments[1] = false;
                    researchSegments[2] = false;
                    break;
                case 2:
                    Debug.Log("3X");
                    researchSegments[0] = false;
                    researchSegments[1] = false;
                    researchSegments[2] = false;
                    break;
            }
        }

        // Display outcomes sequentially and the vfx.

        StartCoroutine(DisplayVFX());
    }

    /// <summary>
    /// Displays the vfx before the outcomes appear
    /// </summary>

    private IEnumerator DisplayVFX()
    {
        for (int i = 0; i < researchSegments.Length; i++)
        {
            VFX[i].SetActive(true);

            yield return new WaitForSeconds(displayDelay);
        }
        yield return new WaitForSeconds(displayDelay);
        //Display the outcome
        StartCoroutine(DisplayResearchOutcome());
    }

    /// <summary>
    /// Displays each research segment outcome one by one.
    /// For segments with a failure (XP), the XP object is shown for a duration then replaced by a Retry button.
    /// </summary>
    private IEnumerator DisplayResearchOutcome()
    {
        for (int i = 0; i < researchSegments.Length; i++)
        {
            // Refresh display for this segment.
            if (i < dnaSegments.Length && dnaSegments[i] != null)
                dnaSegments[i].SetActive(false);
            if (i < xpSegments.Length && xpSegments[i] != null)
                xpSegments[i].SetActive(false);
            if (i < retryButtons.Length && retryButtons[i] != null)
                retryButtons[i].SetActive(false);

            yield return new WaitForSeconds(resetDelay);

            FlashVFX[i].SetActive(true);
            yield return new WaitForSeconds(0.1f);
            FlashVFX[i].SetActive(false);
            VFX[i].GetComponent<Animator>().Play("FlashDissapear");
            yield return new WaitForSeconds(resetDelay);
            VFX[i].SetActive(false);

            // Show outcome.
            if (researchSegments[i])
            {
                if (i < dnaSegments.Length && dnaSegments[i] != null)
                    dnaSegments[i].SetActive(true);
            }
            else
            {
                // First show the XP segment to indicate failure.
                if (i < xpSegments.Length && xpSegments[i] != null)
                    xpSegments[i].SetActive(true);
                // Wait for a set duration.
                yield return new WaitForSeconds(xpDisplayDuration);
                // Then hide the XP segment and show the Retry button.
                if (i < xpSegments.Length && xpSegments[i] != null)
                    xpSegments[i].SetActive(false);
                if (i < retryButtons.Length && retryButtons[i] != null)
                    retryButtons[i].SetActive(true);
            }
            yield return new WaitForSeconds(displayDelay);
        }
        // After displaying all segments, if the required attempts have been reached or exceeded, complete research.
        if (currentAttempts >= requiredAttempts)
        {
            CompleteResearch();
            ClosePanel();
            SelectablesManager.Instance.UnselectAll();
        }
    }

    /// <summary>
    /// Called by a Retry button for the given segment index.
    /// This method performs a new roll for that segment.
    /// Each retry costs one buck. If the player does not have enough bucks,
    /// the not enough bucks panel is triggered via the CurrencySystem.
    /// </summary>
    /// <param name="index">Index of the segment to retry.</param>
    public void RetrySegment(int index)
    {
        // Only allow retries if an attempt is in progress and not already successful.
        if (!attemptInProgress || attemptSuccessful)
            return;

        // Check if the player has at least one buck.
        if (!CurrencySystem.Instance.HasEnoughCurrency(CurrencyType.Bucks, 1))
        {
            // This call will trigger the not enough bucks panel in CurrencySystem.
            CurrencySystem.Instance.AddCurrency(new CurrencyChangeGameEvent(-1, CurrencyType.Bucks));
            Debug.Log("Not enough Bucks to retry!");
            return;
        }
        
        // Deduct one buck for the retry.
        CurrencySystem.Instance.AddCurrency(new CurrencyChangeGameEvent(-1, CurrencyType.Bucks));

        // Hide the Retry button immediately.
        if (index < retryButtons.Length && retryButtons[index] != null)
            retryButtons[index].SetActive(false);

        float roll = Random.Range(0f, 100f);
        if (roll <= successRate)
        {
            researchSegments[index] = true;
            Debug.Log($"Segment {index} retried: Success (DNA).");
        }
        else
        {
            researchSegments[index] = false;
            Debug.Log($"Segment {index} retried: Failed (XP).");
        }
        // Refresh this segment's display after the retry.
        StartCoroutine(RefreshSegmentDisplay(index));
    }

    /// <summary>
    /// Refreshes the display for a single segment after a retry.
    /// Hides the retry button, then after a brief delay shows the outcome of the reroll.
    /// For a failure, the XP segment is shown briefly then the Retry button is reactivated.
    /// </summary>
    private IEnumerator RefreshSegmentDisplay(int index)
    {
        // Hide both objects.
        if (index < dnaSegments.Length && dnaSegments[index] != null)
            dnaSegments[index].SetActive(false);
        if (index < xpSegments.Length && xpSegments[index] != null)
            xpSegments[index].SetActive(false);
        if (index < retryButtons.Length && retryButtons[index] != null)
            retryButtons[index].SetActive(false);

        yield return new WaitForSeconds(resetDelay);

        if (researchSegments[index])
        {
            // If the retry succeeded, show the DNA segment.
            if (index < dnaSegments.Length && dnaSegments[index] != null)
                dnaSegments[index].SetActive(true);
        }
        else
        {
            // If the retry failed, show the XP segment briefly then display the Retry button.
            if (index < xpSegments.Length && xpSegments[index] != null)
                xpSegments[index].SetActive(true);
            yield return new WaitForSeconds(xpDisplayDuration);
            if (index < xpSegments.Length && xpSegments[index] != null)
                xpSegments[index].SetActive(false);
            if (index < retryButtons.Length && retryButtons[index] != null)
                retryButtons[index].SetActive(true);
        }

        // After each retry update, check if all segments are now DNA.
        CheckResearchCompletion();
    }

    /// <summary>
    /// Checks if all segments are DNA; if so, marks the research attempt as successful.
    /// </summary>
    private void CheckResearchCompletion()
    {
        bool allTrue = true;
        for (int i = 0; i < researchSegments.Length; i++)
        {
            if (!researchSegments[i])
            {
                allTrue = false;
                break;
            }
        }

        if (allTrue && !attemptSuccessful)
        {
            attemptSuccessful = true;
            Debug.Log("Research attempt succeeded via retries.");
            currentAttempts++;
            SaveResearchProgress();

            // Hide all Retry buttons.
            foreach (var btn in retryButtons)
            {
                if (btn != null)
                    btn.SetActive(false);
            }

            // If the required attempts are met or exceeded, complete research.
            if (currentAttempts >= requiredAttempts)
            {
                CompleteResearch();
                ClosePanel();
                SelectablesManager.Instance.UnselectAll();
            }
        }
    }

    private void CompleteResearch()
    {
        int index = GetCurrentAmberIndex();
        DinoAmber.EnableDinoAndEnableOtherDecodeButtons(index);
        currentAttempts = 0;
        Debug.Log($"Research completed, dinosaur {index} unlocked and research attempts reset to: {currentAttempts}");
        SaveResearchProgress();
    }

    public void SaveResearchProgress()
    {
        SaveManager.Instance.SaveData.ResearchData.CurrentAttempts = currentAttempts;
        SaveManager.Instance.SaveData.ResearchData.LastDecodedAmberIndex = DinoAmber.lastDecodedAmberIndex;
        SaveManager.Instance.SaveData.ResearchData.TutorialDebrisSpawned = TutorialDebrisSpawner.tutorialDebrisSpawned;
    }

    public void Load()
    {
        currentAttempts = SaveManager.Instance.SaveData.ResearchData.CurrentAttempts;
        DinoAmber.lastDecodedAmberIndex = SaveManager.Instance.SaveData.ResearchData.LastDecodedAmberIndex;
        TutorialDebrisSpawner.tutorialDebrisSpawned = SaveManager.Instance.SaveData.ResearchData.TutorialDebrisSpawned;
        Debug.Log($"Loaded research progress, saved attempts: {currentAttempts}");
        Debug.Log($"Loaded dino decoding index: {DinoAmber.lastDecodedAmberIndex} (if -1 means there's no dino being decoded)");

        if (TutorialDebrisSpawner.tutorialDebrisSpawned)
        {
            Debug.Log("Tutorial debris already spawned.");
        }
        else
        {
            Debug.Log("Spawning tutorial debris.");
        }
    }
}
