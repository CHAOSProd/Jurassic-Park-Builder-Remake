using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopManager : Singleton<ShopManager>
{
    public static Dictionary<CurrencyType, Sprite> currencySprites = new Dictionary<CurrencyType, Sprite>();

    [SerializeField] private List<Sprite> sprites;

    private RectTransform rt;
    private RectTransform prt;
    private bool opened;

    [SerializeField] public TabGroup shopTabs;

    [SerializeField] private List<AnimalToggle> _animalToggles;
    private void Awake()
    {

        rt = GetComponent<RectTransform>();
        prt = transform.parent.GetComponent<RectTransform>();
        Debug.Log(gameObject.name);
    }

    private void Start()
    {
        currencySprites.Add(CurrencyType.Coins, sprites[0]);
        currencySprites.Add(CurrencyType.Crystals, sprites[1]);
        
        gameObject.SetActive(false);
    }

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
    public void ShopButton_Click()
    {
        float time = 0.2f;
        if (!opened)
        {
            LeanTween.moveX(prt, prt.anchoredPosition.x + rt.sizeDelta.x, time);
            opened = true;
            gameObject.SetActive(true);
        }
        else
        {
            LeanTween.moveX(prt, prt.anchoredPosition.x - rt.sizeDelta.x, time)
                .setOnComplete(delegate()
                {
                    gameObject.SetActive(false);
                });
            opened = false;
        }
    }
}
