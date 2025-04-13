using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class NewSpeciesManager : Singleton<NewSpeciesManager>
{
    [Header("UI")]
    [SerializeField] private GameObject NewSpeciesPanel;
    [SerializeField] private GameObject Dinos;

    private HatchingData data;
    private Transform child;

    private void GetDinoInfo()
    {
        for (int i = 0; i < Dinos.transform.childCount; i++)
        {
            child = Dinos.transform.GetChild(i);

            if (child.name.Equals(data.DinoName, System.StringComparison.OrdinalIgnoreCase))
            { 
                child.gameObject.SetActive(true);
                break; 
            }
        }
    }

    public void OpenPanel(HatchingData hatchingData)
    {
        data = hatchingData;

        NewSpeciesPanel.SetActive(true);
        NewSpeciesPanel.GetComponent<Animator>().Play("openAnimation");

        GetDinoInfo();

        UIManager.Instance.ChangeFixedTo("PanelUI");
        UIManager.Instance.DisableCurrent();
        UIManager.Instance.ChangeCameraPanningStatus(false);
    }

    public void ClosePanel()
    {
        NewSpeciesPanel.SetActive(false);

        UIManager.Instance.ChangeFixedTo("DefaultUI");
        UIManager.Instance.EnableCurrent();

        child.gameObject.SetActive(false);
        UIManager.Instance.ChangeCameraPanningStatus(true);
    }
}
