using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EvolutionCompleteManager : Singleton<EvolutionCompleteManager>
{
    [SerializeField] private GameObject EvolutionCompletePanel;
    [SerializeField] private GameObject PanelOpeningSound;
    [SerializeField] private List<TextMeshProUGUI> evolutionTexts;
    public void OpenPanel()
    {
        PanelOpeningSound.GetComponent<AudioSource>().Play();
        EvolutionCompletePanel.SetActive(true);
        UpdateEvolutionText();
        UIManager.Instance.ChangeFixedTo("PanelUI");
        UIManager.Instance.DisableCurrent();
        UIManager.Instance.ChangeCameraPanningStatus(false);
    }

    public void ClosePanel()
    {
        EvolutionCompletePanel.SetActive(false);
        UIManager.Instance.ChangeFixedTo("DefaultUI");
        UIManager.Instance.EnableCurrent();
        UIManager.Instance.ChangeCameraPanningStatus(true);
    }
    private void UpdateEvolutionText()
    {
        foreach (var text in evolutionTexts)
        {
            text.gameObject.SetActive(false);
        }

        int indexToShow = GetTextIndexByEvolutionAndStage(EvolutionManager.lastEvolutionIndex, EvolutionManager.lastStageIndex);

        if (indexToShow >= 0 && indexToShow < evolutionTexts.Count)
        {
            evolutionTexts[indexToShow].gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"Text for evolution {EvolutionManager.lastEvolutionIndex} and stage {EvolutionManager.lastStageIndex} not found.");
        }
    }

    private int GetTextIndexByEvolutionAndStage(int evolutionIndex, int stageIndex)
    {
        return evolutionIndex * 3 + stageIndex;
    }
}

