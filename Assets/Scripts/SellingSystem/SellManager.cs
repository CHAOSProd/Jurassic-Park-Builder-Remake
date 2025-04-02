using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SellManager : Singleton<SellManager>
{
    [Header("Sell Panel Components")]
    [SerializeField] private GameObject sellPanel;
    [SerializeField] private TextMeshProUGUI sellSubtitle;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button panelSellButton;

    [Header("VFX Objects")]
    [SerializeField] private GameObject _moneyCounter;
    [SerializeField] private TextMeshProUGUI _moneyCountText;

    [Header("Sell Button Component")]
    [SerializeField] private Button sellButton;

    private PlaceableObject _objectToSell;
    private void Start()
    {
        sellPanel.SetActive(false);

        exitButton.onClick.AddListener(OnClose);
        panelSellButton.onClick.AddListener(OnSell);
        sellButton.onClick.AddListener(OnUIOpen);
    }

    public void OnUIOpen()
    {
        _objectToSell = SelectablesManager.Instance.CurrentSelectable.GetComponent<PlaceableObject>();
        if(_objectToSell == null) _objectToSell = SelectablesManager.Instance.CurrentSelectable.transform.parent.GetComponent<PlaceableObject>();
        string name = _objectToSell.data.ItemName;
        int sellRefund = _objectToSell.data.SellRefund;

        sellPanel.SetActive(true);
        sellSubtitle.text = $"Are you sure you want to sell {name} for <sprite name=\"money_icon\"> {sellRefund}?";

        UIManager.Instance.ChangeFixedTo("PanelUI");
        UIManager.Instance.DisableCurrent();
        DinosaurFeedingUIManager.Instance.DisableEvolutionButton();
    }

    public void OnClose()
    {
        _objectToSell = null;
        sellPanel.SetActive(false);

        UIManager.Instance.ChangeFixedTo("DefaultUI");
        UIManager.Instance.EnableCurrent();
        DinosaurFeedingUIManager.Instance.UpdateUI();
    }

    public void OnSell()
    {
        EventManager.Instance.TriggerEvent(new CurrencyChangeGameEvent(_objectToSell.data.SellRefund, CurrencyType.Coins));
        SaveManager.Instance.SaveData.PlaceableObjects.Remove(_objectToSell.data);

        _moneyCounter.transform.position = _objectToSell.transform.position + new Vector3(-0.1f, .408f);
        _moneyCounter.SetActive(true);
        _moneyCounter.GetComponentInChildren<MoneyCountDisplayer>().DisplayCount(_objectToSell.data.SellRefund);

        // Clear occupied tiles from Placed Object
        Vector3Int positionInt = GridBuildingSystem.Instance.GridLayout.WorldToCell(_objectToSell.transform.position);
        BoundsInt areaTemp = _objectToSell.Area;
        areaTemp.position = positionInt;
        GridBuildingSystem.Instance.SetAreaWhite(areaTemp, GridBuildingSystem.Instance.MainTilemap);
            
        if (Paddock.SelectedPaddock != null)
        {

            DinosaurLevelManager dinoManager = Paddock.SelectedPaddock.GetComponentInChildren<DinosaurLevelManager>();
            if (dinoManager != null)
            {
                string paddockName = Paddock.SelectedPaddock.gameObject.name;

                dinoManager.CurrentLevel = 1;
                dinoManager._feedingSystem.feedCount = 0;

                Attributes.SetInt("CurrentLevel" + paddockName, 1);
                Attributes.SetInt("FeedCount" + paddockName, 0);

                Debug.Log($"Reset completed for {paddockName}: CurrentLevel -> {Attributes.GetInt("CurrentLevel" + paddockName)}, FeedCount -> {Attributes.GetInt("FeedCount" + paddockName)}");
            }
            else
            {
                Debug.LogWarning("DinosaurLevelManager NON trovato nel paddock selezionato!");
            }
        }

        // Make Animal Purchasable again and reset its stats
        if(_objectToSell.data.AnimalIndex != null)
        {
            EvolutionChanger evolutionChanger = _objectToSell.GetComponentInChildren<EvolutionChanger>();
            if (evolutionChanger != null)
            {
                evolutionChanger.ChangeSkin(0);
            }
            ShopManager.Instance.GetAnimalByIndex(_objectToSell.data.AnimalIndex ?? 0).SetPurchased(false);
            _objectToSell.GetComponentInChildren<HatchingTimer>(true).RemoveData();
        }

        SelectablesManager.Instance.UnselectAll();
        UIManager.Instance.ChangeFixedTo("DefaultUI");

        Destroy(_objectToSell.gameObject);
        _objectToSell = null;
        sellPanel.SetActive(false);

        UnityTimer.Instance.Wait(.75f, () =>
        {
            _moneyCounter.SetActive(false);
        });
    }
}
