using System;
using Architecture_M;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using Zenject;

[Serializable]
public enum MoneyCubeType {
    PlayerBank,
    Bet
}


public class MoneyCube : MonoBehaviour {
    [SerializeField] private MoneyCubeType _moneyCubeType;
    [SerializeField, HideIf(nameof(IsBetCube))] private TMP_Text _cubeText;

    [Header("Настройка размеров ")]
    [SerializeField] private float _scaleDivider = 2f; 
    
    

    [Header("Настройка размеров ")]
    [SerializeField] private float _maxSide = 5f; 
    private float _baseSide = 1f;
    
    
    [Header("Настройка всего тайлинга ")]
    [SerializeField] private Renderer _rend;
    [SerializeField] private float _tileWorldSize = 1f;
    [SerializeField] private Vector2 _tilingRatio = new Vector2(5f, 11f); // тайлинг пачек

    [Header("Индекс материалов")] 
    [SerializeField] private int[] _moneyMaterialSlots;
    
    [Header("Настройка верхнего тайлинга")]
    [SerializeField] private Vector2 _upSideTiling;
    [SerializeField] private int _upSideMaterialSlot;
    
    
    [Header("Настройка верхниза")] 
    [SerializeField] private Transform _bottomPoint; // точка низа куба
    
    
    private bool IsBetCube => _moneyCubeType == MoneyCubeType.Bet;
    private float _maxYScale;
    private float _distanceBetween2Rangs;

    
    
    [Inject] private NumberFormatter _formatter; 
    [Inject] private LocalizationDataPC _localization; 
    [Inject] private PlayerBank _playerBank; 
    [Inject] private RangConfig _rangConfig;
    
    [Inject] IGameSave<GameSavePC> _gameSave;

    private void Awake() {
        _maxYScale = transform.localScale.y;
        _distanceBetween2Rangs = _maxYScale / _rangConfig.Rangs.Count;

        Debug.LogWarning("_maxCubeHeight = " + _maxYScale);
    }

    private float _maxScaleYCube; 
    private void Start() {
        SetMoneyAmountForBank(_gameSave.GetSave.Money);
    }

    
    public void SetMoneyAmountForBank(long amount) {
        Vector3 newScale = NewScaleBank(amount);
        
        transform.localScale = newScale;

        // По сути опускает прст кучу в точку боттом
        if (_bottomPoint != null) {
            Bounds b = _rend.bounds;
            float deltaY = _bottomPoint.position.y - b.min.y;
            transform.position += new Vector3(0, deltaY, 0);
        }

        UpdateTiling();
        SetCubeHeighVisual();
    }
    
    public void SetMoneyAmountForBet(long amount) {
        // Мб другой метод
        Vector3 newScale = NewScaleBet(amount);
        transform.localScale = newScale;
        
        // По сути опускает прст кучу в точку боттом
        if (_bottomPoint != null) {
            Bounds b = _rend.bounds;
            float deltaY = _bottomPoint.position.y - b.min.y;
            transform.position += new Vector3(0, deltaY, 0);
        }

        UpdateTiling();
    }
    
    
    private void UpdateTiling() {
        Vector3 size = transform.localScale;

        float baseX = size.x / _tileWorldSize;
        float baseY = size.y / _tileWorldSize;

        Vector2 finalTiling = new Vector2(
            Mathf.Round(baseX * _tilingRatio.x),
            Mathf.Round(baseY * _tilingRatio.y)
        );

        Material[] mats = _rend.materials;

        foreach (int id in _moneyMaterialSlots) {
            Vector2 tiling = finalTiling;
            tiling.x = Mathf.Min(tiling.x, _upSideTiling.x);
            
            if (id == _upSideMaterialSlot) {
                tiling.y = Mathf.Min(tiling.y, _upSideTiling.y);
            }

            mats[id].mainTextureScale = tiling;
        }

        _rend.materials = mats;
    }
    

    private void SetCubeHeighVisual() {
        if (!IsBetCube) {
            _cubeText.text = _formatter.ValuteFormatter(transform.localScale.y / _scaleDivider);
        }
    }

    
    [SerializeField] private float _maxSideAmount = 1000000;
    private Vector3 NewScaleBank(long amount) {
        Debug.Log("amount = " + amount);
        float percent = amount / _maxSideAmount;
        Debug.Log("percent =  " + percent);
        float linearSide = _maxSide * percent;

        float height =  _maxYScale * CalculateAmountPercent(amount);

        // Нормальный рост во все стороны
        float side = Mathf.Min(linearSide, _maxSide);

        Vector3 newScale = new Vector3(side, height, side);
        Debug.LogWarning("newScale = " + newScale);
        return newScale;
    }
    
    
    private Vector3 NewScaleBet(long amount)
    {
        float percent = Mathf.Clamp01((float)amount / _maxSideAmount);

        float sidePercent = Mathf.Sqrt(percent);          // быстрее расширяется
        float heightPercent = Mathf.Pow(percent, 1.5f);   // медленнее растёт вверх

        float side = _maxSide * sidePercent;
        float height = _maxYScale * heightPercent;

        return new Vector3(side, height, side);
    }
    
    private float CalculateAmountPercent(long currentAmount) {
        int nextRangIndex = GetNextRangIndex(currentAmount);
        RangData nextRang =  _rangConfig.Rangs[nextRangIndex];

        
        float currentY = _distanceBetween2Rangs * (nextRangIndex+1);
        float previousY = _distanceBetween2Rangs *  nextRangIndex;
        
        // Процент между предыдущим и  некст рангом
        float percent = Mathf.Clamp01((float)currentAmount / nextRang.Money);
        float newY = previousY + percent * (currentY - previousY);
        percent = Mathf.Clamp01(newY / _maxYScale);
        return percent;
    }
    
    private int GetNextRangIndex(long amount) {
        int index = 0;
        foreach (var rang in _rangConfig.Rangs) {
            if (rang.Money >= amount) {
                Debug.Log("Номер некст ранга: " + index);
                return index;
            }
            index++;
        }
        Debug.Log("Игрок переплюнул ласт ранг");
        return _rangConfig.Rangs.Count-1;
    }
    

}