using UnityEngine;
using UnityEngine.UI;

public class ResearchButtonHandler : MonoBehaviour
{
    private static int amberIndex = -1;
    private int stageIndex;
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
        if (AmberManager.Instance.HasUndecodedActivatedAmber())
        {
            int indexToUse = (DinoAmber.lastDecodedAmberIndex != -1) ? DinoAmber.lastDecodedAmberIndex : amberIndex;
            AmberData selectedAmber = AmberManager.Instance.GetAmberList().Find(a => a.Index == indexToUse);
            if (selectedAmber != null && selectedAmber.IsDecoded || amberIndex == -1)
            {
                AmberData firstUndecodedAmber = AmberManager.Instance.GetAmberList().Find(a => !a.IsDecoded && a.IsActivated);
                if (firstUndecodedAmber != null && firstUndecodedAmber.Index != DinoAmber.lastDecodedAmberIndex && DinoAmber.lastDecodedAmberIndex != -1)
                {
                    indexToUse = DinoAmber.lastDecodedAmberIndex;
                }
                else if (firstUndecodedAmber != null)
                {
                    indexToUse = firstUndecodedAmber.Index;
                }
            }
            if (indexToUse != -1)
            {
                ResearchManager.Instance.SetAmberIndex(indexToUse);
                DinoAmber.DisableOtherDecodeButtons(indexToUse);
                ResearchManager.Instance.OpenPanel();
            }
        }
        else
        {
            ResearchManager.Instance.OpenNoAmberPanel();
        }
    }
}