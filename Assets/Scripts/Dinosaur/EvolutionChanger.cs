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

        foreach (Button button in _buttons)
        {
            button.onClick.AddListener(delegate { ChangeSkin(_buttons.IndexOf(button)); });
        }
    }

    public void ChangeSkin(int index)
    {
        _currentSkin = index;

        Attributes.SetInt("CurrentSkin" + _parrentName, index);

        switch (index)
        {
            case 0:
                _dinosaurLevelManager.SetLevel(1);
                break;
            case 1:
                _dinosaurLevelManager.SetLevel(11);
                break;
            case 2:
                _dinosaurLevelManager.SetLevel(21);
                break;
            case 3:
                _dinosaurLevelManager.SetLevel(31);
                break;
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
