using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ResearchData
{
    public int CurrentAttempts { get; set; }
    public int LastDecodedAmberIndex { get; set; } = -1;
    public int LastEvolutionIndex { get; set; } = -1;
    public bool TutorialDebrisSpawned { get; set; } = false;
}