using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class RangVisual : MonoBehaviour {
    [SerializeField] private Image _currentLevel;
    [SerializeField] private Image _recordLevel;
    [SerializeField] private RectTransform _barWidth;
    [SerializeField] private RectTransform _playerIcon;
    [SerializeField] private RangUnit _rangPrefab;

    
    [Header("Рекорд игрока")]
    [SerializeField] private float _record;

    
    private RangConfig _config;
    private PlayerBank _playerBank;
    private float _maxMoney;
    private float _xStart;
    private float _xMax;
    private float _yPos;
    


    [Inject]
    public void Init(RangConfig config, PlayerBank playerBank) {
        _config = config;
        _playerBank = playerBank;
        _playerBank.BankChanged += PlayerBankOnBankChanged;
    }
    
    private void Start() {
        CalculateMaxMoney();
        CalculateX();

        InstanceRangs();

    }


    private void InstanceRangs() {
        foreach (var rang in _config.Rangs) {
            float rangPercent = rang.Money / _maxMoney;
            Debug.Log(rang.Name + " - "+ rangPercent);
            
            float rangXPos = Mathf.Lerp(_xStart, _xMax, rangPercent);
            
            
            RangUnit rangInstance = Instantiate(_rangPrefab, _barWidth);
            RectTransform rt = rangInstance.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(rangXPos, 0f);
            
            Debug.Log(rt.position);
            rangInstance.SetData(rang.Name, rang.Sprite);
        }
    }
    
    
    private void PlayerBankOnBankChanged(float amount) {
        if (amount > _record) {
            _record =  amount;
            _recordLevel.fillAmount = GetFillAmount(_record);
        }

        float percent = GetFillAmount(amount);
        _currentLevel.fillAmount = percent;
        float rangXPos = Mathf.Lerp(_xStart, _xMax, percent);
        _playerIcon.anchoredPosition = new Vector2(rangXPos, _playerIcon.anchoredPosition.y);

        // А еще игрока сдвигать относительно продвижения зеленого fillAmount
    }


#region Helpers
    private float GetFillAmount(float money) => Mathf.Clamp01(money / _maxMoney);

    private void CalculateX() {
        float barWidth = _barWidth.rect.width;
        _xStart = -barWidth * 0.5f;
        _xMax = barWidth * 0.5f;
        
        
        Debug.Log(_xStart);
        Debug.Log(_xMax);
    }


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
