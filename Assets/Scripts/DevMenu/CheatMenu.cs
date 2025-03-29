using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CheatMenu : MonoBehaviour
{
    void clicked()
    {
        var a = GameObject.FindGameObjectWithTag("CheatMenu");
        a.SetActive(true);
        return;
    }
    void OnLevelWasLoaded(int level)
    {
        if (level != 0)
        {
            var a = GameObject.FindGameObjectWithTag("CheatButton");
            a.GetComponent<Button>().onClick.AddListener(clicked);
        }else 
        {
            return;
        }
    }
}
