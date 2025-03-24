using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchManager : Singleton<ResearchManager>
{
    [Header("UI")]
    [SerializeField] private GameObject ResearchPanel;
    [SerializeField] private GameObject NoAmberPanel;
    [SerializeField] private GameObject AmberNotification;

    [Header("Sound")]
    [SerializeField] private GameObject PanelOpeningSound;

    private int currentAmberIndex = -1;

    public void SetAmberIndex(int index)
    {
        currentAmberIndex = index;
    }
    public int GetCurrentAmberIndex()
    {
        return currentAmberIndex;
    }

    public void OpenPanel()
    {
        if (PanelOpeningSound != null)
        {
            PanelOpeningSound.GetComponent<AudioSource>().Play();
        }

        Debug.Log($"Currently researching dino amber index: {currentAmberIndex}");
        
        ResearchPanel.SetActive(true);
        UIManager.Instance.ChangeFixedTo("PanelUI");
        UIManager.Instance.DisableCurrent();
        UIManager.Instance.ChangeCameraPanningStatus(false);
    }
    public void OpenNoAmberPanel()
    {
        if (PanelOpeningSound != null)
        {
            PanelOpeningSound.GetComponent<AudioSource>().Play();
        }

        Debug.Log($"No amber to research");
        
        NoAmberPanel.SetActive(true);
        UIManager.Instance.ChangeFixedTo("PanelUI");
        UIManager.Instance.DisableCurrent();
        UIManager.Instance.ChangeCameraPanningStatus(false);
    }

    public void ActivateAmberNotification()
    {
        bool allAmbersDecoded = AmberManager.Instance.GetAmberList().TrueForAll(a => a.IsDecoded);
        if (AmberManager.Instance.HasAnyAmberActivated() && !allAmbersDecoded)
        {
            AmberNotification.SetActive(true);
        }
    }
    public void DeactivateAmberNotification()
    {
        bool allAmbersDecoded = AmberManager.Instance.GetAmberList().TrueForAll(a => a.IsDecoded);
        if (AmberManager.Instance.HasAnyAmberActivated() && allAmbersDecoded)
        {
            AmberNotification.SetActive(false);
        }
    }


    public void ClosePanel()
    {
        ResearchPanel.SetActive(false);
        UIManager.Instance.ChangeFixedTo("DefaultUI");
        UIManager.Instance.EnableCurrent();
        UIManager.Instance.ChangeCameraPanningStatus(true);
    }
}
