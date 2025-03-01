using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmberManager : Singleton<AmberManager>
{
    [Header("UI")]
    [SerializeField] private GameObject AmberPanel;
    [SerializeField] private GameObject AmberDisplay; // Object to show amber details

    [Header("Sound")]
    [SerializeField] private GameObject PanelOpeningSound;

    private void ShowAmberInfo()
    {
        AmberDisplay.SetActive(true);
    }

    public void OpenPanel()
    {
        PanelOpeningSound.GetComponent<AudioSource>().Play();

        AmberPanel.SetActive(true);
        AmberPanel.GetComponent<Animator>().Play("openAnimation");

        ShowAmberInfo();

        UIManager.Instance.ChangeFixedTo("PanelUI");
        UIManager.Instance.DisableCurrent();
        UIManager.Instance.ChangeCameraPanningStatus(false);
    }

    public void ClosePanel()
    {
        AmberPanel.SetActive(false);

        UIManager.Instance.ChangeFixedTo("DefaultUI");
        UIManager.Instance.EnableCurrent();

        AmberDisplay.SetActive(false);
        UIManager.Instance.ChangeCameraPanningStatus(true);
    }
}
