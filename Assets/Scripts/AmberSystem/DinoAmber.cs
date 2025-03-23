using UnityEngine;
using UnityEngine.UI;

public class DinoAmber : MonoBehaviour
{
    [SerializeField] public int DinoAmberIndex;
    [SerializeField] private bool IsUniversalAmber;
    [SerializeField] private GameObject AmberNotFound;
    [SerializeField] private GameObject UnknownDinoImage;
    [SerializeField] private GameObject AmberDecodingImage;
    [SerializeField] private GameObject DecodeButton;
    [SerializeField] private GameObject ResearchInProgressText;
    public static int lastDecodedAmberIndex = -1;

    public static void AddCollectedAmber(int index)
    {
        AmberManager.Instance.ActivateAmber(index);
        AmberManager.Instance.CheckAndEnableDinoAmbers();
        StartDecodingHandler startDecodingHandler = FindObjectOfType<StartDecodingHandler>(true);
        if (startDecodingHandler != null)
        {
            startDecodingHandler.SetAmberIndex(index);
        }
        ResearchButtonHandler researchButtonHandler = FindObjectOfType<ResearchButtonHandler>(true);
        if (researchButtonHandler != null)
        {
            researchButtonHandler.SetAmberIndex(index);
        }
    }

    public void ActivateAmber()
    {
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
            if (lastDecodedAmberIndex == -1 || lastDecodedAmberIndex == DinoAmberIndex)
            {
                DecodeButton.SetActive(true);
            }
            else
            {
                ResearchInProgressText.SetActive(true);
            }
            DecodeButton.GetComponent<Button>().onClick.RemoveAllListeners();
            DecodeButton.GetComponent<Button>().onClick.AddListener(() => 
            {
                ResearchManager.Instance.SetAmberIndex(DinoAmberIndex);
                ResearchManager.Instance.OpenPanel();
                DisableOtherDecodeButtons(DinoAmberIndex);
            });
        }
    }

    public static void DisableOtherDecodeButtons(int activeIndex)
    {
        lastDecodedAmberIndex = activeIndex;
        AmberManager.Instance.SetLastDecodedAmber(activeIndex); 
        DinoAmber[] allDinoAmbers = FindObjectsOfType<DinoAmber>(true);
        foreach (var dinoAmber in allDinoAmbers)
        {
            if (dinoAmber.DinoAmberIndex != activeIndex && dinoAmber.DecodeButton != null)
            {
                if (dinoAmber.DecodeButton.activeSelf)
                {
                    dinoAmber.DecodeButton.SetActive(false);
                    if (dinoAmber.ResearchInProgressText != null)
                    {
                        dinoAmber.ResearchInProgressText.SetActive(true);
                    }
                }
            }
        }
    }

    public bool ShouldActivate()
    {
        return IsUniversalAmber ? AmberManager.Instance.HasAnyAmberActivated() : AmberManager.Instance.IsAmberActivated(DinoAmberIndex);
    }
}