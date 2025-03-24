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
        bool allAmbersDecoded = AmberManager.Instance.GetAmberList().TrueForAll(a => a.IsDecoded);
        if (AmberManager.Instance.HasAnyAmberActivated() && !allAmbersDecoded)
        {
            int lastCollectedAmberIndex = AmberManager.Instance.GetLastCollectedAmberIndex();
            int indexToUse = (DinoAmber.lastDecodedAmberIndex != -1) ? DinoAmber.lastDecodedAmberIndex : lastCollectedAmberIndex;
            AmberData selectedAmber = AmberManager.Instance.GetAmberList().Find(a => a.Index == indexToUse);
            if (selectedAmber != null && selectedAmber.IsDecoded)
            {
                AmberData firstUndecodedAmber = AmberManager.Instance.GetAmberList().Find(a => !a.IsDecoded);
                if (firstUndecodedAmber != null)
                {
                    indexToUse = firstUndecodedAmber.Index;
                }
            }
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