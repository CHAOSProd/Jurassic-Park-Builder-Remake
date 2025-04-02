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
    
    private void Start()
    {
        // Get the level manager and parent name for reference (if needed elsewhere).
        _dinosaurLevelManager = GetComponentInParent<DinosaurLevelManager>();

        _skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        // Retrieve the current skin index (if saved) or default to 0.
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

        // Removed: the code that retrieved and set the dinosaur level.

        // Set up the button listeners for skin changing.
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

        // Removed: code that would set the dinosaur level based on skin index.
        // The switch statement below is removed because it was only used to determine levelToSet.
        // If you want to later tie skin changes to visual cues only, you can add other logic here.

        if (!_skinMaterials.Contains(_skinMaterials[index]))
            return;

        // Make all buttons interactable.
        foreach (Button button in _buttons)
        {
            button.interactable = true;
        }

        // Update stars for visual indication.
        for (int i = 0; i < _stars.Count; i++)
        {
            _stars[i].SetActive(i < index);
        }

        for (int i = 0; i < _editingStars.Count; i++)
        {
            _editingStars[i].SetActive(i < index);
        }

        // Disable the button corresponding to the current skin.
        _buttons[index].interactable = false;

        // Change all SkinnedMeshRenderers' materials to the selected skin.
        foreach (var renderer in _skinnedMeshRenderers)
        {
            renderer.material = _skinMaterials[index];
        }
    }
}
