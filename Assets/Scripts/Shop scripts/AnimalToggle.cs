using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalToggle : MonoBehaviour
{
    [SerializeField] private GameObject _availablePanel;
    [SerializeField] private GameObject _boughtPanel;

    [Header("Put UnknownSpeciesText there")]
    [SerializeField] private GameObject _amber_researchCheck;

    public bool Purchased { get; private set; }

    public void SetPurchased(bool purchased)
    {
        if (_amber_researchCheck != null && _amber_researchCheck.activeSelf) return;
        Purchased = purchased;
            if(purchased)
            {
                _availablePanel.SetActive(false);
                _boughtPanel.SetActive(true);
            } 
            else
            {
                _availablePanel.SetActive(true);
                _boughtPanel.SetActive(false);
            }
    }
}
