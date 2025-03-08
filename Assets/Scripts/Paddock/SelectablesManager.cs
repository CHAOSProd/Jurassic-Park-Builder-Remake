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
        if (CurrentSelectable is VisitorCenter previousVisitorCenter)
        {
            previousVisitorCenter.ResearchButton.SetActive(false);
        }

        CurrentSelectable = selectable;
        InitializeUI();

        if (CurrentSelectable is VisitorCenter visitorCenter)
        {
            visitorCenter.ResearchButton.SetActive(true);
        }
    }

    public void UnselectAll()
    {
        if (CurrentSelectable == null)
        {
            InitializeUI();
            return;
        }

        if (CurrentSelectable is VisitorCenter visitorCenter)
        {
            visitorCenter.ResearchButton.SetActive(false);
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
            if (CurrentSelectable is Paddock paddock && paddock.is_hatching)
            {
                UIManager.Instance.ChangeTo("BuildingsSelectedUI");
            }
            else if (CurrentSelectable is Paddock)
            {
                UIManager.Instance.ChangeTo("PaddockUI");
            }
            else if (CurrentSelectable is Building)
            {
                UIManager.Instance.ChangeTo("BuildingUI");
            }
            else if (CurrentSelectable is TreeChopper tree)
            {
                if (tree.canSpeedUp)
                {
                    UIManager.Instance.ChangeTo("SelectedUI");
                }
                else
                {
                    UIManager.Instance.ChangeTo("ExpansionUI");
                }
            }
            else if (CurrentSelectable is DebrisObject debris)
            {
                if (debris.removing)
                {
                    UIManager.Instance.ChangeTo("SelectedUI");
                }
                else
                {
                    UIManager.Instance.ChangeTo("DebrisUI");
                }
            }
            else if (CurrentSelectable is VisitorCenter)
            {
                UIManager.Instance.ChangeTo("VisitorCenterUI");
            }
            else
            {
                UIManager.Instance.ChangeTo("DefaultUI");
            }
        }
        else
        {
            UIManager.Instance.ChangeTo("DefaultUI");
        }
    }
}
