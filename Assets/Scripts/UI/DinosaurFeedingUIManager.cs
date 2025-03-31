using UnityEngine;
using UnityEngine.UI;

public class DinosaurFeedingUIManager : MonoBehaviour
{
    public static DinosaurFeedingUIManager Instance;

    [Header("UI Components")]
    public Image feedFillImage;    // Image with Image Type = Filled (Horizontal)
    public FeedButton feedButton;  // Your custom feed button script
    public Button evolveButton;    // Evolve button (standard Button)

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
            // Subscribe to your custom button’s event.
            feedButton.OnClickEvent.AddListener(OnFeedButtonClicked);
        }
        else
        {
            Debug.LogWarning("FeedButton reference missing in DinosaurFeedingUIManager.");
        }
    }

    // Called when a dinosaur’s paddock is selected.
    // (Ensure your selection logic calls this with the dinosaur’s DinosaurFeedingSystem component.)
    public void SetSelectedDinosaur(DinosaurFeedingSystem dinosaur)
    {
        currentDinosaur = dinosaur;
        UpdateUI();
    }

    // Called when the feed button is clicked.
    public void OnFeedButtonClicked()
    {
        if (currentDinosaur != null)
        {
            // Only feed if the dinosaur’s paddock is currently selected.
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
            Debug.LogWarning("No dinosaur is selected.");
        }
    }

    // Update the UI elements based on the current dinosaur’s feed progress and level.
    public void UpdateUI()
    {
        if (currentDinosaur != null && feedFillImage != null)
        {
            float fill = (float)currentDinosaur.feedCount / (float)currentDinosaur.feedsPerLevel;
            feedFillImage.fillAmount = fill;
        }

        // At or above level 10, disable feeding and enable the evolve button.
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
