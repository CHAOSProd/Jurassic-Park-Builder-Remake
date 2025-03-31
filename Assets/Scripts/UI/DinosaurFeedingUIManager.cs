using UnityEngine;
using UnityEngine.UI;

public class DinosaurFeedingUIManager : MonoBehaviour
{
    public static DinosaurFeedingUIManager Instance;

    [Header("UI Components")]
    public Image feedFillImage;    // Image with Image Type = Filled (Horizontal)
    public FeedButton feedButton;  // Your custom FeedButton script
    public Button evolveButton;    // Standard evolve button

    // The currently selected dinosaur’s feeding system.
    private DinosaurFeedingSystem currentDinosaur;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Attempt to assign the feed button from its singleton if not set.
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

        // If feedFillImage is not assigned via the Inspector, try to find it even if inactive.
        if (feedFillImage == null)
        {
            Debug.LogWarning("FeedFillImage is not assigned in the Inspector. Attempting to locate it...");
            Image[] allImages = Resources.FindObjectsOfTypeAll<Image>();
            foreach (var img in allImages)
            {
                if (img.gameObject.CompareTag("FeedBar"))
                {
                    feedFillImage = img;
                    Debug.Log("FeedFillImage found via Resources.");
                    break;
                }
            }
            if (feedFillImage == null)
            {
                Debug.LogError("FeedFillImage still not found!");
            }
        }
    }

    // Call this when a dinosaur’s paddock is selected.
    public void SetSelectedDinosaur(DinosaurFeedingSystem dinosaur)
    {
        currentDinosaur = dinosaur;
        Debug.Log("Selected dinosaur set in UI Manager.");
        UpdateUI();
    }

    // Called when the feed button is clicked.
    public void OnFeedButtonClicked()
    {
        if (currentDinosaur != null)
        {
            // Only feed if the dinosaur's paddock is currently selected.
            if (currentDinosaur.parentPaddock == Paddock.SelectedPaddock)
            {
                currentDinosaur.FeedDinosaur();
                UpdateUI();
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

    // Updates the UI based on the current dinosaur's feed progress.
    public void UpdateUI()
    {
        if (currentDinosaur != null && feedFillImage != null)
        {
            float fill = (float)currentDinosaur.feedCount / (float)currentDinosaur.feedsPerLevel;
            feedFillImage.fillAmount = fill;
            Debug.Log("FeedFillImage updated: " + fill);
        }

        // At or above level 10, disable feeding and enable evolution.
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
}
