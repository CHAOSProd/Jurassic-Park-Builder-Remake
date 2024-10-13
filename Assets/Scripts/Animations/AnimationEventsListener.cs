using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventsListener : MonoBehaviour
{
    [SerializeField] private UnityEvent _eventOnAnimationEnded;
    public bool IsAnimationEnded { get; set; } = true;

    public void OnAnimationEnded()
    {
        IsAnimationEnded = true;
        _eventOnAnimationEnded?.Invoke();
    }

    public void OnAnimationStarted()
    {
        IsAnimationEnded = false;
    }
}
