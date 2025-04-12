using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class EvolutionConfermationButtonHandler : MonoBehaviour
{
    [SerializeField] private int dinoIndex;
    [SerializeField] private int stageIndex;
    public Button evolutionConfermationButton;

    void Start()
    {
        if (evolutionConfermationButton != null)
        {
            evolutionConfermationButton.onClick.AddListener(OnEvolutionButtonClick);
        }
    }

    void OnEvolutionButtonClick()
    {
        ResearchManager.Instance.SetEvolutionIndex(dinoIndex, stageIndex);
        DinoEvolution dinoEvolution = FindObjectsOfType<DinoEvolution>(true)
            .FirstOrDefault(e => e.DinoEvolutionIndex == dinoIndex);
        if (dinoEvolution != null)
        {
            if (dinoEvolution.DinoToDisable != null)
            {
                dinoEvolution.DinoToDisable.SetActive(false);
            }

            if (dinoEvolution.evolutionIconToEnable != null)
            {
                dinoEvolution.evolutionIconToEnable.SetActive(true);
            }
        }
        EvolutionManager.Instance.ClosePanel();
        ResearchManager.Instance.OpenPanel();
        DinosaurFeedingUIManager.Instance.DisableEvolutionButton();
    }
}
