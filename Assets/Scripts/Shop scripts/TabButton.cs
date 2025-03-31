using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TabButton : MonoBehaviour, IPointerClickHandler
{
    public TabGroup tabGroup;
    [NonSerialized] public Image background;

    // New fields for custom sprite support.
    // Set this flag to true if you want to use different sprites for this button.
    public bool useCustomSprites = false;
    public Sprite customIdle;
    public Sprite customActive;

    private void Awake()
    {
        background = GetComponent<Image>();
        tabGroup.Subscribe(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabSelected(this);
    }
}

