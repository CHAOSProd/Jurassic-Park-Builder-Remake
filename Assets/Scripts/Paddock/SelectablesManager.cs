using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectablesManager : Singleton<SelectablesManager>
{
    [SerializeField] private GameObject[] _paddockSelectedUI;
    [SerializeField] private GameObject[] _buildingSelectedUI;
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

    public void InitializeUI()
    {
        if (GridBuildingSystem.Instance.TempPlaceableObject)
            return;

        if (CurrentSelectable != null)
        {
            if (CurrentSelectable.GetComponent<Paddock>())
            {
                for (int i = 0; i < _buildingSelectedUI.Length; i++)
                {
                    _buildingSelectedUI[i].SetActive(false);
                }

                for (int i = 0; i < _nothingIsSelected.Length; i++)
                {
                    _nothingIsSelected[i].SetActive(false);
                }

                for (int i = 0; i < _paddockSelectedUI.Length; i++)
                {
                    _paddockSelectedUI[i].SetActive(true);
                }
            }
            else if (CurrentSelectable.GetComponent<Building>())
            {
                for (int i = 0; i < _paddockSelectedUI.Length; i++)
                {
                    _paddockSelectedUI[i].SetActive(false);
                }

                for (int i = 0; i < _nothingIsSelected.Length; i++)
                {
                    _nothingIsSelected[i].SetActive(false);
                }

                for (int i = 0; i < _buildingSelectedUI.Length; i++)
                {
                    _buildingSelectedUI[i].SetActive(true);
                }
            }
        }
        else
        {
            for (int i = 0; i < _paddockSelectedUI.Length; i++)
            {
                _paddockSelectedUI[i].SetActive(false);
            }

            for (int i = 0; i < _buildingSelectedUI.Length; i++)
            {
                _buildingSelectedUI[i].SetActive(false);
            }

            for (int i = 0; i < _nothingIsSelected.Length; i++)
            {
                _nothingIsSelected[i].SetActive(true);
            }
        }
    }
}

public enum SelectableType
{
    Paddock,
    Building
}