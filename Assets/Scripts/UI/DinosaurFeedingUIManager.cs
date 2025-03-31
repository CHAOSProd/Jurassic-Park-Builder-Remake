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

        // If feedButton is not assigned, use its singleton.
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

        // Try to assign the feedFillImage via Inspector or find it if necessary.
        if (feedFillImage == null)
        {
            Debug.LogWarning("FeedFillImage not assigned in Inspector. Attempting to locate it...");
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
                // Access the PlaceableObject script from the main GameObject
                PlaceableObject po = currentDinosaur.parentPaddock.GetComponentInParent<PlaceableObject>();
                if (po != null)
                {
                    Debug.Log("PlaceableObject found on parent GameObject.");
                    // Check if the hatching GameObject is active
                    if (po.Hatching != null && po.Hatching.activeSelf)
                    {
                        Debug.LogWarning("Cannot feed: Dinosaur is still hatching.");
                        return;
                    }
                }
                else
                {
                    Debug.LogError("PlaceableObject not found on parent GameObject.");
                }

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

    // Updates the UI based on the current dinosaur's feed progress and state.
    public void UpdateUI()
    {
        if (currentDinosaur != null)
        {
            // Access the PlaceableObject script from the main GameObject
            PlaceableObject po = currentDinosaur.parentPaddock.GetComponentInParent<PlaceableObject>();
            if (po != null)
            {
                Debug.Log("PlaceableObject found on parent GameObject.");
                // Check if the hatching GameObject is active
                if (po.Hatching != null && po.Hatching.activeSelf)
                {
                    if (feedButton != null)
                        feedButton.gameObject.SetActive(false);
                    if (evolveButton != null)
                        evolveButton.gameObject.SetActive(false);
                    Debug.Log("Hatching is in progress, hiding feed and evolve buttons.");
                    return;
                }
            }
            else
            {
                Debug.LogError("PlaceableObject not found on parent GameObject.");
            }
        }

        // Update the feed progress fill amount.
        if (currentDinosaur != null && feedFillImage != null)
        {
            float fill = (float)currentDinosaur.feedCount / (float)currentDinosaur.feedsPerLevel;
            feedFillImage.fillAmount = fill;
            Debug.Log("FeedFillImage updated: " + fill);
        }

        // Enable/disable buttons based on the dinosaur's level.
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
