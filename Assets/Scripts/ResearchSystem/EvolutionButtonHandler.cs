using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvolutionButtonHandler : MonoBehaviour
{
    private int stageIndex;
    public Button evolutionButton;
    Paddock selectedPaddock = Paddock.SelectedPaddock;

    void Start()
    {
        if (evolutionButton != null)
        {
            evolutionButton.onClick.AddListener(OnEvolutionButtonClick);
        }
    }

    void OnEvolutionButtonClick()
    {
        Paddock selectedPaddock = Paddock.SelectedPaddock;
        DinoEvolution dinoEvolution = selectedPaddock.GetComponentInChildren<DinoEvolution>(true);
        DinosaurLevelManager levelManager = selectedPaddock.GetComponentInChildren<DinosaurLevelManager>(true);
        if (levelManager != null)
        {
            int currentLevel = levelManager.CurrentLevel;

            int calculatedStage = (currentLevel / 10) - 1;
            calculatedStage = Mathf.Clamp(calculatedStage, 0, 2);

            dinoEvolution.DinoStageIndex = calculatedStage;
            Debug.Log($"DinoStageIndex set at {calculatedStage} based on level {currentLevel}");
        }
        int evolutionIndex = dinoEvolution.DinoEvolutionIndex;
        int stageIndex = dinoEvolution.DinoStageIndex;
        if (DinoAmber.lastDecodedAmberIndex == -1)
        {
            if (EvolutionManager.lastEvolutionIndex == -1)
            {
                EvolutionManager.Instance.OpenPanel();
            }
            else if (EvolutionManager.lastEvolutionIndex != -1 && EvolutionManager.lastEvolutionIndex == evolutionIndex)
            {
                ResearchManager.Instance.SetEvolutionIndex(evolutionIndex, stageIndex);
                DinosaurFeedingUIManager.Instance.DisableEvolutionButton();
                ResearchManager.Instance.OpenPanel();
            }
        }
    }
}
