using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeedUp : MonoBehaviour
{
    [Header("Time Display Fields")]
    [SerializeField] private TMP_Text hoursField;
    [SerializeField] private TMP_Text minutesField;
    [SerializeField] private TMP_Text secondsField;
    [SerializeField] private TMP_Text moneyField;

    [Header("UI Elements")]
    [SerializeField] private Button speedUpButton;

    private Selectable currentSelectable;
    private TimerBar timerBar;

    private void Start()
    {
        if (speedUpButton != null)
        {
            speedUpButton.onClick.AddListener(SpeedUpRemoval);
        }
    }

    private void Update()
    {
        currentSelectable = SelectablesManager.Instance.CurrentSelectable;

        // Update the timer display for the current debris
        if (currentSelectable != null)
        {
            timerBar = currentSelectable.gameObject.GetComponentInChildren<TimerBar>(true);

            if (currentSelectable is Paddock paddock && paddock.is_hatching)
            {
                timerBar = paddock.hatchingScript.GetComponentInChildren<TimerBar>(true);
            }

            if (timerBar == null) return;

            int remainingTime = timerBar.GetRemainingTime();
            int requiredMoney = GetRequiredMoney();

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
        //add an hour to the timer
        int requiredMoney = GetRequiredMoney();

        if (CurrencySystem.Instance.HasEnoughCurrency(CurrencyType.Bucks, requiredMoney))
        {
            CurrencySystem.Instance.AddCurrency(new CurrencyChangeGameEvent(-requiredMoney, CurrencyType.Bucks));
            timerBar.endTimer = true;
            SelectablesManager.Instance.UnselectAll();
        }
    }

    private int GetRequiredMoney()
    {
        int remainingTime = timerBar.GetRemainingTime();

        // Calculate the required money (1 money per hour, rounded up)
        int requiredMoney = Mathf.CeilToInt(remainingTime / 3600f);

        return requiredMoney;
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
        if (moneyField != null) moneyField.text = GetRequiredMoney().ToString("0");
    }

    /// <summary>
    /// Clears the time display when no debris is selected.
    /// </summary>
    private void ClearTimeDisplay()
    {
        if (hoursField != null) hoursField.text = "00";
        if (minutesField != null) minutesField.text = "00";
        if (secondsField != null) secondsField.text = "00";
        if (moneyField != null) moneyField.text = "0";
    }
}







