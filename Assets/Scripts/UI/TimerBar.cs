using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerBar : MonoBehaviour
{
    [SerializeField] private Transform _barForeground;

    private float _startX;
    private float _deltaX;
    private float _deltaScaleX;
    private void Awake()
    {
        _deltaScaleX = _barForeground.localScale.x;
        _startX = _barForeground.localPosition.x - _deltaScaleX * .5f;
        _deltaX = _barForeground.localPosition.x - _startX;
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

    private IEnumerator FillOverTimeEnumerator(float seconds, VoidCallback onFinished)
    {
        float elapsed = 0;
        while (elapsed < seconds)
        {
            elapsed += Time.deltaTime;
            SetProgress(elapsed / seconds);
            yield return new WaitForEndOfFrame();
        }
        onFinished.Invoke();
    }

    private IEnumerator FillOverIntervalEnumerator(float seconds, float interval, VoidCallback onInterval, VoidCallback onFinished, float startTime)
    {
        float elapsed = startTime;
        int amount = 1;
        while (elapsed < seconds)
        {
            elapsed += Time.deltaTime;
            SetProgress(elapsed / seconds);

            if(elapsed >= interval * amount)
            {
                onInterval.Invoke();
                amount++;
            }

            yield return new WaitForEndOfFrame();
        }
        onFinished.Invoke();
    }

    public void FillOverTime(float seconds, VoidCallback onFinished)
    {
        StartCoroutine(FillOverTimeEnumerator(seconds, onFinished));
    }

    public void FillOverInterval(float seconds, float interval, VoidCallback onInterval, VoidCallback onFinished, float startTime = 0f)
    {
        StartCoroutine(FillOverIntervalEnumerator(seconds, interval, onInterval, onFinished,startTime));
    }
}
