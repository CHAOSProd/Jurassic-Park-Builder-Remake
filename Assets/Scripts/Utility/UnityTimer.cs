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
}
