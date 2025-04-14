using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    private int currentEvolutionIndex = -1;
    private int currentResearchAttempts = 0;
    private int currentEvolutionAttempts = 0;
    private bool isCurrentAttemptEvolution = false;

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
    [SerializeField] private float displayDelay = 0.4f;        // Delay between displaying each segment (initial attempt).
    [SerializeField] private float resetDelay = 0.05f;          // Delay to force a visual refresh.
    [SerializeField] private float xpDisplayDuration = 1.0f;   // How long the XP segment is shown before switching to a Retry button.

    [Header("Bar")]
    [SerializeField] private Image progressBar;
    [SerializeField] private Image whiteBar;
    [SerializeField] private Image activeBar;
    [SerializeField] private Image backgroundActiveBar;
    [SerializeField] private GameObject progressIndicator;
    [SerializeField] private RectTransform startPoint;
    [SerializeField] private RectTransform endPoint;
    [SerializeField] private float indicatorYPosition = 72.157f;

    [Header("Other UI")]
    [SerializeField] private Image successEffect;
    [SerializeField] private GameObject attemptResearchButton;
    [SerializeField] private GameObject V;
    [SerializeField] private GameObject X;
    [SerializeField] private TextMeshProUGUI attemptMessageText;
    [SerializeField] private TMPro.TextMeshProUGUI allAmberFoundText;

    [Header("Audio")]
    [SerializeField] private GameObject progressBarSound;
    [SerializeField] private GameObject researchSlotSound;
    [SerializeField] private GameObject goodResearchSlotSound;
    [SerializeField] private GameObject wrongResearchSlotSound;
    [SerializeField] private GameObject retryButtonSound;

    private string currentSpeciesName = "";

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
            string knownSpeciesName = selectedAmber.transform
                .Find("KnownSpeciesText(TMP)")?.GetComponent<TMP_Text>()?.text;

            if (!string.IsNullOrEmpty(knownSpeciesName))
            {
                currentSpeciesName = knownSpeciesName;
            }
            UpdateAttemptCostText();
            isCurrentAttemptEvolution = false;
            Debug.Log($"IsCurrentAttemptEvolution set to {isCurrentAttemptEvolution}");
        }
    }
    public void SetEvolutionIndex(int dinoIndex, int stageIndex)
    {
        currentEvolutionIndex = dinoIndex;
        EvolutionManager.lastEvolutionIndex = dinoIndex;
        EvolutionManager.lastStageIndex = stageIndex;

        DinoEvolution selectedEvolution = FindObjectsOfType<DinoEvolution>(true)
            .FirstOrDefault(e => e.DinoEvolutionIndex == dinoIndex);

        if (selectedEvolution != null)
        {
            EvolutionStage stage = selectedEvolution.GetStage(stageIndex);
            if (stage != null)
            {
                successRate = stage.successRate;
                requiredAttempts = stage.requiredAttempts;
                attemptCost = stage.attemptCost;

                Debug.Log($"ResearchManager - Stats from DinoEvolution {dinoIndex}, Stage {stageIndex}:");
                Debug.Log($"Success Rate: {successRate}%");
                Debug.Log($"Required Attempts: {requiredAttempts}");
                Debug.Log($"Attempt Cost: {attemptCost}");
                UpdateAttemptCostText();
                isCurrentAttemptEvolution = true;
                Debug.Log($"IsCurrentAttemptEvolution set to {isCurrentAttemptEvolution}");
            }
            else
            {
                Debug.LogWarning($"Evolution stage {stageIndex} not found for dino {dinoIndex}.");
            }
        }
        else
        {
            Debug.LogWarning($"DinoEvolution with index {dinoIndex} not found in scene.");
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
        
        ResearchPanel.SetActive(true);
        UIManager.Instance.ChangeFixedTo("PanelUI");
        UIManager.Instance.DisableCurrent();
        UIManager.Instance.ChangeCameraPanningStatus(false);
        SetProgressAndActiveBar();
        ResetSegmentsDisplay();
        SaveResearchProgress();
    }

    public void OpenNoAmberPanel()
    {
        if (PanelOpeningSound != null)
        {
            PanelOpeningSound.GetComponent<AudioSource>().Play();
        }

        Debug.Log("No amber to research");
        int amberCount = AmberManager.Instance.GetAmberList().Count;
        if (amberCount < 4)
        {
            allAmberFoundText.text = "Remove jungle elements to discover amber                                                    and research new dinosaur species";
        }
        else
        {
            allAmberFoundText.text = "Research unavailable, all amber found";
        }
        
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
        ResetSegmentsDisplay();
    }

    /// <summary>
    /// Begins a new research attempt.
    /// </summary>
    public void AttemptResearch()
    {
        // Deduct coins for this research attempt.
        if (!CurrencySystem.Instance.HasEnoughCurrency(CurrencyType.Coins, attemptCost))
        {
            CurrencySystem.Instance.AddCurrency(new CurrencyChangeGameEvent(-attemptCost, CurrencyType.Coins));
            return;
        }
        EventManager.Instance.TriggerEvent(new CurrencyChangeGameEvent(-attemptCost, CurrencyType.Coins));
        attemptResearchButton.SetActive(false);
        if (attemptMessageText != null)
        {
            attemptMessageText.gameObject.SetActive(true);
            attemptMessageText.text = "Processing";
        }

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
            if (!isCurrentAttemptEvolution)
            {
                currentResearchAttempts++;
                Debug.Log("Incremented currentResearchAttempts.");
            }
            else
            {
                currentEvolutionAttempts++;
                Debug.Log("Incremented currentEvolutionAttempts.");
            }
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
                    attemptSuccessful = false;
                    break;
                case 1:
                    Debug.Log("1V, 2X");
                    researchSegments[0] = true;
                    researchSegments[1] = false;
                    researchSegments[2] = false;
                    attemptSuccessful = false;
                    break;
                case 2:
                    Debug.Log("3X");
                    researchSegments[0] = false;
                    researchSegments[1] = false;
                    researchSegments[2] = false;
                    attemptSuccessful = false;
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
            AudioSource audio = researchSlotSound.GetComponent<AudioSource>();
            if (audio != null)
                audio.Play();
            yield return new WaitForSeconds(0.35f);
        }
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
            yield return new WaitForSeconds(displayDelay);
            // Refresh display for this segment.
            if (i < dnaSegments.Length && dnaSegments[i] != null)
                dnaSegments[i].SetActive(false);
            if (i < xpSegments.Length && xpSegments[i] != null)
                xpSegments[i].SetActive(false);
            if (i < retryButtons.Length && retryButtons[i] != null)
                retryButtons[i].SetActive(false);

            yield return new WaitForSeconds(resetDelay);

            FlashVFX[i].SetActive(true);
            yield return new WaitForSeconds(resetDelay);
            FlashVFX[i].SetActive(false);
            VFX[i].GetComponent<Animator>().Play("FlashDissapear");
            yield return new WaitForSeconds(resetDelay);
            VFX[i].SetActive(false);

            // Show outcome.
            if (researchSegments[i])
            {
                if (i < dnaSegments.Length && dnaSegments[i] != null)
                    dnaSegments[i].SetActive(true);
                    AudioSource audio = goodResearchSlotSound.GetComponent<AudioSource>();
                    if (audio != null)
                        audio.Play();
            }
            else
            {
                StartCoroutine(ShowFailureFeedback(i));
            }
        }
        if (!attemptSuccessful)
        {
            X.SetActive(true);
            if (attemptMessageText != null)
            {
                attemptMessageText.gameObject.SetActive(false);
            }
            yield return new WaitForSeconds(1.2f);
        }
        if (attemptSuccessful)
        {
            V.SetActive(true);
            if (attemptMessageText != null)
            {
                attemptMessageText.text = "   Success";
            }
            UpdateProgressBar();
            FadeEffectIn(0.2f);
            if (!isCurrentAttemptEvolution)
            {
                if (currentResearchAttempts >= requiredAttempts)
                {
                    yield return new WaitForSeconds(1f);
                }
                else
                {
                    yield return new WaitForSeconds(1.9f);
                }
            }
            else
            {
                if (currentEvolutionAttempts >= requiredAttempts)
                {
                    yield return new WaitForSeconds(1f);
                }
                else
                {
                    yield return new WaitForSeconds(1.9f);
                }
            }
            FadeEffectOut(0.4f);
        }
        // After displaying all segments, if the required attempts have been reached or exceeded, complete research.
        if (!isCurrentAttemptEvolution)
        {
            if (currentResearchAttempts >= requiredAttempts)
            {
                ClosePanel();
                ResearchCompleteManager.Instance.SetSpeciesName(currentSpeciesName);
                ResearchCompleteManager.Instance.OpenPanel();
                CompleteResearch();
                SelectablesManager.Instance.UnselectAll();
            }
        }
        else
        {  
            if (currentEvolutionAttempts >= requiredAttempts)
            {
                ClosePanel();
                CompleteResearch();
                SelectablesManager.Instance.UnselectAll();
            }
        }
        attemptMessageText.gameObject.SetActive(false);
        X.SetActive(false);
        V.SetActive(false);
        bool anyVFXActive = VFX.Any(v => v.activeSelf);
        if (!anyVFXActive)
        {
            if (attemptSuccessful)
            {
                yield return new WaitForSeconds(0.25f);
            }
            StartCoroutine(FadeInButton(attemptResearchButton));
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
        for (int i = 0; i < retryButtons.Length; i++)
        {
            if (i != index && retryButtons[i] != null)
                retryButtons[i].SetActive(false);
        }

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
        X.SetActive(false);
        V.SetActive(false);
        attemptResearchButton.SetActive(false);
        if (attemptMessageText != null)
        {
            attemptMessageText.gameObject.SetActive(true);
            attemptMessageText.text = "Processing";
        }
        AudioSource audio = researchSlotSound.GetComponent<AudioSource>();
        if (audio != null)
            audio.Play();
        VFX[index].SetActive(true);
        yield return new WaitForSeconds(0.8f);
        // Hide all visuals for the segment
        if (index < dnaSegments.Length && dnaSegments[index] != null)
            dnaSegments[index].SetActive(false);
        if (index < xpSegments.Length && xpSegments[index] != null)
            xpSegments[index].SetActive(false);
        if (index < retryButtons.Length && retryButtons[index] != null)
            retryButtons[index].SetActive(false);

        yield return new WaitForSeconds(resetDelay);

        // Play the appearance animation again (Flash & VFX)
        if (index < FlashVFX.Length && FlashVFX[index] != null)
        {
            FlashVFX[index].SetActive(true);
            yield return new WaitForSeconds(resetDelay);
            FlashVFX[index].SetActive(false);
        }

        if (index < VFX.Length && VFX[index] != null)
        {
            VFX[index].SetActive(true);
            var animator = VFX[index].GetComponent<Animator>();
            if (animator != null)
            {
                animator.Play("FlashDissapear");
            }
            yield return new WaitForSeconds(resetDelay);
            VFX[index].SetActive(false);
        }

        // Show the result after the animation
        if (researchSegments[index])
        {
            X.SetActive(false);
            V.SetActive(true);
            if (index < dnaSegments.Length && dnaSegments[index] != null)
                dnaSegments[index].SetActive(true);
            audio = goodResearchSlotSound.GetComponent<AudioSource>();
            if (audio != null)
                audio.Play();
            if (attemptMessageText != null)
            {
                attemptMessageText.text = "   Success";
            }
        }
        else
        {
            int xpToGive = Mathf.RoundToInt(5 * LevelManager.Instance.GetCurrentLevel());
            EventManager.Instance.TriggerEvent(new XPAddedGameEvent(xpToGive));
            Debug.Log($"Xp given: {xpToGive}");
            X.SetActive(true);
            V.SetActive(false);
            // Show XP segment, then retry button
            if (index < xpSegments.Length && xpSegments[index] != null)
                xpSegments[index].SetActive(true);
            audio = wrongResearchSlotSound.GetComponent<AudioSource>();
            if (audio != null)
                audio.Play();
            if (attemptMessageText != null)
            {
                attemptMessageText.gameObject.SetActive(false);
            }
            yield return new WaitForSeconds(0.77f);
            audio = retryButtonSound.GetComponent<AudioSource>();
            if (audio != null)
                audio.Play();
            yield return new WaitForSeconds(0.48f);
            if (index < xpSegments.Length && xpSegments[index] != null)
                xpSegments[index].SetActive(false);
            if (index < retryButtons.Length && retryButtons[index] != null)
                retryButtons[index].SetActive(true);
        }
        CheckResearchCompletion();
        if (attemptSuccessful)
        {
            V.SetActive(true);
            UpdateProgressBar();
            FadeEffectIn(0.2f);
            if (!isCurrentAttemptEvolution)
            {
                if (currentResearchAttempts >= requiredAttempts)
                {
                    yield return new WaitForSeconds(1f);
                }
                else
                {
                    yield return new WaitForSeconds(1.9f);
                }
            }
            else
            {
                if (currentEvolutionAttempts >= requiredAttempts)
                {
                    yield return new WaitForSeconds(1f);
                }
                else
                {
                    yield return new WaitForSeconds(1.9f);
                }
            }
            FadeEffectOut(0.4f);
            // If the required attempts are met or exceeded, complete research.
            if (!isCurrentAttemptEvolution)
            {
                if (currentResearchAttempts >= requiredAttempts)
                {
                    ClosePanel();
                    ResearchCompleteManager.Instance.SetSpeciesName(currentSpeciesName);
                    ResearchCompleteManager.Instance.OpenPanel();
                    CompleteResearch();
                    SelectablesManager.Instance.UnselectAll();
                }
            }
            else
            {  
                if (currentEvolutionAttempts >= requiredAttempts)
                {
                    ClosePanel();
                    CompleteResearch();
                    SelectablesManager.Instance.UnselectAll();
                }
            }
        }
        else if (!attemptSuccessful && V.activeSelf)
        {
            yield return new WaitForSeconds(1f);
        }
        for (int i = 0; i < researchSegments.Length; i++)
        {
            if (i != index && researchSegments[i] == false)
            {
                if (i < retryButtons.Length && retryButtons[i] != null)
                    retryButtons[i].SetActive(true);
            }
        }
        if (attemptMessageText != null)
        {
            attemptMessageText.gameObject.SetActive(false);
        }
        V.SetActive(false);
        X.SetActive(false);
        if (attemptSuccessful)
        {
            yield return new WaitForSeconds(0.25f);
        }
        StartCoroutine(FadeInButton(attemptResearchButton));
    }

    private IEnumerator ShowFailureFeedback(int index)
    {
        AudioSource audio = wrongResearchSlotSound.GetComponent<AudioSource>();
        int xpToGive = Mathf.RoundToInt(5 * LevelManager.Instance.GetCurrentLevel());
        EventManager.Instance.TriggerEvent(new XPAddedGameEvent(xpToGive));
        Debug.Log($"Xp given: {xpToGive}");
        if (audio != null)
            audio.Play();
        if (index < xpSegments.Length && xpSegments[index] != null)
            xpSegments[index].SetActive(true);

        yield return new WaitForSeconds(0.77f);
        audio = retryButtonSound.GetComponent<AudioSource>();
        if (audio != null)
            audio.Play();
        yield return new WaitForSeconds(0.48f);

        if (index < xpSegments.Length && xpSegments[index] != null)
            xpSegments[index].SetActive(false);

        bool anyVFXActive = VFX.Any(v => v.activeSelf);
        if (index < retryButtons.Length && retryButtons[index] != null && !anyVFXActive)
            retryButtons[index].SetActive(true);
    }

    private void SetProgressAndActiveBar()
    {
        float targetFill = 0;
        if (!isCurrentAttemptEvolution)
        {
            targetFill = Mathf.Clamp01((float)currentResearchAttempts / requiredAttempts);
        }
        else
        {
            targetFill = Mathf.Clamp01((float)currentEvolutionAttempts / requiredAttempts);
        }
        float targetX = Mathf.Lerp(startPoint.localPosition.x, endPoint.localPosition.x, targetFill);
        progressBar.fillAmount = targetFill;
        activeBar.fillAmount = targetFill;
        backgroundActiveBar.fillAmount = targetFill;
        progressIndicator.transform.localPosition = new Vector3(targetX, indicatorYPosition, progressIndicator.transform.localPosition.z);
    }
    private Coroutine moveIndicatorRoutine;

    private void UpdateProgressIndicator()
    {
        float progressPercentage = 0;
        if (!isCurrentAttemptEvolution)
        {
            progressPercentage = Mathf.Clamp01((float)currentResearchAttempts / requiredAttempts);
        }
        else
        {
            progressPercentage = Mathf.Clamp01((float)currentEvolutionAttempts / requiredAttempts);
        }
        float targetX = Mathf.Lerp(startPoint.localPosition.x, endPoint.localPosition.x, progressPercentage);

        if (progressIndicator != null)
        {
            if (moveIndicatorRoutine != null)
            {
                StopCoroutine(moveIndicatorRoutine);
            }

            moveIndicatorRoutine = StartCoroutine(SmoothMoveIndicator(targetX));
        }
    }

    private IEnumerator SmoothMoveIndicator(float targetX)
    {
        float startX = progressIndicator.transform.localPosition.x;
        float speed = Mathf.Abs(targetX - startX) / 1f;
        float time = 0f;
        while (Mathf.Abs(progressIndicator.transform.localPosition.x - targetX) > 0.01f)
        {
            float step = speed * Time.deltaTime;
            progressIndicator.transform.localPosition = Vector3.MoveTowards(
                progressIndicator.transform.localPosition, 
                new Vector3(targetX, indicatorYPosition, progressIndicator.transform.localPosition.z), 
                step
            );

            yield return null;
        }
        progressIndicator.transform.localPosition = new Vector3(targetX, indicatorYPosition, progressIndicator.transform.localPosition.z);
    }

    private Coroutine progressFillRoutine;

    private void UpdateProgressBar()
    {
        AudioSource audio = progressBarSound.GetComponent<AudioSource>();
        if (audio != null)
            audio.Play();
        UpdateProgressIndicator();
        if (progressBar == null || requiredAttempts <= 0) return;
        float targetFill = 0;
        if (!isCurrentAttemptEvolution)
        {
            targetFill = Mathf.Clamp01((float)currentResearchAttempts / requiredAttempts);
        }
        else
        {
            targetFill = Mathf.Clamp01((float)currentEvolutionAttempts / requiredAttempts);
        }
        if (progressFillRoutine != null)
            StopCoroutine(progressFillRoutine);
        progressFillRoutine = StartCoroutine(FillProgressAndTriggerSecond(targetFill));
    }

    private IEnumerator FillProgressAndTriggerSecond(float target)
    {
        yield return StartCoroutine(SmoothFillBar(target));

        if (whiteBar != null)
        {
            float newFill = 0;
            if (!isCurrentAttemptEvolution)
            {
                newFill = Mathf.Clamp01((float)currentResearchAttempts / requiredAttempts);
            }
            else
            {
                newFill = Mathf.Clamp01((float)currentEvolutionAttempts / requiredAttempts);
            }

            whiteBar.fillAmount = newFill;

            StartCoroutine(FadeInOutBar());
        }
    }

    private IEnumerator SmoothFillBar(float target)
    {
        Color color = progressBar.color;
        progressBar.color = new Color(color.r, color.g, color.b, 1f);
        float start = progressBar.fillAmount;
        float duration = 1f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            progressBar.fillAmount = Mathf.Lerp(start, target, t);
            yield return null;
        }

        progressBar.fillAmount = target;
    }

    private IEnumerator FadeInOutBar()
    {
        if (whiteBar == null) yield break;

        Color originalColor = whiteBar.color;

        // FADE IN
        float fadeInTime = 0.05f;
        float t = 0f;
        while (t < fadeInTime)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, t / fadeInTime);
            whiteBar.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        Color color = progressBar.color;
        progressBar.color = new Color(color.r, color.g, color.b, 0f);
        float targetFill = 0;
        if (!isCurrentAttemptEvolution)
        {
            targetFill = Mathf.Clamp01((float)currentResearchAttempts / requiredAttempts);
        }
        else
        {
            targetFill = Mathf.Clamp01((float)currentEvolutionAttempts / requiredAttempts);
        }
        backgroundActiveBar.fillAmount = targetFill;
        // FADE OUT
        float fadeOutTime = 0.2f;
        t = 0f;
        while (t < fadeOutTime)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / fadeOutTime);
            whiteBar.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        whiteBar.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        activeBar.fillAmount = targetFill;
    }

    public void FadeEffectIn(float duration)
    {
        StartCoroutine(FadeEffectCoroutine(0f, 0.8f, duration));
    }

    public void FadeEffectOut(float duration)
    {
        StartCoroutine(FadeEffectCoroutine(0.8f, 0f, duration));
    }

    private IEnumerator FadeEffectCoroutine(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;

        // Set the initial alpha value
        Color startColor = successEffect.color;
        successEffect.color = new Color(startColor.r, startColor.g, startColor.b, startAlpha);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            successEffect.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        // Ensure the final alpha value is set (in case the loop finishes early)
        successEffect.color = new Color(startColor.r, startColor.g, startColor.b, endAlpha);
    }
    private IEnumerator FadeInButton(GameObject buttonObj, float duration = 0.2f)
    {
        CanvasGroup cg = buttonObj.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            Debug.LogWarning("CanvasGroup not found on attemptResearchButton.");
            yield break;
        }

        buttonObj.SetActive(true);
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        cg.alpha = 1f;
        cg.interactable = true;
        cg.blocksRaycasts = true;
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
            if (!isCurrentAttemptEvolution)
            {
                currentResearchAttempts++;
                Debug.Log("Incremented currentResearchAttempts.");
            }
            else
            {
                currentEvolutionAttempts++;
                Debug.Log("Incremented currentEvolutionAttempts.");
            }
            SaveResearchProgress();

            // Hide all Retry buttons.
            foreach (var btn in retryButtons)
            {
                if (btn != null)
                    btn.SetActive(false);
            }
        }
    }

    private void CompleteResearch()
    {
        if (!isCurrentAttemptEvolution)
        {
            int index = GetCurrentAmberIndex();
            DinoAmber.EnableDinoAndEnableOtherDecodeButtons(index);
            Debug.Log($"Research completed, dinosaur with amber index {index}, ({currentSpeciesName}) unlocked and research attempts reset to: {currentResearchAttempts}");
            currentResearchAttempts = 0;
        }
        else
        {
            int evolutionIndex = EvolutionManager.lastEvolutionIndex;
            int stageIndex = EvolutionManager.lastStageIndex;
            DinoEvolution[] allEvolutions = FindObjectsOfType<DinoEvolution>(true);
            DinoEvolution targetEvolution = allEvolutions.FirstOrDefault(e => e.DinoEvolutionIndex == evolutionIndex);
            
            if (targetEvolution != null)
            {
                if (targetEvolution.DinoToDisable != null)
                {
                    targetEvolution.DinoToDisable.SetActive(true);
                }

                if (targetEvolution.evolutionIconToEnable != null)
                {
                    targetEvolution.evolutionIconToEnable.SetActive(false);
                }
                Paddock paddock = targetEvolution.GetComponentInParent<Paddock>();
                if (paddock != null)
                {
                    DinosaurLevelManager dinoLevelManager = paddock.GetComponentInChildren<DinosaurLevelManager>(true);
                    if (dinoLevelManager != null)
                    {
                        int targetLevel = 11 + (stageIndex * 10);
                        dinoLevelManager.CurrentLevel = targetLevel;
                        dinoLevelManager._feedingSystem.feedCount = 0;
                        string paddockName = paddock.gameObject.name;
                        Attributes.SetInt("CurrentLevel" + paddockName, targetLevel);
                        Attributes.SetInt("FeedCount" + paddockName, 0);
                        dinoLevelManager.Initialize();
                        dinoLevelManager._feedingSystem.disableModels();
                        EvolutionChanger evolutionChanger = paddock.GetComponentInChildren<EvolutionChanger>(true);
                        if (evolutionChanger != null)
                        {
                            Debug.Log("EvolutionChanger found, updating...");
                            evolutionChanger.UpdateSkinBasedOnLevel();
                        }
                        else
                        {
                            Debug.LogWarning("EvolutionChanger not found");
                        }
                        HatchingTimer hatchingTimer = FindObjectsOfType<HatchingTimer>(true)
                            .FirstOrDefault(ht => ht.paddockScript == paddock);
                        if (hatchingTimer != null)
                        {
                            if (!hatchingTimer.gameObject.activeInHierarchy)
                            {
                                hatchingTimer.gameObject.SetActive(true);
                            }
                            hatchingTimer.StartHatchingTimer();
                            Debug.Log($"Hatching started for dinosauro'{paddock.name}'");
                        }
                        else
                        {
                            Debug.LogWarning($"No hatching found on '{paddock.name}'");
                        }
                        Debug.Log($"Dinosaur in paddock '{paddockName}' set to level {targetLevel} and feedCount reset to 0");
                    }
                }
            }
            else
            {
                Debug.LogWarning($"No DinoEvolution found with EvolutionIndex {evolutionIndex}");
            }

            DinoAmber.EnableDinoAndEnableOtherDecodeButtons(-1);
            EvolutionManager.lastEvolutionIndex = -1;
            EvolutionManager.lastStageIndex = -1;
            currentEvolutionAttempts = 0;
        }
        SaveResearchProgress();
    }

    public void SaveResearchProgress()
    {
        SaveManager.Instance.SaveData.ResearchData.CurrentResearchAttempts = currentResearchAttempts;
        SaveManager.Instance.SaveData.ResearchData.CurrentEvolutionAttempts = currentEvolutionAttempts;
        SaveManager.Instance.SaveData.ResearchData.LastDecodedAmberIndex = DinoAmber.lastDecodedAmberIndex;
        SaveManager.Instance.SaveData.ResearchData.LastEvolutionIndex = EvolutionManager.lastEvolutionIndex;
        SaveManager.Instance.SaveData.ResearchData.LastStageIndex = EvolutionManager.lastStageIndex;
        SaveManager.Instance.SaveData.ResearchData.TutorialDebrisSpawned = TutorialDebrisSpawner.tutorialDebrisSpawned;
    }

    public void Load()
    {
        currentResearchAttempts = SaveManager.Instance.SaveData.ResearchData.CurrentResearchAttempts;
        currentEvolutionAttempts = SaveManager.Instance.SaveData.ResearchData.CurrentEvolutionAttempts;
        DinoAmber.lastDecodedAmberIndex = SaveManager.Instance.SaveData.ResearchData.LastDecodedAmberIndex;
        EvolutionManager.lastEvolutionIndex = SaveManager.Instance.SaveData.ResearchData.LastEvolutionIndex;
        EvolutionManager.lastStageIndex = SaveManager.Instance.SaveData.ResearchData.LastStageIndex;
        TutorialDebrisSpawner.tutorialDebrisSpawned = SaveManager.Instance.SaveData.ResearchData.TutorialDebrisSpawned;
        Debug.Log($"Loaded research progress, saved attempts: {currentResearchAttempts}");
        Debug.Log($"Loaded evolution progress, saved attempts: {currentEvolutionAttempts}");
        Debug.Log($"Loaded dino decoding index: {DinoAmber.lastDecodedAmberIndex} (if -1 means there's no dino being decoded)");
        Debug.Log($"Loaded evolution index: {EvolutionManager.lastEvolutionIndex} (if -1 means there's no dino in evolution)");
        Debug.Log($"Loaded stage index: {EvolutionManager.lastStageIndex}");

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