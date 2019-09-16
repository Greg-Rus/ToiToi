using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private HudView _hudView;

    [SerializeField] private float _currentMoments;

    [SerializeField] private float _distance;

    [SerializeField] private float _maxMoment;
    [SerializeField] private float _momentValue;
    [SerializeField] private float _momentDecaySpeed;
    [SerializeField] private float _paradoxDamage;
    [SerializeField] private float _distanceMultiplier;

    private DateTime _date;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _currentMoments = _maxMoment;
        _date = DateTime.Now;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMoments();
    }

    public void OnMomentCollected()
    {
        _distanceMultiplier+=100;
        _currentMoments += _momentValue;
        if (_currentMoments > _maxMoment) _currentMoments = _maxMoment;
    }

    public void UpdateDistance(float distance)
    {
        _distance += distance;
        _date = _date.AddSeconds(-distance * _distanceMultiplier);
        _hudView.Distance = string.Format("{0}-{1}-{2} {3}:{4}:{5}", _date.Year, _date.Month, _date.Day, _date.Hour,
            _date.Minute, _date.Second);
    }

    public void UpdateMoments()
    {
        _currentMoments -= Time.deltaTime * _momentDecaySpeed;
        _hudView.Slider = _currentMoments / _maxMoment;
    }

    public void OnParadoxHit()
    {
        _currentMoments -= _paradoxDamage;
        if (_currentMoments < 0) _currentMoments = 0;
    }
}
