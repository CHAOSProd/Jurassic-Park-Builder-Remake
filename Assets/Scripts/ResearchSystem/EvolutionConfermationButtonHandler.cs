using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class EvolutionConfermationButtonHandler : MonoBehaviour
{
    [SerializeField] private int stageIndex;
    public Button evolutionConfermationButton;
    Paddock selectedPaddock = Paddock.SelectedPaddock;

    void Start()
    {
        if (evolutionConfermationButton != null)
        {
            evolutionConfermationButton.onClick.AddListener(OnEvolutionButtonClick);
        }
    }

    void OnEvolutionButtonClick()
    {
        Paddock selectedPaddock = Paddock.SelectedPaddock;
        DinoEvolution dinoEvolution = selectedPaddock.GetComponentInChildren<DinoEvolution>(true);

        if (dinoEvolution == null)
        {
            Debug.LogWarning("DinoEvolution not found on the selected paddock");
            return;
        }

        selectedPaddock.HandleEvolutionStart();
        int evolutionIndex = dinoEvolution.DinoEvolutionIndex;
        int stageIndex = dinoEvolution.DinoStageIndex;
        ResearchManager.Instance.SetEvolutionIndex(evolutionIndex, stageIndex);

        if (dinoEvolution.DinoToDisable != null)
        {
            dinoEvolution.DinoToDisable.SetActive(false);
        }

        if (dinoEvolution.evolutionIconToEnable != null)
        {
            dinoEvolution.evolutionIconToEnable.SetActive(true);
        }

        EvolutionManager.Instance.ClosePanel();
        ResearchManager.Instance.OpenPanel();
        DinosaurFeedingUIManager.Instance.DisableEvolutionButton();
    }
}
