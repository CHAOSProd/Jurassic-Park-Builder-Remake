using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config : MonoBehaviour
{
    public enum ProjectConfig
    {
        Release,
        Debug
    }

    public ProjectConfig selectedreleasetype;

    void Start()
    {
        Debug.Log($"Project is configured as {Enum.GetName(typeof(ProjectConfig),selectedreleasetype)}");       
    }

    void Awake()
    {
        GameObject dontdestroy = new GameObject("CheatMenu");
        switch (selectedreleasetype)
        {
            case ProjectConfig.Release:
                return;
            case ProjectConfig.Debug:
                dontdestroy.AddComponent<CheatMenu>();
                DontDestroyOnLoad(dontdestroy);
                return;
            default:
                break;
        }       
    }
}
