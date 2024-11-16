using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;

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

    private Dictionary<DebrisType, DebrisInfo> _debrisTypes = new Dictionary<DebrisType, DebrisInfo>();
    private int _currentExpansion = 0;

    private Bounds _totalArea;
    private List<Bounds> _occupied;
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
        Vector2 startPos = new(tc.transform.position.x + tc.Area.x * _gridLayout.cellSize.x, tc.transform.position.y + tc.Area.y * _gridLayout.cellSize.y);
        _occupied = new List<Bounds>();
        _availablePositions = new List<Vector2>();
        int prevSize = -1;

        foreach(DebrisAmountField daf in _debrisExpansionAmounts[_currentExpansion].DebrisAmounts)
        {
            int size = _debrisTypes[daf.DebrisType].AreaLength;

            if (size != prevSize)
            {
                GetAvailableFields(startPos, size);
            }

            int index = UnityEngine.Random.Range(0, _availablePositions.Count);
            Instantiate(_debrisTypes[daf.DebrisType].Prefab, _availablePositions[index], Quaternion.identity);
            _availablePositions.RemoveAt(index);
        }

        _currentExpansion++;
    }
    private void GetAvailableFields(Vector2 startPos, int currentSize)
    {
        _availablePositions.Clear();

        Vector2 currentPos = new Vector2(startPos.x + currentSize * .5f * _gridLayout.cellSize.x, startPos.y + currentSize * .5f * _gridLayout.cellSize.y);
        float step = _totalArea.extents.x / currentSize;
        int steps = Mathf.RoundToInt( _totalArea.extents.x / step);

        for (int y = 0; y < steps; y++)
        {
            Vector2 tmp = currentPos;
            for (int x = 0; x < steps; x++)
            {
                bool allowedPoint = true;
                foreach(Bounds b in _occupied)
                {
                    if(b.Contains(currentPos))
                    {
                        allowedPoint = false;
                        break;
                    }
                }

                if(allowedPoint) _availablePositions.Add(currentPos);
                currentPos += Vector2.right * step;
            }
            currentPos = tmp + Vector2.down * step;
        }
    }
}
