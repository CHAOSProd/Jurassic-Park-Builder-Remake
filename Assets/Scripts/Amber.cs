using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmberManager : Singleton<AmberManager>
{
    [Header("UI")]
    [SerializeField] private GameObject AmberPanel;

    [Header("Sound")]
    [SerializeField] private GameObject PanelOpeningSound;


    public void OpenPanel()
    {
        PanelOpeningSound.GetComponent<AudioSource>().Play();

        AmberPanel.SetActive(true);
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
}
