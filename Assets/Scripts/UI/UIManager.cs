using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIManager : Singleton<UIManager>
{
    [Serializable]
    public struct UIManagerField
    {
        public string key;
        public List<GameObject> objects;
    }

    [SerializeField] private GameObject cam;

    [SerializeField] private List<UIManagerField> groups;
    [SerializeField] private List<UIManagerField> fixedGroups;

    // Enum to choose which zoom script to use.
    public enum ZoomType
    {
        Mobile,
        PC
    }

    // Set the desired zoom type in the Inspector.
    [SerializeField] private ZoomType zoomType;

    private readonly Dictionary<string, List<GameObject>> _UIElements = new Dictionary<string, List<GameObject>>();
    private readonly Dictionary<string, List<GameObject>> _fixedUIElements = new Dictionary<string, List<GameObject>>();

    private string _activeKey;
    private bool _currentEnabled = true;

    private string _fixedKey;
    private bool _fixedEnabled = true;

    public void ChangeCameraPanningStatus(bool enabled)
    {
        Debug.Log("omg: " + zoomType);
        // Enable the selected zoom script and disable the other one.
        if (zoomType == ZoomType.Mobile)
        {
            // Enable Mobile zoom/pan script if attached.
            var mobile = cam.GetComponent<PanZoomMobile>();
            if (mobile != null)
            {
                mobile.enabled = enabled;
            }

            // Disable the PC script if it exists.
            var pc = cam.GetComponent<PanZoomPC>();
            Debug.Log("best num: " + pc);
            if (pc != null)
            {
                Debug.Log("Disabled");
                pc.enabled = false;
            }
        }
        else if (zoomType == ZoomType.PC)
        {
            // Enable PC zoom/pan script if attached.
            var pc = cam.GetComponent<PanZoomPC>();
            Debug.Log("best num: " + pc);
            if (pc != null)
            {
                Debug.Log("Enabled");
                pc.enabled = enabled;
            }

            // Disable the Mobile script if it exists.
            var mobile = cam.GetComponent<PanZoomMobile>();
            if (mobile != null)
            {
                mobile.enabled = false;
            }
        }
    }

    private void Start()
    {
        ChangeCameraPanningStatus(true);
    }

    private void Awake()
    {
        foreach (UIManagerField field in groups)
        {
            _UIElements.Add(field.key, field.objects);
        }
        _activeKey = groups[0].key;

        foreach (UIManagerField field in fixedGroups)
        {
            _fixedUIElements.Add(field.key, field.objects);
        }
        _fixedKey = fixedGroups[0].key;
    }

    /// <summary>
    /// Changes the to the specified UI Elements. Disables the previous Elements.
    /// </summary>
    /// <param name="key">The name of the group you want to change to.</param>
    public void ChangeTo(string key)
    {
        if (_currentEnabled)
        {
            foreach (GameObject go in _UIElements[_activeKey])
            {
                go.SetActive(false);
            }
        }

        foreach (GameObject go in _UIElements[key])
        {
            go.SetActive(true);
        }
        _activeKey = key;
        _currentEnabled = true;
        ChangeCameraPanningStatus(true);
    }

    /// <summary>
    /// Disables the currently active UI Elements.
    /// </summary>
    public void DisableCurrent()
    {
        foreach (GameObject go in _UIElements[_activeKey])
        {
            go.SetActive(false);
        }

        _currentEnabled = false;
        ChangeCameraPanningStatus(false);
    }

    /// <summary>
    /// Enables the currently active UI Elements.
    /// </summary>
    public void EnableCurrent()
    {
        foreach (GameObject go in _UIElements[_activeKey])
        {
            go.SetActive(true);
        }

        _currentEnabled = true;
        ChangeCameraPanningStatus(true);
    }

    public void ChangeFixedTo(string key)
    {
        if (_fixedEnabled)
        {
            foreach (GameObject go in _fixedUIElements[_fixedKey])
            {
                go.SetActive(false);
            }
        }

        foreach (GameObject go in _fixedUIElements[key])
        {
            go.SetActive(true);
        }
        _fixedKey = key;
        _fixedEnabled = true;
    }
    /// <summary>
    /// Disables the fixed UI (Resources, Mission button, ...).
    /// </summary>
    public void DisableCurrentFixed()
    {
        foreach (GameObject go in _fixedUIElements[_fixedKey])
        {
            go.SetActive(false);
        }

        _fixedEnabled = false;
    }

    /// <summary>
    /// Enables the fixed UI (Resources, Mission button, ...).
    /// </summary>
    public void EnableCurrentFixed()
    {
        foreach (GameObject go in _fixedUIElements[_fixedKey])
        {
            go.SetActive(true);
        }

        _fixedEnabled = true;
    }
}

