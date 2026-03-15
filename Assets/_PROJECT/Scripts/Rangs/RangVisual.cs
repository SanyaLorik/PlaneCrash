using System;
using Unity.Mathematics.Geometry;
using UnityEngine;
using Zenject;

public class RangVisual : MonoBehaviour {
    [SerializeField] private float _pointerOffset = 12f;
    [SerializeField] private RectTransform _currentImageRt;
    [SerializeField] private RectTransform _recordImageRt;
    [SerializeField] private RectTransform _barWidth;
    [SerializeField] private RectTransform _pointerIcon;
    [SerializeField] private RectTransform _recordPointerIcon;
    [SerializeField] private RangUnit[] _rangPrefabs;

    [Header("Поверхности")]
    [SerializeField] private RangPlatformUnit[] _rangPlanes;
    
    [Header("Куча БАБЛА ЕБАНОГО")]
    [SerializeField] private Transform _cubeRoof;
    [SerializeField] private Transform _cubeFloor;
    
    
    [Inject] private PlayerBank _playerBank;
    [Inject] private RangConfig _config;
    [Inject] private MoneyCube _moneyCube;
    [Inject] private LocalizationDataPC _localization;
    [Inject] private NumberFormatter _formatter;

    private void Awake() {
        SetPlanes();
    }


    private void OnEnable() {
        _playerBank.BankChanged += PlayerBankOnBankChanged;
        SystemEvents.WindowScaleChanged += ReclculateCanvas;
    }
    


    private void ReclculateCanvas() {
        InstanceRangs();
        PlayerBankOnBankChanged(_playerBank.PlayerCapital);
        RecalculateRecord();
    }


    private void Start() {
        ReclculateCanvas();
    }

    private void SetPlanes() {
        for (var i = 0; i < _config.Rangs.Count; i++) {
            InstancePlane(i);
        }
    }

    private void InstancePlane(int i) {
        float percent = (i + 1f) / _config.Rangs.Count;
        float y = Mathf.Lerp(_cubeFloor.position.y, _cubeRoof.position.y, percent);    
        
        
        Vector3 position = new Vector3(
            _rangPlanes[i].transform.position.x, 
            y, 
            _rangPlanes[i].transform.position.z
        );
        _rangPlanes[i].transform.position = position;

    }

    private void InstanceRangs() {
        float xEnd = RectTransformHelper.CalculateXEnd(_barWidth);
        for (int i = 0; i < _config.Rangs.Count; i++) {
            SetRangInPlace(i, xEnd);
        }
    }

    private void SetRangInPlace(int i, float xEnd) {
        RangData rang = _config.Rangs[i];
        // Равномерно распределить
        // float rangPercent = (float)i / _config.Rangs.Count;
        // _rtHelper.SetPointer(_rangPrefabs[i]._rt, rangPercent, xEnd);
        float xInside = RectTransformHelper.GetXInsideBar(_rangPrefabs[i].Rt, _barWidth);
        _rangPrefabs[i].SetData(rang.Money, rang.Sprite, xInside);
    }


    private int GetNextRangIndex(long amount) {
        int index = 0;
        foreach (var rang in _rangPrefabs) {
            if (rang.Money >= amount) {
                Debug.Log("Номер некст ранга: " + index);
                return index;
            }
            index++;
        }
        Debug.Log("Игрок переплюнул ласт ранг");
        return _rangPrefabs.Length-1;
    }

    private void PlayerBankOnBankChanged(long currentAmount) {
        float percent = CalculatePointerPercent(currentAmount);
        RectTransformHelper.SetFillAmountWithPointer(_currentImageRt, _barWidth, _pointerIcon, percent, _pointerOffset);

        
        // Рекорд
        Debug.Log($"currentAmount = {currentAmount}, _playerBank.PlayerRecord = {_playerBank.PlayerRecord}");
        if (currentAmount >= _playerBank.PlayerRecord) {
            RecalculateRecord();
        }
    }

    private void RecalculateRecord() {
        float percent = CalculatePointerPercent(_playerBank.PlayerRecord);
        RectTransformHelper.SetFillAmountWithPointer(_recordImageRt, _barWidth, _recordPointerIcon, percent, _pointerOffset);
    }

    
    private float CalculatePointerPercent(long currentAmount) {
        int nextRangIndex = GetNextRangIndex(currentAmount);
        RangUnit nextRang =  _rangPrefabs[nextRangIndex];

        float xNext = nextRang.XInside;
        float xPrevious = 0;
        if (nextRangIndex != 0) {
            xPrevious = _rangPrefabs[nextRangIndex - 1].XInside;
        }

        float percent = Mathf.Clamp01((float)currentAmount / nextRang.Money);
        float x = xPrevious +  (xNext - xPrevious) * percent;
        var calculatePointerPercent = x / RectTransformHelper .CalculateXEnd(_barWidth);
        Debug.Log(calculatePointerPercent);
        return calculatePointerPercent;

    }

    
    private void OnDisable() {
        _playerBank.BankChanged -= PlayerBankOnBankChanged;
        SystemEvents.WindowScaleChanged -= ReclculateCanvas;
    }
    
    private void OnDestroy() {
        if (_playerBank != null)
            _playerBank.BankChanged -= PlayerBankOnBankChanged;
        SystemEvents.WindowScaleChanged -= ReclculateCanvas;
    }

}
