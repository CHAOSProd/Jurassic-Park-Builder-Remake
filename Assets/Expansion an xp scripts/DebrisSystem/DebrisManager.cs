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

    private BoundsInt _totalArea;
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
        Vector2 startPos = tc.transform.position;
        _occupied = new List<Bounds>();
        _availablePositions = new List<Vector2>();
        _totalArea = tc.Area;

        int prevSize = -1;

        foreach(DebrisAmountField daf in _debrisExpansionAmounts[_currentExpansion].DebrisAmounts)
        {
            int size = _debrisTypes[daf.DebrisType].AreaLength;

            if (size != prevSize)
            {
                GetAvailableFields(startPos, size);
            }

            foreach(Vector2 pos in _availablePositions)
            {
                Instantiate(_debrisTypes[daf.DebrisType].Prefab, pos, Quaternion.identity);
            }
            int index = UnityEngine.Random.Range(0, _availablePositions.Count);
            _occupied.Add(new Bounds(_availablePositions[index] + (Vector2)(size * .5f * _gridLayout.cellSize), size * .5f * _gridLayout.cellSize));
            _availablePositions.RemoveAt(index);
        }

        _currentExpansion++;
    }
    private void GetAvailableFields(Vector2 startPos, int currentSize)
    {
        _availablePositions.Clear();

        Vector2 currentPos = startPos;
       
        int steps = _totalArea.size.x / currentSize;
        int step = _totalArea.size.x / steps;

        Debug.Log($"Step = {step}");
        Debug.Log($"Steps = {steps}");

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
                        Debug.Log("Disallowed.");
                        allowedPoint = false;
                        break;
                    }
                }

                if(allowedPoint) _availablePositions.Add(currentPos);
                //Use cellSize instead of cellSize.x
                currentPos += Vector2.one * step * _gridLayout.cellSize.x * .5f;
            }
            currentPos = tmp + new Vector2(.5f,-.5f) * step * _gridLayout.cellSize.y;
        }
    }
}
