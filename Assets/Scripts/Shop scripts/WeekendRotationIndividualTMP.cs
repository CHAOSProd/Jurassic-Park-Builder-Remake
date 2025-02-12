using System;
using UnityEngine;
using TMPro;

public class WeekendRotationIndividualTMP : MonoBehaviour
{
    [Header("Rotating Objects")]
    [Tooltip("Drag the objects to rotate here. Each should have an AnimalToggle component and at least one TextMeshProUGUI child named with 'Countdown' in its name.")]
    [SerializeField] private GameObject[] rotatingObjects;

    // Set a baseline weekend start date that is a Friday.
    // This date determines the cycle for the weekend rotation.
    // For example, January 7, 2022 is a Friday.
    private DateTime baselineWeekend = new DateTime(2022, 1, 7);

    private void Start()
    {
        UpdateAvailability();
    }

    /// <summary>
    /// Updates the active status and countdown text of each rotating object.
    /// During the weekend (Friday–Sunday) only one (or any purchased) object is active.
    /// The active object is determined based on the current weekend (which starts on Friday).
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

        // Only calculate a selected index if today is a weekend day.
        int selectedIndex = -1;
        if (rotatingObjects != null && rotatingObjects.Length > 0 && isWeekend)
        {
            // Determine the start of this weekend (i.e. the most recent Friday).
            // If today is Friday, it is the weekend start.
            int daysSinceFriday = (int)currentDay - (int)DayOfWeek.Friday;
            if (daysSinceFriday < 0)
            {
                daysSinceFriday += 7;
            }
            DateTime thisWeekendStart = now.Date.AddDays(-daysSinceFriday);

            // Calculate how many whole weeks have passed since our baseline Friday.
            int weekendIndex = (int)((thisWeekendStart - baselineWeekend).TotalDays / 7);

            // Use modulo to cycle through the array of objects.
            selectedIndex = weekendIndex % rotatingObjects.Length;
            if (selectedIndex < 0)
            {
                selectedIndex += rotatingObjects.Length;
            }
        }

        // Loop through each rotating object.
        for (int i = 0; i < rotatingObjects.Length; i++)
        {
            GameObject obj = rotatingObjects[i];

            // Check if this object has been purchased.
            AnimalToggle toggle = obj.GetComponent<AnimalToggle>();
            bool purchased = (toggle != null && toggle.Purchased);

            // Determine if the object should be active:
            // - Purchased objects remain active at all times.
            // - Otherwise, only the object matching the weekend index is active during the weekend.
            bool shouldBeActive = purchased || (isWeekend && i == selectedIndex);
            obj.SetActive(shouldBeActive);

            // Find all TextMeshProUGUI components in the object's children.
            TextMeshProUGUI[] tmpTexts = obj.GetComponentsInChildren<TextMeshProUGUI>();

            foreach (TextMeshProUGUI tmpText in tmpTexts)
            {
                // Only update text components whose GameObject name contains "Countdown".
                if (tmpText.gameObject.name.Contains("Countdown"))
                {
                    if (purchased)
                    {
                        // Purchased objects show no countdown.
                        tmpText.text = "";
                    }
                    else if (shouldBeActive)
                    {
                        // Active objects show the appropriate countdown message.
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


