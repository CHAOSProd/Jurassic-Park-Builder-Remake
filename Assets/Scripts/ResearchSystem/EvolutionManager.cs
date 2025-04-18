using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EvolutionManager : Singleton<EvolutionManager>
{
    [SerializeField] private GameObject StartingEvolutionPanel;
    public static int lastEvolutionIndex = -1;
    public static int lastStageIndex = -1;
    void Start()
    {
        DinoEvolution[] allEvolutions = FindObjectsOfType<DinoEvolution>(true);
        DinoEvolution targetEvolution = allEvolutions.FirstOrDefault(e => e.DinoEvolutionIndex == lastEvolutionIndex);

        if (targetEvolution != null)
        {
            if (targetEvolution.DinoToDisable != null)
                targetEvolution.DinoToDisable.SetActive(false);

            if (targetEvolution.evolutionIconToEnable != null)
                targetEvolution.evolutionIconToEnable.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"No DinoEvolution found with EvolutionIndex {lastEvolutionIndex}");
        }
    }

    public void OpenPanel()
    {
        DinosaurFeedingUIManager.Instance.DisableEvolutionButton();
        StartingEvolutionPanel.SetActive(true);
        UIManager.Instance.ChangeFixedTo("PanelUI");
        UIManager.Instance.DisableCurrent();
        UIManager.Instance.ChangeCameraPanningStatus(false);
    }

    public void ClosePanel()
    {
        StartingEvolutionPanel.SetActive(false);
        UIManager.Instance.ChangeFixedTo("DefaultUI");
        UIManager.Instance.EnableCurrent();
        UIManager.Instance.ChangeCameraPanningStatus(true);
        DinosaurFeedingUIManager.Instance.UpdateUI();
    }
}
