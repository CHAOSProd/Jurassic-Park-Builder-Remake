using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
public class ResearchManager : Singleton<ResearchManager>
{
    [Header("UI")]
    [SerializeField] private GameObject ResearchPanel;
    [SerializeField] private GameObject NoAmberPanel;
    [SerializeField] private GameObject AmberNotification;
    [SerializeField] private TMP_Text attemptCostText;

    [Header("Sound")]
    [SerializeField] private GameObject PanelOpeningSound;
        
    [Header("Research Settings")]
    private float successRate;
    private int requiredAttempts;
    private int attemptCost;

    private int currentAmberIndex = -1;
    private int currentAttempts = 0;
    bool[] researchSegments = new bool[3];
    public void UpdateAttemptCostText()
    {
        if (attemptCostText != null)
        {
            attemptCostText.text = attemptCost.ToString();
        }
        else
        {
            Debug.LogWarning("Attempt Cost Text is not assigned in the ResearchManager.");
        }
    }

    public void SetAmberIndex(int index)
    {
        currentAmberIndex = index;
        DinoAmber selectedAmber = FindObjectsOfType<DinoAmber>(true)
        .FirstOrDefault(a => a.DinoAmberIndex == index);
        if (selectedAmber != null)
        {
            successRate = selectedAmber.GetSuccessRate();
            requiredAttempts = selectedAmber.GetRequiredAttempts();
            attemptCost = selectedAmber.GetAttemptCost();
            Debug.Log($"ResearchManager - Stats taken from DinoAmber {index}:");
            Debug.Log($"Success Rate: {successRate}%");
            Debug.Log($"Required Attempts: {requiredAttempts}");
            Debug.Log($"Attempt Cost: {attemptCost} coins");
            UpdateAttemptCostText();
        }
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
        
        float roll = Random.Range(0f, 100f);
        
        if (roll <= successRate)
        {
            researchSegments[0] = true;
            researchSegments[1] = true;
            researchSegments[2] = true;
        }
        else
        {
            int failureType = Random.Range(0, 3);
            switch (failureType)
            {
                case 0:
                    Debug.Log("2V, 1X");
                    researchSegments[0] = true;
                    researchSegments[1] = true;
                    researchSegments[2] = false;
                    break;
                case 1:
                    Debug.Log("1V, 2X");
                    researchSegments[0] = true;
                    researchSegments[1] = false;
                    researchSegments[2] = false;
                    break;
                case 2:
                    Debug.Log("3X");
                    researchSegments[0] = false;
                    researchSegments[1] = false;
                    researchSegments[2] = false;
                    break;
            }
        }

        if ((researchSegments[0] && researchSegments[1] && researchSegments[2]))
        {
            Debug.Log("3V, Research attempt succeeded");
            currentAttempts++;
            SaveResearchProgress();
        }

        Debug.Log($"Progress: {currentAttempts}/{requiredAttempts}");

        if (currentAttempts == requiredAttempts)
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
        Debug.Log($"Reserch completed, dinosaur {index} unlocked and research attempts resetted to: {currentAttempts}");
        SaveResearchProgress();
    }
    public void SaveResearchProgress()
    {
        SaveManager.Instance.SaveData.ResearchData.CurrentAttempts = currentAttempts;
        SaveManager.Instance.SaveData.ResearchData.LastDecodedAmberIndex = DinoAmber.lastDecodedAmberIndex;
        SaveManager.Instance.SaveData.ResearchData.TutorialDebrisSpawned = TutorialDebrisSpawner.tutorialDebrisSpawned;
    }
    public void Load()
    {
        currentAttempts = SaveManager.Instance.SaveData.ResearchData.CurrentAttempts;
        DinoAmber.lastDecodedAmberIndex = SaveManager.Instance.SaveData.ResearchData.LastDecodedAmberIndex;
        TutorialDebrisSpawner.tutorialDebrisSpawned = SaveManager.Instance.SaveData.ResearchData.TutorialDebrisSpawned;
        Debug.Log($"Loaded research progress, saved attempts: {currentAttempts}");
        Debug.Log($"Loaded dino decoding index: {DinoAmber.lastDecodedAmberIndex}, (if -1 means theres no dino being decoded)");
        if (TutorialDebrisSpawner.tutorialDebrisSpawned)
        {
            Debug.Log("Tutorial debris already spawned.");
        }
        else
        {
            Debug.Log("Spawning tutorial debris.");
        }
    }
}
