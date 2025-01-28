using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvolutionChanger : MonoBehaviour
{
    [SerializeField] private List<Material> _skinMaterials;
    [SerializeField] private List<Button> _buttons;
    [SerializeField] private List<GameObject> _stars;
    [SerializeField] private List<GameObject> _editingStars;

    private DinosaurLevelManager _dinosaurLevelManager;
    private SkinnedMeshRenderer[] _skinnedMeshRenderers;
    private int _currentSkin;
    private string _parentName;
    private int levelToSet;

    private void Start()
    {
        _dinosaurLevelManager = GetComponentInParent<DinosaurLevelManager>();
        _parentName = GetComponentInParent<Paddock>().gameObject.name;

        _skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        if (Attributes.HaveKey("CurrentSkin" + _parentName))
        {
            _currentSkin = Attributes.GetInt("CurrentSkin" + _parentName);
            ChangeSkin(_currentSkin);
        }
        else
        {
            _currentSkin = 0;
            ChangeSkin(_currentSkin);
        }

        int savedLevel = Attributes.GetInt("CurrentLevel" + _parentName, 1);
        _dinosaurLevelManager.SetLevel(savedLevel);

        foreach (Button button in _buttons)
        {
            int index = _buttons.IndexOf(button);
            button.onClick.AddListener(() => ChangeSkin(index));
        }
    }

    public void ChangeSkin(int index)
    {
        _currentSkin = index;

        Attributes.SetInt("CurrentSkin" + _parentName, index);
        Attributes.SetInt("CurrentLevel" + _parentName, levelToSet);

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
        Attributes.SetInt("CurrentLevel" + _parentName, levelToSet);

        if (_dinosaurLevelManager._dinosaurLevelResourcesManager != null)
        {
            float newMaximumMoney = _dinosaurLevelManager._dinosaurLevelResourcesManager.GetMaximumMoneyByLevel(_dinosaurLevelManager.CurrentLevel);
            MoneyObject moneyObject = GetComponentInParent<MoneyObject>();
            if (moneyObject != null)
            {
                moneyObject.MaximumMoney = newMaximumMoney;
                moneyObject.InitMoneyPerSecond();
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

        for (int i = 0; i < _editingStars.Count; i++)
        {
            _editingStars[i].SetActive(i < index);
        }

        _buttons[index].interactable = false;

        foreach (var renderer in _skinnedMeshRenderers)
        {
            renderer.material = _skinMaterials[index];
        }
    }
}