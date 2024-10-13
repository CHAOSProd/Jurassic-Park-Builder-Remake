using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectablesManager : Singleton<SelectablesManager>
{
    public Selectable CurrentSelectable { get; private set; } = null;

    private void Start()
    {
        InitializeUI();
    }

    public void SetSelected(Selectable selectable)
    {
        CurrentSelectable = selectable;
        InitializeUI();
    }

    public void UnselectAll()
    {
        if (CurrentSelectable == null)
        {
            InitializeUI();
            return;
        }
            

        CurrentSelectable.Unselect();
        CurrentSelectable = null;
        InitializeUI();
    }
    public void InitializeUI()
    {
        if (GridBuildingSystem.Instance.TempPlaceableObject)
            return;

        if (CurrentSelectable != null)
        {
            if(CurrentSelectable is Paddock)
            {
                UIManager.Instance.ChangeTo("PaddockUI");
            }
            else if(CurrentSelectable is Building)
            {
                UIManager.Instance.ChangeTo("BuildingUI");
            }
            else if(CurrentSelectable is TreeChopper)
            {
                UIManager.Instance.ChangeTo("ExpansionUI");
            }
        }
        else
        {
            UIManager.Instance.ChangeTo("DefaultUI");
        }
    }
}