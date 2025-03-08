using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalToggle : MonoBehaviour
{
    [SerializeField] private GameObject _availablePanel;
    [SerializeField] private GameObject _boughtPanel;

    public bool Purchased { get; private set; }

    public void SetPurchased(bool purchased)
    {
        Purchased = purchased;

        if(purchased)
        {
            _availablePanel.SetActive(false);
            _boughtPanel.SetActive(true);
        } else
        {
            _availablePanel.SetActive(true);
            _boughtPanel.SetActive(false);
        }
    }
}
