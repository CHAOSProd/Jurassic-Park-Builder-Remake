using System;
using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    // Default sprites for buttons that don't use custom sprites.
    public Sprite defaultTabIdle;
    public Sprite defaultTabActive;

    public List<TabButton> tabButtons = new List<TabButton>();
    public List<GameObject> objectsToSwap = new List<GameObject>();

    [NonSerialized] public TabButton selectedTab;

    private void Start()
    {
        if(tabButtons.Count > 0)
            OnTabSelected(tabButtons[0]);
    }

    public void Subscribe(TabButton button)
    {
        tabButtons.Add(button);
    }

    private void ResetTabs()
    {
        foreach (var button in tabButtons)
        {
            if (selectedTab != null && button == selectedTab)
                continue;

            // Use custom idle sprite if flagged, otherwise default idle.
            if (button.useCustomSprites && button.customIdle != null)
                button.background.sprite = button.customIdle;
            else
                button.background.sprite = defaultTabIdle;
        }
    }

    public void OnTabSelected(TabButton button)
    {
        selectedTab = button;
        ResetTabs();

        // Set active sprite, checking for custom active if flagged.
        if (button.useCustomSprites && button.customActive != null)
            button.background.sprite = button.customActive;
        else
            button.background.sprite = defaultTabActive;

        int index = button.transform.GetSiblingIndex();
        for (int i = 0; i < objectsToSwap.Count; i++)
        {
            objectsToSwap[i].SetActive(i == index);
        }
    }
}
