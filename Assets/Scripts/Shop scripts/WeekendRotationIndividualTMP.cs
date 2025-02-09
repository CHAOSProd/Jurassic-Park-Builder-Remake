using System;
using UnityEngine;
using TMPro;

public class WeekendRotationIndividualTMP : MonoBehaviour
{
    [Header("Rotating Objects")]
    [Tooltip("Drag the objects to rotate here. Each should have an AnimalToggle component and at least one TextMeshProUGUI child named with 'Countdown' in its name.")]
    [SerializeField] private GameObject[] rotatingObjects;

    // Baseline date for week rotation calculations (adjust as needed)
    private DateTime baselineDate = new DateTime(2022, 1, 1);

    private void Start()
    {
        UpdateAvailability();
    }

    /// <summary>
    /// Checks the day of the week, determines which object should be available (unless purchased),
    /// and updates each object's designated TextMeshProUGUI field(s) with the proper countdown message.
    /// </summary>
    public void UpdateAvailability()
    {
        DateTime now = DateTime.Now;
        DayOfWeek currentDay = now.DayOfWeek;

        // Check if today is Friday, Saturday, or Sunday.
        bool isWeekend = (currentDay == DayOfWeek.Friday ||
                          currentDay == DayOfWeek.Saturday ||
                          currentDay == DayOfWeek.Sunday);

        // Determine the countdown message (only relevant on weekend days)
        string countdownMessage = "";
        if (isWeekend)
        {
            if (currentDay == DayOfWeek.Friday)
            {
                countdownMessage = "3 day(s) left";
            }
            else if (currentDay == DayOfWeek.Saturday)
            {
                countdownMessage = "2 day(s) left";
            }
            else if (currentDay == DayOfWeek.Sunday)
            {
                countdownMessage = "last day";
            }
        }

        // Determine which object should be available for this week (if any)
        int selectedIndex = 0;
        if (rotatingObjects != null && rotatingObjects.Length > 0)
        {
            TimeSpan span = now.Date - baselineDate.Date;
            int weekNumber = span.Days / 7; // integer division for full weeks passed
            selectedIndex = weekNumber % rotatingObjects.Length;
        }

        // Loop through each rotating object
        for (int i = 0; i < rotatingObjects.Length; i++)
        {
            GameObject obj = rotatingObjects[i];

            // Check if the object was purchased using the AnimalToggle component.
            AnimalToggle toggle = obj.GetComponent<AnimalToggle>();
            bool purchased = (toggle != null && toggle.Purchased);

            // Decide whether the object should be active:
            // - Purchased objects remain active at all times.
            // - Otherwise, only the selected object is active during the weekend.
            bool shouldBeActive = purchased || (isWeekend && i == selectedIndex);
            obj.SetActive(shouldBeActive);

            // Find all TextMeshProUGUI components in the object's children.
            TextMeshProUGUI[] tmpTexts = obj.GetComponentsInChildren<TextMeshProUGUI>();

            foreach (TextMeshProUGUI tmpText in tmpTexts)
            {
                // Only update TMP fields that have "Countdown" in their GameObject name.
                if (tmpText.gameObject.name.Contains("Countdown"))
                {
                    // If the object is purchased, leave the text empty.
                    // Otherwise, if the object is active, show the countdown message.
                    // Otherwise, clear the text.
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
    }
}

