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
    [SerializeField] private AudioSource _sellSoundPlayer;

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
    }

    public void OnClose()
    {
        _objectToSell = null;
        sellPanel.SetActive(false);
    }

    public void OnSell()
    {
        EventManager.Instance.TriggerEvent(new CurrencyChangeGameEvent(_objectToSell.data.SellRefund, CurrencyType.Coins));
        SaveManager.Instance.SaveData.PlaceableObjects.Remove(_objectToSell.data);

        _moneyCounter.transform.position = _objectToSell.transform.position + new Vector3(0, .108f);
        _moneyCounter.SetActive(true);
        _moneyCounter.GetComponentInChildren<MoneyCountDisplayer>().DisplayCount(_objectToSell.data.SellRefund);

        _sellSoundPlayer.gameObject.SetActive(true);
        _sellSoundPlayer.Play();

        // Clear occupied tiles from Placed Object
        Vector3Int positionInt = GridBuildingSystem.Instance.GridLayout.WorldToCell(_objectToSell.transform.position);
        BoundsInt areaTemp = _objectToSell.Area;
        areaTemp.position = positionInt;
        GridBuildingSystem.Instance.SetAreaWhite(areaTemp, GridBuildingSystem.Instance.MainTilemap);

        Destroy(_objectToSell.gameObject);
        _objectToSell = null;
        sellPanel.SetActive(false);

        UnityTimer.Instance.Wait(.5f, () =>
        {
            _moneyCounter.SetActive(false);
            _sellSoundPlayer.gameObject.SetActive(false);
            _sellSoundPlayer.Stop();
        });
    }
}