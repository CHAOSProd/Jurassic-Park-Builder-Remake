using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DinosaurFeedingUIManager : MonoBehaviour
{
    public static DinosaurFeedingUIManager Instance;

    [Header("UI Components")]
    public Image feedFillImage;    // Image with Image Type = Filled (Horizontal)
    public FeedButton feedButton;  // Your custom FeedButton script
    public Button evolveButton;    // Standard evolve button
    public Image pulsingBar;  // New pulsing bar Image

    // The currently selected dinosaur’s feeding system.
    private DinosaurFeedingSystem currentDinosaur;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (feedButton == null)
            feedButton = FeedButton.Instance;

        if (feedButton != null)
        {
            feedButton.OnClickEvent.AddListener(OnFeedButtonClicked);
        }
        else
        {
            Debug.LogWarning("FeedButton reference missing in DinosaurFeedingUIManager.");
        }
    }

    public void SetSelectedDinosaur(DinosaurFeedingSystem dinosaur)
    {
        currentDinosaur = dinosaur;
        UpdateUI();
        Debug.Log("Selected dinosaur set in UI Manager.");
    }

    public void DisableEvolutionButton()
    {
        if (evolveButton != null)
        {
            evolveButton.gameObject.SetActive(false);
        }
        Debug.Log("Paddock deselected, evolve button hidden.");
    }

    public void OnFeedButtonClicked()
    {
        if (currentDinosaur != null)
        {
            if (currentDinosaur.parentPaddock == Paddock.SelectedPaddock)
            {
                currentDinosaur.FeedDinosaur();
                UpdateUI();
                TriggerPulsingBar();
            }
            else
            {
                Debug.LogWarning("The selected dinosaur’s paddock is not active!");
            }
        }
        else
        {
            Debug.LogWarning("No dinosaur is selected in the UI Manager.");
        }
    }

    public void UpdateUI()
    {
        if (currentDinosaur != null)
        {
            if (currentDinosaur.IsHatching())
            {
                if (feedButton != null)
                    feedButton.gameObject.SetActive(false);
                if (evolveButton != null)
                    evolveButton.gameObject.SetActive(false);
                Debug.Log("Hatching is in progress, hiding feed and evolve buttons.");
                return;
            }
        }

        if (currentDinosaur != null && feedFillImage != null)
        {
            float fill = (float)currentDinosaur.feedCount / (float)currentDinosaur.feedsPerLevel;
            feedFillImage.fillAmount = fill;
            Debug.Log("FeedFillImage updated: " + fill);
        }

        if (currentDinosaur != null)
        {
            if (currentDinosaur.levelManager.CurrentLevel >= 10)
            {
                if (feedButton != null)
                    feedButton.gameObject.SetActive(false);
                if (evolveButton != null)
                    evolveButton.gameObject.SetActive(true);
            }
            else
            {
                if (feedButton != null)
                    feedButton.gameObject.SetActive(true);
                if (evolveButton != null)
                    evolveButton.gameObject.SetActive(false);
            }
        }
    }
    private Coroutine pulsingBarCoroutine;

    private void TriggerPulsingBar()
    {
        int currentLevel = currentDinosaur.levelManager.CurrentLevel;
        int feedCost = currentDinosaur.GetFeedCostForLevel(currentLevel);

        if (currentDinosaur.dinosaurDiet == Diet.Herbivore)
        {
            if (!CurrencySystem.Instance.HasEnoughCurrency(CurrencyType.Crops, feedCost))
            {
                return;
            }
        }
        else if (currentDinosaur.dinosaurDiet == Diet.Carnivore)
        {
            if (!CurrencySystem.Instance.HasEnoughCurrency(CurrencyType.Meat, feedCost))
            {
                return;
            }
        }

        if (pulsingBar != null)
        {
            pulsingBar.fillAmount = (float)currentDinosaur.feedCount / (float)currentDinosaur.feedsPerLevel;

            if (pulsingBarCoroutine != null)
            {
                StopCoroutine(pulsingBarCoroutine);
            }

            pulsingBarCoroutine = StartCoroutine(DeactivatePulsingBar());
        }
    }

    private IEnumerator DeactivatePulsingBar()
    {
        if (pulsingBar != null)
        {
            float fadeDuration = 0.4f;
            float elapsedTime = 0f;
            Color initialColor = pulsingBar.color;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
                pulsingBar.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
                yield return null;
            }

            yield return new WaitForSeconds(0.7f);
            pulsingBar.fillAmount = 0f;
        }

        pulsingBarCoroutine = null;
    }
}
