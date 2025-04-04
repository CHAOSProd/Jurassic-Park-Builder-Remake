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
    
    // Base feed cost (fallback if levelFeedCosts is not defined)
    public int baseFeedCost = 10;
    // Custom feed cost per level (index 0 = level 1, etc.)
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

        // Find the nearest parent with the Paddock script.
        parentPaddock = GetComponentInParent<Paddock>();
        if (parentPaddock == null)
        {
            Debug.LogWarning("DinosaurFeedingSystem could not find a parent Paddock.");
        }
        if (!IsHatching())
        {
            // set the model based on level.
            if (levelManager != null)
            {
                if (levelManager.CurrentLevel >= 5)
                {
                    if (babyModel != null && adultModel != null)
                    {
                        babyModel.SetActive(false);
                        adultModel.SetActive(true);
                        Debug.Log("Model set to Adult based on level " + levelManager.CurrentLevel);
                    }
                }
                else
                {
                    if (babyModel != null && adultModel != null)
                    {
                        babyModel.SetActive(true);
                        adultModel.SetActive(false);
                        Debug.Log("Model set to Baby based on level " + levelManager.CurrentLevel);
                    }
                }
            }
        }
    }

    private void Start()
    {
        // If the dinosaur is hatching, ensure no model is visible.
        if (IsHatching())
        {
            if (babyModel != null)
                babyModel.SetActive(false);
            if (adultModel != null)
                adultModel.SetActive(false);
            Debug.Log("Dinosaur is hatching; models disabled.");
        }
        else
        {
            // else set the model based on level.
            if (levelManager != null)
            {
                if (levelManager.CurrentLevel >= 5)
                {
                    if (babyModel != null && adultModel != null)
                    {
                        babyModel.SetActive(false);
                        adultModel.SetActive(true);
                        Debug.Log("Model set to Adult based on level " + levelManager.CurrentLevel);
                    }
                }
                else
                {
                    if (babyModel != null && adultModel != null)
                    {
                        babyModel.SetActive(true);
                        adultModel.SetActive(false);
                        Debug.Log("Model set to Baby based on level " + levelManager.CurrentLevel);
                    }
                }
            }  
        } 
    }

    /// <summary>
    /// Checks if the dinosaur is currently hatching.
    /// It does so by retrieving the PlaceableObject from the paddock's parent.
    /// </summary>
    public bool IsHatching()
    {
        PlaceableObject placeableObject = GetComponentInParent<PlaceableObject>();
        if (parentPaddock != null && parentPaddock.is_hatching || parentPaddock != null && parentPaddock.hatching_completed)
        {
            return true;
        }
        return false;
    }

    // Called when the UI manager instructs this dinosaur to feed.
    public void FeedDinosaur()
    {
        // Do not allow feeding if the dinosaur is hatching.
        if (IsHatching())
        {
            Debug.Log("Dinosaur is currently hatching; feeding is disabled.");
            return;
        }

        if (levelManager.CurrentLevel >= 10)
        {
            Debug.Log("Dinosaur is at level 10 and ready to evolve!");
            return;
        }

        int currentLevel = levelManager.CurrentLevel;
        int feedCost = GetFeedCostForLevel(currentLevel);

        // Deduct resources via your CurrencySystem.
        if (dinosaurDiet == Diet.Herbivore)
        {
            if (!CurrencySystem.Instance.HasEnoughCurrency(CurrencyType.Crops, feedCost))
            {
                CurrencySystem.Instance._notEnoughCropsPanel.ShowNotEnoughCoinsPanel(feedCost);
                Debug.Log("Not enough crops to feed dinosaur.");
                return;
            }
            CurrencyChangeGameEvent cropsDeduction = new CurrencyChangeGameEvent
            {
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
            CurrencyChangeGameEvent meatDeduction = new CurrencyChangeGameEvent
            {
                CurrencyType = CurrencyType.Meat,
                Amount = -feedCost
            };
            CurrencySystem.Instance.AddCurrency(meatDeduction);
            Debug.Log("Meat deducted: " + feedCost);
        }

        feedCount++;
        string parentName = parentPaddock.gameObject.name;
        Attributes.SetInt("FeedCount" + parentName, feedCount);
        Debug.Log("Dinosaur fed: " + feedCount + "/" + feedsPerLevel);

        if (feedCount >= feedsPerLevel)
        {
            LevelUp();
        }
    }

    public int GetFeedCostForLevel(int level)
    {
        if (levelFeedCosts != null && levelFeedCosts.Length >= level)
            return levelFeedCosts[level - 1];
        return baseFeedCost * level;
    }

    private void LevelUp()
    {
        levelManager.LevelUp();
        feedCount = 0;  // Reset feeding progress (the level is preserved)
        Debug.Log("Dinosaur leveled up! New level: " + levelManager.CurrentLevel);

        // Only update the model if the dinosaur is not hatching.
        if (!IsHatching())
        {
            if (levelManager.CurrentLevel >= 5)
            {
                if (babyModel != null && adultModel != null)
                {
                    babyModel.SetActive(false);
                    adultModel.SetActive(true);
                    Debug.Log("Dinosaur evolved from baby to adult!");
                }
            }
            else
            {
                if (babyModel != null && adultModel != null)
                {
                    babyModel.SetActive(true);
                    adultModel.SetActive(false);
                    Debug.Log("Dinosaur remains as baby.");
                }
            }
        }
    }
}
