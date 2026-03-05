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
    [SerializeField] private GameObject _planePrefab;
    [SerializeField] private Transform _planesParent;
    
    [Inject] private PlayerBank _playerBank;
    [Inject] private RangConfig _config;
    [Inject] private MoneyCube _moneyCube;
    [Inject] private LocalizationDataPC _localization;
    [Inject] private NumberFormatter _formatter;
    [Inject] private RectTransformHelper _fillAmounthMover;



    private void OnEnable() {
        _playerBank.BankChanged += PlayerBankOnBankChanged;
    }


    private void Start() {
        InstanceRangs();
        SetPlanes();
        PlayerBankOnBankChanged(_playerBank.PlayerCapital);
        RecalculateRecord();
    }

    private void SetPlanes() {
        Debug.Log("Установка рангов");
        foreach (var rang in _config.Rangs) {
            float planeY = _moneyCube.GetCubeHeight(rang.Money);
            Vector3 position = new Vector3(_planesParent.transform.position.x, planeY, _planesParent.transform.position.z);
            // Debug.Log($"Для ранга: {rang.Name} высота будет: {planeY}");
            Instantiate(_planePrefab, position, Quaternion.identity, _planesParent);
            // plane.transform.position = position;
        }
        
    }
    
    private void InstanceRangs() {
        float xEnd = _fillAmounthMover.CalculateXEnd(_barWidth);
        for (int i = 0; i < _config.Rangs.Count; i++) {
            SetRangInPlace(i, xEnd);
        }
    }

    private void SetRangInPlace(int i, float xEnd) {
        RangData rang = _config.Rangs[i];
        // Равномерно распределить
        float rangPercent = (i+1f) / _config.Rangs.Count;
        _fillAmounthMover.SetPointer(_rangPrefabs[i]._rt, rangPercent, xEnd);
        _rangPrefabs[i].SetData(rang.Money, rang.Sprite, (rangPercent*xEnd));
        Debug.Log("Установка ранга в " + _rangPrefabs[i].XValue);
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
        _fillAmounthMover.SetFillAmountWithPointer(_currentImageRt, _barWidth, _pointerIcon, percent, _pointerOffset);

        
        // Рекорд
        Debug.Log($"currentAmount = {currentAmount}, _playerBank.PlayerRecord = {_playerBank.PlayerRecord}");
        if (currentAmount >= _playerBank.PlayerRecord) {
            RecalculateRecord();
        }
    }

    private void RecalculateRecord() {
        float percent = CalculatePointerPercent(_playerBank.PlayerRecord);
        _fillAmounthMover.SetFillAmountWithPointer(_recordImageRt, _barWidth, _recordPointerIcon, percent, _pointerOffset);
    }

    private float CalculatePointerPercent(long currentAmount) {
        int nextRangIndex = GetNextRangIndex(currentAmount);
        RangUnit nextRang =  _rangPrefabs[nextRangIndex];
        
        float previousX = nextRang.XValue 
                          - 
                          _fillAmounthMover.Calculate1PeaceWidth(_barWidth, _rangPrefabs.Length);

        float nextX = nextRang.XValue;
        
        // Процент между предыдущим и  некст рангом
        float percent = Mathf.Clamp01((float)currentAmount / nextRang.Money);
        float newX = previousX + percent * (nextX - previousX);
        return newX / _fillAmounthMover.CalculateXEnd(_barWidth);
    }
}
