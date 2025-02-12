using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpeedUp : MonoBehaviour
{
    [Header("Time Display Fields")]
    [SerializeField] private TMP_Text hoursField;
    [SerializeField] private TMP_Text minutesField;
    [SerializeField] private TMP_Text secondsField;
    [SerializeField] private TMP_Text moneyField;

    [Header("UI Elements")]
    [SerializeField] private Button speedUpButton;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource speedUpSound;

    private Selectable currentSelectable;
    private TimerBar timerBar;

    private void Start()
    {
        if (speedUpButton != null)
        {
            speedUpButton.onClick.AddListener(SpeedUpRemoval);
            UIManager.Instance.ChangeCameraPanningStatus(false);
        }
    }

    private IEnumerator HidePanelWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
        SelectablesManager.Instance.UnselectAll();
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
                StartCoroutine(HidePanelWithDelay(1.1f));
            }
        }
        else
        {
            ClearTimeDisplay(); // Clear display if no debris is selected
        }
    }

    /// <summary>
    /// Speeds up the removal of the selected debris or placeable object.
    /// Plays a sound only if the object is a placeable object.
    /// </summary>
    private void SpeedUpRemoval()
    {
        int requiredMoney = GetRequiredMoney();

        if (!CurrencySystem.Instance.HasEnoughCurrency(CurrencyType.Bucks, requiredMoney))
        {
            CurrencySystem.Instance._notEnoughBucksPanel.ShowNotEnoughCoinsPanel(requiredMoney);
            return;
        }

        // Deduct currency
        CurrencySystem.Instance.AddCurrency(new CurrencyChangeGameEvent(-requiredMoney, CurrencyType.Bucks));
        timerBar.endTimer = true;

        // Check if the current selectable is a PlaceableObject
        if (currentSelectable != null && currentSelectable.GetComponent<PlaceableObject>() != null)
        {
            PlaySpeedUpSound();
        }

        SelectablesManager.Instance.UnselectAll();
        UIManager.Instance.ChangeCameraPanningStatus(true);
    }

    /// <summary>
    /// Plays the speed-up sound effect if AudioSource is assigned.
    /// </summary>
    private void PlaySpeedUpSound()
    {
        if (speedUpSound != null)
        {
            speedUpSound.Play();
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










