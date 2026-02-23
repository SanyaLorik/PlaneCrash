using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class RangVisual : MonoBehaviour {
    [SerializeField] private float _offsetPointer;
    
    
    [SerializeField] private RectTransform _currentImageRt;
    [SerializeField] private RectTransform _recordImageRt;

    
    [SerializeField] private RectTransform _barWidth;
    [SerializeField] private RectTransform _pointerIcon;
    [SerializeField] private RectTransform _recordPointerIcon;
    
    [SerializeField] private RangUnit[] _rangPrefabs;

    [Header("Поверхности")]
    [SerializeField] private GameObject _planePrefab;
    [SerializeField] private Transform _planesParent;
    [SerializeField] private Transform _moneyCubeBottomPoint;

    
    private float _maxMoney;
    
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
        CalculateMaxMoney();
        InstanceRangs();
        SetPlanes();
        
        PlayerBankOnBankChanged(_playerBank.PlayerCapital);
        UpdateRecord();
    }

    private void SetPlanes() {
        Debug.Log("Установка рангов");
        foreach (var rang in _config.Rangs) {
            float planeY = _moneyCube.GetCubeHeight(rang.Money) - _moneyCubeBottomPoint.position.y;
            Vector3 position = new Vector3(_planesParent.transform.position.x, planeY, _planesParent.transform.position.z);
            // Debug.Log($"Для ранга: {rang.Name} высота будет: {planeY}");
            Instantiate(_planePrefab, position, Quaternion.identity, _planesParent);
            // plane.transform.position = position;
        }
        
    }
    
    private void InstanceRangs() {
        for (var i = 0; i < _config.Rangs.Count; i++) {
            var rang = _config.Rangs[i];
            float rangPercent = rang.Money / _maxMoney;
            float xEnd = _fillAmounthMover.CalculateXEnd(_barWidth);
            _fillAmounthMover.SetPointer(_rangPrefabs[i]._rt, rangPercent, xEnd);
            _rangPrefabs[i].SetData(_formatter.ValuteFormatter(rang.Money), rang.Sprite);
            
        }
    }
    
    
    private void PlayerBankOnBankChanged(long amount) {
        float percent = Mathf.Clamp01(amount / _maxMoney);
        
        // Текущий
        _fillAmounthMover.SetFillAmountWithPointer(_currentImageRt, _barWidth, _pointerIcon, percent, _offsetPointer);

        // Рекорд
        if (amount > _playerBank.PlayerRecord) {
            UpdateRecord();
        }
    }

    private void UpdateRecord() {
        float percent = Mathf.Clamp01(_playerBank.PlayerRecord / _maxMoney);
        _fillAmounthMover.SetFillAmountWithPointer(_recordImageRt, _barWidth, _recordPointerIcon, percent, _offsetPointer);
    }


#region Helpers

    private void CalculateMaxMoney() {
        _maxMoney = _config.Rangs[^1].Money;
        foreach (var rang in _config.Rangs) {
            if (_maxMoney < rang.Money) {
                _maxMoney = rang.Money;
            }
        }
    }
    
#endregion
}
