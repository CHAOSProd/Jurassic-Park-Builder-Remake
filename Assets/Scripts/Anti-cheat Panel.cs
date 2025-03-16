using System;
using UnityEngine;

public class CheatWarningManager : MonoBehaviour
{
    [SerializeField] private GameObject warningObject;

    private void Start()
    {
        int futureSaveCount = PlayerPrefs.GetInt("FutureSaveCount", 0);
        int warningDisplayed = PlayerPrefs.GetInt("WarningDisplayed", 0);
        if (futureSaveCount == 1 && warningDisplayed == 0)
        {
            ActivateWarning();
            PlayerPrefs.SetInt("WarningDisplayed", 1);
            PlayerPrefs.Save();
        }
    }

    private void ActivateWarning()
    {
        if (warningObject != null)
        {
            warningObject.SetActive(true);
            Debug.Log("Panel enabled");
        }
    }
}