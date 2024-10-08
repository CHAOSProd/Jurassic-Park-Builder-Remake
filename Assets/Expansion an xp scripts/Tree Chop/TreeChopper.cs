using UnityEngine;

// Handles tree chopping mechanics and visual changes
public class TreeChopper : Selectable
{
    public BoundsInt Area;

    [Header("Display Objects")]
    [SerializeField] private GameObject _trees;
    [SerializeField] private GameObject _debris;
    [SerializeField] private GameObject _selected;

    [Header("XP Objects")]
    [SerializeField] private GameObject _xpNotification;
    [SerializeField] private GameObject _tapVFX;
    [SerializeField] private GameObject _xpCounter;
    [SerializeField] private MoneyCountDisplayer _xpCountDisplayer;

    [Header("Selection")]
    [SerializeField] private bool _selectableFromBeginning;

    public bool AllowSelection { get; private set; }
    public (int x, int y) MappedPosition { get; private set; }

    private bool hasTreeDebris = false;
    private bool chopped = false;

    private TreeData _treeData;

    private void Awake()
    {
        AllowSelection = _selectableFromBeginning;
    }
    private void Start() 
    {
        _trees.SetActive(true);
        _debris.SetActive(false);
    }

    private void OnMouseDown() 
    {
        if (chopped || PointerOverUIChecker.Instance.IsPointerOverUIObject() || GridBuildingSystem.Instance.TempPlaceableObject) 
            return;

        if (hasTreeDebris) 
        {
            CollectDebris();
            return;
        }

        if(AllowSelection && SelectablesManager.Instance.CurrentSelectable != this)
        {
            TreeChopManager.Instance.SetExpansionCostText();
            Select();
        }
    }

    public void PerformChopAction() 
    {
        AllowSelection = false;

        _trees.SetActive(false);
        _debris.SetActive(true);

        _xpNotification.SetActive(true);
        hasTreeDebris = true;
    }
    private void CollectDebris() 
    {
        hasTreeDebris = false;
        _xpNotification.SetActive(false);
        _tapVFX.SetActive(true);
        _xpCounter.SetActive(true);
        _xpCountDisplayer.DisplayCount(TreeChopManager.Instance.CurrentXP);
        EventManager.Instance.TriggerEvent(new XPAddedGameEvent(TreeChopManager.Instance.CurrentXP));
        TreeChopManager.Instance.UpdateXP();
        _debris.SetActive(false);

        _treeData.Chopped = true;
        TreeChopManager.Instance.UnlockAdjacentTrees(this);

        BoundsInt tempArea = Area;
        Vector3Int positionInt = GridBuildingSystem.Instance.GridLayout.LocalToCell(transform.position);
        tempArea.position = new Vector3Int(tempArea.position.x + positionInt.x, tempArea.position.y + positionInt.y, 0);
        GridBuildingSystem.Instance.SetAreaWhite(tempArea, GridBuildingSystem.Instance.MainTilemap);

        chopped = true;
        Destroy(gameObject, .5f);
    }

    public override void Unselect()
    {
        _selected.SetActive(false);
        base.Unselect();
    }
    public override void Select()
    {
        _selected.SetActive(true);
        PlaySound(0);
        base.Select();
    }

    public void Unlock()
    {
        AllowSelection = true;
        _treeData.Selectable = true;
    }
    public void SetData(TreeData td)
    {
        _treeData = td;
        if(_selectableFromBeginning)
        {
            AllowSelection = true;
            _treeData.Selectable = true;
            return;
        }

        AllowSelection = td.Selectable;
    }
    public void SetMappedPosition(int x, int y)
    {
        MappedPosition = (x,y);
    }
}
