using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopManager : Singleton<ShopManager>
{
    [SerializeField] private List<AnimalToggle> _animalToggles;

    public void InitalizeAnimals(List<AnimalShopData> animalShopData)
    {
        if(animalShopData == null || animalShopData.Count == 0)
        {
            _animalToggles[0].SetPurchased(true);
            return;
        }

        for (int i = 0; i < animalShopData.Count; i++)
        {
            _animalToggles[i].SetPurchased(animalShopData[i].Purchased);
        }
    }
    public List<AnimalShopData> GetAnimalShopData()
    {
        List<AnimalShopData> animalShopData = new List<AnimalShopData>();
        foreach (AnimalToggle at in _animalToggles)
        {
            animalShopData.Add(new AnimalShopData() { Purchased = at.Purchased });
        }

        return animalShopData;
    }
    public int IndexOfAnimal(AnimalToggle animal)
    {
        return _animalToggles.IndexOf(animal);
    }
    public AnimalToggle GetAnimalByIndex(int index)
    {
        return _animalToggles[index];
    }
}
