using UnityEngine;

public enum Diet
{
    Herbivore,
    Carnivore
}

public class DinosaurFeedingSystem : MonoBehaviour
{
    [Header("Dinosaur Settings")]
    public Diet dinosaurDiet;
    [HideInInspector]
    public int feedCount = 0;
    public int feedsPerLevel = 5;
    
    // Base feed cost (fallback if levelFeedCosts is not set up)
    public int baseFeedCost = 10;
    // Custom feed cost per level (index 0 for level 1, etc.)
    public int[] levelFeedCosts;

    [Header("Model References")]
    public GameObject babyModel;    // Baby dinosaur model
    public GameObject adultModel;   // Adult dinosaur model

    [Header("Level Manager")]
    public DinosaurLevelManager levelManager;  // This dinosaur’s level manager

    // Reference to the paddock that contains this dinosaur.
    [HideInInspector]
    public Paddock parentPaddock;

    private void Awake()
    {
        if (levelManager == null)
            levelManager = GetComponent<DinosaurLevelManager>();

        parentPaddock = GetComponentInParent<Paddock>();
    }

    // Called when the UI manager instructs this dinosaur to feed.
    public void FeedDinosaur()
    {
        if (levelManager.CurrentLevel >= 10)
        {
            Debug.Log("Dinosaur is at level 10 and ready to evolve!");
            return;
        }

        int currentLevel = levelManager.CurrentLevel;
        int feedCost = GetFeedCostForLevel(currentLevel);

        // Deduct resources via the CurrencySystem.
        if (dinosaurDiet == Diet.Herbivore)
        {
            if (!CurrencySystem.Instance.HasEnoughCurrency(CurrencyType.Crops, feedCost))
            {
                CurrencySystem.Instance._notEnoughCropsPanel.ShowNotEnoughCoinsPanel(feedCost);
                Debug.Log("Not enough crops to feed dinosaur.");
                return;
            }
            CurrencyChangeGameEvent cropsDeduction = new CurrencyChangeGameEvent {
                CurrencyType = CurrencyType.Crops,
                Amount = -feedCost
            };
            CurrencySystem.Instance.AddCurrency(cropsDeduction);
            Debug.Log("Crops deducted: " + feedCost);
        }
        else if (dinosaurDiet == Diet.Carnivore)
        {
            if (!CurrencySystem.Instance.HasEnoughCurrency(CurrencyType.Meat, feedCost))
            {
                CurrencySystem.Instance._notEnoughMeatPanel.ShowNotEnoughCoinsPanel(feedCost);
                Debug.Log("Not enough meat to feed dinosaur.");
                return;
            }
            CurrencyChangeGameEvent meatDeduction = new CurrencyChangeGameEvent {
                CurrencyType = CurrencyType.Meat,
                Amount = -feedCost
            };
            CurrencySystem.Instance.AddCurrency(meatDeduction);
            Debug.Log("Meat deducted: " + feedCost);
        }

        feedCount++;
        Debug.Log("Dinosaur fed: " + feedCount + "/" + feedsPerLevel);

        if (feedCount >= feedsPerLevel)
        {
            LevelUp();
        }
    }

    private int GetFeedCostForLevel(int level)
    {
        if (levelFeedCosts != null && levelFeedCosts.Length >= level)
            return levelFeedCosts[level - 1];
        return baseFeedCost * level;
    }

    private void LevelUp()
    {
        levelManager.LevelUp();
        feedCount = 0;  // Reset feeding progress (the level itself is maintained)
        Debug.Log("Dinosaur leveled up! New level: " + levelManager.CurrentLevel);

        // When reaching level 5, swap the baby model for the adult model.
        if (levelManager.CurrentLevel == 5)
        {
            if (babyModel != null && adultModel != null)
            {
                babyModel.SetActive(false);
                adultModel.SetActive(true);
                Debug.Log("Dinosaur evolved from baby to adult!");
            }
        }
    }
}
