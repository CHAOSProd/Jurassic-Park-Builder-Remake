using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeedUp : MonoBehaviour
{
    [Header("Time Display Fields")]
    [SerializeField] private TMP_Text hoursField;
    [SerializeField] private TMP_Text minutesField;
    [SerializeField] private TMP_Text secondsField;

    [Header("UI Elements")]
    [SerializeField] private Button speedUpButton;

    private DebrisObject currentDebris; // Currently selected debris

    private void Start()
    {
        if (speedUpButton != null)
        {
            speedUpButton.onClick.AddListener(SpeedUpRemoval);
        }
    }

    private void Update()
    {
        // Detect the currently selected debris object
        DebrisObject selectedDebris = SelectablesManager.Instance.CurrentSelectable as DebrisObject;

        if (selectedDebris != currentDebris)
        {
            // If the selected debris has changed, update tracking
            currentDebris = selectedDebris;
        }

        // Update the timer display for the current debris
        if (currentDebris != null && currentDebris.removing)
        {
            int remainingTime = currentDebris.GetRemainingTime();

            if (remainingTime > 0)
            {
                UpdateTimeDisplay(remainingTime); // Refresh display
            }
            else
            {
                ClearTimeDisplay(); // Clear display if time runs out
            }
        }
        else
        {
            ClearTimeDisplay(); // Clear display if no debris is selected
        }
    }

    /// <summary>
    /// Speeds up the removal of the selected debris by setting elapsed time to match remove time.
    /// </summary>
    private void SpeedUpRemoval()
    {
        if (currentDebris == null || !currentDebris.removing) return;

        // Set elapsed time to remove time to trigger immediate removal
        if (currentDebris._data.Progress != null)
        {
            currentDebris._data.Progress.ElapsedTime = currentDebris._removeTime;
        }

        // Call OnRemovalComplete directly to finalize removal
        currentDebris.Invoke("OnRemovalComplete", 0);

        // Clear the timer display
        ClearTimeDisplay();
    }

    /// <summary>
    /// Updates the TMP fields to display time in HH:MM:SS format.
    /// </summary>
    /// <param name="totalSeconds">Total seconds to be displayed.</param>
    private void UpdateTimeDisplay(int totalSeconds)
    {
        int hours = totalSeconds / 3600;
        int minutes = (totalSeconds % 3600) / 60;
        int seconds = totalSeconds % 60;

        if (hoursField != null) hoursField.text = hours.ToString("00");
        if (minutesField != null) minutesField.text = minutes.ToString("00");
        if (secondsField != null) secondsField.text = seconds.ToString("00");
    }

    /// <summary>
    /// Clears the time display when no debris is selected.
    /// </summary>
    private void ClearTimeDisplay()
    {
        if (hoursField != null) hoursField.text = "00";
        if (minutesField != null) minutesField.text = "00";
        if (secondsField != null) secondsField.text = "00";
    }
}







