using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvolutionChanger : MonoBehaviour
{
    [SerializeField] private List<Material> _skinMaterials;
    [SerializeField] private List<GameObject> _stars;
    [SerializeField] private List<GameObject> _editingStars;

    private DinosaurLevelManager _dinosaurLevelManager;
    private SkinnedMeshRenderer[] _skinnedMeshRenderers;
    private string _parentName;

    private void Start()
    {
        _dinosaurLevelManager = GetComponentInParent<DinosaurLevelManager>();
        _parentName = GetComponentInParent<Paddock>().gameObject.name;
        _skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        UpdateSkinBasedOnLevel();
    }
    private void OnEnable()
    {
        _dinosaurLevelManager = GetComponentInParent<DinosaurLevelManager>();
        _parentName = GetComponentInParent<Paddock>().gameObject.name;
        _skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        UpdateSkinBasedOnLevel();
    }

    public void UpdateSkinBasedOnLevel()
    {
        if (_dinosaurLevelManager == null)
        {
            Debug.LogWarning("DinosaurLevelManager not found.");
            return;
        }

        int level = _dinosaurLevelManager.CurrentLevel;
        int skinIndex = GetSkinIndexFromLevel(level);
        ChangeSkin(skinIndex);
    }

    private int GetSkinIndexFromLevel(int level)
    {
        if (level >= 11 && level <= 20)
            return 1;
        else if (level >= 21 && level <= 30)
            return 2;
        else if (level >= 31 && level <= 40)
            return 3;
        else
            return 0;
    }

    public void ChangeSkin(int index)
    {
        if (index < 0 || index >= _skinMaterials.Count)
        {
            Debug.LogWarning($"Invalid skin index {index}");
            return;
        }

        // Save current skin index
        Attributes.SetInt("CurrentSkin" + _parentName, index);

        int level = _dinosaurLevelManager.CurrentLevel;

        int starsToShow = index;

        if (level == 40)
        {
            starsToShow += 1;
        }

        // Update stars for visual indication
        for (int i = 0; i < _stars.Count; i++)
        {
            _stars[i].SetActive(i < starsToShow);
        }

        for (int i = 0; i < _editingStars.Count; i++)
        {
            _editingStars[i].SetActive(i < starsToShow);
        }

        // Apply material to all SkinnedMeshRenderers
        foreach (var renderer in _skinnedMeshRenderers)
        {
            renderer.material = _skinMaterials[index];
        }

        Debug.Log($"Skin changed to index {index} based on level {_dinosaurLevelManager.CurrentLevel}");
    }
}
