using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class HatchingTimer : MonoBehaviour
{
    [Header("Hatching Values")]
    [SerializeField] private int _hatchDuration = 10;
    [SerializeField] private int HatchXP = 16;

    [Header("Objects")]
    [SerializeField] private GameObject _eggVisual;
    [SerializeField] private GameObject dino;
    [SerializeField] private GameObject _xpNotification;
    [SerializeField] private GameObject _tapVFX;
    [SerializeField] private GameObject _xpCounter;
    [SerializeField] private GameObject _construction;
    [SerializeField] private GameObject _paddockVisual;

    [Header("Scripts")]
    [SerializeField] private Paddock paddockScript;
    [SerializeField] private MoneyCountDisplayer _xpCountDisplayer;

    [Header("UI")]
    [SerializeField] private GameObject _timerBarPrefab;

    private TimerBar _timerBarInstance;
    private HatchingData data;

    private void Start()
    {
        if (!paddockScript.is_hatching && !paddockScript.hatching_completed)
        {
            data = new HatchingData();
            StartHatchingTimer();
        }
    }

    public void InitializeTriceratops()
    {
        data = new HatchingData();

        data.isHatching = false;
        data.HatchingFinished = true;
        data.DinoName = _paddockVisual.GetComponent<PaddockInfo>()._dinosaurName;
    }

    public void Load(HatchingData hatchingData)
    { 
        data = hatchingData;

        if (data.HatchingFinished)
        {
            _construction.SetActive(false);
            dino.SetActive(true);
            _eggVisual.SetActive(false);
            gameObject.SetActive(false);
            _paddockVisual.SetActive(true);
        }
        else if (data.HatchingProgress != null)
        {
            _construction.SetActive(false);
            dino.SetActive(false);
            _eggVisual.SetActive(true);
            gameObject.SetActive(true);
            _paddockVisual.SetActive(true);

            int newTime = (int)Math.Floor((DateTime.Now - hatchingData.HatchingProgress.LastTick).TotalSeconds) + hatchingData.HatchingProgress.ElapsedTime;

            if (newTime >= _hatchDuration)
            {
                paddockScript.is_hatching = false;
                paddockScript.hatching_completed = true;

                OnHatchComplete();
            }
            else
            {
                data.HatchingProgress = new ProgressData(_hatchDuration - newTime, DateTime.Now);

                paddockScript.is_hatching = true;
                paddockScript.hatching_completed = false;

                _timerBarInstance = Instantiate(_timerBarPrefab, transform).GetComponent<TimerBar>();
                _timerBarInstance.transform.position = _eggVisual.transform.position + 2.5f * _eggVisual.transform.lossyScale.y * Vector3.down;
                _timerBarInstance.transform.localScale = new Vector3(1f / transform.localScale.x, 1f / transform.localScale.y);

                _timerBarInstance.FillOverInterval(_hatchDuration, 1, UpdateProgress, OnHatchComplete, newTime);
            }
        }
    }

    private void StartHatchingTimer()
    {
        CreateTimer();
        
        _timerBarInstance.FillOverInterval(_hatchDuration, 1, UpdateProgress, OnHatchComplete);
        
        paddockScript.is_hatching = true;
        paddockScript.hatching_completed = false;
    }

    private void CreateTimer()
    {
        data.HatchingProgress = new ProgressData(0, DateTime.Now);
        data.HatchingFinished = false;
        data.isHatching = true;
        data.DinoName = _paddockVisual.GetComponent<PaddockInfo>()._dinosaurName;

        SaveManager.Instance.SaveData.HatchingData.Add(data);

        _timerBarInstance = Instantiate(_timerBarPrefab, transform).GetComponent<TimerBar>();
        _timerBarInstance.transform.position = _eggVisual.transform.position + 2.5f * _eggVisual.transform.lossyScale.y * Vector3.down;
        _timerBarInstance.transform.localScale = new Vector3(1f / transform.localScale.x, 1f / transform.localScale.y);
    }

    private void UpdateProgress()
    {
        data.HatchingProgress.ElapsedTime += 1;
        data.HatchingProgress.LastTick = DateTime.Now;
    }

    private void OnHatchComplete()
    {
        _xpNotification.SetActive(true);
        _xpNotification.transform.position = _eggVisual.transform.position + 4f * _eggVisual.transform.lossyScale.y * Vector3.up;

        if (_timerBarInstance != null)
        {
            Destroy(_timerBarInstance.gameObject);
        }

        Debug.Log("wow");

        paddockScript.is_hatching = false;
        paddockScript.hatching_completed = true;
    }

    public void OnPaddockClicked()
    {
        dino.SetActive(true);
        _eggVisual.SetActive(false);
        _xpNotification.SetActive(false);
        _tapVFX.SetActive(true);
        _xpCounter.SetActive(true);

        data.HatchingFinished = true;
        data.isHatching = false;

        _xpCountDisplayer.DisplayCount(HatchXP);
        NewSpeciesManager.Instance.OpenPanel(data);
    }

    public void RemoveData()
    {
        SaveManager.Instance.SaveData.HatchingData.Remove(data);
    }
}
