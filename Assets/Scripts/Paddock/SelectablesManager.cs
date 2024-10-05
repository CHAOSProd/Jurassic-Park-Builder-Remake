using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectablesManager : Singleton<SelectablesManager>
{
    [SerializeField] private GameObject[] _paddockSelectedUI;
    [SerializeField] private GameObject[] _buildingSelectedUI;
    [SerializeField] private GameObject[] _expansionSelectedUI;
    [SerializeField] private GameObject[] _nothingIsSelected;

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
        if (CurrentSelectable == null) return;

        CurrentSelectable.Unselect();
        CurrentSelectable = null;
        InitializeUI();
    }
    private void SetUIActive(GameObject[] objects, bool active)
    {
        foreach(GameObject obj in objects)
        {
            obj.SetActive(active);
        }
    }
    public void InitializeUI()
    {
        if (GridBuildingSystem.Instance.TempPlaceableObject)
            return;

        if (CurrentSelectable != null)
        {
            switch (CurrentSelectable)
            {
                case Paddock:
                    SetUIActive(_buildingSelectedUI, false);
                    SetUIActive(_paddockSelectedUI, true);
                    SetUIActive(_expansionSelectedUI, false);
                    SetUIActive(_nothingIsSelected, false);
                    break;
                case Building:
                    SetUIActive(_buildingSelectedUI, true);
                    SetUIActive(_paddockSelectedUI, false);
                    SetUIActive(_expansionSelectedUI, false);
                    SetUIActive(_nothingIsSelected, false);
                    break;
                case TreeChopper:
                    SetUIActive(_buildingSelectedUI, false);
                    SetUIActive(_paddockSelectedUI, false);
                    SetUIActive(_expansionSelectedUI, true);
                    SetUIActive(_nothingIsSelected, false);
                    break;
            }
        }
        else
        {
            SetUIActive(_buildingSelectedUI, false);
            SetUIActive(_paddockSelectedUI, false);
            SetUIActive(_expansionSelectedUI, false);
            SetUIActive(_nothingIsSelected, true);
        }
    }
}

public enum SelectableType
{
    Paddock,
    Building
}