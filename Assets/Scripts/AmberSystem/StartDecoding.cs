using UnityEngine;
using UnityEngine.UI;

public class StartDecodingHandler : MonoBehaviour
{
    private static int amberIndex = -1;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnStartDecodingClick);
        }
    }

    public void SetAmberIndex(int index)
    {
        amberIndex = index;
    }

    private void OnStartDecodingClick()
    {
        int lastCollectedAmberIndex = AmberManager.Instance.GetLastCollectedAmberIndex();
        ResearchManager.Instance.SetAmberIndex(lastCollectedAmberIndex);
        ResearchManager.Instance.OpenPanel();
        DinoAmber.DisableOtherDecodeButtons(lastCollectedAmberIndex);
    }
}