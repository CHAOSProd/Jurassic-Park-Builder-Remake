using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmberManager : Singleton<AmberManager>
{
    [Header("UI")]
    [SerializeField] private GameObject AmberPanel;
    [SerializeField] private GameObject objectToDisable1;
    [SerializeField] private GameObject objectToDisable2;
    [SerializeField] private GameObject objectToEnable;
    [SerializeField] public TMPro.TextMeshProUGUI mainText;

    [Header("Sound")]
    [SerializeField] private GameObject PanelOpeningSound;
    private List<AmberData> _amberList = new List<AmberData>();
    private HashSet<int> _activatedAmberIndices = new HashSet<int>();

    public void OpenPanel()
    {
        PanelOpeningSound.GetComponent<AudioSource>().Play();
        AmberPanel.SetActive(true);
        if (DinoAmber.lastDecodedAmberIndex != -1 || EvolutionManager.lastEvolutionIndex != -1)
        {
            if (objectToDisable1 != null)
            {
                objectToDisable1.SetActive(false);
            }

            if (objectToDisable2 != null)
            {
                objectToDisable2.SetActive(false);
            }

            if (objectToEnable != null)
            {
                objectToEnable.SetActive(true);
            }

            if (mainText != null)
            {
                mainText.text = "You just found an Amber Piece!\nYou still have a research in progress. Complete it before decoding the new piece of amber!";
            }
        }
        else
        {
            if (objectToDisable1 != null)
            {
                objectToDisable1.SetActive(true);
            }

            if (objectToDisable2 != null)
            {
                objectToDisable2.SetActive(true);
            }

            if (objectToEnable != null)
            {
                objectToEnable.SetActive(false);
            }

            if (mainText != null)
            {
                mainText.text = "You just found an Amber Piece!\nStart decoding it to unlock a new Dinosaur species!";
            }
        }
        AmberPanel.GetComponent<Animator>().Play("openAnimation");
        UIManager.Instance.ChangeFixedTo("PanelUI");
        UIManager.Instance.DisableCurrent();
        UIManager.Instance.ChangeCameraPanningStatus(false);
    }

    public void ClosePanel()
    {
        AmberPanel.SetActive(false);
        UIManager.Instance.ChangeFixedTo("DefaultUI");
        UIManager.Instance.EnableCurrent();
        UIManager.Instance.ChangeCameraPanningStatus(true);
    }
    
    public void AddAmber(int amberIndex)
    {
        int index = _amberList.Count;
        AmberData newAmber = new AmberData(index);
        _amberList.Add(newAmber);
        SaveAmberData();
        Debug.Log($"New amber put on index: {amberIndex}");
    }
    
    public void ActivateAmber(int amberIndex)
    {
        AmberData amber = _amberList.Find(a => a.Index == amberIndex);
        if (amber != null && !amber.IsActivated)
        {
            amber.Activate();
            SaveAmberData();
        }
    }
    
    public void SaveAmberData()
    {
        SaveManager.Instance.SaveData.AmberData = _amberList;
    }
    
    public void Load()
    {
        if (SaveManager.Instance.SaveData.AmberData != null)
        {
            _amberList = SaveManager.Instance.SaveData.AmberData;
        }
        else
        {
            _amberList = new List<AmberData>();
        }
        ResearchManager.Instance.ActivateAmberNotification();
        ResearchManager.Instance.DeactivateAmberNotification();
        CheckAndEnableDinoAmbers();
    }
    
    public void CheckAndEnableDinoAmbers()
    {
        DinoAmber[] allDinoAmbers = FindObjectsOfType<DinoAmber>(true);
        foreach (var dinoAmber in allDinoAmbers)
        {
            if (dinoAmber.ShouldActivate())
            {
                dinoAmber.ActivateAmber();
            }
        }
    }
    
    public List<AmberData> GetAmberList()
    {
        return _amberList;
    }
    
    public int GetAmberIndex(int index)
    {
        return index >= 0 && index < _amberList.Count ? _amberList[index].Index : -1;
    }
    
    public bool IsAmberActivated(int index)
    {
        AmberData amber = _amberList.Find(a => a.Index == index);
        return amber != null && amber.IsActivated;
    }
    
    public bool HasAnyAmberActivated()
    {
        return _amberList.Exists(a => a.IsActivated);
    }
    public bool HasUndecodedActivatedAmber()
    {
        return _amberList.Exists(a => a.IsActivated && !a.IsDecoded);
    }

    public int GetLastCollectedAmberIndex()
    {
        int maxIndex = -1;

        foreach (AmberData amber in _amberList)
        {
            if (amber.IsActivated && amber.Index > maxIndex)
            {
                maxIndex = amber.Index;
            }
        }

        return maxIndex;
    }
}