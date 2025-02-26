using System;
using UnityEngine;
using TMPro;

public class WeekendRotationIndividualTMP : MonoBehaviour
{
    [Header("Rotating Objects")]
    [Tooltip("Drag the objects here. Each should have an AnimalToggle component and at least one TextMeshProUGUI child whose name contains 'Countdown'.")]
    [SerializeField] private GameObject[] rotatingObjects;

    [Header("Paired Panels")]
    [Tooltip("Drag the panels paired with each rotating object.")]
    [SerializeField] private GameObject[] pairedPanels;

    [Header("Parent Panel")]
    [Tooltip("The parent panel that holds the paired panels. It will be active if any child panel is active.")]
    [SerializeField] private GameObject parentPanel;

    private DateTime baselineWeekend = new DateTime(2022, 1, 7);
    private const string weekendObjectKey = "WeekendObjectIndex";
    private const string weekendPanelShownKey = "PanelShownWeekend";

    private void Start()
    {
        UpdateAvailability();
    }

    public void UpdateAvailability()
    {
        DateTime now = DateTime.Now;
        DayOfWeek currentDay = now.DayOfWeek;
        bool isWeekend = (currentDay == DayOfWeek.Friday || currentDay == DayOfWeek.Saturday || currentDay == DayOfWeek.Sunday);

        string countdownMessage = "";
        if (isWeekend)
        {
            switch (currentDay)
            {
                case DayOfWeek.Friday: countdownMessage = "3 day(s) left"; break;
                case DayOfWeek.Saturday: countdownMessage = "2 day(s) left"; break;
                case DayOfWeek.Sunday: countdownMessage = "Last day"; break;
            }
        }

        // Determine this weekend's Friday date
        int daysSinceFriday = (int)currentDay - (int)DayOfWeek.Friday;
        if (daysSinceFriday < 0) daysSinceFriday += 7;
        DateTime thisWeekendStart = now.Date.AddDays(-daysSinceFriday);
        string thisWeekendStr = thisWeekendStart.ToString("yyyyMMdd");

        int selectedIndex = -1;
        if (isWeekend && PlayerPrefs.HasKey(weekendObjectKey))
        {
            string savedWeekend = PlayerPrefs.GetString(weekendObjectKey);
            if (savedWeekend.StartsWith(thisWeekendStr))
            {
                selectedIndex = int.Parse(savedWeekend.Substring(8));
            }
        }

        // If no valid selection was found, determine a new object
        if (isWeekend && selectedIndex == -1)
        {
            int weekendIndex = (int)((thisWeekendStart - baselineWeekend).TotalDays / 7);
            selectedIndex = weekendIndex % rotatingObjects.Length;
            PlayerPrefs.SetString(weekendObjectKey, thisWeekendStr + selectedIndex);
            PlayerPrefs.Save();
        }

        bool anyPanelActivated = false;
        bool panelAlreadyShown = PlayerPrefs.HasKey(weekendPanelShownKey) && PlayerPrefs.GetString(weekendPanelShownKey) == thisWeekendStr;

        for (int i = 0; i < rotatingObjects.Length; i++)
        {
            GameObject obj = rotatingObjects[i];
            AnimalToggle toggle = obj.GetComponent<AnimalToggle>();
            bool purchased = (toggle != null && toggle.Purchased);

            bool shouldBeActive = purchased || (isWeekend && i == selectedIndex);
            obj.SetActive(shouldBeActive);

            if (pairedPanels != null && i < pairedPanels.Length)
            {
                bool shouldShowPanel = !purchased && i == selectedIndex && !panelAlreadyShown;
                pairedPanels[i].SetActive(shouldShowPanel);
                if (shouldShowPanel)
                {
                    anyPanelActivated = true;
                    PlayerPrefs.SetString(weekendPanelShownKey, thisWeekendStr);
                    PlayerPrefs.Save();
                }
            }

            TextMeshProUGUI[] tmpTexts = obj.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (TextMeshProUGUI tmpText in tmpTexts)
            {
                if (tmpText.gameObject.name.Contains("Countdown"))
                {
                    tmpText.text = purchased ? "" : shouldBeActive ? countdownMessage : "";
                }
            }
        }

        if (parentPanel != null)
        {
            parentPanel.SetActive(anyPanelActivated);
        }
    }
}



