using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void VoidCallback();
public class UnityTimer : Singleton<UnityTimer>
{
    public void Wait(float time, VoidCallback onFinished)
    {
        static IEnumerator WaitForTime(float time, VoidCallback onFinished)
        {
            yield return new WaitForSeconds(time);
            onFinished.Invoke();
        }

        StartCoroutine(WaitForTime(time, onFinished));
    }

    public void Tick(float time, float tick, VoidCallback onTick, VoidCallback onFinished = null)
    {
        static IEnumerator TickForTime(float time, float tick, VoidCallback onTick, VoidCallback onFinished = null)
        {
            if (tick > time) yield return null;
            float elapsed = 0;
            while(elapsed < time)
            {
                yield return new WaitForSeconds(tick);
                elapsed += tick;
                onTick.Invoke();
            }
            onFinished?.Invoke();
        }

        StartCoroutine(TickForTime(time, tick, onTick, onFinished));
    }

    public void Interval(float time, float interval, VoidCallback onTick, VoidCallback onIntervalTick, VoidCallback onFinished = null)
    {
        static IEnumerator TickForTime(float time, float interval, VoidCallback onTick, VoidCallback onIntervalTick, VoidCallback onFinished = null)
        {
            float elapsed = 0;
            int intervalCount = 1;
            while (elapsed < time)
            {
                onTick.Invoke();

                if(elapsed > interval * intervalCount)
                {
                    onIntervalTick.Invoke();
                    intervalCount++;
                }

                elapsed += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            onFinished?.Invoke();
        }

        StartCoroutine(TickForTime(time,interval, onTick, onIntervalTick, onFinished));
    }
}
