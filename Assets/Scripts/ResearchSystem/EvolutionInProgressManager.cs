using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EvolutionInProgressManager : Singleton<EvolutionInProgressManager>
{
    [SerializeField] private GameObject EvolutionInProgressPanel;
    [SerializeField] private TextMeshProUGUI Text;
    public void OpenPanel()
    {
        EvolutionInProgressPanel.SetActive(true);
        UIManager.Instance.ChangeFixedTo("PanelUI");
        UIManager.Instance.DisableCurrent();
        UIManager.Instance.ChangeCameraPanningStatus(false);
        PaddockInfo[] allPaddockInfos = FindObjectsOfType<PaddockInfo>(true);
        foreach (PaddockInfo paddockInfo in allPaddockInfos)
        {
            DinoEvolution dinoEvolution = paddockInfo.GetComponent<DinoEvolution>();
            if (dinoEvolution != null && dinoEvolution.DinoEvolutionIndex == EvolutionManager.lastEvolutionIndex)
            {
                Text.text = $"{paddockInfo._dinosaurName} is already undergoing Evolution Research. Complete its research to make room for the next species.";
                return;
            }
        }
    }

    public void ClosePanel()
    {
        EvolutionInProgressPanel.SetActive(false);
        UIManager.Instance.ChangeFixedTo("DefaultUI");
        UIManager.Instance.EnableCurrent();
        UIManager.Instance.ChangeCameraPanningStatus(true);
    }
}
