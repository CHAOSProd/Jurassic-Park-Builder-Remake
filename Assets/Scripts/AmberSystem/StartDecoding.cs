using UnityEngine;
using UnityEngine.UI;

public class StartDecodingHandler : MonoBehaviour
{
    private int amberIndex = -1;
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
        ResearchManager.Instance.SetAmberIndex(amberIndex);
        ResearchManager.Instance.OpenPanel();
        DinoAmber.DisableOtherDecodeButtons(amberIndex);
    }
}