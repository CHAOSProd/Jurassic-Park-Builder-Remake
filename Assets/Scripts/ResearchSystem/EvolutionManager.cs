using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolutionManager : Singleton<EvolutionManager>
{
    [SerializeField] private GameObject StartingEvolutionPanel;
    public static int lastEvolutionIndex = -1;

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
