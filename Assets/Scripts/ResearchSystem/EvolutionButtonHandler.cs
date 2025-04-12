using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvolutionButtonHandler : MonoBehaviour
{
    private int dinoIndex;
    private int stageIndex;
    public Button evolutionButton;

    void Start()
    {
        if (evolutionButton != null)
        {
            evolutionButton.onClick.AddListener(OnEvolutionButtonClick);
        }
    }

    void OnEvolutionButtonClick()
    {
        if (DinoAmber.lastDecodedAmberIndex == -1)
        {
            if (EvolutionManager.lastEvolutionIndex == -1)
            {
                EvolutionManager.Instance.OpenPanel();
            }
            else
            {
                ResearchManager.Instance.SetEvolutionIndex(dinoIndex, stageIndex);
                DinosaurFeedingUIManager.Instance.DisableEvolutionButton();
                ResearchManager.Instance.OpenPanel();
            }
        }
    }
}
