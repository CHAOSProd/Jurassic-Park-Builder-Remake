using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ResearchData
{
    public int CurrentResearchAttempts { get; set; }
    public int CurrentEvolutionAttempts { get; set; }
    public int LastDecodedAmberIndex { get; set; } = -1;
    public int LastEvolutionIndex { get; set; } = -1;
    public int LastStageIndex { get; set; } = -1;
    public bool TutorialDebrisSpawned { get; set; } = false;
}