using UnityEngine;

public class DinoAmber : MonoBehaviour
{
    [SerializeField] public int DinoAmberIndex;
    [SerializeField] private bool IsUniversalAmber;
    [SerializeField] private GameObject AmberNotFound;
    [SerializeField] private GameObject UnknownDinoImage;
    [SerializeField] private GameObject AmberDecodingImage;
    [SerializeField] private GameObject DecodeButton;

    public static void AddCollectedAmber(int index)
    {
        AmberManager.Instance.ActivateAmber(index);
        AmberManager.Instance.CheckAndEnableDinoAmbers();
    }

    public void ActivateAmber()
    {
        if (IsUniversalAmber)
        {
            Debug.Log("DinoAmber activated on the visitor center");
        }
        else
        {
            Debug.Log($"DinoAmber with index {DinoAmberIndex} activated.");
        }
        if (AmberNotFound != null)
        {
            AmberNotFound.SetActive(false);
        }
        if (UnknownDinoImage != null)
        {
            UnknownDinoImage.SetActive(false);
        }
        if (AmberDecodingImage != null)
        {
            AmberDecodingImage.SetActive(true);
        }
        if (DecodeButton != null)
        {
            DecodeButton.SetActive(true);
        }
    }

    public bool ShouldActivate()
    {
        return IsUniversalAmber ? AmberManager.Instance.HasAnyAmberActivated() : AmberManager.Instance.IsAmberActivated(DinoAmberIndex);
    }
}