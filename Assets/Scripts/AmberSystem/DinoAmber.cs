using UnityEngine;
using UnityEngine.UI;

public class DinoAmber : MonoBehaviour
{
    [SerializeField] public int DinoAmberIndex;
    [SerializeField] private float successRate;
    [SerializeField] private int requiredAttempts;
    [SerializeField] private int attemptCost;
    [SerializeField] private bool IsUniversalAmber;
    [SerializeField] private GameObject AmberNotFound;
    [SerializeField] private GameObject BuyButton;
    [SerializeField] private GameObject UnKnownSpeciesText;
    [SerializeField] private GameObject KnownSpeciesText;
    [SerializeField] private GameObject UnknownDinoImage;
    [SerializeField] private GameObject KnownDinoImage;
    [SerializeField] private GameObject AmberDecodingImage;
    [SerializeField] public GameObject DecodeButton;
    [SerializeField] private GameObject ResearchInProgressText;
    public static int lastDecodedAmberIndex = -1;
    public float GetSuccessRate() => successRate;
    public int GetRequiredAttempts() => requiredAttempts;
    public int GetAttemptCost() => attemptCost;

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
        ShopVisibility.UpdateShopVisibility();
    }

    public void ActivateAmber()
    {
        ResearchManager.Instance.ActivateAmberNotification();
        AmberData amber = AmberManager.Instance.GetAmberList().Find(a => a.Index == DinoAmberIndex);
        AnimalToggle animalToggle = GetComponent<AnimalToggle>();
        if (amber != null && (amber.IsDecoded && amber.IsActivated))
        {
            if (AmberNotFound != null)
            {
                AmberNotFound.SetActive(false);
            }
            if (BuyButton != null && (animalToggle == null || !animalToggle.Purchased))
            {
                BuyButton.SetActive(true);
            }
            if (UnKnownSpeciesText != null)
            {
                UnKnownSpeciesText.SetActive(false);
            }
            if (KnownSpeciesText != null)
            {
                KnownSpeciesText.SetActive(true);
            }
            if (UnknownDinoImage != null)
            {
                UnknownDinoImage.SetActive(false);
            }
            if (KnownDinoImage != null)
            {
                KnownDinoImage.SetActive(true);
            }
            if (ResearchInProgressText != null)
            {
                ResearchInProgressText.SetActive(false);
            }
        }
        else
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
                    ResearchInProgressText.SetActive(false);
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
    }

    public static void DisableOtherDecodeButtons(int activeIndex)
    {
        lastDecodedAmberIndex = activeIndex;
        ResearchManager.Instance.SaveResearchProgress();
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
                        Debug.Log($"Disabled the decoding possibility to dinosaur {dinoAmber.DinoAmberIndex}");
                    }
                }
            }
        }
    }
    public static void EnableDinoAndEnableOtherDecodeButtons(int activeIndex)
    {
        Debug.Log($"Enabling dinosaur {activeIndex}");
        Debug.Log($"lastDecodedAmberIndex before setting it to -1: {lastDecodedAmberIndex}");
        AmberData amber = AmberManager.Instance.GetAmberList().Find(a => a.Index == activeIndex);
        if (amber != null)
        {
            amber.SetDecoded(true);
            AmberManager.Instance.SaveAmberData();
            ResearchManager.Instance.DeactivateAmberNotification();
        }
        DinoAmber[] allDinoAmbers = FindObjectsOfType<DinoAmber>(true);
        
        foreach (var dinoAmber in allDinoAmbers)
        {
            if (dinoAmber.DinoAmberIndex == activeIndex && dinoAmber.DecodeButton != null)
            {
                if (dinoAmber.DecodeButton.activeSelf)
                {
                    dinoAmber.DecodeButton.SetActive(false);
                    if (dinoAmber.BuyButton != null)
                    {
                        dinoAmber.BuyButton.SetActive(true);
                    }
                    if (dinoAmber.KnownSpeciesText != null)
                    {
                        dinoAmber.KnownSpeciesText.SetActive(true);
                    }
                    if (dinoAmber.KnownDinoImage != null)
                    {
                        dinoAmber.KnownDinoImage.SetActive(true);
                    }
                    if (dinoAmber.AmberDecodingImage != null)
                    {
                        dinoAmber.AmberDecodingImage.SetActive(false);
                    }
                    if (dinoAmber.UnKnownSpeciesText != null)
                    {
                        dinoAmber.UnKnownSpeciesText.SetActive(false);
                    }
                }
            }
            if (dinoAmber.DinoAmberIndex != activeIndex && dinoAmber.DecodeButton != null)
            {
                if (dinoAmber.ResearchInProgressText.activeSelf)
                {
                    dinoAmber.ResearchInProgressText.SetActive(false);
                    if (dinoAmber.DecodeButton != null)
                    {
                        dinoAmber.DecodeButton.SetActive(true);
                        Debug.Log($"Enabled the decoding possibility to dinosaur {dinoAmber.DinoAmberIndex}");
                    }
                }
            }
        }
        lastDecodedAmberIndex = -1;
        ResearchManager.Instance.SaveResearchProgress();
        Debug.Log($"lastDecodedAmberIndex after setting it to -1: {lastDecodedAmberIndex}");
    }

    public bool ShouldActivate()
    {
        return IsUniversalAmber ? AmberManager.Instance.HasAnyAmberActivated() : AmberManager.Instance.IsAmberActivated(DinoAmberIndex);
    }
}