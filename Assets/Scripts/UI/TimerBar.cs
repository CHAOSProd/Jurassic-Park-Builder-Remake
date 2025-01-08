using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TimerBar : MonoBehaviour
{
    [SerializeField] private Transform _barForeground;
    [SerializeField] private float remainingTimeInSeconds;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Button resetButton;

    private float _startX;
    private float _deltaX;
    private float _deltaScaleX;

    private void Awake()
    {
        Sprite sprite = _barForeground.GetComponent<SpriteRenderer>().sprite;
        _deltaScaleX = _barForeground.localScale.x;
        _startX = _barForeground.localPosition.x - sprite.bounds.size.x * _barForeground.localScale.x * 0.5f;
        _deltaX = _barForeground.localPosition.x - _startX;

        if (resetButton != null)
        {
            resetButton.onClick.AddListener(ResetTimeAndReduceScore);
        }
    }

    public void SetProgress(float value)
    {
        value = Mathf.Clamp01(value);

        Vector3 scale = _barForeground.localScale;
        scale.x = _deltaScaleX * value;
        _barForeground.localScale = scale;

        Vector3 position = _barForeground.localPosition;
        position.x = _startX + _deltaX * value;
        _barForeground.localPosition = position;
    }

    private IEnumerator FillOverTimeEnumerator(float seconds, System.Action onFinished)
    {
        float elapsed = 0;
        remainingTimeInSeconds = seconds;

        while (elapsed < seconds)
        {
            elapsed += Time.deltaTime;
            remainingTimeInSeconds = Mathf.Max(seconds - elapsed, 0);
            SetProgress(elapsed / seconds);
            yield return new WaitForEndOfFrame();
        }

        remainingTimeInSeconds = 0;
        onFinished?.Invoke();
    }

    private IEnumerator FillOverIntervalEnumerator(float seconds, float interval, System.Action onInterval, System.Action onFinished, float startTime)
    {
        float elapsed = startTime;
        int amount = 1;
        remainingTimeInSeconds = seconds - startTime;

        while (elapsed < seconds)
        {
            elapsed += Time.deltaTime;
            remainingTimeInSeconds = Mathf.Max(seconds - elapsed, 0);
            SetProgress(elapsed / seconds);

            if (elapsed >= interval * amount)
            {
                onInterval?.Invoke();
                amount++;
            }

            yield return new WaitForEndOfFrame();
        }

        remainingTimeInSeconds = 0;
        onFinished?.Invoke();
    }

    public void FillOverTime(float seconds, System.Action onFinished)
    {
        StartCoroutine(FillOverTimeEnumerator(seconds, onFinished));
    }

    public void FillOverInterval(float seconds, float interval, System.Action onInterval, System.Action onFinished, float startTime = 0f)
    {
        StartCoroutine(FillOverIntervalEnumerator(seconds, interval, onInterval, onFinished, startTime));
    }

    public void ResetTimeAndReduceScore()
    {
        remainingTimeInSeconds = 0;
        SetProgress(0);

        if (scoreText != null)
        {
            int currentScore = int.Parse(scoreText.text);
            currentScore = Mathf.Max(0, currentScore - 1000);
            scoreText.text = currentScore.ToString();
        }
    }
}
