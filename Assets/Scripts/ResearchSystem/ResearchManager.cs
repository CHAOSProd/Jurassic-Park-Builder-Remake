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
        
    [Header("Research Settings")]
    [SerializeField] private float successRate = 50f;
    [SerializeField] private int requiredAttempts = 3;
    [SerializeField] private int attemptCost = 100;

    private int currentAmberIndex = -1;
    private int currentAttempts = 0;

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
        if (AmberManager.Instance.HasUndecodedActivatedAmber())
        {
            AmberNotification.SetActive(true);
        }
    }
    public void DeactivateAmberNotification()
    {
        if (!AmberManager.Instance.HasUndecodedActivatedAmber())
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
    public void AttemptResearch()
    {
        EventManager.Instance.TriggerEvent(new CurrencyChangeGameEvent(-attemptCost, CurrencyType.Coins)); 
        if (Random.Range(0f, 100f) <= successRate)
        {
            currentAttempts++;
            SaveResearchProgress();
            Debug.Log("Research attempt succeeded");
        }
        else
        {
            Debug.Log("Research attempt failed");
        }
        Debug.Log($"Progress: {currentAttempts}/{requiredAttempts}");

        if (currentAttempts == requiredAttempts )
        {
            CompleteResearch();
            ClosePanel();
            SelectablesManager.Instance.UnselectAll();
        }
    }

    private void CompleteResearch()
    {
        int index = GetCurrentAmberIndex();
        DinoAmber.EnableDinoAndEnableOtherDecodeButtons(index);
        currentAttempts = 0;
        SaveResearchProgress();
    }
    private void SaveResearchProgress()
    {
        SaveManager.Instance.SaveData.ResearchData.CurrentAttempts = currentAttempts;
    }
    public void Load()
    {
        currentAttempts = SaveManager.Instance.SaveData.ResearchData.CurrentAttempts;
        Debug.Log($"Loaded research progress, saved attempts: {currentAttempts}");
    }
}
