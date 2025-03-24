using UnityEngine;
using UnityEngine.UI;

public class AttemptResearchButtonHandler : MonoBehaviour
{
    private static int amberIndex = -1;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnAttemptResearchButtonClick);
        }
    }
    public void SetAmberIndex(int index)
    {
        amberIndex = index;
    }

    private void OnAttemptResearchButtonClick()
    {
        ResearchManager.Instance.AttemptResearch();
    }
}