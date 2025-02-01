using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventsListener : MonoBehaviour
{
    [SerializeField] private UnityEvent _eventOnAnimationEnded;
    public bool IsAnimationEnded { get; set; } = true;
    public bool IsEatAnimationEnded { get; set; } = true;

    public void OnAnimationEnded()
    {
        IsAnimationEnded = true;
        _eventOnAnimationEnded?.Invoke();
    }
    public void OnEatAnimationEnded()
    {
        IsEatAnimationEnded = true;
        _eventOnAnimationEnded?.Invoke();
    }

    public void OnAnimationStarted()
    {
        IsAnimationEnded = false;
    }
    public void OnEatAnimationStarted()
    {
        IsEatAnimationEnded = false;
    }
}