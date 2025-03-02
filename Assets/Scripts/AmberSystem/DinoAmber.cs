using UnityEngine;

public class DinoAmber : MonoBehaviour
{
    [SerializeField] public int DinoAmberIndex;

    public static void AddCollectedAmber(int index)
    {
        AmberManager.Instance.ActivateAmber(index);
        AmberManager.Instance.CheckAndEnableDinoAmbers();
    }

    public void ActivateAmber()
    {
        Debug.Log($"DinoAmber with index {DinoAmberIndex} activated.");
        gameObject.SetActive(true);
    }
}
