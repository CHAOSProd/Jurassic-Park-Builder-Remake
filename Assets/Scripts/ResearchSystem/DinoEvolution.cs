using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EvolutionStage
{
    public float successRate;
    public int requiredAttempts;
    public int attemptCost;
    public float GetSuccessRate() => successRate;
    public int GetRequiredAttempts() => requiredAttempts;
    public int GetAttemptCost() => attemptCost;
}

public class DinoEvolution : MonoBehaviour
{
    [SerializeField] public int DinoEvolutionIndex;
    [SerializeField] public GameObject DinoToDisable;
    [SerializeField] public GameObject evolutionIconToEnable;
    [SerializeField] private List<EvolutionStage> evolutionStages = new List<EvolutionStage>(3);
    public EvolutionStage GetStage(int index)
    {
        if (index >= 0 && index < evolutionStages.Count)
            return evolutionStages[index];
        else
            return null;
    }
}
