using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CheatMenu : MonoBehaviour
{
    void clicked()
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects) //copy n paste of the shitty hack
        {
            if (obj.CompareTag("CheatMenu"))
            {
                obj.SetActive(true);
                break;
            }
        }
        return;
    }
    void OnLevelWasLoaded(int level)
    {
        if (level != 0)
        {
            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                if (obj.CompareTag("CheatButton"))//shitty hack cus this version of unity cant find disabled objects
                {
                    obj.SetActive(true);
                    obj.GetComponent<Button>().onClick.AddListener(clicked);
                    break;
                }
            }
        }else 
        {
            return;
        }
    }
}
