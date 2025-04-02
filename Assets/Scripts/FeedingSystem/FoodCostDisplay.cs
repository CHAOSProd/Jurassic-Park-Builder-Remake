using UnityEngine;
using TMPro;

public class FoodCostDisplay : MonoBehaviour
{
    public TextMeshProUGUI foodCostText;

    private void Update()
    {
        if (Paddock.SelectedPaddock != null)
        {
            DinosaurFeedingSystem feedingSystem = Paddock.SelectedPaddock.GetComponentInChildren<DinosaurFeedingSystem>();

            if (feedingSystem != null)
            {
                int currentLevel = feedingSystem.levelManager.CurrentLevel;
                int feedCost = feedingSystem.GetFeedCostForLevel(currentLevel);
                foodCostText.text = feedCost.ToString();
            }
        }
    }
}