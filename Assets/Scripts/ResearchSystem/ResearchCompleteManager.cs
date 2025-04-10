using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResearchCompleteManager : Singleton<ResearchCompleteManager>
{
    [SerializeField] private GameObject ResearchCompletePanel;
    [SerializeField] private GameObject PanelOpeningSound;
    [SerializeField] private TextMeshProUGUI speciesNameText;
    public void OpenPanel()
    {
        PanelOpeningSound.GetComponent<AudioSource>().Play();
        ResearchCompletePanel.SetActive(true);
        UIManager.Instance.ChangeFixedTo("PanelUI");
        UIManager.Instance.DisableCurrent();
        UIManager.Instance.ChangeCameraPanningStatus(false);
    }

    public void ClosePanel()
    {
        ResearchCompletePanel.SetActive(false);
        UIManager.Instance.ChangeFixedTo("DefaultUI");
        UIManager.Instance.EnableCurrent();
        UIManager.Instance.ChangeCameraPanningStatus(true);
    }
    public void SetSpeciesName(string name)
    {
        if (speciesNameText != null)
            speciesNameText.text = name;
    }
}
