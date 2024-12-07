using System;
using System.Collections.Generic;
using System.Drawing;
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

        foreach(DebrisAmountField daf in _debrisExpansionAmounts[_currentExpansion].DebrisAmounts)
        {
            int size = _debrisTypes[daf.DebrisType].AreaLength;
           
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
        _debrisCostText.text = $"<sprite name=\"money_icon\">{cost}";
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
