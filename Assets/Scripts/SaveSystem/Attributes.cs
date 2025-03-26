using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Attributes
{
    private static Dictionary<string, object> _attributes = new Dictionary<string, object>();

    public static void SetAttribute<T>(string key, T value) where T : struct
    {
        if(_attributes.ContainsKey(key))
        {
            _attributes[key] = value;
            return;
        }

        _attributes.Add(key, value);
    }

    public static void SetInt(string key, int value)
    {
        if (_attributes.ContainsKey(key))
        {
            _attributes[key] = value;
            return;
        }

        _attributes.Add(key, value);
    }

    public static void SetFloat(string key, float value)
    {   
        if (_attributes.ContainsKey(key))
        {
            _attributes[key] = value;
            return;
        }

        _attributes.Add(key, value);
    }

    public static void SetBool(string key, bool value)
    {   
        if (_attributes.ContainsKey(key))
        {
            _attributes[key] = value;
            return;
        }

        _attributes.Add(key, value);
    }

    public static bool TryGetAttribute<T>(string key, out T attribute) where T : struct
    {
        if(_attributes.ContainsKey(key))
        {
            attribute = (T)_attributes[key];
            return true;
        }
        attribute = default;
        return false;
    }
    public static T GetAttribute<T>(string key) where T : struct
    {
        return (T)_attributes[key];
    }

    public static bool GetBool(string key)
    {
        return Convert.ToBoolean(_attributes[key]);
    }

    public static float GetFloat(string key)
    {
        return float.Parse(_attributes[key].ToString());
    }

    public static int GetInt(string key)
    {
        return Convert.ToInt32(_attributes[key]);
    }

    public static T GetAttribute<T>(string key, T defaultValue) where T : struct
    {
        if(_attributes.ContainsKey(key))
            return (T)_attributes[key];

        return defaultValue;
    }

    public static bool GetBool(string key, bool defaultValue)
    {
        if(_attributes.ContainsKey(key))
            return Convert.ToBoolean(_attributes[key]);

        return defaultValue;
    }

    public static float GetFloat(string key, float defaultValue)
    {
        if(_attributes.ContainsKey(key))
            return float.Parse(_attributes[key].ToString());

        return defaultValue;
    }

    public static int GetInt(string key, int defaultValue)
    {
        if(_attributes.ContainsKey(key))
            return Convert.ToInt32(_attributes[key]);

        return defaultValue;
    }

    public static bool HaveKey(string key)
    {
        return _attributes.ContainsKey(key);
    }

    public static void SetAttributes(Dictionary<string, object> attributes)
    {
        if (attributes == null) return;

        Dictionary<string, object> clone = new Dictionary<string, object>();
        foreach (KeyValuePair<string, object> kvp in attributes)
        {
            clone.Add(kvp.Key, kvp.Value);
        }
        _attributes = clone;
    }
    public static Dictionary<string, object> Export()
    {
        Dictionary<string, object> clone = new Dictionary<string, object>();
        foreach(KeyValuePair<string, object> kvp in _attributes)
        {
            clone.Add(kvp.Key, kvp.Value);
        }

        return clone;
    }
}
