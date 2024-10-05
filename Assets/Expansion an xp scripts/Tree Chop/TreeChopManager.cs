using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Manages the available tree chops and their state
public class TreeChopManager : Singleton<TreeChopManager>
{
    [SerializeField] private PurchasePanel _notEnoughCoinsPanel;
    [SerializeField] private TextMeshProUGUI _expansionCoinText;

    [SerializeField] private Vector3 _cellSize;

    private int availableTreeChops = 10; // Number of available tree chops

    private int _currentXPAdder = 20;
    private int _currentCostAdder = 600;

    private Dictionary<float, Dictionary<float, TreeChopper>> _treeMap;
    private (float x, float y) _halfCellSize;

    public int CurrentXP { get; private set; } = 16;
    public int CurrentCost { get; private set; } = 200;

    private void Awake()
    {
        _halfCellSize = (_cellSize.x * .5f, _cellSize.y * .5f);
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

        if(!CurrencySystem.Instance.HasEnoughCurrency(CurrencyType.Coins, CurrentCost))
        {
            _notEnoughCoinsPanel.ShowNotEnoughCoinsPanel(CurrentCost);
            return;
        }


        (SelectablesManager.Instance.CurrentSelectable as TreeChopper).PerformChopAction();
        EventManager.Instance.TriggerEvent(new CurrencyChangeGameEvent(-CurrentCost, CurrencyType.Coins));

        CurrentCost += _currentCostAdder;
        _currentCostAdder += 400;

        Attributes.SetInt("TreeExpansionCost", CurrentCost);
        Attributes.SetInt("TreeExpansionCostAdder", _currentCostAdder);

        availableTreeChops = Mathf.Max(0, availableTreeChops - 1);
    }

    public void SetExpansionCostText()
    {
        _expansionCoinText.text = "<sprite name=\"money_icon\"> " + CurrentCost;
    }

    public void SetTreeMap(Dictionary<float, Dictionary<float, TreeChopper>> treeMap)
    {
        _treeMap = treeMap;
    }

    public void UnlockAdjacentTrees(TreeChopper currentTree)
    {

        float leftX = currentTree.transform.localPosition.x - _halfCellSize.x;
        float rightX = currentTree.transform.localPosition.x + _halfCellSize.x;

        UnlockLeftRight(currentTree.transform.localPosition.y - _halfCellSize.y, leftX, rightX);
        UnlockLeftRight(currentTree.transform.localPosition.y + _halfCellSize.y, leftX, rightX);
    }

    private void UnlockLeftRight(float currentY, float leftX, float rightX)
    {
        if (_treeMap.ContainsKey(currentY))
        {
            if (_treeMap[currentY].ContainsKey(leftX))
            {
                _treeMap[currentY][leftX].Unlock();
            }
            if (_treeMap[currentY].ContainsKey(rightX))
            {
                _treeMap[currentY][rightX].Unlock();
            }
        }
    }
}
