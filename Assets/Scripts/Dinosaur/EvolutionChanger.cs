using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvolutionChanger : MonoBehaviour
{
    [SerializeField] private List<Material> _skinMaterials;
    [SerializeField] private List<Button> _buttons;
    [SerializeField] private List<GameObject> _stars;

    private DinosaurLevelManager _dinosaurLevelManager;
    private SkinnedMeshRenderer _skinnedMeshRenderer;
    private int _currentSkin;
    private string _parrentName;
    int levelToSet;

    private void Start()
    {
        _dinosaurLevelManager = GetComponentInParent<DinosaurLevelManager>();
        _parrentName = GetComponentInParent<Paddock>().gameObject.name;

        _skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        if(Attributes.HaveKey("CurrentSkin" + _parrentName))
        {
            _currentSkin = Attributes.GetInt("CurrentSkin" + _parrentName);
            ChangeSkin(_currentSkin);
        }
        else
        {
            _currentSkin = 0;
            ChangeSkin(_currentSkin);
        }

        int savedLevel = Attributes.GetInt("CurrentLevel" + _parrentName, 1);
        _dinosaurLevelManager.SetLevel(savedLevel);

        foreach (Button button in _buttons)
        {
            button.onClick.AddListener(delegate { ChangeSkin(_buttons.IndexOf(button)); });
        }
    }

    public void ChangeSkin(int index)
    {
        _currentSkin = index;

        Attributes.SetInt("CurrentSkin" + _parrentName, index);
        Attributes.SetInt("CurrentLevel" + _parrentName, levelToSet);

        _dinosaurLevelManager.SetLevel(levelToSet);

        _dinosaurLevelManager.Initialize();
        switch (index)
        {
            case 0:
                levelToSet = 1;
                break;
            case 1:
                levelToSet = 11;
                break;
            case 2:
                levelToSet = 21;
                break;
            case 3:
                levelToSet = 31;
                break;
        }

        _dinosaurLevelManager.SetLevel(levelToSet);
        Attributes.SetInt("CurrentLevel" + _parrentName, levelToSet);

        //Tried to fix the max cap by level save when quitting there but it still doesn't get saved
        if (_dinosaurLevelManager._dinosaurLevelResourcesManager != null)
        {
            float newMaximumMoney = _dinosaurLevelManager._dinosaurLevelResourcesManager.GetMaximumMoneyByLevel(_dinosaurLevelManager.CurrentLevel);
            MoneyObject moneyObject = GetComponentInParent<MoneyObject>();
            if (moneyObject != null)
            {
                moneyObject.MaximumMoney = newMaximumMoney;
                moneyObject.InitializeMoneyPerSecond();
            }
        }

        if (!_skinMaterials.Contains(_skinMaterials[index]))
            return;

        for (int i = 0; i < _buttons.Count; i++)
        {
            _buttons[i].interactable = true;
        }

        for (int i = 0; i < _stars.Count; i++)
        {
            _stars[i].SetActive(i < index);
        }

        _buttons[index].interactable = false;

        _skinnedMeshRenderer.material = _skinMaterials[index];
    }
}
