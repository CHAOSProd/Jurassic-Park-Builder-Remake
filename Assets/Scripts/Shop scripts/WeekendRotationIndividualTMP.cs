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

    // A baseline Friday used for weekend rotation calculations.
    private DateTime baselineWeekend = new DateTime(2022, 1, 7);

    // Key used to record that the panel has been shown this weekend.
    private const string weekendShownKey = "LastWeekendShown";

    private void Start()
    {
        UpdateAvailability();
    }

    /// <summary>
    /// Updates each rotating object and its paired panel.
    /// During the weekend (Friday–Sunday), one (non-purchased) object is chosen
    /// to show its panel—but only on the first entry that weekend.
    /// Purchased objects always remain active.
    /// </summary>
    public void UpdateAvailability()
    {
        DateTime now = DateTime.Now;
        DayOfWeek currentDay = now.DayOfWeek;

        // Check if today is Friday, Saturday, or Sunday.
        bool isWeekend = (currentDay == DayOfWeek.Friday ||
                          currentDay == DayOfWeek.Saturday ||
                          currentDay == DayOfWeek.Sunday);

        // Prepare the countdown message for weekend days.
        string countdownMessage = "";
        if (isWeekend)
        {
            switch (currentDay)
            {
                case DayOfWeek.Friday:
                    countdownMessage = "3 day(s) left";
                    break;
                case DayOfWeek.Saturday:
                    countdownMessage = "2 day(s) left";
                    break;
                case DayOfWeek.Sunday:
                    countdownMessage = "last day";
                    break;
            }
        }

        // Determine if this is the first time this weekend that the panel is shown.
        bool firstTimeThisWeekend = true;
        string thisWeekendStr = "";
        DateTime thisWeekendStart = now;
        if (isWeekend)
        {
            // Determine the most recent Friday (start of the weekend).
            int daysSinceFriday = (int)currentDay - (int)DayOfWeek.Friday;
            if (daysSinceFriday < 0)
            {
                daysSinceFriday += 7;
            }
            thisWeekendStart = now.Date.AddDays(-daysSinceFriday);
            thisWeekendStr = thisWeekendStart.ToString("yyyyMMdd");

            // Check PlayerPrefs to see if we've already shown the panel this weekend.
            if (PlayerPrefs.HasKey(weekendShownKey))
            {
                string lastShownWeekend = PlayerPrefs.GetString(weekendShownKey);
                if (lastShownWeekend == thisWeekendStr)
                {
                    firstTimeThisWeekend = false;
                }
            }
        }

        // Calculate which rotating object should be active this weekend.
        int selectedIndex = -1;
        if (rotatingObjects != null && rotatingObjects.Length > 0 && isWeekend)
        {
            int weekendIndex = (int)((thisWeekendStart - baselineWeekend).TotalDays / 7);
            selectedIndex = weekendIndex % rotatingObjects.Length;
            if (selectedIndex < 0)
            {
                selectedIndex += rotatingObjects.Length;
            }
        }

        // Flag to track if any paired panel is activated (to drive the parent panel).
        bool anyPanelActivated = false;

        // Loop through each rotating object.
        for (int i = 0; i < rotatingObjects.Length; i++)
        {
            GameObject obj = rotatingObjects[i];

            // Check if this object has been purchased.
            AnimalToggle toggle = obj.GetComponent<AnimalToggle>();
            bool purchased = (toggle != null && toggle.Purchased);

            bool shouldBeActive = false;
            if (purchased)
            {
                // Purchased objects remain active regardless.
                shouldBeActive = true;
            }
            else if (isWeekend && i == selectedIndex && firstTimeThisWeekend)
            {
                // Only the designated object is active on the weekend—and only the first time.
                shouldBeActive = true;
            }
            else
            {
                shouldBeActive = false;
            }

            // Set the active state for the rotating object.
            obj.SetActive(shouldBeActive);

            // Toggle the corresponding panel if it exists.
            if (pairedPanels != null && i < pairedPanels.Length)
            {
                pairedPanels[i].SetActive(shouldBeActive);
                if (shouldBeActive)
                {
                    anyPanelActivated = true;
                }
            }

            // Update any child TextMeshProUGUI components whose name contains "Countdown".
            TextMeshProUGUI[] tmpTexts = obj.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (TextMeshProUGUI tmpText in tmpTexts)
            {
                if (tmpText.gameObject.name.Contains("Countdown"))
                {
                    if (purchased)
                    {
                        tmpText.text = "";
                    }
                    else if (shouldBeActive)
                    {
                        tmpText.text = countdownMessage;
                    }
                    else
                    {
                        tmpText.text = "";
                    }
                }
            }
        }

        // If it's the weekend, and this is the first time panels are being activated,
        // record that they've been shown so subsequent entries this weekend won't re-show them.
        if (isWeekend && firstTimeThisWeekend && anyPanelActivated)
        {
            PlayerPrefs.SetString(weekendShownKey, thisWeekendStr);
            PlayerPrefs.Save();
        }

        // Activate the parent panel if any child panel is active.
        if (parentPanel != null)
        {
            parentPanel.SetActive(anyPanelActivated);
        }
    }
}


