using System; 
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;

public class DebrisManager : Singleton<DebrisManager>
{
    [Serializable]
    struct DebrisTypeField
    {
        public DebrisType DebrisType;
        public DebrisInfo DebrisInfo;
    }

    [Serializable]
    struct DebrisAmountField
    {
        public DebrisType DebrisType;
        public int Amount;
    }

    [Serializable]
    class DebrisListElement
    {
        public List<DebrisAmountField> DebrisAmounts;
    }
    
    [SerializeField] private List<DebrisTypeField> _debrisTypeFields;
    [SerializeField] private List<DebrisListElement> _debrisExpansionAmounts;
    [SerializeField] private GridLayout _gridLayout;
    [SerializeField] private Transform _debrisParent;

    [SerializeField] private TextMeshProUGUI _debrisCostText;

    private Dictionary<DebrisType, DebrisInfo> _debrisTypes = new Dictionary<DebrisType, DebrisInfo>();
    private int _currentExpansion = 0;

    private BoundsInt _totalArea;
    private List<Vector2> _availablePositions;

    private void Awake()
    {
        foreach(DebrisTypeField def in _debrisTypeFields)
        {
            _debrisTypes.Add(def.DebrisType, def.DebrisInfo);
        }
    }

    public void SpawnDebris(TreeChopper tc)
    {
        _availablePositions = new List<Vector2>();
        _totalArea = tc.Area;

        int prevSize = -1;
        bool amberFound = false;

        List<DebrisAmountField> debrisAmounts = _debrisExpansionAmounts[_currentExpansion].DebrisAmounts;

        // Check for debris to expand
        if (debrisAmounts.Count > 0)
        {
            // Find the highest level among the debris
            int highestDebrisLevel = debrisAmounts
                .Select(d => _debrisTypes[d.DebrisType].DebrisLevel)
                .DefaultIfEmpty(-1)
                .Max();

            // Highest level found
            Debug.Log($"Highest debris level found: {highestDebrisLevel}");

            // Find the 2nd highest level among the debris
            int secondHighestDebrisLevel = debrisAmounts
                .Where(d => _debrisTypes[d.DebrisType].DebrisLevel < highestDebrisLevel)
                .Select(d => _debrisTypes[d.DebrisType].DebrisLevel)
                .DefaultIfEmpty(-1)
                .Max();

            // 2nd highest level found
            Debug.Log($"Second highest debris level found: {secondHighestDebrisLevel}");

            bool foundAmberInSecondLevel = false;
            bool searchSecondLevelFirst = true;

            // Check if the difference between the highest level and the 2nd highest level isn't higher than 1
            if (highestDebrisLevel - secondHighestDebrisLevel > 1)
            {
                searchSecondLevelFirst = false;
                Debug.Log("Theres too much difference between highest and 2nd highest, search on second highest skipped");
            }

            // Search on the 2nd highest level debris with a 20% chance of finding amber
            if (searchSecondLevelFirst)
            {
                Debug.Log("Since theres just 1 level of difference between highest and 2nd highest, searching in second highest debris level first with 20% chance.");

                foreach (DebrisAmountField daf in debrisAmounts)
                {
                    int debrisLevel = _debrisTypes[daf.DebrisType].DebrisLevel;

                    if (debrisLevel == secondHighestDebrisLevel && !amberFound && !foundAmberInSecondLevel)
                    {
                        if (UnityEngine.Random.Range(0f, 1f) <= 0.2f)
                        {
                            amberFound = true;
                            foundAmberInSecondLevel = true;
                            Debug.Log($"Amber found in 2nd highest debris: {daf.DebrisType}, Level: {debrisLevel}");
                        }
                        else
                        {
                            Debug.Log("Amber not found in 2nd highest debris");
                        }
                    }
                }
            }

            // If it has not been found in the 2nd highest level or has not been searched, it must find amber in the highest level
            if (!amberFound)
            {
                foreach (DebrisAmountField daf in debrisAmounts)
                {
                    int debrisLevel = _debrisTypes[daf.DebrisType].DebrisLevel;

                    if (debrisLevel == highestDebrisLevel && !amberFound)
                    {
                        amberFound = true;
                        Debug.Log($"Amber found in highest debris: {daf.DebrisType}, Level: {debrisLevel}");
                    }
                }
            }

            foreach (DebrisAmountField daf in debrisAmounts)
            {
                int size = _debrisTypes[daf.DebrisType].AreaLength;
                int debrisLevel = _debrisTypes[daf.DebrisType].DebrisLevel;

                if (size != prevSize)
                {
                    Vector2 startPos = (Vector2)tc.transform.position - TranslateFromGrid(Axis.X, 7 - size + ((size + 1) / 2 - 1)) - TranslateFromGrid(Axis.Y, 7 - size + size / 4);
                    GetAvailableFields(startPos, size);
                }

                for (int i = 0; i < daf.Amount; i++)
                {
                    int index = UnityEngine.Random.Range(0, _availablePositions.Count);
                    GameObject debris = Instantiate(_debrisTypes[daf.DebrisType].Prefab, _availablePositions[index], Quaternion.identity, _debrisParent);

                    if (debris.TryGetComponent(out DebrisObject debrisObject))
                    {
                        debrisObject.Initialize(size, daf.DebrisType);
                    }
                    _availablePositions.RemoveAt(index);
                }
            }
        }
        else
        {
            Debug.Log("No debris available for expansion.");
        }

        _currentExpansion++;
        Attributes.SetAttribute("DebrisManagerCurrentExpansion", _currentExpansion);
    }
    
    private void GetAvailableFields(Vector2 startPos, int currentSize)
    {
        _availablePositions.Clear();

        Vector2 currentPos = startPos;
       
        int steps = _totalArea.size.x / currentSize;
        int step = _totalArea.size.x / steps;

        for (int y = 0; y < steps; y++)
        {
            Vector2 tmp = currentPos;
            for (int x = 0; x < steps; x++)
            {
                BoundsInt b = new BoundsInt(GridBuildingSystem.Instance.MainTilemap.WorldToCell(currentPos) - Vector3Int.one * (currentSize >> 1), Vector3Int.one * currentSize);

                if (GridBuildingSystem.Instance.CanTakeArea(b))
                    _availablePositions.Add(currentPos);


                currentPos += TranslateFromGrid(Axis.X, step);
            }
            currentPos = tmp + TranslateFromGrid(Axis.Y, step);
        }
    }
    private Vector2 TranslateFromGrid(Axis axis, int tiles)
    {
        if (axis == Axis.X)
        {
            return .5f * tiles * (Vector2)_gridLayout.cellSize;
        }

        //Inverted Y Axis because of unity
        return .5f * tiles * new Vector2(_gridLayout.cellSize.x, -_gridLayout.cellSize.y);
    }

    public void UpdateCoinText(int cost)
    {
        _debrisCostText.text = $"<sprite name=\"money_icon\">{cost.ToString("N0", CultureInfo.GetCultureInfo("en-US"))}";
    }

    public void RemoveCurrentDebris()
    {
        (SelectablesManager.Instance.CurrentSelectable as DebrisObject).OnRemoveClick();
    }

    public void LoadDebris(DebrisData d)
    {
        DebrisObject debris = Instantiate(_debrisTypes[d.DebrisType].Prefab, _debrisParent).GetComponent<DebrisObject>();
        debris.transform.position = new Vector3(d.Position.x, d.Position.y, d.Position.z);
        debris.Load(d, _debrisTypes[d.DebrisType].AreaLength);
    }

    public void Load()
    {
        _currentExpansion = Attributes.GetInt("DebrisManagerCurrentExpansion", 0);
    }
}