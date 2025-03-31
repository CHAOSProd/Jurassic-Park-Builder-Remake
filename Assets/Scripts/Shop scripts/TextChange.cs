using UnityEngine;
using TMPro;

public class TextChange : MonoBehaviour
{
    public TextMeshProUGUI mainText;
    public bool shop = false;
    public bool home = false;

    void Start()
    {
        UpdateText();
    }

    void UpdateText()
    {
        if (shop)
        {
            mainText.text = "RESOURCES";
        }
        else if (home)
        {
            mainText.text = "MAILBOX";
        }
    }
}