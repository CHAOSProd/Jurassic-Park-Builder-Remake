using UnityEngine;
using UnityEngine.UI;

public class ResearchButtonHandler : MonoBehaviour
{
    private static int amberIndex = -1;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnResearchButtonClick);
        }
    }

    public void SetAmberIndex(int index)
    {
        amberIndex = index;
    }

    private void OnResearchButtonClick()
    {
        if (AmberManager.Instance.HasAnyAmberActivated())
        {
            int lastCollectedAmberIndex = AmberManager.Instance.GetLastCollectedAmberIndex();
            int indexToUse = (DinoAmber.lastDecodedAmberIndex != -1) ? DinoAmber.lastDecodedAmberIndex : lastCollectedAmberIndex;
            ResearchManager.Instance.SetAmberIndex(indexToUse);
            ResearchManager.Instance.OpenPanel();
            DinoAmber.DisableOtherDecodeButtons(indexToUse);
        }
        else
        {
            ResearchManager.Instance.OpenNoAmberPanel();
        }
    }
}