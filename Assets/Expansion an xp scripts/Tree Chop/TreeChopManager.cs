using NUnit.Framework;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

// Manages the available tree chops and their state
public class TreeChopManager : Singleton<TreeChopManager>
{
    [SerializeField] private PurchasePanel _notEnoughCoinsPanel;
    [SerializeField] private TextMeshProUGUI _expansionCoinText;
    [SerializeField] private Grid _cellGrid;
    [SerializeField] private GameObject _chopSoundEffect;

    public (float width, float height) CellSize { get; private set; }

    private int availableTreeChops = 10; // Number of available tree chops

    private int _currentXPAdder = 20;
    private int _currentCostAdder = 600;
    private int _chopeedTrees = 0;

    [SerializeField] private List<int> _chopTime;
    private Dictionary<(int x, int y), TreeChopper> _treeMap;

    public int CurrentXP { get; private set; } = 16;
    public int CurrentCost { get; private set; } = 200;

    private void Awake()
    {
        CellSize = (_cellGrid.cellSize.x, _cellGrid.cellSize.y);
    }
    // Increases the number of available tree chops
    public void IncreaseTreeChops() 
    {
        availableTreeChops = Mathf.Max(0, availableTreeChops + 1); // Ensure chops don't go below zero
        Debug.Log($"Tree chops: {availableTreeChops}");
    }
    public void Load()
    {
        CurrentXP = Attributes.GetInt("TreeExpansionXP", 16);
        _currentXPAdder = Attributes.GetInt("TreeExpansionXPAdder", 20);
        CurrentCost = Attributes.GetInt("TreeExpansionCost", 200);
        _currentCostAdder = Attributes.GetInt("TreeExpansionCostAdder", 600);
        _chopeedTrees = Attributes.GetInt("ChoppedTrees", 0);
    }
    public void UpdateXP()
    {
        CurrentXP += _currentXPAdder;
        _currentXPAdder += 8;

        Attributes.SetInt("TreeExpansionXP", CurrentXP);
        Attributes.SetInt("TreeExpansionXPAdder", _currentXPAdder);
    }

    public void ChopTree() 
    {
        if (availableTreeChops < 1 || SelectablesManager.Instance.CurrentSelectable == null || SelectablesManager.Instance.CurrentSelectable is not TreeChopper) return;

        if (!EventManager.Instance.TriggerEvent(new CurrencyChangeGameEvent(-CurrentCost, CurrencyType.Coins)))
        {
            return;
        }

        (SelectablesManager.Instance.CurrentSelectable as TreeChopper).chopTime = _chopTime[_chopeedTrees];
        (SelectablesManager.Instance.CurrentSelectable as TreeChopper).PerformChopAction();
        _chopSoundEffect.GetComponent<AudioSource>().Play();
        CurrentCost += _currentCostAdder;
        _currentCostAdder += 400;
        _chopeedTrees += 1;

        Attributes.SetInt("TreeExpansionCost", CurrentCost);
        Attributes.SetInt("TreeExpansionCostAdder", _currentCostAdder);
        Attributes.SetInt("ChoppedTrees", _chopeedTrees);

        availableTreeChops = Mathf.Max(0, availableTreeChops - 1);

        SelectablesManager.Instance.UnselectAll();
    }

    public void SetExpansionCostText()
    {
        _expansionCoinText.text = $"<sprite name=\"money_icon\"> {CurrentCost.ToString("N0",CultureInfo.GetCultureInfo("en-US"))}";
    }

    public void SetTreeMap(Dictionary<(int x, int y), TreeChopper> treeMap)
    {
        _treeMap = treeMap;
    }

    public void UnlockAdjacentTrees(TreeChopper currentTree)
    {
        int top = currentTree.MappedPosition.y + 1;
        int left = currentTree.MappedPosition.x - 1;

        int bottom = currentTree.MappedPosition.y - 1;
        int right = currentTree.MappedPosition.x + 1;


        if (_treeMap.TryGetValue((left, top), out TreeChopper chopper))
        {
            chopper.Unlock();
        }
        if (_treeMap.TryGetValue((right, top), out chopper))
        {
            chopper.Unlock();
        }
        if (_treeMap.TryGetValue((left, bottom), out chopper))
        {
            chopper.Unlock();
        }
        if (_treeMap.TryGetValue((right, bottom), out chopper))
        {
            chopper.Unlock();
        }
    }
}
