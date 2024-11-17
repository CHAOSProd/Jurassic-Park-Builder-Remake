using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;
using System.ComponentModel;

public class EventManager : Singleton<EventManager>
{
    public delegate bool EventDelegate<T>(T eventData);

    private Dictionary<Type, List<Delegate>> _events = new();
    public void AddListener<T>(EventDelegate<T> listener)
    {
        if(!_events.ContainsKey(typeof(T)))
        {
            _events.Add(typeof(T), new List<Delegate>());
        }
        _events[typeof(T)].Add(listener);
    }
    public void RemoveListener<T>(EventDelegate<T> listener)
    {
        if(_events.ContainsKey(typeof(T)))
        {
            bool res = _events[typeof(T)].Remove(listener);
            if (!res)
            {
                Debug.LogWarning("Listener coulnd't be removed since it isn't in list anyways");
            }
            else if (_events[typeof(T)].Count == 0)
            {
                _events.Remove(typeof(T));
            }
        }
    }
    public bool TriggerEvent<T>(T eventData)
    {
        if(_events.ContainsKey(typeof(T)))
        {
            foreach(Delegate d in _events[typeof(T)])
            {
                EventDelegate<T> eventDelegate = d as EventDelegate<T>;
                if(!eventDelegate.Invoke(eventData)) return false;
            }
            return true;
        }
        return false;
    }
}